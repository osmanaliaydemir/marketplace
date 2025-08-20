using Serilog;
using Api.Configuration;
using Application.Services;
using Application.Validation;
using Infrastructure;
using Infrastructure.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using FluentValidation;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Dapper column mapping - snake_case to PascalCase
DefaultTypeMap.MatchNamesWithUnderscores = true;

// Serilog Configuration
builder.Host.UseSerilogConfiguration();

builder.Services.AddControllers();

// FluentValidation'ı ekle
builder.Services.AddApplicationValidation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Marketplace API", Version = "v1" });
    
    // XML documentation dosyasını ekle
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    
    c.AddSecurityDefinition("Bearer", new()
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Id = "Bearer", Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme } },
            Array.Empty<string>()
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Marketplace API v1");
        c.RoutePrefix = string.Empty; // Root'ta Swagger UI'ı göster
    });
}

app.UseSerilogRequestLogging();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
