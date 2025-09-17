using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Api.Configuration;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration cfg)
    {
        // Health checks are already registered in Program.cs
        // No need to register them again here
        return services;
    }
}
