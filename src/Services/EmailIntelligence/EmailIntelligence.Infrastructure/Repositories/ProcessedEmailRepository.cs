using Microsoft.EntityFrameworkCore;
using EmailIntelligence.Domain.Entities;
using EmailIntelligence.Domain.Repositories;
using EmailIntelligence.Domain.Enums;
using EmailIntelligence.Infrastructure.Data;

namespace EmailIntelligence.Infrastructure.Repositories;

public class ProcessedEmailRepository : IProcessedEmailRepository
{
    private readonly EmailIntelligenceDbContext _context;

    public ProcessedEmailRepository(EmailIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<ProcessedEmail?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ProcessedEmails.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<ProcessedEmail?> GetByEmailIdAsync(string emailId, CancellationToken cancellationToken = default)
    {
        return await _context.ProcessedEmails
            .FirstOrDefaultAsync(e => e.EmailId == emailId, cancellationToken);
    }

    public async Task<IEnumerable<ProcessedEmail>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.ProcessedEmails
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.ReceivedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProcessedEmail>> GetByPriorityAsync(string userId, EmailPriority priority, CancellationToken cancellationToken = default)
    {
        return await _context.ProcessedEmails
            .Where(e => e.UserId == userId && e.Priority == priority)
            .OrderByDescending(e => e.ReceivedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProcessedEmail>> GetByCategoryAsync(string userId, EmailCategory category, CancellationToken cancellationToken = default)
    {
        return await _context.ProcessedEmails
            .Where(e => e.UserId == userId && e.Category == category)
            .OrderByDescending(e => e.ReceivedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProcessedEmail>> GetRequiringResponseAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.ProcessedEmails
            .Where(e => e.UserId == userId && e.RequiresResponse)
            .OrderByDescending(e => e.ReceivedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProcessedEmail>> GetUserCorrectionsForLearningAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.ProcessedEmails
            .Where(e => e.UserId == userId && (e.UserCorrectedPriority || e.UserCorrectedCategory))
            .OrderByDescending(e => e.ReceivedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ProcessedEmail email, CancellationToken cancellationToken = default)
    {
        _context.ProcessedEmails.Add(email);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ProcessedEmail email, CancellationToken cancellationToken = default)
    {
        _context.ProcessedEmails.Update(email);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var email = await GetByIdAsync(id, cancellationToken);
        if (email != null)
        {
            _context.ProcessedEmails.Remove(email);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
