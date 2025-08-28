using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Abstractions;
using Application.DTOs.Users;
using Microsoft.AspNetCore.Http;

namespace Web.Pages.Auth;

public class ResetPasswordModel : PageModel
{
    private readonly IPasswordResetService _passwordResetService;
    private readonly ILogger<ResetPasswordModel> _logger;

    [BindProperty]
    public string Token { get; set; } = string.Empty;

    [BindProperty]
    public string NewPassword { get; set; } = string.Empty;

    [BindProperty]
    public string ConfirmPassword { get; set; } = string.Empty;

    [BindProperty]
    public bool IsSuccess { get; set; }

    [BindProperty]
    public string Message { get; set; } = string.Empty;

    [BindProperty]
    public string ErrorMessage { get; set; } = string.Empty;

    public ResetPasswordModel(
        IPasswordResetService passwordResetService,
        ILogger<ResetPasswordModel> logger)
    {
        _passwordResetService = passwordResetService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            ErrorMessage = "Geçersiz şifre sıfırlama bağlantısı.";
            return Page();
        }

        Token = token;

        // Validate token
        var isValid = await _passwordResetService.ValidateResetTokenAsync(token);
        if (!isValid)
        {
            ErrorMessage = "Geçersiz veya süresi dolmuş şifre sıfırlama bağlantısı.";
            return Page();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var request = new ResetPasswordRequest
            {
                Token = Token,
                NewPassword = NewPassword,
                ConfirmPassword = ConfirmPassword
            };

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var response = await _passwordResetService.ResetPasswordAsync(request, ipAddress, userAgent);

            if (response.IsSuccess)
            {
                IsSuccess = true;
                Message = response.Message;
                return Page();
            }
            else
            {
                ErrorMessage = response.Message;
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in reset password process for token: {Token}", Token);
            ErrorMessage = "Şifre sıfırlama işlemi sırasında bir hata oluştu. Lütfen tekrar deneyiniz.";
            return Page();
        }
    }
}
