using Application.Abstractions;
using Application.DTOs.Categories;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ILogger<CategoryService> _logger;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public CategoryService(
        ILogger<CategoryService> logger,
        ICategoryRepository categoryRepository,
        IProductRepository productRepository)
    {
        _logger = logger;
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public async Task<CategoryDetailDto> CreateAsync(CategoryCreateRequest request)
    {
        _logger.LogInformation("Creating new category: {CategoryName}", request.Name);
        
        try
        {
            // Validate parent category if specified
            if (request.ParentId.HasValue)
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentId.Value);
                if (parentCategory == null)
                {
                    _logger.LogWarning("Parent category not found: {ParentId}", request.ParentId.Value);
                    throw new ArgumentException($"Parent category with ID {request.ParentId.Value} not found");
                }
            }

            // Create category entity
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                Slug = GenerateSlug(request.Name),
                ParentId = request.ParentId,
                DisplayOrder = request.DisplayOrder,
                IsActive = true,
                IsFeatured = false,
                MetaTitle = request.MetaTitle ?? request.Name,
                MetaDescription = request.MetaDescription ?? request.Description,
                // MetaKeywords = request.MetaKeywords, // Not available in DTO
                CreatedAt = DateTime.UtcNow
            };
            
            var createdCategory = await _categoryRepository.AddAsync(category);
            _logger.LogInformation("Category created successfully: {CategoryId}", createdCategory.Id);
            
            return await MapToCategoryDetailDtoAsync(createdCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category: {CategoryName}", request.Name);
            throw;
        }
    }

    public async Task<CategoryDetailDto> UpdateAsync(long id, CategoryUpdateRequest request)
    {
        _logger.LogInformation("Updating category: {CategoryId}", id);
        
        try
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                _logger.LogWarning("Category not found for update: {CategoryId}", id);
                throw new ArgumentException($"Category with ID {id} not found");
            }
            
            // Update category properties
            existingCategory.Name = request.Name;
            existingCategory.Description = request.Description;
            existingCategory.DisplayOrder = request.DisplayOrder;
            existingCategory.MetaTitle = request.MetaTitle ?? request.Name;
            existingCategory.MetaDescription = request.MetaDescription ?? request.Description;
            // existingCategory.MetaKeywords = request.MetaKeywords; // Not available in DTO
            existingCategory.ModifiedAt = DateTime.UtcNow;
            
            // Generate new slug if name changed
            if (existingCategory.Name != request.Name)
            {
                existingCategory.Slug = GenerateSlug(request.Name);
            }
            
            var updatedCategory = await _categoryRepository.UpdateAsync(existingCategory);
            _logger.LogInformation("Category updated successfully: {CategoryId}", id);
            
            return await MapToCategoryDetailDtoAsync(updatedCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category: {CategoryId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long id)
    {
        _logger.LogInformation("Deleting category: {CategoryId}", id);
        
        try
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                _logger.LogWarning("Category not found for deletion: {CategoryId}", id);
                return false;
            }

            // Check if category has children
            var hasChildren = await _categoryRepository.HasChildrenAsync(id);
            if (hasChildren)
            {
                _logger.LogWarning("Cannot delete category with children: {CategoryId}", id);
                throw new InvalidOperationException("Cannot delete category with children");
            }

            // Check if category has products
            var productCount = await _categoryRepository.GetProductCountAsync(id);
            if (productCount > 0)
            {
                _logger.LogWarning("Cannot delete category with products: {CategoryId}, ProductCount: {Count}", id, productCount);
                throw new InvalidOperationException("Cannot delete category with products");
            }

            await _categoryRepository.DeleteAsync(id);
            _logger.LogInformation("Category deleted successfully: {CategoryId}", id);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category: {CategoryId}", id);
            throw;
        }
    }

    public async Task<CategoryDetailDto?> GetByIdAsync(long id)
    {
        _logger.LogInformation("Getting category by ID: {CategoryId}", id);
        
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category not found: {CategoryId}", id);
                return null;
            }

            return await MapToCategoryDetailDtoAsync(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category by ID: {CategoryId}", id);
            throw;
        }
    }

    public async Task<CategoryDetailDto?> GetBySlugAsync(string slug)
    {
        _logger.LogInformation("Getting category by slug: {Slug}", slug);
        
        try
        {
            var category = await _categoryRepository.GetBySlugAsync(slug);
            if (category == null)
            {
                _logger.LogWarning("Category not found: {Slug}", slug);
                return null;
            }

            return await MapToCategoryDetailDtoAsync(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category by slug: {Slug}", slug);
            throw;
        }
    }

    public async Task<CategoryListResponse> ListAsync(CategoryListRequest request)
    {
        _logger.LogInformation("Listing categories: Page {Page}, PageSize {PageSize}", request.Page, request.PageSize);
        
        try
        {
            var categories = await _categoryRepository.GetActiveCategoriesAsync();
            
            // Apply sorting
            var sortedCategories = categories.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name);
            
            // Apply pagination
            var totalCount = sortedCategories.Count();
            var pagedCategories = sortedCategories
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            
            // Map to DTOs
            var categoryDtos = await MapToCategoryListDtosAsync(pagedCategories);
            
            var response = new CategoryListResponse
            {
                Categories = categoryDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasNextPage = request.Page < (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasPreviousPage = request.Page > 1
            };
            
            _logger.LogInformation("Listed {Count} categories, Page {Page} of {TotalPages}", 
                categoryDtos.Count(), request.Page, response.TotalPages);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing categories");
            throw;
        }
    }

    public async Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync()
    {
        _logger.LogInformation("Getting root categories");
        
        try
        {
            var categories = await _categoryRepository.GetRootCategoriesAsync();
            var categoryDtos = await MapToCategoryListDtosAsync(categories);
            
            _logger.LogInformation("Found {Count} root categories", categoryDtos.Count());
            return categoryDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting root categories");
            throw;
        }
    }

    public async Task<IEnumerable<CategoryDto>> GetSubCategoriesAsync(long parentId)
    {
        _logger.LogInformation("Getting sub categories for parent: {ParentId}", parentId);
        
        try
        {
            var categories = await _categoryRepository.GetSubCategoriesAsync(parentId);
            var categoryDtos = await MapToCategoryListDtosAsync(categories);
            
            _logger.LogInformation("Found {Count} sub categories for parent {ParentId}", categoryDtos.Count(), parentId);
            return categoryDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sub categories for parent: {ParentId}", parentId);
            throw;
        }
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoryTreeAsync()
    {
        _logger.LogInformation("Getting category tree");
        
        try
        {
            var rootCategories = await _categoryRepository.GetRootCategoriesAsync();
            var categoryTree = new List<CategoryDto>();
            
            foreach (var root in rootCategories)
            {
                var rootDto = await MapToCategoryDtoAsync(root);
                // rootDto.Children = await GetSubCategoriesAsync(root.Id); // Children property not available
                categoryTree.Add(rootDto);
            }
            
            _logger.LogInformation("Built category tree with {Count} root categories", categoryTree.Count);
            return categoryTree;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building category tree");
            throw;
        }
    }

    public async Task<bool> SetActiveAsync(long id, bool isActive)
    {
        _logger.LogInformation("Setting category active status: {CategoryId}, Active: {IsActive}", id, isActive);
        
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category not found for status change: {CategoryId}", id);
                return false;
            }

            category.IsActive = isActive;
            category.ModifiedAt = DateTime.UtcNow;
            
            await _categoryRepository.UpdateAsync(category);
            _logger.LogInformation("Category status updated successfully: {CategoryId}", id);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting category active status: {CategoryId}", id);
            throw;
        }
    }

    public async Task<bool> SetFeaturedAsync(long id, bool isFeatured)
    {
        _logger.LogInformation("Setting category featured status: {CategoryId}, Featured: {IsFeatured}", id, isFeatured);
        
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category not found for featured status change: {CategoryId}", id);
                return false;
            }

            category.IsFeatured = isFeatured;
            category.ModifiedAt = DateTime.UtcNow;
            
            await _categoryRepository.UpdateAsync(category);
            _logger.LogInformation("Category featured status updated successfully: {CategoryId}", id);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting category featured status: {CategoryId}", id);
            throw;
        }
    }

    public async Task<bool> UpdateDisplayOrderAsync(long id, int displayOrder)
    {
        _logger.LogInformation("Updating category display order: {CategoryId}, Order: {Order}", id, displayOrder);
        
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category not found for display order update: {CategoryId}", id);
                return false;
            }

            category.DisplayOrder = displayOrder;
            category.ModifiedAt = DateTime.UtcNow;
            
            await _categoryRepository.UpdateAsync(category);
            _logger.LogInformation("Category display order updated successfully: {CategoryId}", id);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category display order: {CategoryId}", id);
            throw;
        }
    }

    public async Task<int> GetProductCountAsync(long categoryId)
    {
        _logger.LogInformation("Getting product count for category: {CategoryId}", categoryId);
        
        try
        {
            var count = await _categoryRepository.GetProductCountAsync(categoryId);
            _logger.LogInformation("Category {CategoryId} has {Count} products", categoryId, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product count for category: {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesWithProductCountAsync()
    {
        _logger.LogInformation("Getting categories with product counts");
        
        try
        {
            var categories = await _categoryRepository.GetActiveCategoriesAsync();
            var categoryDtos = new List<CategoryDto>();
            
            foreach (var category in categories)
            {
                var categoryDto = await MapToCategoryDtoAsync(category);
                // categoryDto.ProductCount = await _categoryRepository.GetProductCountAsync(category.Id); // Property is init-only
                categoryDtos.Add(categoryDto);
            }
            
            _logger.LogInformation("Retrieved {Count} categories with product counts", categoryDtos.Count);
            return categoryDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories with product counts");
            throw;
        }
    }
    
    #region Private Helper Methods
    
    private string GenerateSlug(string name)
    {
        if (string.IsNullOrEmpty(name)) return string.Empty;
        
        // Convert to lowercase and replace spaces with hyphens
        var slug = name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("ı", "i")
            .Replace("ğ", "g")
            .Replace("ü", "u")
            .Replace("ş", "s")
            .Replace("ö", "o")
            .Replace("ç", "c");
        
        // Remove special characters
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");
        
        // Remove multiple hyphens
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");
        
        // Remove leading and trailing hyphens
        slug = slug.Trim('-');
        
        return slug;
    }
    
    private async Task<CategoryDetailDto> MapToCategoryDetailDtoAsync(Category category)
    {
        var parentCategory = category.ParentId.HasValue 
            ? await _categoryRepository.GetByIdAsync(category.ParentId.Value) 
            : null;
        
        return new CategoryDetailDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Slug = category.Slug,
            ParentId = category.ParentId,
            ParentName = parentCategory?.Name,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            IsFeatured = category.IsFeatured,
            MetaTitle = category.MetaTitle,
            MetaDescription = category.MetaDescription,
            // MetaKeywords = category.MetaKeywords, // Not available in entity
            CreatedAt = category.CreatedAt,
            ModifiedAt = category.ModifiedAt
        };
    }
    
    private async Task<CategoryDto> MapToCategoryDtoAsync(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Slug = category.Slug,
            ParentId = category.ParentId,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            IsFeatured = category.IsFeatured,
            ProductCount = 0 // Will be set by caller if needed
        };
    }
    
    private async Task<IEnumerable<CategoryDto>> MapToCategoryListDtosAsync(IEnumerable<Category> categories)
    {
        var categoryDtos = new List<CategoryDto>();
        
        foreach (var category in categories)
        {
            categoryDtos.Add(await MapToCategoryDtoAsync(category));
        }
        
        return categoryDtos;
    }
    
    #endregion

    public async Task<bool> UpdateMetaAsync(long id, CategoryMetaUpdateRequest request)
    {
        // Implementation will be added later
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateImageAsync(long id, string imageUrl)
    {
        // Implementation will be added later
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateIconAsync(long id, string iconClass)
    {
        // Implementation will be added later
        throw new NotImplementedException();
    }

    public async Task<CategoryStatsDto> GetStatsAsync()
    {
        // Implementation will be added later
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CategoryDto>> GetInactiveCategoriesAsync()
    {
        // Implementation will be added later
        throw new NotImplementedException();
    }

    public async Task<bool> MoveCategoryAsync(long categoryId, long? newParentId)
    {
        // Implementation will be added later
        throw new NotImplementedException();
    }
}
