using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Dashboard.Services;
using Application.DTOs.Users;

namespace Dashboard.Pages;

public class LoginModel : PageModel
{
    private readonly ApiClient _api;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(ApiClient api, ILogger<LoginModel> logger)
    {
        _api = api;
        _logger = logger;
    }

    [BindProperty]
    public LoginForm LoginForm { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public void OnGet(string? error)
    {
        if (error == "unauthorized")
        {
            ErrorMessage = "Bu sayfaya erişim yetkiniz bulunmamaktadır.";
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var request = new UserLoginRequest
        {
            EmailOrUsername = LoginForm.EmailOrUsername,
            Password = LoginForm.Password
        };

        try
        {
            var response = await _api.PostAsync<UserLoginRequest, UserLoginResponse>("/api/auth/login", request);
            
            if (response?.Success == true && response.User.Role == "Admin")
            {
                // API token'ı cookie'de sakla (HttpOnly)
                Response.Cookies.Append("API_TOKEN", response.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = HttpContext.Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(8)
                });

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, response.User.Id.ToString()),
                    new Claim(ClaimTypes.Email, response.User.Email),
                    new Claim(ClaimTypes.Name, response.User.FullName),
                    new Claim(ClaimTypes.Role, response.User.Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = response?.Message ?? "Giriş başarısız. Lütfen bilgilerinizi kontrol edin.";
                if (response?.User.Role != "Admin")
                {
                    ErrorMessage = "Bu panele erişim yetkiniz bulunmamaktadır.";
                }
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during admin login for email: {Email}", LoginForm.EmailOrUsername);
            ErrorMessage = "Giriş yapılamadı. Lütfen daha sonra tekrar deneyin.";
            return Page();
        }
    }
}

public sealed class LoginForm
{
    public string EmailOrUsername { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
