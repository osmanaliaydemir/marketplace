using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog.Sinks.Elasticsearch;

namespace Infrastructure.Logging;

public static class SerilogRegistration
{
    public static IHostBuilder UseSerilogConfiguration(this IHostBuilder builder)
    {
        return builder.UseSerilog((context, services, configuration) =>
        {
            var logConfig = context.Configuration.GetSection("Serilog");
            
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .Enrich.WithCorrelationId()
                .WriteTo.Console(
                    theme: AnsiConsoleTheme.Code,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/marketplace-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.Seq(
                    serverUrl: logConfig["Seq:ServerUrl"] ?? "http://localhost:5341",
                    apiKey: logConfig["Seq:ApiKey"])
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(logConfig["Elasticsearch:DefaultNode"] ?? "http://localhost:9200"))
                {
                    IndexFormat = logConfig["Elasticsearch:IndexFormat"] ?? "marketplace-logs-{0:yyyy.MM.dd}",
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7
                })
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning);
        });
    }

    public static IHostBuilder UseSerilogConfigurationForDevelopment(this IHostBuilder builder)
    {
        return builder.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .WriteTo.Console(
                    theme: AnsiConsoleTheme.Code,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/marketplace-dev-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Information);
        });
    }

    public static IHostBuilder UseSerilogConfigurationForProduction(this IHostBuilder builder)
    {
        return builder.UseSerilog((context, services, configuration) =>
        {
            var logConfig = context.Configuration.GetSection("Serilog");
            
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .Enrich.WithCorrelationId()
                .WriteTo.Console(
                    theme: AnsiConsoleTheme.Code,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/marketplace-prod-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 90,
                    fileSizeLimitBytes: 10485760, // 10MB
                    rollOnFileSizeLimit: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.Seq(
                    serverUrl: logConfig["Seq:ServerUrl"] ?? "http://seq:5341",
                    apiKey: logConfig["Seq:ApiKey"])
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(logConfig["Elasticsearch:DefaultNode"] ?? "http://elasticsearch:9200"))
                {
                    IndexFormat = logConfig["Elasticsearch:IndexFormat"] ?? "marketplace-logs-{0:yyyy.MM.dd}",
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7
                })
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning);
        });
    }

    public static IHostBuilder UseSerilogConfigurationForDocker(this IHostBuilder builder)
    {
        return builder.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .WriteTo.Console(
                    theme: AnsiConsoleTheme.Code,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.Seq(
                    serverUrl: Environment.GetEnvironmentVariable("SEQ_SERVER_URL") ?? "http://seq:5341",
                    apiKey: Environment.GetEnvironmentVariable("SEQ_API_KEY"))
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning);
        });
    }
}
