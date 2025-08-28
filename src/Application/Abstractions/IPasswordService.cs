using Application.DTOs.Customers;

namespace Application.Abstractions;

public interface IPasswordService
{
    Task<bool> ChangePasswordAsync(ChangePasswordRequest request, long userId);
    Task<bool> ValidateCurrentPasswordAsync(string currentPassword, long userId);
    Task<bool> IsPasswordValidAsync(string password);
}
