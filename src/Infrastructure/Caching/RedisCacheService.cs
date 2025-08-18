using StackExchange.Redis;

namespace Infrastructure.Caching;

public sealed class RedisCacheService
{
    private readonly IConnectionMultiplexer _mux;
    public RedisCacheService(IConnectionMultiplexer mux) => _mux = mux;
}
