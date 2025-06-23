using EmailIntelligence.Domain.Entities;

namespace EmailIntelligence.Domain.Repositories;

public interface IUserToneProfileRepository
{
    Task<UserToneProfile?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(UserToneProfile profile, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserToneProfile profile, CancellationToken cancellationToken = default);
    Task DeleteAsync(string userId, CancellationToken cancellationToken = default);
}
