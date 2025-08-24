using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.Extensions.Logging;
using Dapper;
using Application.Abstractions;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Naming;

namespace Infrastructure.Persistence.Repositories;

public sealed class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(IDbContext context, ILogger<ProductRepository> logger, ITableNameResolver tableNameResolver, IColumnNameResolver columnNameResolver) 
        : base(context, logger, tableNameResolver, columnNameResolver)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        try
        {
            var allProducts = await GetAllAsync();
            return allProducts.Where(p => p.IsActive).OrderByDescending(p => p.CreatedAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active products");
            throw;
        }
    }

    public async Task<IEnumerable<Product>> GetPublishedProductsAsync()
    {
        try
        {
            var allProducts = await GetAllAsync();
            return allProducts.Where(p => p.IsActive && p.IsPublished).OrderByDescending(p => p.CreatedAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting published products");
            throw;
        }
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(long categoryId)
    {
        try
        {
            var allProducts = await GetAllAsync();
            return allProducts.Where(p => p.CategoryId == categoryId && p.IsActive && p.IsPublished)
                           .OrderBy(p => p.DisplayOrder)
                           .ThenByDescending(p => p.CreatedAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products by category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IEnumerable<Product>> GetProductsByStoreAsync(long storeId)
    {
        try
        {
            var allProducts = await GetAllAsync();
            return allProducts.Where(p => p.StoreId == storeId && p.IsActive && p.IsPublished)
                           .OrderByDescending(p => p.CreatedAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products by store {StoreId}", storeId);
            throw;
        }
    }

    public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
    {
        try
        {
            var allProducts = await GetAllAsync();
            return allProducts.Where(p => p.IsFeatured && p.IsActive && p.IsPublished)
                           .OrderBy(p => p.DisplayOrder)
                           .ThenByDescending(p => p.CreatedAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting featured products");
            throw;
        }
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10)
    {
        try
        {
            var allProducts = await GetAllAsync();
            return allProducts.Where(p => p.StockQty <= threshold && p.IsActive)
                           .OrderBy(p => p.StockQty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting low stock products with threshold {Threshold}", threshold);
            throw;
        }
    }

    public async Task<Product?> GetBySlugAsync(string slug)
    {
        try
        {
            var allProducts = await GetAllAsync();
            return allProducts.FirstOrDefault(p => p.Slug == slug && p.IsActive && p.IsPublished);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product by slug {Slug}", slug);
            throw;
        }
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, long? categoryId = null, long? storeId = null)
    {
        try
        {
            var allProducts = await GetAllAsync();
            var filteredProducts = allProducts.Where(p => p.IsActive && p.IsPublished);

            // Apply search term filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchTermLower = searchTerm.ToLowerInvariant();
                filteredProducts = filteredProducts.Where(p => 
                    p.Name.ToLowerInvariant().Contains(searchTermLower) ||
                    (p.Description?.ToLowerInvariant().Contains(searchTermLower) == true) ||
                    (p.ShortDescription?.ToLowerInvariant().Contains(searchTermLower) == true));
            }

            // Apply category filter
            if (categoryId.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.CategoryId == categoryId.Value);
            }

            // Apply store filter
            if (storeId.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.StoreId == storeId.Value);
            }

            return filteredProducts.OrderByDescending(p => p.CreatedAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products with term {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<int> GetProductCountByCategoryAsync(long categoryId)
    {
        try
        {
            var allProducts = await GetAllAsync();
            return allProducts.Count(p => p.CategoryId == categoryId && p.IsActive);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product count by category {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<int> GetProductCountByStoreAsync(long storeId)
    {
        try
        {
            var allProducts = await GetAllAsync();
            return allProducts.Count(p => p.StoreId == storeId && p.IsActive);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product count by store {StoreId}", storeId);
            throw;
        }
    }

    public async Task<bool> UpdateStockAsync(long productId, int quantity)
    {
        try
        {
            var product = await GetByIdAsync(productId);
            if (product == null)
            {
                _logger.LogWarning("Product not found for stock update: {ProductId}", productId);
                return false;
            }

            product.StockQty = quantity;
            product.ModifiedAt = DateTime.UtcNow;
            
            await UpdateAsync(product);
            _logger.LogInformation("Updated stock for product {ProductId} to {Quantity}", productId, quantity);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock for product {ProductId}", productId);
            throw;
        }
    }

    public async Task<int> GetCurrentStockAsync(long productId)
    {
        try
        {
            var product = await GetByIdAsync(productId);
            return product?.StockQty ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current stock for product {ProductId}", productId);
            throw;
        }
    }

    public async Task<bool> BulkUpdateStatusAsync(IEnumerable<long> productIds, bool isActive)
    {
        try
        {
            var products = await GetAsync(p => productIds.Contains(p.Id));
            var updateCount = 0;

            foreach (var product in products)
            {
                product.IsActive = isActive;
                product.ModifiedAt = DateTime.UtcNow;
                await UpdateAsync(product);
                updateCount++;
            }

            _logger.LogInformation("Bulk updated status for {Count} products to {IsActive}", updateCount, isActive);
            return updateCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk updating status for products");
            throw;
        }
    }

    public async Task<bool> BulkUpdateFeaturedAsync(IEnumerable<long> productIds, bool isFeatured)
    {
        try
        {
            var products = await GetAsync(p => productIds.Contains(p.Id));
            var updateCount = 0;

            foreach (var product in products)
            {
                product.IsFeatured = isFeatured;
                product.ModifiedAt = DateTime.UtcNow;
                await UpdateAsync(product);
                updateCount++;
            }

            _logger.LogInformation("Bulk updated featured status for {Count} products to {IsFeatured}", updateCount, isFeatured);
            return updateCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk updating featured status for products");
            throw;
        }
    }
}
