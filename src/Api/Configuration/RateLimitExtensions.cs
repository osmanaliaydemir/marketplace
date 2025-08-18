namespace Api.Configuration;

public static class RateLimitExtensions
{
    public static IServiceCollection AddBasicRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(o => { o.RejectionStatusCode = StatusCodes.Status429TooManyRequests; });
        return services;
    }
}
