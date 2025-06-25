using Microsoft.EntityFrameworkCore;
using EmailIntelligence.Domain.Entities;
using EmailIntelligence.Domain.Enums;
using EmailIntelligence.Domain.Repositories;
using EmailIntelligence.Infrastructure.Data;

namespace EmailIntelligence.Infrastructure.Repositories;

public class EmailDraftRepository : IEmailDraftRepository
{
    private readonly EmailIntelligenceDbContext _context;

    public EmailDraftRepository(EmailIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<EmailDraft?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.EmailDrafts
            .Include(d => d.ProcessedEmail)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<EmailDraft>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.EmailDrafts
            .Include(d => d.ProcessedEmail)
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<EmailDraft>> GetByStatusAsync(string userId, DraftStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.EmailDrafts
            .Include(d => d.ProcessedEmail)
            .Where(d => d.UserId == userId && d.Status == status)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<EmailDraft>> GetUserEditedDraftsForLearningAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.EmailDrafts
            .Include(d => d.ProcessedEmail)
            .Where(d => d.UserId == userId && 
                       d.UserEditedContent != null && 
                       d.UserEditCount > 0)
            .OrderByDescending(d => d.EditedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<EmailDraft?> GetByProcessedEmailIdAsync(Guid processedEmailId, CancellationToken cancellationToken = default)
    {
        return await _context.EmailDrafts
            .Include(d => d.ProcessedEmail)
            .FirstOrDefaultAsync(d => d.ProcessedEmailId == processedEmailId, cancellationToken);
    }

    public async Task AddAsync(EmailDraft draft, CancellationToken cancellationToken = default)
    {
        _context.EmailDrafts.Add(draft);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(EmailDraft draft, CancellationToken cancellationToken = default)
    {
        _context.EmailDrafts.Update(draft);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var draft = await _context.EmailDrafts.FindAsync(new object[] { id }, cancellationToken);
        if (draft != null)
        {
            _context.EmailDrafts.Remove(draft);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
