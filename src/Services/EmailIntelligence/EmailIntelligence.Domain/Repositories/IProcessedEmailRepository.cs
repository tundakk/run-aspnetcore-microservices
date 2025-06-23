using EmailIntelligence.Domain.Entities;
using EmailIntelligence.Domain.Enums;

namespace EmailIntelligence.Domain.Repositories;

public interface IProcessedEmailRepository
{
    Task<ProcessedEmail?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProcessedEmail?> GetByEmailIdAsync(string emailId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProcessedEmail>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProcessedEmail>> GetByPriorityAsync(string userId, EmailPriority priority, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProcessedEmail>> GetByCategoryAsync(string userId, EmailCategory category, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProcessedEmail>> GetRequiringResponseAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProcessedEmail>> GetUserCorrectionsForLearningAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(ProcessedEmail processedEmail, CancellationToken cancellationToken = default);
    Task UpdateAsync(ProcessedEmail processedEmail, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
