using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Services;
using Application.DTOs.Users;

namespace Web.Pages;

public sealed class CustomerRegisterModel : PageModel
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<CustomerRegisterModel> _logger;

    public CustomerRegisterModel(ApiClient apiClient, ILogger<CustomerRegisterModel> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public void OnGet()
    {
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

        var request = new UserRegisterRequest
        {
            FirstName = Input.FirstName,
            LastName = Input.LastName,
            Email = Input.Email,
            Phone = Input.Phone,
            Password = Input.Password,
            AcceptMarketing = Input.AcceptMarketing
        };

        try
        {
            var response = await _apiClient.PostAsync<UserRegisterRequest, UserRegisterResponse>("/api/auth/register", request);
            
            if (response is null || !response.Success)
            {
                ErrorMessage = response?.Message ?? "Kayıt işlemi başarısız";
                return Page();
            }

            SuccessMessage = "Kayıt işlemi başarılı! Giriş yapabilirsiniz.";
            
            // 3 saniye sonra login sayfasına yönlendir
            Response.Headers.Append("Refresh", "3;url=/giris");
            
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration for email: {Email}", Input.Email);
            ErrorMessage = "Kayıt işlemi sırasında bir hata oluştu. Lütfen tekrar deneyin.";
            return Page();
        }
    }

    public sealed class RegisterInput
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public bool AcceptTerms { get; set; }
        public bool AcceptMarketing { get; set; }
    }
}