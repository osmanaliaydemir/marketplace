using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dashboard.Services;

namespace Dashboard.Pages;

public class LoginModel : PageModel
{
    private readonly ApiClient _api;

    public LoginModel(ApiClient api)
    {
        _api = api;
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

        try
        {
            // API'ye giriş isteği gönder
            var loginRequest = new
            {
                emailOrUsername = LoginForm.EmailOrUsername,
                password = LoginForm.Password
            };

            var response = await _api.PostAsync<object, LoginResponse>("/api/auth/login", loginRequest);
            
            if (response?.Success == true)
            {
                // Başarılı giriş - token'ı session'a kaydet
                HttpContext.Session.SetString("AuthToken", response.Token);
                HttpContext.Session.SetString("UserRole", response.User.Role);
                HttpContext.Session.SetString("UserName", response.User.FullName);
                HttpContext.Session.SetString("UserId", response.User.Id.ToString());
                
                // Dashboard ana sayfasına yönlendir
                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = response?.Message ?? "Giriş başarısız. Lütfen bilgilerinizi kontrol edin.";
                return Page();
            }
        }
        catch (Exception ex)
        {
            // Geliştirme modunda dev-login ile test et
            if (LoginForm.EmailOrUsername == "admin@marketplace.local" && LoginForm.Password == "admin123")
            {
                try
                {
                    var devResponse = await _api.PostAsync<object, DevLoginResponse>("/api/auth/dev-login", 
                        new { userId = 1, role = "Admin" });
                    
                    if (devResponse?.access_token != null)
                    {
                        // Dev token ile giriş yap
                        HttpContext.Session.SetString("AuthToken", devResponse.access_token);
                        HttpContext.Session.SetString("UserRole", "Admin");
                        HttpContext.Session.SetString("UserName", "Admin User");
                        HttpContext.Session.SetString("UserId", "1");
                        
                        return RedirectToPage("/Index");
                    }
                }
                catch
                {
                    // Dev login de başarısız
                }
            }
            
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

public sealed class LoginResponse
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public UserInfo User { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public sealed class UserInfo
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class DevLoginResponse 
{ 
    public string access_token { get; set; } = string.Empty; 
}
