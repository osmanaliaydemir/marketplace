using StackExchange.Redis;

namespace Api.Configuration;

public static class CachingExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration cfg)
    {
        var cs = cfg.GetConnectionString("Redis") ?? "localhost:6379";
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(cs));
        return services;
    }
}
