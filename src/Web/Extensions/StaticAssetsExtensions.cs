using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace Web.Extensions;

public static class StaticAssetsExtensions
{
    public static IApplicationBuilder MapStaticAssets(this IApplicationBuilder app)
    {
        app.UseStaticFiles();
        
        // Configure images directory if it exists
        var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        if (Directory.Exists(imagesPath))
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(imagesPath),
                RequestPath = "/images"
            });
        }
        
        // Configure uploads directory if it exists
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (Directory.Exists(uploadsPath))
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsPath),
                RequestPath = "/uploads"
            });
        }
        
        return app;
    }
    
    public static IEndpointRouteBuilder WithStaticAssets(this IEndpointRouteBuilder endpoints)
    {
        // Additional endpoint configuration for static assets if needed
        return endpoints;
    }
}
