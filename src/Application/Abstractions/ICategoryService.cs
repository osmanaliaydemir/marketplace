using Application.DTOs.Categories;

namespace Application.Abstractions;

public interface ICategoryService
{
    // CRUD Operations
    Task<CategoryDetailDto> CreateAsync(CategoryCreateRequest request);
    Task<CategoryDetailDto> UpdateAsync(long id, CategoryUpdateRequest request);
    Task<bool> DeleteAsync(long id);
    Task<CategoryDetailDto?> GetByIdAsync(long id);
    Task<CategoryDetailDto?> GetBySlugAsync(string slug);
    
    // Listing and Hierarchy
    Task<CategoryListResponse> ListAsync(CategoryListRequest request);
    Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync();
    Task<IEnumerable<CategoryDto>> GetSubCategoriesAsync(long parentId);
    Task<IEnumerable<CategoryDto>> GetCategoryTreeAsync();
    
    // Status Management
    Task<bool> SetActiveAsync(long id, bool isActive);
    Task<bool> SetFeaturedAsync(long id, bool isFeatured);
    Task<bool> UpdateDisplayOrderAsync(long id, int displayOrder);
    
    // Product Relations
    Task<int> GetProductCountAsync(long categoryId);
    Task<IEnumerable<CategoryDto>> GetCategoriesWithProductCountAsync();
    
    // SEO and Meta
    Task<bool> UpdateMetaAsync(long id, CategoryMetaUpdateRequest request);
    Task<bool> UpdateImageAsync(long id, string imageUrl);
    Task<bool> UpdateIconAsync(long id, string iconClass);
    
    // Dashboard and Admin
    Task<CategoryStatsDto> GetStatsAsync();
    Task<IEnumerable<CategoryDto>> GetInactiveCategoriesAsync();
    Task<bool> MoveCategoryAsync(long categoryId, long? newParentId);
}
