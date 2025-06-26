namespace EmailIntelligence.Domain.Entities;

public class EmailEmbedding
{
    public Guid Id { get; private set; }
    public string EmailId { get; private set; } = default!;
    public string UserId { get; private set; } = default!;
    public string ContentType { get; private set; } = default!; // "email", "draft", "response"
    public string Content { get; private set; } = default!;
    public float[] Embedding { get; private set; } = default!;
    public Dictionary<string, object> Metadata { get; private set; } = new();
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private EmailEmbedding() { } // EF Core constructor

    public static EmailEmbedding Create(
        string emailId,
        string userId,
        string contentType,
        string content,
        float[] embedding,
        Dictionary<string, object>? metadata = null)
    {
        return new EmailEmbedding
        {
            Id = Guid.NewGuid(),
            EmailId = emailId,
            UserId = userId,
            ContentType = contentType,
            Content = content,
            Embedding = embedding,
            Metadata = metadata ?? new Dictionary<string, object>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdateEmbedding(float[] newEmbedding, string? newContent = null)
    {
        Embedding = newEmbedding;
        if (newContent != null)
            Content = newContent;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateMetadata(Dictionary<string, object> metadata)
    {
        Metadata = metadata;
        UpdatedAt = DateTime.UtcNow;
    }

    public double CalculateSimilarity(float[] otherEmbedding)
    {
        if (Embedding.Length != otherEmbedding.Length)
            return 0.0;

        // Cosine similarity calculation
        var dotProduct = 0.0;
        var magnitudeA = 0.0;
        var magnitudeB = 0.0;

        for (int i = 0; i < Embedding.Length; i++)
        {
            dotProduct += Embedding[i] * otherEmbedding[i];
            magnitudeA += Embedding[i] * Embedding[i];
            magnitudeB += otherEmbedding[i] * otherEmbedding[i];
        }

        if (magnitudeA == 0.0 || magnitudeB == 0.0)
            return 0.0;

        return dotProduct / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
    }
}
