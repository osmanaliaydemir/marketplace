using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Services;
using Application.DTOs.Users;

namespace Web.Pages.Auth;

public sealed class ForgotPasswordModel : PageModel
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<ForgotPasswordModel> _logger;

    public ForgotPasswordModel(ApiClient apiClient, ILogger<ForgotPasswordModel> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    [BindProperty]
    public ForgotPasswordInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public bool IsEmailSent { get; set; }
    public string Email { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var request = new ForgotPasswordRequest
        {
            Email = Input.Email
        };

        try
        {
            var response = await _apiClient.PostAsync<ForgotPasswordRequest, ForgotPasswordResponse>("/api/auth/forgot-password", request);
            
            if (response is null || !response.Success)
            {
                ErrorMessage = response?.Message ?? "Şifre sıfırlama işlemi başarısız";
                return Page();
            }

            IsEmailSent = true;
            Email = Input.Email;
            SuccessMessage = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.";
            
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password for email: {Email}", Input.Email);
            ErrorMessage = "Şifre sıfırlama işlemi sırasında bir hata oluştu. Lütfen tekrar deneyin.";
            return Page();
        }
    }

    public sealed class ForgotPasswordInput
    {
        public string Email { get; set; } = string.Empty;
    }
}