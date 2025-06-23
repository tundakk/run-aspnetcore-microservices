using EmailIntelligence.Infrastructure.Configuration;
using System.Text.Json;
using System.Text;

namespace EmailIntelligence.Infrastructure.Services;

public class EmailAnalysisService(
    HttpClient httpClient,
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
            var prompt = BuildAnalysisPrompt(subject, body, from);
            var response = await CallLLMAsync(prompt, cancellationToken);
            
            return ParseAnalysisResponse(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error analyzing email for user {UserId}", userId);
            
            // Fallback to rule-based analysis
            return FallbackAnalysis(subject, body, from);
        }
    }

    private string BuildAnalysisPrompt(string subject, string body, string from)
    {
        return $@"
Analyze this email and provide classification in JSON format:

Subject: {subject}
From: {from}
Body: {body}

Please respond with ONLY a JSON object containing:
{{
    ""priority"": 0-3 (0=Low, 1=Medium, 2=High, 3=Critical),
    ""category"": ""RequiresResponse|Informational|ActionRequired|Meeting|Support|Marketing|Newsletter|Spam|Personal|Internal"",
    ""requiresResponse"": true/false,
    ""confidenceScore"": 0.0-1.0,
    ""keywords"": [""keyword1"", ""keyword2""],
    ""actionItems"": ""description of actions needed or null""
}}

Consider:
- Urgency indicators (URGENT, ASAP, deadline)
- Sender importance (CEO, manager, client)
- Content type (question, request, information)
- Meeting invitations or calendar items
- Support tickets or issues
- Marketing or promotional content
";
    }

    private async Task<string> CallLLMAsync(string prompt, CancellationToken cancellationToken)
    {
        var requestBody = new
        {
            model = _settings.Model,
            messages = new[]
            {
                new { role = "system", content = "You are an expert email classifier. Always respond with valid JSON only." },
                new { role = "user", content = prompt }
            },
            max_tokens = _settings.MaxTokens,
            temperature = _settings.Temperature
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _settings.ApiKey);

        var response = await httpClient.PostAsync("/v1/chat/completions", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var responseObj = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        return responseObj
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "";
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
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to parse LLM response, using fallback");
            throw;
        }
    }

    private EmailAnalysisResult FallbackAnalysis(string subject, string body, string from)
    {
        // Simple rule-based fallback
        var priority = EmailPriority.Medium;
        var category = EmailCategory.Informational;
        var requiresResponse = false;

        var text = $"{subject} {body}".ToLowerInvariant();
        
        // Check for urgent indicators
        if (text.Contains("urgent") || text.Contains("asap") || text.Contains("immediately"))
            priority = EmailPriority.High;
            
        // Check for response requirements
        if (text.Contains("?") || text.Contains("please respond") || text.Contains("reply"))
        {
            requiresResponse = true;
            category = EmailCategory.RequiresResponse;
        }
        
        // Simple keyword extraction
        var keywords = new[] { "email", "message" };

        return new EmailAnalysisResult(
            priority,
            category,
            requiresResponse,
            0.5, // Low confidence for fallback
            keywords,
            null
        );
    }
}
