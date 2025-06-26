using EmailIntelligence.Domain.Entities;
using EmailIntelligence.Domain.Repositories;
using EmailIntelligence.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmailIntelligence.Infrastructure.Repositories;

public class LearningPatternRepository : ILearningPatternRepository
{
    private readonly EmailIntelligenceDbContext _context;
    private readonly ILogger<LearningPatternRepository> _logger;

    public LearningPatternRepository(
        EmailIntelligenceDbContext context,
        ILogger<LearningPatternRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<LearningPattern?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LearningPatterns
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<LearningPattern>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.LearningPatterns
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.LastUsedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LearningPattern>> GetByPatternTypeAsync(string userId, string patternType, CancellationToken cancellationToken = default)
    {
        return await _context.LearningPatterns
            .Where(p => p.UserId == userId && p.PatternType == patternType)
            .OrderByDescending(p => p.ConfidenceScore)
            .ThenByDescending(p => p.UsageCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LearningPattern>> FindApplicablePatternsAsync(
        string userId, 
        string content, 
        Dictionary<string, object>? context = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get all patterns for the user with high confidence
            var patterns = await _context.LearningPatterns
                .Where(p => p.UserId == userId && p.ConfidenceScore > 0.6)
                .OrderByDescending(p => p.ConfidenceScore)
                .ThenByDescending(p => p.UsageCount)
                .ToListAsync(cancellationToken);

            // Filter patterns that are applicable to the content
            var applicablePatterns = patterns
                .Where(p => p.IsApplicable(content, context))
                .ToList();

            return applicablePatterns;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding applicable patterns for user {UserId}", userId);
            throw;
        }
    }

    public async Task<LearningPattern> AddAsync(LearningPattern pattern, CancellationToken cancellationToken = default)
    {
        _context.LearningPatterns.Add(pattern);
        await _context.SaveChangesAsync(cancellationToken);
        return pattern;
    }

    public async Task UpdateAsync(LearningPattern pattern, CancellationToken cancellationToken = default)
    {
        _context.LearningPatterns.Update(pattern);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pattern = await _context.LearningPatterns
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        
        if (pattern != null)
        {
            _context.LearningPatterns.Remove(pattern);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IReadOnlyList<LearningPattern>> GetMostUsedPatternsAsync(string userId, int limit = 10, CancellationToken cancellationToken = default)
    {
        return await _context.LearningPatterns
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.UsageCount)
            .ThenByDescending(p => p.ConfidenceScore)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}
