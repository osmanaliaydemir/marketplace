using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Services;
using Application.DTOs.Users;

namespace Web.Pages.Auth;

public sealed class ResetPasswordModel : PageModel
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<ResetPasswordModel> _logger;

    public ResetPasswordModel(ApiClient apiClient, ILogger<ResetPasswordModel> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    [BindProperty]
    public ResetPasswordInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public bool IsPasswordReset { get; set; }

    public void OnGet(string? token)
    {
        if (string.IsNullOrEmpty(token))
        {
            ErrorMessage = "Geçersiz şifre sıfırlama bağlantısı";
            return;
        }

        Input.Token = token;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Input.Password != Input.ConfirmPassword)
        {
            ModelState.AddModelError(nameof(Input.ConfirmPassword), "Şifreler eşleşmiyor");
            return Page();
        }

        var request = new ResetPasswordRequest
        {
            Token = Input.Token,
            Password = Input.Password
        };

        try
        {
            var response = await _apiClient.PostAsync<ResetPasswordRequest, ResetPasswordResponse>("/api/auth/reset-password", request);
            
            if (response is null || !response.Success)
            {
                ErrorMessage = response?.Message ?? "Şifre sıfırlama işlemi başarısız";
                return Page();
            }

            IsPasswordReset = true;
            SuccessMessage = "Şifreniz başarıyla sıfırlandı.";
            
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset for token: {Token}", Input.Token);
            ErrorMessage = "Şifre sıfırlama işlemi sırasında bir hata oluştu. Lütfen tekrar deneyin.";
            return Page();
        }
    }

    public sealed class ResetPasswordInput
    {
        public string Token { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}