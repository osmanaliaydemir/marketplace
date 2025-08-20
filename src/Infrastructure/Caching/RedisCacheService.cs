using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<bool> DeleteAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task<bool> ExpireAsync(string key, TimeSpan expiry);
    Task<long> IncrementAsync(string key, long value = 1);
    Task<bool> SetHashAsync(string key, string field, object value);
    Task<T?> GetHashAsync<T>(string key, string field);
    Task<Dictionary<string, object>> GetHashAllAsync(string key);
    Task<bool> DeleteHashAsync(string key, string field);
}

public sealed class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly IDatabase _database;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _logger = logger;
        _database = redis.GetDatabase();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var value = await _database.StringGetAsync(key);
            if (!value.HasValue)
            {
                _logger.LogDebug("Cache miss for key: {Key}", key);
                return default;
            }

            var result = JsonSerializer.Deserialize<T>(value!, _jsonOptions);
            _logger.LogDebug("Cache hit for key: {Key}", key);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting value from cache for key: {Key}", key);
            return default;
        }
    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        try
        {
            var jsonValue = JsonSerializer.Serialize(value, _jsonOptions);
            var result = await _database.StringSetAsync(key, jsonValue, expiry);
            
            if (result)
            {
                _logger.LogDebug("Successfully cached value for key: {Key} with expiry: {Expiry}", key, expiry);
            }
            else
            {
                _logger.LogWarning("Failed to cache value for key: {Key}", key);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting value in cache for key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(string key)
    {
        try
        {
            var result = await _database.KeyDeleteAsync(key);
            if (result)
            {
                _logger.LogDebug("Successfully deleted cache key: {Key}", key);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting cache key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            return await _database.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of cache key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> ExpireAsync(string key, TimeSpan expiry)
    {
        try
        {
            var result = await _database.KeyExpireAsync(key, expiry);
            if (result)
            {
                _logger.LogDebug("Successfully set expiry for key: {Key} to {Expiry}", key, expiry);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting expiry for cache key: {Key}", key);
            return false;
        }
    }

    public async Task<long> IncrementAsync(string key, long value = 1)
    {
        try
        {
            var result = await _database.StringIncrementAsync(key, value);
            _logger.LogDebug("Successfully incremented key: {Key} by {Value} to {Result}", key, value, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing cache key: {Key}", key);
            return 0;
        }
    }

    public async Task<bool> SetHashAsync(string key, string field, object value)
    {
        try
        {
            var jsonValue = JsonSerializer.Serialize(value, _jsonOptions);
            var result = await _database.HashSetAsync(key, field, jsonValue);
            
            if (result)
            {
                _logger.LogDebug("Successfully set hash field: {Field} for key: {Key}", field, key);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting hash field: {Field} for key: {Key}", field, key);
            return false;
        }
    }

    public async Task<T?> GetHashAsync<T>(string key, string field)
    {
        try
        {
            var value = await _database.HashGetAsync(key, field);
            if (!value.HasValue)
            {
                _logger.LogDebug("Hash field not found: {Field} for key: {Key}", field, key);
                return default;
            }

            var result = JsonSerializer.Deserialize<T>(value!, _jsonOptions);
            _logger.LogDebug("Successfully retrieved hash field: {Field} for key: {Key}", field, key);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hash field: {Field} for key: {Key}", field, key);
            return default;
        }
    }

    public async Task<Dictionary<string, object>> GetHashAllAsync(string key)
    {
        try
        {
            var hashEntries = await _database.HashGetAllAsync(key);
            var result = new Dictionary<string, object>();

            foreach (var entry in hashEntries)
            {
                if (entry.Value.HasValue)
                {
                    try
                    {
                        var deserialized = JsonSerializer.Deserialize<object>(entry.Value!, _jsonOptions);
                        result[entry.Name!] = deserialized ?? string.Empty;
                    }
                    catch
                    {
                        result[entry.Name!] = entry.Value.ToString() ?? string.Empty;
                    }
                }
            }

            _logger.LogDebug("Successfully retrieved all hash fields for key: {Key}", key);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all hash fields for key: {Key}", key);
            return new Dictionary<string, object>();
        }
    }

    public async Task<bool> DeleteHashAsync(string key, string field)
    {
        try
        {
            var result = await _database.HashDeleteAsync(key, field);
            if (result)
            {
                _logger.LogDebug("Successfully deleted hash field: {Field} for key: {Key}", field, key);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting hash field: {Field} for key: {Key}", field, key);
            return false;
        }
    }

    // Utility methods
    public async Task<bool> SetWithSlidingExpiryAsync<T>(string key, T value, TimeSpan expiry)
    {
        try
        {
            var success = await SetAsync(key, value, expiry);
            if (success)
            {
                // Set up a background task to extend expiry on access
                _ = Task.Run(async () =>
                {
                    while (await ExistsAsync(key))
                    {
                        await Task.Delay(expiry.Subtract(TimeSpan.FromMinutes(1)));
                        if (await ExistsAsync(key))
                        {
                            await ExpireAsync(key, expiry);
                        }
                    }
                });
            }
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting sliding expiry for key: {Key}", key);
            return false;
        }
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
    {
        var cached = await GetAsync<T>(key);
        if (cached != null)
        {
            return cached;
        }

        try
        {
            var value = await factory();
            await SetAsync(key, value, expiry);
            return value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetOrSet for key: {Key}", key);
            return default;
        }
    }
}
