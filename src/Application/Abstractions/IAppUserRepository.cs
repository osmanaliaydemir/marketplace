using Domain.Entities;

namespace Application.Abstractions;

public interface IAppUserRepository : IAuditableRepository<AppUser>
{
    Task<AppUser?> GetByEmailAsync(string email);
    Task<AppUser?> GetByUsernameAsync(string username);
    Task<AppUser?> GetByEmailOrUsernameAsync(string emailOrUsername);
    Task<bool> IsEmailUniqueAsync(string email, long? excludeUserId = null);
    Task<bool> IsUsernameUniqueAsync(string username, long? excludeUserId = null);
}
