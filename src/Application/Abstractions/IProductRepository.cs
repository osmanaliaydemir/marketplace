using Domain.Entities;

namespace Application.Abstractions;

public interface IProductRepository : IRepository<Product>
{
    // Product-specific queries
    Task<IEnumerable<Product>> GetActiveProductsAsync();
    Task<IEnumerable<Product>> GetPublishedProductsAsync();
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(long categoryId);
    Task<IEnumerable<Product>> GetProductsByStoreAsync(long storeId);
    Task<IEnumerable<Product>> GetFeaturedProductsAsync();
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10);
    Task<Product?> GetBySlugAsync(string slug);
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, long? categoryId = null, long? storeId = null);
    Task<int> GetProductCountByCategoryAsync(long categoryId);
    Task<int> GetProductCountByStoreAsync(long storeId);
    
    // Stock management
    Task<bool> UpdateStockAsync(long productId, int quantity);
    Task<int> GetCurrentStockAsync(long productId);
    
    // Bulk operations
    Task<bool> BulkUpdateStatusAsync(IEnumerable<long> productIds, bool isActive);
    Task<bool> BulkUpdateFeaturedAsync(IEnumerable<long> productIds, bool isFeatured);
}
