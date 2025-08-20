using Application.DTOs.Products;
using Domain.Entities;

namespace Application.Abstractions;

public interface IProductService
{
    // CRUD Operations
    Task<ProductDetailDto> CreateAsync(ProductCreateRequest request);
    Task<ProductDetailDto> UpdateAsync(long id, ProductUpdateRequest request);
    Task<bool> DeleteAsync(long id);
    Task<ProductDetailDto?> GetByIdAsync(long id);
    Task<ProductDetailDto?> GetBySlugAsync(string slug);
    
    // Listing and Search
    Task<ProductListResponse> ListAsync(ProductListRequest request);
    Task<ProductListResponse> SearchAsync(ProductSearchRequest request);
    Task<ProductListResponse> GetByCategoryAsync(long categoryId, ProductListRequest request);
    Task<ProductListResponse> GetByStoreAsync(long storeId, ProductListRequest request);
    
    // Status Management
    Task<bool> PublishAsync(long id);
    Task<bool> UnpublishAsync(long id);
    Task<bool> SetActiveAsync(long id, bool isActive);
    Task<bool> SetFeaturedAsync(long id, bool isFeatured);
    
    // Inventory Management
    Task<bool> UpdateStockAsync(long id, int quantity);
    Task<bool> ReserveStockAsync(long id, int quantity);
    Task<bool> ReleaseStockAsync(long id, int quantity);
    
    // Variants and Images
    Task<ProductVariantDto> AddVariantAsync(long productId, ProductVariantCreateRequest request);
    Task<bool> UpdateVariantAsync(long variantId, ProductVariantUpdateRequest request);
    Task<bool> DeleteVariantAsync(long variantId);
    Task<ProductImageDto> AddImageAsync(long productId, ProductImageCreateRequest request);
    Task<bool> UpdateImageAsync(long imageId, ProductImageUpdateRequest request);
    Task<bool> DeleteImageAsync(long imageId);
    
    // Dashboard and Admin
    Task<ProductStatsDto> GetStatsAsync();
    Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold = 10);
    Task<IEnumerable<ProductDto>> GetExpiredProductsAsync();
}
