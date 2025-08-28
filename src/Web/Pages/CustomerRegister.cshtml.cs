using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Services;
using Application.DTOs.Users;
using System.Security.Claims;

namespace Web.Pages;

public sealed class CustomerRegisterModel : PageModel
{
    private readonly ApiClient _apiClient;

    public CustomerRegisterModel(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

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
            ErrorMessage = "Şifreler uyuşmuyor";
            return Page();
        }

        var req = new CreateUserRequest
        {
            Email = Input.Email,
            FullName = Input.FullName,
            Password = Input.Password,
            Role = "Customer" // API zaten Customer'a zorluyor; yine de açıkça belirtiyoruz
        };

        try
        {
            var res = await _apiClient.PostAsync<CreateUserRequest, object>("/api/auth/register", req);
            if (res is null)
            {
                ErrorMessage = "Kayıt başarısız";
                return Page();
            }

            // Kayıt sonrası otomatik giriş akışı yerine login sayfasına yönlendirelim
            return Redirect("/giris");
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }

    public sealed class RegisterInput
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
