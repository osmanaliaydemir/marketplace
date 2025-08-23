using Application.Abstractions;
using Dapper;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Infrastructure.Persistence.Repositories;

public sealed class InventoryRepository : Repository<Inventory>, IInventoryRepository
{
    private readonly ILogger<InventoryRepository> _logger;

    public InventoryRepository(
        IDbContext context, 
        ILogger<InventoryRepository> logger, 
        ISqlConnectionFactory connectionFactory) 
        : base(context, logger, null, null)
    {
        _logger = logger;
        _connectionFactory = connectionFactory;
    }
    
    private readonly ISqlConnectionFactory _connectionFactory;

    public async Task<Inventory?> GetByProductIdAsync(long productId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Inventory 
                WHERE ProductId = @ProductId AND IsActive = 1";
            
            var inventory = await connection.QueryFirstOrDefaultAsync<Inventory>(sql, new { ProductId = productId });
            
            _logger.LogInformation("Retrieved inventory for product {ProductId}", productId);
            return inventory;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory for product {ProductId}", productId);
            throw;
        }
    }

    public async Task<IEnumerable<Inventory>> GetLowStockAsync(int threshold = 10)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Inventory 
                WHERE (StockQty - ReservedQty) <= @Threshold 
                AND IsActive = 1
                ORDER BY (StockQty - ReservedQty) ASC";
            
            var inventories = await connection.QueryAsync<Inventory>(sql, new { Threshold = threshold });
            
            _logger.LogInformation("Retrieved {Count} low stock items with threshold {Threshold}", inventories.Count(), threshold);
            return inventories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting low stock items with threshold {Threshold}", threshold);
            throw;
        }
    }

    public async Task<IEnumerable<Inventory>> GetOutOfStockAsync()
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Inventory 
                WHERE (StockQty - ReservedQty) <= 0 
                AND IsActive = 1
                ORDER BY LastUpdatedAt DESC";
            
            var inventories = await connection.QueryAsync<Inventory>(sql);
            
            _logger.LogInformation("Retrieved {Count} out of stock items", inventories.Count());
            return inventories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting out of stock items");
            throw;
        }
    }

    public async Task<bool> UpdateStockAsync(long productId, int newStock)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE Inventory 
                SET StockQty = @NewStock, 
                    LastUpdatedAt = @LastUpdatedAt
                WHERE ProductId = @ProductId";
            
            var rowsAffected = await connection.ExecuteAsync(sql, new 
            { 
                NewStock = newStock, 
                ProductId = productId,
                LastUpdatedAt = DateTime.UtcNow
            });
            
            var success = rowsAffected > 0;
            _logger.LogInformation("Updated stock for product {ProductId} to {NewStock}. Success: {Success}", 
                productId, newStock, success);
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock for product {ProductId} to {NewStock}", productId, newStock);
            throw;
        }
    }

    public async Task<bool> ReserveStockAsync(long productId, int quantity)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            // Transaction kullanarak atomik işlem yapalım
            using var transaction = connection.BeginTransaction();
            
            try
            {
                // Önce mevcut durumu kontrol et
                const string checkSql = @"
                    SELECT StockQty, ReservedQty 
                    FROM Inventory 
                    WHERE ProductId = @ProductId AND IsActive = 1";
                
                var current = await connection.QueryFirstOrDefaultAsync<(int StockQty, int ReservedQty)>(
                    checkSql, new { ProductId = productId }, transaction);
                
                if (current.StockQty == 0 && current.ReservedQty == 0)
                {
                    _logger.LogWarning("Inventory not found for product {ProductId}", productId);
                    transaction.Rollback();
                    return false;
                }
                
                var availableStock = current.StockQty - current.ReservedQty;
                if (availableStock < quantity)
                {
                    _logger.LogWarning("Insufficient stock for product {ProductId}. Available: {Available}, Requested: {Requested}", 
                        productId, availableStock, quantity);
                    transaction.Rollback();
                    return false;
                }
                
                // Reserve işlemini gerçekleştir
                const string reserveSql = @"
                    UPDATE Inventory 
                    SET ReservedQty = ReservedQty + @Quantity,
                        LastUpdatedAt = @LastUpdatedAt
                    WHERE ProductId = @ProductId";
                
                var rowsAffected = await connection.ExecuteAsync(reserveSql, new 
                { 
                    Quantity = quantity, 
                    ProductId = productId,
                    LastUpdatedAt = DateTime.UtcNow
                }, transaction);
                
                if (rowsAffected > 0)
                {
                    transaction.Commit();
                    _logger.LogInformation("Reserved {Quantity} stock for product {ProductId}", quantity, productId);
                    return true;
                }
                else
                {
                    transaction.Rollback();
                    return false;
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reserving {Quantity} stock for product {ProductId}", quantity, productId);
            throw;
        }
    }

    public async Task<bool> ReleaseStockAsync(long productId, int quantity)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE Inventory 
                SET ReservedQty = CASE 
                    WHEN ReservedQty >= @Quantity THEN ReservedQty - @Quantity 
                    ELSE 0 
                END,
                LastUpdatedAt = @LastUpdatedAt
                WHERE ProductId = @ProductId";
            
            var rowsAffected = await connection.ExecuteAsync(sql, new 
            { 
                Quantity = quantity, 
                ProductId = productId,
                LastUpdatedAt = DateTime.UtcNow
            });
            
            var success = rowsAffected > 0;
            _logger.LogInformation("Released {Quantity} stock for product {ProductId}. Success: {Success}", 
                quantity, productId, success);
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error releasing {Quantity} stock for product {ProductId}", quantity, productId);
            throw;
        }
    }

    public async Task<int> GetAvailableStockAsync(long productId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT (StockQty - ReservedQty) as AvailableStock
                FROM Inventory 
                WHERE ProductId = @ProductId AND IsActive = 1";
            
            var availableStock = await connection.QueryFirstOrDefaultAsync<int>(sql, new { ProductId = productId });
            
            _logger.LogInformation("Available stock for product {ProductId}: {AvailableStock}", productId, availableStock);
            return Math.Max(0, availableStock); // Negatif olamaz
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available stock for product {ProductId}", productId);
            throw;
        }
    }

    public async Task<int> GetReservedStockAsync(long productId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT ReservedQty 
                FROM Inventory 
                WHERE ProductId = @ProductId AND IsActive = 1";
            
            var reservedStock = await connection.QueryFirstOrDefaultAsync<int>(sql, new { ProductId = productId });
            
            _logger.LogInformation("Reserved stock for product {ProductId}: {ReservedStock}", productId, reservedStock);
            return reservedStock;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reserved stock for product {ProductId}", productId);
            throw;
        }
    }
}
