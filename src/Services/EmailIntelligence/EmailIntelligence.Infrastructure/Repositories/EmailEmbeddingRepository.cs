using EmailIntelligence.Domain.Entities;
using EmailIntelligence.Domain.Repositories;
using EmailIntelligence.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pgvector.EntityFrameworkCore;

namespace EmailIntelligence.Infrastructure.Repositories;

public class EmailEmbeddingRepository : IEmailEmbeddingRepository
{
    private readonly EmailIntelligenceDbContext _context;
    private readonly ILogger<EmailEmbeddingRepository> _logger;

    public EmailEmbeddingRepository(
        EmailIntelligenceDbContext context,
        ILogger<EmailEmbeddingRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<EmailEmbedding?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.EmailEmbeddings
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<EmailEmbedding>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.EmailEmbeddings
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EmailEmbedding>> GetByEmailIdAsync(string emailId, CancellationToken cancellationToken = default)
    {
        return await _context.EmailEmbeddings
            .Where(e => e.EmailId == emailId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EmailEmbedding>> FindSimilarAsync(
        float[] queryEmbedding, 
        string userId, 
        int limit = 10, 
        double minSimilarity = 0.7, 
        string? contentType = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.EmailEmbeddings
                .Where(e => e.UserId == userId);

            if (!string.IsNullOrEmpty(contentType))
            {
                query = query.Where(e => e.ContentType == contentType);
            }

            // Use pgvector similarity search
            var results = await query
                .OrderBy(e => e.Embedding.CosineDistance(new Pgvector.Vector(queryEmbedding)))
                .Take(limit * 2) // Get more results to filter by similarity threshold
                .ToListAsync(cancellationToken);

            // Filter by minimum similarity and limit results
            var filteredResults = results
                .Where(e => e.CalculateSimilarity(queryEmbedding) >= minSimilarity)
                .Take(limit)
                .ToList();

            return filteredResults;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding similar embeddings for user {UserId}", userId);
            throw;
        }
    }

    public async Task<EmailEmbedding> AddAsync(EmailEmbedding embedding, CancellationToken cancellationToken = default)
    {
        _context.EmailEmbeddings.Add(embedding);
        await _context.SaveChangesAsync(cancellationToken);
        return embedding;
    }

    public async Task UpdateAsync(EmailEmbedding embedding, CancellationToken cancellationToken = default)
    {
        _context.EmailEmbeddings.Update(embedding);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var embedding = await _context.EmailEmbeddings
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        
        if (embedding != null)
        {
            _context.EmailEmbeddings.Remove(embedding);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> GetCountByUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.EmailEmbeddings
            .CountAsync(e => e.UserId == userId, cancellationToken);
    }
}
