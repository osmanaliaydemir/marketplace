using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Api.Configuration;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy());
        return services;
    }
}
