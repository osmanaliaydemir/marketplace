using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.Extensions.Logging;
using Dapper;

namespace Infrastructure.Persistence.Repositories;

public sealed class StoreRepository : AuditableRepository<Store>, IStoreRepository
{
    public StoreRepository(
        IDbContext context, 
        ILogger<StoreRepository> logger, 
        ISqlConnectionFactory connectionFactory) 
        : base(context, logger, null, null)
    {
        _connectionFactory = connectionFactory;
    }
    
    private readonly ISqlConnectionFactory _connectionFactory;

    public async Task<Store?> GetBySlugAsync(string slug)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = "SELECT * FROM Stores WHERE Slug = @Slug AND IsActive = 1";
        return await connection.QueryFirstOrDefaultAsync<Store>(sql, new { Slug = slug });
    }

    public async Task<Store?> GetByUserIdAsync(long userId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            SELECT s.* FROM Stores s 
            INNER JOIN Sellers se ON s.SellerId = se.Id 
            WHERE se.UserId = @UserId AND s.IsActive = 1";
        return await connection.QueryFirstOrDefaultAsync<Store>(sql, new { UserId = userId });
    }

    public async Task<IEnumerable<Store>> GetActiveStoresAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = "SELECT * FROM Stores WHERE IsActive = 1 ORDER BY CreatedAt DESC";
        return await connection.QueryAsync<Store>(sql);
    }

    public async Task<IEnumerable<Store>> GetFeaturedStoresAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = "SELECT * FROM Stores WHERE IsFeatured = 1 AND IsActive = 1 ORDER BY DisplayOrder, CreatedAt DESC";
        return await connection.QueryAsync<Store>(sql);
    }

    public async Task<bool> IsSlugUniqueAsync(string slug, long? excludeId = null)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = excludeId.HasValue 
            ? "SELECT COUNT(*) FROM Stores WHERE Slug = @Slug AND Id != @ExcludeId"
            : "SELECT COUNT(*) FROM Stores WHERE Slug = @Slug";
        var count = await connection.QuerySingleAsync<int>(sql, new { Slug = slug, ExcludeId = excludeId });
        return count == 0;
    }
}
