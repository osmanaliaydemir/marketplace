using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Dashboard.Middleware;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public AuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Login sayfası ve static dosyalar için authentication kontrolü yapma
        if (IsPublicPath(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // Session'dan token kontrolü
        
        var token = context.Session.GetString("AuthToken");
        var userRole = context.Session.GetString("UserRole");

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userRole))
        {
            // Oturum yok, login'e yönlendir
            context.Response.Redirect("/Login");
            return;
        }

        // Admin sayfaları için Admin rolü kontrolü
        if (IsAdminPath(context.Request.Path) && userRole != "Admin")
        {
            context.Response.Redirect("/Login?error=unauthorized");
            return;
        }

        await _next(context);
    }

    private static bool IsPublicPath(PathString path)
    {
        var publicPaths = new[]
        {
            "/Login",
            "/lib",
            "/css",
            "/js",
            "/images",
            "/favicon.ico"
        };

        return publicPaths.Any(publicPath => path.StartsWithSegments(publicPath));
    }

    private static bool IsAdminPath(PathString path)
    {
        var adminPaths = new[]
        {
            "/StoreApplications",
            "/Stores",
            "/Sellers",
            "/Products",
            "/Orders",
            "/Payments",
            "/Reports",
            "/Settings"
        };

        return adminPaths.Any(adminPath => path.StartsWithSegments(adminPath));
    }
}
