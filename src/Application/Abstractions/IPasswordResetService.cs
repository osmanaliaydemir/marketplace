using Application.DTOs.Users;

namespace Application.Abstractions;

public interface IPasswordResetService
{
    Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request, string? ipAddress = null, string? userAgent = null);
    Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request, string? ipAddress = null, string? userAgent = null);
    Task<bool> ValidateResetTokenAsync(string token);
    Task<bool> InvalidateResetTokenAsync(string token);
}
