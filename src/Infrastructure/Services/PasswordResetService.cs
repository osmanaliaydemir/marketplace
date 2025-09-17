using Application.Abstractions;
using Application.DTOs.Users;
using Application.Exceptions;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services;

public sealed class PasswordResetService : IPasswordResetService
{
    private readonly IAppUserService _appUserService;
    private readonly IPasswordResetRepository _passwordResetRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<PasswordResetService> _logger;

    public PasswordResetService(
        IAppUserService appUserService,
        IPasswordResetRepository passwordResetRepository,
        IEmailService emailService,
        ILogger<PasswordResetService> logger)
    {
        _appUserService = appUserService;
        _passwordResetRepository = passwordResetRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request, string? ipAddress = null, string? userAgent = null)
    {
        try
        {
            _logger.LogInformation("Password reset requested for email: {Email}", request.Email);

            // Check if user exists
            var user = await _appUserService.GetByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal if user exists or not for security
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Email);
                return new ForgotPasswordResponse
                {
                    Success = true,
                    Message = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi. E-posta kutunuzu kontrol ediniz.",
                    Email = request.Email
                };
            }

            // Generate reset token
            var token = GenerateResetToken();
            var expiresAt = DateTime.UtcNow.AddHours(24); // 24 saat geçerli

            // Save reset token
            var passwordReset = new PasswordReset
            {
                Email = request.Email,
                Token = token,
                ExpiresAt = expiresAt,
                IsUsed = false,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                CreatedAt = DateTime.UtcNow
            };

            await _passwordResetRepository.CreateAsync(passwordReset);

            // Send email
            var resetLink = $"/reset-password?token={token}";
            var emailSubject = "Şifre Sıfırlama";
            var emailBody = GenerateResetPasswordEmail(user.FullName ?? "Kullanıcı", resetLink);

            await _emailService.SendEmailAsync(request.Email, emailSubject, emailBody, true);

            _logger.LogInformation("Password reset email sent successfully for: {Email}", request.Email);

            return new ForgotPasswordResponse
            {
                Success = true,
                Message = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi. E-posta kutunuzu kontrol ediniz.",
                Email = request.Email
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in forgot password process for email: {Email}", request.Email);
            throw new BusinessRuleViolationException("PasswordReset", "Şifre sıfırlama işlemi sırasında bir hata oluştu", ex);
        }
    }

    public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request, string? ipAddress = null, string? userAgent = null)
    {
        try
        {
            _logger.LogInformation("Password reset attempt with token: {Token}", request.Token);

            // Validate token
            var isValid = await ValidateResetTokenAsync(request.Token);
            if (!isValid)
            {
                return new ResetPasswordResponse
                {
                    Success = false,
                    Message = "Geçersiz veya süresi dolmuş şifre sıfırlama bağlantısı."
                };
            }

            // Get password reset record
            var passwordReset = await _passwordResetRepository.GetByTokenAsync(request.Token);
            if (passwordReset == null)
            {
                return new ResetPasswordResponse
                {
                    Success = false,
                    Message = "Geçersiz şifre sıfırlama bağlantısı."
                };
            }

            // Check if already used
            if (passwordReset.IsUsed)
            {
                return new ResetPasswordResponse
                {
                    Success = false,
                    Message = "Bu şifre sıfırlama bağlantısı daha önce kullanılmış."
                };
            }

            // Check if expired
            if (passwordReset.ExpiresAt < DateTime.UtcNow)
            {
                return new ResetPasswordResponse
                {
                    Success = false,
                    Message = "Şifre sıfırlama bağlantısının süresi dolmuş."
                };
            }

            // Get user
            var user = await _appUserService.GetByEmailAsync(passwordReset.Email);
            if (user == null)
            {
                return new ResetPasswordResponse
                {
                    Success = false,
                    Message = "Kullanıcı bulunamadı."
                };
            }

            // Update password
            var newPasswordHash = HashPassword(request.Password);
            await _appUserService.UpdatePasswordAsync(user.Id, newPasswordHash);

            // Mark token as used
            passwordReset.IsUsed = true;
            passwordReset.UsedAt = DateTime.UtcNow;
            passwordReset.ModifiedAt = DateTime.UtcNow;
            await _passwordResetRepository.UpdateAsync(passwordReset);

            // Send confirmation email
            var emailSubject = "Şifreniz Başarıyla Değiştirildi";
            var emailBody = GeneratePasswordChangedEmail(user.FullName ?? "Kullanıcı");

            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody, true);

            _logger.LogInformation("Password reset completed successfully for user: {UserId}", user.Id);

            return new ResetPasswordResponse
            {
                Success = true,
                Message = "Şifreniz başarıyla değiştirildi. Yeni şifrenizle giriş yapabilirsiniz.",
                Email = user.Email
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in reset password process for token: {Token}", request.Token);
            throw new BusinessRuleViolationException("PasswordReset", "Şifre sıfırlama işlemi sırasında bir hata oluştu", ex);
        }
    }

    public async Task<bool> ValidateResetTokenAsync(string token)
    {
        try
        {
            var passwordReset = await _passwordResetRepository.GetByTokenAsync(token);
            if (passwordReset == null)
                return false;

            if (passwordReset.IsUsed)
                return false;

            if (passwordReset.ExpiresAt < DateTime.UtcNow)
                return false;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating reset token: {Token}", token);
            return false;
        }
    }

    public async Task<bool> InvalidateResetTokenAsync(string token)
    {
        try
        {
            var passwordReset = await _passwordResetRepository.GetByTokenAsync(token);
            if (passwordReset == null)
                return false;

            passwordReset.IsUsed = true;
            passwordReset.ModifiedAt = DateTime.UtcNow;
            await _passwordResetRepository.UpdateAsync(passwordReset);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating reset token: {Token}", token);
            throw;
        }
    }

    #region Private Helper Methods

    private string GenerateResetToken()
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private string GenerateResetPasswordEmail(string userName, string resetLink)
    {
        return $@"
            <html>
            <body>
                <h2>Merhaba {userName},</h2>
                <p>Şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayınız:</p>
                <p><a href='{resetLink}'>Şifremi Sıfırla</a></p>
                <p>Bu bağlantı 24 saat geçerlidir.</p>
                <p>Eğer bu işlemi siz yapmadıysanız, bu e-postayı görmezden gelebilirsiniz.</p>
                <br>
                <p>Saygılarımızla,<br>Marketplace Ekibi</p>
            </body>
            </html>";
    }

    private string GeneratePasswordChangedEmail(string userName)
    {
        return $@"
            <html>
            <body>
                <h2>Merhaba {userName},</h2>
                <p>Şifreniz başarıyla değiştirildi.</p>
                <p>Eğer bu işlemi siz yapmadıysanız, lütfen hemen bizimle iletişime geçiniz.</p>
                <br>
                <p>Saygılarımızla,<br>Marketplace Ekibi</p>
            </body>
            </html>";
    }

    #endregion
}
