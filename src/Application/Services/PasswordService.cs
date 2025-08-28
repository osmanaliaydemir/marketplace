using Application.Abstractions;
using Application.DTOs.Customers;
using Application.Exceptions;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services;

public class PasswordService : IPasswordService
{
    private readonly IAppUserService _appUserService;
    private readonly ILogger<PasswordService> _logger;

    public PasswordService(
        IAppUserService appUserService,
        ILogger<PasswordService> logger)
    {
        _appUserService = appUserService;
        _logger = logger;
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request, long userId)
    {
        try
        {
            // Mevcut şifreyi doğrula
            if (!await ValidateCurrentPasswordAsync(request.CurrentPassword, userId))
            {
                throw new BusinessRuleViolationException("PasswordValidation", "Mevcut şifre yanlış");
            }

            // Yeni şifre validasyonu
            if (!await IsPasswordValidAsync(request.NewPassword))
            {
                throw new BusinessRuleViolationException("PasswordSecurity", "Yeni şifre güvenlik gereksinimlerini karşılamıyor");
            }

            // Kullanıcıyı bul
            var user = await _appUserService.GetByIdAsync(userId);
            if (user == null)
            {
                throw new EntityNotFoundException("AppUser", userId, "Kullanıcı bulunamadı");
            }

            // Yeni şifreyi hash'le
            var hashedPassword = HashPassword(request.NewPassword);
            
            // TODO: AppUserService'e UpdatePasswordAsync metodu eklenmeli
            // await _appUserService.UpdatePasswordAsync(userId, hashedPassword);

            _logger.LogInformation("Password changed successfully for user ID: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> ValidateCurrentPasswordAsync(string currentPassword, long userId)
    {
        try
        {
            var user = await _appUserService.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // TODO: User entity'de PasswordHash field'ı yok, eklenmeli
            // var hashedCurrentPassword = HashPassword(currentPassword);
            // return user.PasswordHash == hashedCurrentPassword;
            
            return false; // Geçici olarak false döndür
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating current password for user ID: {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> IsPasswordValidAsync(string password)
    {
        // Şifre güvenlik gereksinimleri
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            return false;
        }

        // En az bir büyük harf
        if (!password.Any(char.IsUpper))
        {
            return false;
        }

        // En az bir küçük harf
        if (!password.Any(char.IsLower))
        {
            return false;
        }

        // En az bir rakam
        if (!password.Any(char.IsDigit))
        {
            return false;
        }

        // En az bir özel karakter
        var specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
        if (!password.Any(c => specialChars.Contains(c)))
        {
            return false;
        }

        return true;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
