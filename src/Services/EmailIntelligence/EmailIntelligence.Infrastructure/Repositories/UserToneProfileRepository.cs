using Microsoft.EntityFrameworkCore;
using EmailIntelligence.Domain.Entities;
using EmailIntelligence.Domain.Repositories;
using EmailIntelligence.Infrastructure.Data;

namespace EmailIntelligence.Infrastructure.Repositories;

public class UserToneProfileRepository : IUserToneProfileRepository
{
    private readonly EmailIntelligenceDbContext _context;

    public UserToneProfileRepository(EmailIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<UserToneProfile?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserToneProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(UserToneProfile profile, CancellationToken cancellationToken = default)
    {
        _context.UserToneProfiles.Add(profile);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UserToneProfile profile, CancellationToken cancellationToken = default)
    {
        _context.UserToneProfiles.Update(profile);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(string userId, CancellationToken cancellationToken = default)
    {
        var profile = await _context.UserToneProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        
        if (profile != null)
        {
            _context.UserToneProfiles.Remove(profile);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
