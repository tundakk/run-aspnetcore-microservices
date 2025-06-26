using EmailIntelligence.Domain.Entities;

namespace EmailIntelligence.Domain.Repositories;

public interface IEmailEmbeddingRepository
{
    Task<EmailEmbedding?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailEmbedding>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailEmbedding>> GetByEmailIdAsync(string emailId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailEmbedding>> FindSimilarAsync(
        float[] queryEmbedding, 
        string userId, 
        int limit = 10, 
        double minSimilarity = 0.7, 
        string? contentType = null,
        CancellationToken cancellationToken = default);
    Task<EmailEmbedding> AddAsync(EmailEmbedding embedding, CancellationToken cancellationToken = default);
    Task UpdateAsync(EmailEmbedding embedding, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetCountByUserAsync(string userId, CancellationToken cancellationToken = default);
}
