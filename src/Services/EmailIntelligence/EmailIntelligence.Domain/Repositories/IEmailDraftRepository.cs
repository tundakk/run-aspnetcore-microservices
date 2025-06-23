using EmailIntelligence.Domain.Entities;
using EmailIntelligence.Domain.Enums;

namespace EmailIntelligence.Domain.Repositories;

public interface IEmailDraftRepository
{
    Task<EmailDraft?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmailDraft>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmailDraft>> GetByStatusAsync(string userId, DraftStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmailDraft>> GetUserEditedDraftsForLearningAsync(string userId, CancellationToken cancellationToken = default);
    Task<EmailDraft?> GetByProcessedEmailIdAsync(Guid processedEmailId, CancellationToken cancellationToken = default);
    Task AddAsync(EmailDraft draft, CancellationToken cancellationToken = default);
    Task UpdateAsync(EmailDraft draft, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
