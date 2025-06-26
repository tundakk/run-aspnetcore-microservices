using EmailIntelligence.Domain.Entities;

namespace EmailIntelligence.Domain.Repositories;

public interface ILearningPatternRepository
{
    Task<LearningPattern?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LearningPattern>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LearningPattern>> GetByPatternTypeAsync(string userId, string patternType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LearningPattern>> FindApplicablePatternsAsync(
        string userId, 
        string content, 
        Dictionary<string, object>? context = null,
        CancellationToken cancellationToken = default);
    Task<LearningPattern> AddAsync(LearningPattern pattern, CancellationToken cancellationToken = default);
    Task UpdateAsync(LearningPattern pattern, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LearningPattern>> GetMostUsedPatternsAsync(string userId, int limit = 10, CancellationToken cancellationToken = default);
}
