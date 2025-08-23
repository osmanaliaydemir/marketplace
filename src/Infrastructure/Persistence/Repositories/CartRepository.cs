using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.Extensions.Logging;
using Dapper;

namespace Infrastructure.Persistence.Repositories;

public sealed class CartRepository : AuditableRepository<Cart>, ICartRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public CartRepository(
        IDbContext context, 
        ILogger<CartRepository> logger, 
        ISqlConnectionFactory connectionFactory) 
        : base(context, logger, null, null)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Cart?> GetByCustomerIdAsync(long customerId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            SELECT c.*, ci.* 
            FROM Carts c 
            LEFT JOIN CartItems ci ON c.Id = ci.CartId 
            WHERE c.CustomerId = @CustomerId AND c.IsActive = 1";
        
        var cartDictionary = new Dictionary<long, Cart>();
        
        await connection.QueryAsync<Cart, CartItem, Cart>(sql, (cart, item) =>
        {
            if (!cartDictionary.TryGetValue(cart.Id, out var existingCart))
            {
                existingCart = cart;
                existingCart.Items = new List<CartItem>();
                cartDictionary.Add(cart.Id, existingCart);
            }
            
            if (item != null)
                existingCart.Items.Add(item);
                
            return existingCart;
        }, new { CustomerId = customerId }, splitOn: "Id");
        
        return cartDictionary.Values.FirstOrDefault();
    }

    public async Task<Cart?> GetBySessionIdAsync(string sessionId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            SELECT c.*, ci.* 
            FROM Carts c 
            LEFT JOIN CartItems ci ON c.Id = ci.CartId 
            WHERE c.SessionId = @SessionId AND c.IsActive = 1";
        
        var cartDictionary = new Dictionary<long, Cart>();
        
        await connection.QueryAsync<Cart, CartItem, Cart>(sql, (cart, item) =>
        {
            if (!cartDictionary.TryGetValue(cart.Id, out var existingCart))
            {
                existingCart = cart;
                existingCart.Items = new List<CartItem>();
                cartDictionary.Add(cart.Id, existingCart);
            }
            
            if (item != null)
                existingCart.Items.Add(item);
                
            return existingCart;
        }, new { SessionId = sessionId }, splitOn: "Id");
        
        return cartDictionary.Values.FirstOrDefault();
    }

    public async Task<IEnumerable<Cart>> GetExpiredCartsAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            SELECT * FROM Carts 
            WHERE ExpiresAt < @Now AND IsActive = 1";
        
        return await connection.QueryAsync<Cart>(sql, new { Now = DateTime.UtcNow });
    }

    public async Task<bool> ClearCustomerCartAsync(long customerId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            // Delete cart items first
            const string deleteItemsSql = @"
                DELETE FROM CartItems 
                WHERE CartId IN (SELECT Id FROM Carts WHERE CustomerId = @CustomerId)";
            
            await connection.ExecuteAsync(deleteItemsSql, new { CustomerId = customerId }, transaction);
            
            // Delete cart
            const string deleteCartSql = @"
                DELETE FROM Carts 
                WHERE CustomerId = @CustomerId";
            
            var rowsAffected = await connection.ExecuteAsync(deleteCartSql, new { CustomerId = customerId }, transaction);
            
            transaction.Commit();
            return rowsAffected > 0;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> UpdateCartExpiryAsync(long cartId, DateTime newExpiry)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            UPDATE Carts 
            SET ExpiresAt = @NewExpiry, ModifiedAt = @ModifiedAt
            WHERE Id = @CartId";
        
        var rowsAffected = await connection.ExecuteAsync(sql, new 
        { 
            CartId = cartId, 
            NewExpiry = newExpiry,
            ModifiedAt = DateTime.UtcNow
        });
        
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<Cart>> GetAbandonedCartsAsync(TimeSpan threshold)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            SELECT * FROM Carts 
            WHERE ModifiedAt < @Threshold AND IsActive = 1 
            AND (ExpiresAt IS NULL OR ExpiresAt > @Now)
            ORDER BY ModifiedAt ASC";
        
        var cutoffTime = DateTime.UtcNow.Subtract(threshold);
        
        return await connection.QueryAsync<Cart>(sql, new 
        { 
            Threshold = cutoffTime,
            Now = DateTime.UtcNow
        });
    }
}
