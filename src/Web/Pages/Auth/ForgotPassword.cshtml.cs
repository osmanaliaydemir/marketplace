using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Abstractions;
using Application.DTOs.Users;
using Microsoft.AspNetCore.Http;

namespace Web.Pages.Auth;

public class ForgotPasswordModel : PageModel
{
    private readonly IPasswordResetService _passwordResetService;
    private readonly ILogger<ForgotPasswordModel> _logger;

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public bool IsSuccess { get; set; }

    [BindProperty]
    public string Message { get; set; } = string.Empty;

    [BindProperty]
    public string ErrorMessage { get; set; } = string.Empty;

    public ForgotPasswordModel(
        IPasswordResetService passwordResetService,
        ILogger<ForgotPasswordModel> logger)
    {
        _passwordResetService = passwordResetService;
        _logger = logger;
    }

    public void OnGet()
    {
        // GET request - show form
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var request = new ForgotPasswordRequest
            {
                Email = Email
            };

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var response = await _passwordResetService.ForgotPasswordAsync(request, ipAddress, userAgent);

            if (response.IsSuccess)
            {
                IsSuccess = true;
                Message = response.Message;
                Email = response.Email ?? Email;
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
            _logger.LogError(ex, "Error in forgot password process for email: {Email}", Email);
            ErrorMessage = "Şifre sıfırlama işlemi sırasında bir hata oluştu. Lütfen tekrar deneyiniz.";
            return Page();
        }
    }
}
