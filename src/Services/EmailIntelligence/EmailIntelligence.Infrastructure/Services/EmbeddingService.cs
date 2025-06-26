using EmailIntelligence.Application.Services;
using EmailIntelligence.Domain.Entities;
using EmailIntelligence.Domain.Repositories;
using EmailIntelligence.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.Connectors.OpenAI;

#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates.

namespace EmailIntelligence.Infrastructure.Services;

public class EmbeddingService : IEmbeddingService
{
    private readonly OpenAITextEmbeddingGenerationService _embeddingService;
    private readonly IEmailEmbeddingRepository _embeddingRepository;
    private readonly ILogger<EmbeddingService> _logger;
    private readonly LLMSettings _settings;

    public EmbeddingService(
        OpenAITextEmbeddingGenerationService embeddingService,
        IEmailEmbeddingRepository embeddingRepository,
        IOptions<LLMSettings> settings,
        ILogger<EmbeddingService> logger)
    {
        _embeddingService = embeddingService;
        _embeddingRepository = embeddingRepository;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        try
        {
            var embeddings = await _embeddingService.GenerateEmbeddingsAsync([text], cancellationToken: cancellationToken);
            return embeddings.First().ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding for text: {Text}", text[..Math.Min(100, text.Length)]);
            throw;
        }
    }

    public async Task<IReadOnlyList<float[]>> GenerateEmbeddingsAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default)
    {
        try
        {
            var embeddings = await _embeddingService.GenerateEmbeddingsAsync(texts.ToList(), cancellationToken: cancellationToken);
            return embeddings.Select(e => e.ToArray()).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embeddings for {Count} texts", texts.Count());
            throw;
        }
    }

    public Task<double> CalculateSimilarityAsync(float[] embedding1, float[] embedding2)
    {
        if (embedding1.Length != embedding2.Length)
            return Task.FromResult(0.0);

        // Cosine similarity
        var dotProduct = 0.0;
        var magnitudeA = 0.0;
        var magnitudeB = 0.0;

        for (int i = 0; i < embedding1.Length; i++)
        {
            dotProduct += embedding1[i] * embedding2[i];
            magnitudeA += embedding1[i] * embedding1[i];
            magnitudeB += embedding2[i] * embedding2[i];
        }

        if (magnitudeA == 0.0 || magnitudeB == 0.0)
            return Task.FromResult(0.0);

        var similarity = dotProduct / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
        return Task.FromResult(similarity);
    }

    public async Task<float[]> CalculateSemanticDifferenceAsync(string originalText, string modifiedText, CancellationToken cancellationToken = default)
    {
        var originalEmbedding = await GenerateEmbeddingAsync(originalText, cancellationToken);
        var modifiedEmbedding = await GenerateEmbeddingAsync(modifiedText, cancellationToken);

        // Calculate the difference vector
        var difference = new float[originalEmbedding.Length];
        for (int i = 0; i < originalEmbedding.Length; i++)
        {
            difference[i] = modifiedEmbedding[i] - originalEmbedding[i];
        }

        return difference;
    }

    public async Task StoreEmailEmbeddingAsync(
        string emailId,
        string userId,
        string contentType,
        string content,
        Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var embedding = await GenerateEmbeddingAsync(content, cancellationToken);
            var emailEmbedding = EmailEmbedding.Create(
                emailId,
                userId,
                contentType,
                content,
                embedding,
                metadata);

            await _embeddingRepository.AddAsync(emailEmbedding, cancellationToken);
            
            _logger.LogInformation("Stored embedding for email {EmailId}, user {UserId}, type {ContentType}", 
                emailId, userId, contentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing embedding for email {EmailId}", emailId);
            throw;
        }
    }

    public async Task<IReadOnlyList<SimilarContent>> FindSimilarContentAsync(
        string userId,
        string query,
        int limit = 10,
        string? contentType = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryEmbedding = await GenerateEmbeddingAsync(query, cancellationToken);
            var similarEmbeddings = await _embeddingRepository.FindSimilarAsync(
                queryEmbedding,
                userId,
                limit,
                0.7, // Minimum similarity threshold
                contentType,
                cancellationToken);

            var results = new List<SimilarContent>();
            foreach (var embedding in similarEmbeddings)
            {
                var similarity = await CalculateSimilarityAsync(queryEmbedding, embedding.Embedding);
                results.Add(new SimilarContent(
                    embedding.EmailId,
                    embedding.ContentType,
                    embedding.Content,
                    similarity,
                    embedding.Metadata));
            }

            return results.OrderByDescending(r => r.Similarity).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding similar content for user {UserId}, query: {Query}", 
                userId, query[..Math.Min(100, query.Length)]);
            throw;
        }
    }
}
