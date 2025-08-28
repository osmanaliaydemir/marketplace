using Domain.Entities;

namespace Application.Abstractions;

public interface IPasswordResetRepository
{
    Task<PasswordReset?> GetByTokenAsync(string token);
    Task<PasswordReset?> GetByEmailAsync(string email);
    Task<PasswordReset> CreateAsync(PasswordReset passwordReset);
    Task<PasswordReset> UpdateAsync(PasswordReset passwordReset);
    Task<bool> DeleteAsync(long id);
    Task<bool> DeleteExpiredAsync();
    Task<List<PasswordReset>> GetByEmailAndNotUsedAsync(string email);
}
