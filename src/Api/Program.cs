using Serilog;
using Api.Configuration;
using Api.Middlewares;
using Application.Services;
using Application.Validation;
using Infrastructure;
using Infrastructure.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using FluentValidation;
using System.Reflection;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Dapper column mapping - snake_case to PascalCase
DefaultTypeMap.MatchNamesWithUnderscores = true;

// Serilog Configuration
builder.Host.UseSerilogConfiguration();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddMemoryCache(); // Add memory cache
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

// FluentValidation'ı ekle
builder.Services.AddApplicationValidation();

// Add rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy())
    .AddCheck("database", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Database connection is healthy"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    { 
        Title = "Marketplace API", 
        Version = "v1",
        Description = "E-ticaret marketplace API'si - Ürün yönetimi, sipariş işlemleri, ödeme entegrasyonu ve kullanıcı yönetimi"
    });
    
    // XML documentation dosyalarını ekle
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    
    // Application layer XML documentation
    var applicationXmlFile = Path.Combine(AppContext.BaseDirectory, "Application.xml");
    if (File.Exists(applicationXmlFile))
    {
        c.IncludeXmlComments(applicationXmlFile);
    }
    
    // Security definitions
    c.AddSecurityDefinition("Bearer", new()
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });
    
    c.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Id = "Bearer", Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme } },
            new[] { "auth" }
        }
    });
});

// Services
builder.Services.AddAuthNAuthZ(builder.Configuration);
builder.Services.AddBasicRateLimiting();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddCaching(builder.Configuration);
builder.Services.AddObservability(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Marketplace API v1");
        c.RoutePrefix = string.Empty; // Root'ta Swagger UI'ı göster
        c.DocumentTitle = "Marketplace API Documentation";
        c.DefaultModelsExpandDepth(-1); // Models'i gizle
    });
}

app.UseHttpsRedirection();
app.UseResponseCompression(); // Add response compression
app.UseSerilogRequestLogging();

app.UseRateLimiter(); // Add rate limiting middleware
app.UseAuthentication();
app.UseAuthorization();

// Custom Middlewares - ORDER MATTERS! (Exception handling en son)
app.UseMiddleware<ModelValidationMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});

app.Run();
