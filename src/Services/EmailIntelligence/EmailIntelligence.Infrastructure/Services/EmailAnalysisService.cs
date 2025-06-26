using EmailIntelligence.Application.Services;
using EmailIntelligence.Infrastructure.Configuration;
using System.Text.Json;
using System.Text;

namespace EmailIntelligence.Infrastructure.Services;

public class EmailAnalysisService(
    ISemanticKernelService semanticKernelService,
    IEmbeddingService embeddingService,
    IOptions<LLMSettings> llmSettings,
    ILogger<EmailAnalysisService> logger) : IEmailAnalysisService
{
    private readonly LLMSettings _settings = llmSettings.Value;

    public async Task<EmailAnalysisResult> AnalyzeEmailAsync(
        string subject, 
        string body, 
        string from, 
        string userId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // First, try embedding-based classification with similar emails
            var similarEmails = await FindSimilarClassifiedEmails(subject, body, userId, cancellationToken);
            
            EmailAnalysisResult? embeddingBasedResult = null;
            if (similarEmails.Any())
            {
                embeddingBasedResult = ClassifyUsingSimilarEmails(similarEmails);
                logger.LogInformation("Found {Count} similar emails for classification", similarEmails.Count);
            }

            // Get LLM-based classification with contextual history
            var contextualHistory = await BuildContextualHistory(similarEmails);
            var llmResponse = await semanticKernelService.ClassifyEmailAsync(subject, body, from, contextualHistory, cancellationToken);
            var llmResult = ParseAnalysisResponse(llmResponse);

            // Combine embedding-based and LLM-based results
            var finalResult = CombineAnalysisResults(embeddingBasedResult, llmResult);

            // Store the embedding for future use
            await StoreEmailEmbedding(subject, body, from, userId, finalResult, cancellationToken);

            return finalResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error analyzing email for user {UserId}", userId);
            
            // Fallback to rule-based analysis
            return FallbackAnalysis(subject, body, from);
        }
    }

    private async Task<IReadOnlyList<SimilarContent>> FindSimilarClassifiedEmails(
        string subject, 
        string body, 
        string userId, 
        CancellationToken cancellationToken)
    {
        try
        {
            var query = $"{subject} {body}";
            return await embeddingService.FindSimilarContentAsync(
                userId, 
                query, 
                limit: 5, 
                contentType: "email", 
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not find similar emails for user {UserId}", userId);
            return Array.Empty<SimilarContent>();
        }
    }

    private EmailAnalysisResult? ClassifyUsingSimilarEmails(IReadOnlyList<SimilarContent> similarEmails)
    {
        if (!similarEmails.Any()) return null;

        try
        {
            // Extract classification data from metadata of similar emails
            var classifications = similarEmails
                .Where(e => e.Metadata.ContainsKey("classification"))
                .Select(e => {
                    try
                    {
                        return JsonSerializer.Deserialize<EmailAnalysisResult>(e.Metadata["classification"].ToString()!);
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(c => c != null)
                .Cast<EmailAnalysisResult>()
                .ToList();

            if (!classifications.Any()) return null;

            // Weight by similarity and create consensus classification
            var weightedPriority = classifications
                .Zip(similarEmails.Take(classifications.Count), (classification, similar) => 
                    new { Priority = (int)classification.Priority, Weight = similar.Similarity })
                .Sum(x => x.Priority * x.Weight) / similarEmails.Sum(s => s.Similarity);

            var mostCommonCategory = classifications
                .GroupBy(c => c.Category)
                .OrderByDescending(g => g.Count())
                .First().Key;

            var avgConfidence = classifications.Average(c => c.ConfidenceScore);
            var commonKeywords = classifications
                .SelectMany(c => c.Keywords)
                .GroupBy(k => k)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToArray();

            return new EmailAnalysisResult(
                (EmailPriority)Math.Round(weightedPriority),
                mostCommonCategory,
                classifications.Any(c => c.RequiresResponse),
                Math.Min(0.9, avgConfidence + 0.1), // Boost confidence for embedding-based classification
                commonKeywords,
                null // Action items will be determined by LLM
            );
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error creating embedding-based classification");
            return null;
        }
    }

    private async Task<string> BuildContextualHistory(IReadOnlyList<SimilarContent> similarEmails)
    {
        if (!similarEmails.Any()) return "";

        var history = new StringBuilder("Liknande tidigare e-postmeddelanden:\n");
        foreach (var email in similarEmails.Take(3)) // Top 3 most similar
        {
            var summary = await semanticKernelService.SummarizeContentAsync(email.Content, 50);
            history.AppendLine($"- {email.ContentType}: {summary} (Likhet: {email.Similarity:F2})");
        }

        return history.ToString();
    }

    private EmailAnalysisResult CombineAnalysisResults(
        EmailAnalysisResult? embeddingResult, 
        EmailAnalysisResult llmResult)
    {
        if (embeddingResult == null) return llmResult;

        // Combine the results, giving weight to embedding-based classification if confidence is high
        var finalPriority = embeddingResult.ConfidenceScore > 0.8 
            ? embeddingResult.Priority 
            : llmResult.Priority;

        var finalCategory = embeddingResult.ConfidenceScore > 0.8
            ? embeddingResult.Category
            : llmResult.Category;

        var combinedKeywords = embeddingResult.Keywords
            .Union(llmResult.Keywords)
            .Distinct()
            .ToArray();

        var averageConfidence = (embeddingResult.ConfidenceScore + llmResult.ConfidenceScore) / 2;

        return new EmailAnalysisResult(
            finalPriority,
            finalCategory,
            embeddingResult.RequiresResponse || llmResult.RequiresResponse,
            averageConfidence,
            combinedKeywords,
            llmResult.ActionItems // Use LLM's action items as they're more contextual
        );
    }

    private async Task StoreEmailEmbedding(
        string subject, 
        string body, 
        string from, 
        string userId, 
        EmailAnalysisResult analysisResult, 
        CancellationToken cancellationToken)
    {
        try
        {
            var content = $"{subject} {body}";
            var metadata = new Dictionary<string, object>
            {
                ["from"] = from,
                ["subject"] = subject,
                ["classification"] = JsonSerializer.Serialize(analysisResult),
                ["analyzed_at"] = DateTime.UtcNow.ToString("O")
            };

            await embeddingService.StoreEmailEmbeddingAsync(
                Guid.NewGuid().ToString(), // Generate unique email ID
                userId,
                "email",
                content,
                metadata,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not store email embedding for user {UserId}", userId);
            // Don't throw - this is not critical for the main functionality
        }
    }

    private EmailAnalysisResult ParseAnalysisResponse(string response)
    {
        try
        {
            var json = JsonSerializer.Deserialize<JsonElement>(response);
            
            var priority = (EmailPriority)json.GetProperty("priority").GetInt32();
            var categoryStr = json.GetProperty("category").GetString()!;
            var category = Enum.Parse<EmailCategory>(categoryStr);
            var requiresResponse = json.GetProperty("requiresResponse").GetBoolean();
            var confidence = json.GetProperty("confidenceScore").GetDouble();
            
            var keywordsArray = json.GetProperty("keywords").EnumerateArray()
                .Select(k => k.GetString()!)
                .ToArray();
                
            var actionItems = json.TryGetProperty("actionItems", out var actionElement) && 
                              actionElement.ValueKind != JsonValueKind.Null 
                ? actionElement.GetString() 
                : null;

            return new EmailAnalysisResult(
                priority,
                category,
                requiresResponse,
                confidence,
                keywordsArray,
                actionItems
            );
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Failed to parse analysis response: {Response}", response);
            return FallbackAnalysis("", "", "");
        }
    }

    private EmailAnalysisResult FallbackAnalysis(string subject, string body, string from)
    {
        logger.LogInformation("Using fallback analysis for email from {From}", from);
        
        var priority = EmailPriority.Medium;
        var category = EmailCategory.Informational;
        var requiresResponse = false;
        var keywords = new List<string>();

        // Simple rule-based analysis
        var content = $"{subject} {body}".ToLowerInvariant();
        
        // Priority detection
        if (content.Contains("urgent") || content.Contains("asap") || content.Contains("immediately"))
            priority = EmailPriority.High;
        else if (content.Contains("deadline") || content.Contains("important"))
            priority = EmailPriority.High;
        else if (content.Contains("fyi") || content.Contains("newsletter"))
            priority = EmailPriority.Low;

        // Category detection
        if (content.Contains("meeting") || content.Contains("calendar"))
            category = EmailCategory.Meeting;
        else if (content.Contains("question") || content.Contains("?"))
        {
            category = EmailCategory.RequiresResponse;
            requiresResponse = true;
        }
        else if (content.Contains("support") || content.Contains("help"))
            category = EmailCategory.Support;
        else if (content.Contains("marketing") || content.Contains("newsletter"))
            category = EmailCategory.Marketing;

        // Simple keyword extraction
        var commonWords = new[] { "meeting", "deadline", "urgent", "question", "support", "help", "important" };
        keywords.AddRange(commonWords.Where(word => content.Contains(word)));

        return new EmailAnalysisResult(
            priority,
            category,
            requiresResponse,
            0.5, // Low confidence for fallback
            keywords.ToArray(),
            requiresResponse ? "Check if response is needed" : null
        );
    }
}
