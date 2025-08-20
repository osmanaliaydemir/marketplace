using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.Extensions.Logging;
using Dapper;
using Application.Abstractions;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Persistence.Repositories;

public sealed class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ILogger<ProductRepository> _logger;
    private readonly ISqlConnectionFactory _connectionFactory;

    public ProductRepository(IDbContext context, ILogger<ProductRepository> logger, ISqlConnectionFactory connectionFactory) 
        : base(context, logger, null, null)
    {
        _logger = logger;
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        const string sql = @"
            SELECT * FROM products 
            WHERE is_active = @IsActive 
            ORDER BY created_at DESC";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Product>(sql, new { IsActive = true });
    }

    public async Task<IEnumerable<Product>> GetPublishedProductsAsync()
    {
        const string sql = @"
            SELECT * FROM products 
            WHERE is_active = @IsActive 
            AND is_published = @IsPublished 
            ORDER BY created_at DESC";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Product>(sql, new { IsActive = true, IsPublished = true });
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(long categoryId)
    {
        const string sql = @"
            SELECT * FROM products 
            WHERE category_id = @CategoryId 
            AND is_active = @IsActive 
            AND is_published = @IsPublished 
            ORDER BY display_order, created_at DESC";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Product>(sql, new { 
            CategoryId = categoryId, 
            IsActive = true, 
            IsPublished = true 
        });
    }

    public async Task<IEnumerable<Product>> GetProductsByStoreAsync(long storeId)
    {
        const string sql = @"
            SELECT * FROM products 
            WHERE store_id = @StoreId 
            AND is_active = @IsActive 
            AND is_published = @IsPublished 
            ORDER BY created_at DESC";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Product>(sql, new { 
            StoreId = storeId, 
            IsActive = true, 
            IsPublished = true 
        });
    }

    public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
    {
        const string sql = @"
            SELECT * FROM products 
            WHERE is_featured = @IsFeatured 
            AND is_active = @IsActive 
            AND is_published = @IsPublished 
            ORDER BY display_order, created_at DESC";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Product>(sql, new { 
            IsFeatured = true, 
            IsActive = true, 
            IsPublished = true 
        });
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10)
    {
        const string sql = @"
            SELECT * FROM products 
            WHERE stock_qty <= @Threshold 
            AND is_active = @IsActive 
            ORDER BY stock_qty ASC";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Product>(sql, new { 
            Threshold = threshold, 
            IsActive = true 
        });
    }

    public async Task<Product?> GetBySlugAsync(string slug)
    {
        const string sql = @"
            SELECT * FROM products 
            WHERE slug = @Slug 
            AND is_active = @IsActive 
            AND is_published = @IsPublished";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { 
            Slug = slug, 
            IsActive = true, 
            IsPublished = true 
        });
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, long? categoryId = null, long? storeId = null)
    {
        var sql = @"
            SELECT * FROM products 
            WHERE is_active = @IsActive 
            AND is_published = @IsPublished";
        
        object parameters;

        if (categoryId.HasValue && storeId.HasValue)
        {
            sql += " AND category_id = @CategoryId AND store_id = @StoreId";
            parameters = new { 
                IsActive = true, 
                IsPublished = true,
                SearchTerm = $"%{searchTerm}%",
                CategoryId = categoryId.Value,
                StoreId = storeId.Value
            };
        }
        else if (categoryId.HasValue)
        {
            sql += " AND category_id = @CategoryId";
            parameters = new { 
                IsActive = true, 
                IsPublished = true,
                SearchTerm = $"%{searchTerm}%",
                CategoryId = categoryId.Value
            };
        }
        else if (storeId.HasValue)
        {
            sql += " AND store_id = @StoreId";
            parameters = new { 
                IsActive = true, 
                IsPublished = true,
                SearchTerm = $"%{searchTerm}%",
                StoreId = storeId.Value
            };
        }
        else
        {
            parameters = new { 
                IsActive = true, 
                IsPublished = true,
                SearchTerm = $"%{searchTerm}%"
            };
        }

        sql += @" AND (
            LOWER(name) LIKE LOWER(@SearchTerm) 
            OR LOWER(description) LIKE LOWER(@SearchTerm) 
            OR LOWER(short_description) LIKE LOWER(@SearchTerm)
        ) ORDER BY created_at DESC";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Product>(sql, parameters);
    }

    public async Task<int> GetProductCountByCategoryAsync(long categoryId)
    {
        const string sql = @"
            SELECT COUNT(*) FROM products 
            WHERE category_id = @CategoryId 
            AND is_active = @IsActive";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.ExecuteScalarAsync<int>(sql, new { 
            CategoryId = categoryId, 
            IsActive = true 
        });
    }

    public async Task<int> GetProductCountByStoreAsync(long storeId)
    {
        const string sql = @"
            SELECT COUNT(*) FROM products 
            WHERE store_id = @StoreId 
            AND is_active = @IsActive";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.ExecuteScalarAsync<int>(sql, new { 
            StoreId = storeId, 
            IsActive = true 
        });
    }

    public async Task<bool> UpdateStockAsync(long productId, int quantity)
    {
        const string sql = @"
            UPDATE products 
            SET stock_qty = @Quantity, 
                modified_at = @ModifiedAt 
            WHERE id = @ProductId";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var rowsAffected = await connection.ExecuteAsync(sql, new { 
            ProductId = productId, 
            Quantity = quantity, 
            ModifiedAt = DateTime.UtcNow 
        });
        
        _logger.LogInformation("Updated stock for product {ProductId} to {Quantity}", productId, quantity);
        return rowsAffected > 0;
    }

    public async Task<int> GetCurrentStockAsync(long productId)
    {
        const string sql = "SELECT stock_qty FROM products WHERE id = @ProductId";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.ExecuteScalarAsync<int>(sql, new { ProductId = productId });
    }

    public async Task<bool> BulkUpdateStatusAsync(IEnumerable<long> productIds, bool isActive)
    {
        const string sql = @"
            UPDATE products 
            SET is_active = @IsActive, 
                modified_at = @ModifiedAt 
            WHERE id = ANY(@ProductIds)";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var rowsAffected = await connection.ExecuteAsync(sql, new { 
            ProductIds = productIds.ToArray(), 
            IsActive = isActive, 
            ModifiedAt = DateTime.UtcNow 
        });
        
        _logger.LogInformation("Bulk updated status for {Count} products to {IsActive}", productIds.Count(), isActive);
        return rowsAffected > 0;
    }

    public async Task<bool> BulkUpdateFeaturedAsync(IEnumerable<long> productIds, bool isFeatured)
    {
        const string sql = @"
            UPDATE products 
            SET is_featured = @IsFeatured, 
                modified_at = @ModifiedAt 
            WHERE id = ANY(@ProductIds)";
        
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var rowsAffected = await connection.ExecuteAsync(sql, new { 
            ProductIds = productIds.ToArray(), 
            IsFeatured = isFeatured, 
            ModifiedAt = DateTime.UtcNow 
        });
        
        _logger.LogInformation("Bulk updated featured status for {Count} products to {IsFeatured}", productIds.Count(), isFeatured);
        return rowsAffected > 0;
    }
}
