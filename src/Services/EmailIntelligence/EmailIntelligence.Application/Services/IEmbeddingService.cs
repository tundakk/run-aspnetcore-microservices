namespace EmailIntelligence.Application.Services;

public interface IEmbeddingService
{
    Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<float[]>> GenerateEmbeddingsAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default);
    Task<double> CalculateSimilarityAsync(float[] embedding1, float[] embedding2);
    Task<float[]> CalculateSemanticDifferenceAsync(string originalText, string modifiedText, CancellationToken cancellationToken = default);
    Task StoreEmailEmbeddingAsync(string emailId, string userId, string contentType, string content, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SimilarContent>> FindSimilarContentAsync(string userId, string query, int limit = 10, string? contentType = null, CancellationToken cancellationToken = default);
}

public record SimilarContent(
    string EmailId,
    string ContentType,
    string Content,
    double Similarity,
    Dictionary<string, object> Metadata);
