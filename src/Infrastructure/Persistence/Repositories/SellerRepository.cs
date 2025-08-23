using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.Extensions.Logging;
using Dapper;

namespace Infrastructure.Persistence.Repositories;

public sealed class SellerRepository : AuditableRepository<Seller>, ISellerRepository
{
    public SellerRepository(
        IDbContext context, 
        ILogger<SellerRepository> logger, 
        ISqlConnectionFactory connectionFactory) 
        : base(context, logger, null, null)
    {
        _connectionFactory = connectionFactory;
    }
    
    private readonly ISqlConnectionFactory _connectionFactory;

    public async Task<Seller?> GetByUserIdAsync(long userId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = "SELECT * FROM Sellers WHERE UserId = @UserId AND IsActive = 1";
        return await connection.QueryFirstOrDefaultAsync<Seller>(sql, new { UserId = userId });
    }

    public async Task<Seller?> GetByStoreIdAsync(long storeId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = "SELECT * FROM Sellers WHERE Id = (SELECT SellerId FROM Stores WHERE Id = @StoreId) AND IsActive = 1";
        return await connection.QueryFirstOrDefaultAsync<Seller>(sql, new { StoreId = storeId });
    }

    public async Task<IEnumerable<Seller>> GetActiveSellersAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = "SELECT * FROM Sellers WHERE IsActive = 1 ORDER BY CreatedAt DESC";
        return await connection.QueryAsync<Seller>(sql);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, long? excludeId = null)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = excludeId.HasValue 
            ? "SELECT COUNT(*) FROM Sellers WHERE Email = @Email AND Id != @ExcludeId"
            : "SELECT COUNT(*) FROM Sellers WHERE Email = @Email";
        var count = await connection.QuerySingleAsync<int>(sql, new { Email = email, ExcludeId = excludeId });
        return count == 0;
    }
}
