using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Services;
using Application.DTOs.Users;

namespace Web.Pages;

public sealed class LoginModel : PageModel
{
    private readonly ApiClient _apiClient;

    public LoginModel(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [BindProperty]
    public LoginInput Input { get; set; } = new();

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

        var req = new UserLoginRequest
        {
            EmailOrUsername = Input.EmailOrUsername,
            Password = Input.Password
        };

        try
        {
            var res = await _apiClient.PostAsync<UserLoginRequest, UserLoginResponse>("/api/auth/login", req);
            if (res is null || !res.Success)
            {
                ErrorMessage = res?.Message ?? "Giriş başarısız";
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, res.User.Id.ToString()),
                new Claim(ClaimTypes.Email, res.User.Email),
                new Claim(ClaimTypes.Name, res.User.FullName),
                new Claim(ClaimTypes.Role, res.User.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true
            });

            if (string.Equals(res.User.Role, "Seller", StringComparison.OrdinalIgnoreCase))
            {
                return Redirect("/satici/panel");
            }

            return Redirect("/hesabim");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }

    public sealed class LoginInput
    {
        public string EmailOrUsername { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
