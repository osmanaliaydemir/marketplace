using Application.Abstractions;
using Application.DTOs.Products;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public sealed class ProductService : IProductService
{
    private readonly ILogger<ProductService> _logger;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly ISellerRepository _sellerRepository;

    public ProductService(
        ILogger<ProductService> logger,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IStoreRepository storeRepository,
        ISellerRepository sellerRepository)
    {
        _logger = logger;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _storeRepository = storeRepository;
        _sellerRepository = sellerRepository;
    }

    public async Task<ProductDetailDto> CreateAsync(ProductCreateRequest request)
    {
        _logger.LogInformation("Creating new product: {ProductName}", request.Name);
        
        try
        {
            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
            {
                _logger.LogWarning("Category not found: {CategoryId}", request.CategoryId);
                throw new ArgumentException($"Category with ID {request.CategoryId} not found");
            }
            
            // Validate store exists
            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if (store == null)
            {
                _logger.LogWarning("Store not found: {StoreId}", request.StoreId);
                throw new ArgumentException($"Store with ID {request.StoreId} not found");
            }
            
            // Create product entity
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                ShortDescription = request.ShortDescription,
                Slug = GenerateSlug(request.Name),
                Price = request.Price,
                CompareAtPrice = request.CompareAtPrice,
                Currency = request.Currency ?? "TRY",
                StockQty = request.StockQty,
                Weight = request.Weight,
                MinOrderQty = request.MinOrderQty ?? 1,
                MaxOrderQty = request.MaxOrderQty ?? 999,
                IsActive = true,
                IsPublished = false, // Default to unpublished
                IsFeatured = false,
                DisplayOrder = request.DisplayOrder,
                CategoryId = request.CategoryId,
                StoreId = request.StoreId,
                SellerId = request.SellerId,
                MetaTitle = request.MetaTitle ?? request.Name,
                MetaDescription = request.MetaDescription ?? request.ShortDescription,
                MetaKeywords = request.MetaKeywords,
                CreatedAt = DateTime.UtcNow
            };
            
            var createdProduct = await _productRepository.AddAsync(product);
            _logger.LogInformation("Product created successfully: {ProductId}", createdProduct.Id);
            
            return await MapToProductDetailDtoAsync(createdProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product: {ProductName}", request.Name);
            throw;
        }
    }

    public async Task<ProductDetailDto> UpdateAsync(long id, ProductUpdateRequest request)
    {
        _logger.LogInformation("Updating product: {ProductId}", id);
        
        try
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                _logger.LogWarning("Product not found for update: {ProductId}", id);
                throw new ArgumentException($"Product with ID {id} not found");
            }
            
            // Update product properties
            existingProduct.Name = request.Name;
            existingProduct.Description = request.Description;
            existingProduct.ShortDescription = request.ShortDescription;
            existingProduct.Price = request.Price;
            existingProduct.CompareAtPrice = request.CompareAtPrice;
            existingProduct.StockQty = request.StockQty;
            existingProduct.Weight = request.Weight;
            existingProduct.MinOrderQty = request.MinOrderQty ?? 1;
            existingProduct.MaxOrderQty = request.MaxOrderQty ?? 999;
            existingProduct.DisplayOrder = request.DisplayOrder;
            existingProduct.MetaTitle = request.MetaTitle ?? request.Name;
            existingProduct.MetaDescription = request.MetaDescription ?? request.ShortDescription;
            existingProduct.MetaKeywords = request.MetaKeywords;
            existingProduct.ModifiedAt = DateTime.UtcNow;
            
            // Generate new slug if name changed
            if (existingProduct.Name != request.Name)
            {
                existingProduct.Slug = GenerateSlug(request.Name);
            }
            
            var updatedProduct = await _productRepository.UpdateAsync(existingProduct);
            _logger.LogInformation("Product updated successfully: {ProductId}", id);
            
            return await MapToProductDetailDtoAsync(updatedProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product: {ProductId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long id)
    {
        _logger.LogInformation("Deleting product: {ProductId}", id);
        
        try
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                _logger.LogWarning("Product not found for deletion: {ProductId}", id);
                return false;
            }

            await _productRepository.DeleteAsync(id);
            _logger.LogInformation("Product deleted successfully: {ProductId}", id);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product: {ProductId}", id);
            throw;
        }
    }

    public async Task<ProductDetailDto?> GetByIdAsync(long id)
    {
        _logger.LogInformation("Getting product by ID: {ProductId}", id);
        
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found: {ProductId}", id);
                return null;
            }

            return await MapToProductDetailDtoAsync(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product by ID: {ProductId}", id);
            throw;
        }
    }

    public async Task<ProductDetailDto?> GetBySlugAsync(string slug)
    {
        _logger.LogInformation("Getting product by slug: {Slug}", slug);
        
        try
        {
            var product = await _productRepository.GetBySlugAsync(slug);
            if (product == null)
            {
                _logger.LogWarning("Product not found: {Slug}", slug);
                return null;
            }

            return await MapToProductDetailDtoAsync(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product by slug: {Slug}", slug);
            throw;
        }
    }

    public async Task<ProductListResponse> ListAsync(ProductListRequest request)
    {
        _logger.LogInformation("Listing products: Page {Page}, PageSize {PageSize}", request.Page, request.PageSize);
        
        try
        {
            var products = await _productRepository.GetPublishedProductsAsync();
            
            // Apply sorting
            var sortedProducts = request.SortBy?.ToLowerInvariant() switch
            {
                "name" => request.SortOrder?.ToLowerInvariant() == "desc" 
                    ? products.OrderByDescending(p => p.Name)
                    : products.OrderBy(p => p.Name),
                "price" => request.SortOrder?.ToLowerInvariant() == "desc"
                    ? products.OrderByDescending(p => p.Price)
                    : products.OrderBy(p => p.Price),
                "created" => request.SortOrder?.ToLowerInvariant() == "desc"
                    ? products.OrderByDescending(p => p.CreatedAt)
                    : products.OrderBy(p => p.CreatedAt),
                _ => products.OrderByDescending(p => p.CreatedAt)
            };
            
            // Apply pagination
            var totalCount = sortedProducts.Count();
            var pagedProducts = sortedProducts
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            
            // Map to DTOs
            var productDtos = await MapToProductListDtosAsync(pagedProducts);
            
            var response = new ProductListResponse
            {
                Products = productDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasNextPage = request.Page < (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasPreviousPage = request.Page > 1
            };
            
            _logger.LogInformation("Listed {Count} products, Page {Page} of {TotalPages}", 
                productDtos.Count(), request.Page, response.TotalPages);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing products");
            throw;
        }
    }

    public async Task<ProductListResponse> SearchAsync(ProductSearchRequest request)
    {
        _logger.LogInformation("Searching products with criteria: {SearchTerm}, CategoryId: {CategoryId}, Page: {Page}", 
            request.SearchTerm, request.CategoryId, request.Page);
        
        try
        {
            // Use repository for optimized search
            var products = await _productRepository.SearchProductsAsync(
                request.SearchTerm ?? string.Empty,
                request.CategoryId,
                request.StoreId);
            
            // Apply additional filters
            var filteredProducts = ApplyAdvancedFilters(products, request);
            
            // Apply sorting
            var sortedProducts = ApplySorting(filteredProducts, request);
            
            // Apply pagination
            var totalCount = sortedProducts.Count();
            var pagedProducts = sortedProducts
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            
            // Map to DTOs
            var productDtos = await MapToProductListDtosAsync(pagedProducts);
            
            var response = new ProductListResponse
            {
                Products = productDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasNextPage = request.Page < (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasPreviousPage = request.Page > 1
            };
            
            _logger.LogInformation("Search completed: {Count} products found, Page {Page} of {TotalPages}", 
                productDtos.Count(), request.Page, response.TotalPages);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products");
            throw;
        }
    }

    public async Task<ProductListResponse> GetByCategoryAsync(long categoryId, ProductListRequest request)
    {
        _logger.LogInformation("Getting products by category: {CategoryId}, Page {Page}", categoryId, request.Page);
        
        try
        {
            var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
            
            // Apply sorting
            var sortedProducts = request.SortBy?.ToLowerInvariant() switch
            {
                "name" => request.SortOrder?.ToLowerInvariant() == "desc" 
                    ? products.OrderByDescending(p => p.Name)
                    : products.OrderBy(p => p.Name),
                "price" => request.SortOrder?.ToLowerInvariant() == "desc"
                    ? products.OrderByDescending(p => p.Price)
                    : products.OrderBy(p => p.Price),
                _ => products.OrderBy(p => p.DisplayOrder).ThenByDescending(p => p.CreatedAt)
            };
            
            // Apply pagination
            var totalCount = sortedProducts.Count();
            var pagedProducts = sortedProducts
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            
            // Map to DTOs
            var productDtos = await MapToProductListDtosAsync(pagedProducts);
            
            var response = new ProductListResponse
            {
                Products = productDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasNextPage = request.Page < (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasPreviousPage = request.Page > 1
            };
            
            _logger.LogInformation("Found {Count} products in category {CategoryId}", productDtos.Count(), categoryId);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products by category: {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<ProductListResponse> GetByStoreAsync(long storeId, ProductListRequest request)
    {
        _logger.LogInformation("Getting products by store: {StoreId}, Page {Page}", storeId, request.Page);
        
        try
        {
            var products = await _productRepository.GetProductsByStoreAsync(storeId);
            
            // Apply sorting
            var sortedProducts = request.SortBy?.ToLowerInvariant() switch
            {
                "name" => request.SortOrder?.ToLowerInvariant() == "desc" 
                    ? products.OrderByDescending(p => p.Name)
                    : products.OrderBy(p => p.Name),
                "price" => request.SortOrder?.ToLowerInvariant() == "desc"
                    ? products.OrderByDescending(p => p.Price)
                    : products.OrderBy(p => p.Price),
                _ => products.OrderByDescending(p => p.CreatedAt)
            };
            
            // Apply pagination
            var totalCount = sortedProducts.Count();
            var pagedProducts = sortedProducts
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            
            // Map to DTOs
            var productDtos = await MapToProductListDtosAsync(pagedProducts);
            
            var response = new ProductListResponse
            {
                Products = productDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasNextPage = request.Page < (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasPreviousPage = request.Page > 1
            };
            
            _logger.LogInformation("Found {Count} products in store {StoreId}", productDtos.Count(), storeId);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products by store: {StoreId}", storeId);
            throw;
        }
    }

    public async Task<bool> PublishAsync(long id)
    {
        _logger.LogInformation("Publishing product: {ProductId}", id);
        
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found for publishing: {ProductId}", id);
                return false;
            }

            product.IsPublished = true;
            product.PublishedAt = DateTime.UtcNow;
            product.ModifiedAt = DateTime.UtcNow;
            
            await _productRepository.UpdateAsync(product);
            _logger.LogInformation("Product published successfully: {ProductId}", id);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing product: {ProductId}", id);
            throw;
        }
    }

    public async Task<bool> UnpublishAsync(long id)
    {
        _logger.LogInformation("Unpublishing product: {ProductId}", id);
        
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found for unpublishing: {ProductId}", id);
                return false;
            }

            product.IsPublished = false;
            product.PublishedAt = null;
            product.ModifiedAt = DateTime.UtcNow;
            
            await _productRepository.UpdateAsync(product);
            _logger.LogInformation("Product unpublished successfully: {ProductId}", id);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unpublishing product: {ProductId}", id);
            throw;
        }
    }

    public async Task<bool> SetActiveAsync(long id, bool isActive)
    {
        _logger.LogInformation("Setting product active status: {ProductId}, Active: {IsActive}", id, isActive);
        
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found for status change: {ProductId}", id);
                return false;
            }

            product.IsActive = isActive;
            product.ModifiedAt = DateTime.UtcNow;
            
            await _productRepository.UpdateAsync(product);
            _logger.LogInformation("Product status updated successfully: {ProductId}", id);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting product active status: {ProductId}", id);
            throw;
        }
    }

    public async Task<bool> SetFeaturedAsync(long id, bool isFeatured)
    {
        _logger.LogInformation("Setting product featured status: {ProductId}, Featured: {IsFeatured}", id, isFeatured);
        
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found for featured status change: {ProductId}", id);
                return false;
            }

            product.IsFeatured = isFeatured;
            product.ModifiedAt = DateTime.UtcNow;
            
            await _productRepository.UpdateAsync(product);
            _logger.LogInformation("Product featured status updated successfully: {ProductId}", id);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting product featured status: {ProductId}", id);
            throw;
        }
    }

    public async Task<bool> UpdateStockAsync(long id, int quantity)
    {
        _logger.LogInformation("Updating stock for product: {ProductId}, Quantity: {Quantity}", id, quantity);
        
        try
        {
            if (quantity < 0)
            {
                _logger.LogWarning("Invalid stock quantity: {Quantity}", quantity);
                throw new ArgumentException("Stock quantity cannot be negative");
            }

            var success = await _productRepository.UpdateStockAsync(id, quantity);
            if (success)
            {
                _logger.LogInformation("Stock updated successfully for product: {ProductId}", id);
            }
            else
            {
                _logger.LogWarning("Failed to update stock for product: {ProductId}", id);
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock for product: {ProductId}", id);
            throw;
        }
    }

    public async Task<bool> ReserveStockAsync(long id, int quantity)
    {
        _logger.LogInformation("Reserving stock for product: {ProductId}, Quantity: {Quantity}", id, quantity);
        
        try
        {
            if (quantity <= 0)
            {
                _logger.LogWarning("Invalid reserve quantity: {Quantity}", quantity);
                throw new ArgumentException("Reserve quantity must be positive");
            }

            var currentStock = await _productRepository.GetCurrentStockAsync(id);
            if (currentStock < quantity)
            {
                _logger.LogWarning("Insufficient stock for product: {ProductId}, Available: {Available}, Requested: {Requested}", 
                    id, currentStock, quantity);
                return false;
            }

            var newStock = currentStock - quantity;
            var success = await _productRepository.UpdateStockAsync(id, newStock);
            
            if (success)
            {
                _logger.LogInformation("Stock reserved successfully for product: {ProductId}, Reserved: {Quantity}", id, quantity);
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reserving stock for product: {ProductId}", id);
            throw;
        }
    }

    public async Task<bool> ReleaseStockAsync(long id, int quantity)
    {
        _logger.LogInformation("Releasing stock for product: {ProductId}, Quantity: {Quantity}", id, quantity);
        
        try
        {
            if (quantity <= 0)
            {
                _logger.LogWarning("Invalid release quantity: {Quantity}", quantity);
                throw new ArgumentException("Release quantity must be positive");
            }

            var currentStock = await _productRepository.GetCurrentStockAsync(id);
            var newStock = currentStock + quantity;
            var success = await _productRepository.UpdateStockAsync(id, newStock);
            
            if (success)
            {
                _logger.LogInformation("Stock released successfully for product: {ProductId}, Released: {Quantity}", id, quantity);
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error releasing stock for product: {ProductId}", id);
            throw;
        }
    }

    public async Task<ProductVariantDto> AddVariantAsync(long productId, ProductVariantCreateRequest request)
    {
        // Implementation will be added later
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateVariantAsync(long variantId, ProductVariantUpdateRequest request)
    {
        // Implementation will be added later
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteVariantAsync(long variantId)
    {
        // Implementation will be added later
        throw new NotImplementedException();
    }

    public async Task<ProductImageDto> AddImageAsync(long productId, ProductImageCreateRequest request)
    {
        // Implementation will be added later
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateImageAsync(long imageId, ProductImageUpdateRequest request)
    {
        // Implementation will be added later
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteImageAsync(long imageId)
    {
        // Implementation will be added later
        throw new NotImplementedException();
    }

    public async Task<ProductStatsDto> GetStatsAsync()
    {
        // Implementation will be added later
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold = 10)
    {
        _logger.LogInformation("Getting low stock products with threshold: {Threshold}", threshold);
        
        try
        {
            var products = await _productRepository.GetLowStockProductsAsync(threshold);
            var productDtos = await MapToProductListDtosAsync(products);
            
            _logger.LogInformation("Found {Count} low stock products", productDtos.Count());
            return productDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting low stock products");
            throw;
        }
    }

    public async Task<IEnumerable<ProductDto>> GetExpiredProductsAsync()
    {
        _logger.LogInformation("Getting expired products");
        
        try
        {
            // For now, return empty list as we don't have expiry date in Product entity
            // This can be implemented when expiry tracking is added
            _logger.LogInformation("No expired products found (expiry tracking not implemented)");
            return new List<ProductDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting expired products");
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
    
    private IEnumerable<Product> ApplyAdvancedFilters(IEnumerable<Product> products, ProductSearchRequest request)
    {
        var filtered = products.Where(p => p.IsActive && p.IsPublished);
        
        if (request.MinPrice.HasValue)
            filtered = filtered.Where(p => p.Price >= request.MinPrice.Value);
            
        if (request.MaxPrice.HasValue)
            filtered = filtered.Where(p => p.Price <= request.MaxPrice.Value);
            
        if (request.InStock.HasValue)
        {
            if (request.InStock.Value)
                filtered = filtered.Where(p => p.StockQty > 0);
            else
                filtered = filtered.Where(p => p.StockQty <= 0);
        }
        
        if (request.IsFeatured.HasValue)
            filtered = filtered.Where(p => p.IsFeatured == request.IsFeatured.Value);
            
        return filtered;
    }
    
    private IEnumerable<Product> ApplySorting(IEnumerable<Product> products, ProductSearchRequest request)
    {
        return request.SortBy?.ToLowerInvariant() switch
        {
            "name" => request.SortOrder?.ToLowerInvariant() == "desc" 
                ? products.OrderByDescending(p => p.Name)
                : products.OrderBy(p => p.Name),
            "price" => request.SortOrder?.ToLowerInvariant() == "desc"
                ? products.OrderByDescending(p => p.Price)
                : products.OrderBy(p => p.Price),
            "stock" => request.SortOrder?.ToLowerInvariant() == "desc"
                ? products.OrderByDescending(p => p.StockQty)
                : products.OrderBy(p => p.StockQty),
            _ => request.SortOrder?.ToLowerInvariant() == "desc"
                ? products.OrderByDescending(p => p.CreatedAt)
                : products.OrderBy(p => p.CreatedAt)
        };
    }
    
    private async Task<ProductDetailDto> MapToProductDetailDtoAsync(Product product)
    {
        var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
        var store = await _storeRepository.GetByIdAsync(product.StoreId);
        var seller = await _sellerRepository.GetByIdAsync(product.SellerId);
        
        return new ProductDetailDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            ShortDescription = product.ShortDescription,
            Slug = product.Slug,
            Price = product.Price,
            CompareAtPrice = product.CompareAtPrice,
            Currency = product.Currency,
            StockQty = product.StockQty ?? 0,
            Weight = product.Weight,
            MinOrderQty = product.MinOrderQty,
            MaxOrderQty = product.MaxOrderQty,
            IsActive = product.IsActive,
            IsPublished = product.IsPublished,
            IsFeatured = product.IsFeatured,
            DisplayOrder = product.DisplayOrder,
            CategoryId = product.CategoryId,
            CategoryName = category?.Name ?? "Unknown",
            StoreId = product.StoreId,
            StoreName = store?.Name ?? "Unknown",
            SellerId = product.SellerId,
            SellerName = seller?.User?.FullName ?? "Unknown",
            MetaTitle = product.MetaTitle,
            MetaDescription = product.MetaDescription,
            MetaKeywords = product.MetaKeywords,
            PublishedAt = product.PublishedAt,
            CreatedAt = product.CreatedAt,
            ModifiedAt = product.ModifiedAt
        };
    }
    
    private async Task<IEnumerable<ProductDto>> MapToProductListDtosAsync(IEnumerable<Product> products)
    {
        var productDtos = new List<ProductDto>();
        
        foreach (var product in products)
        {
            var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
            var store = await _storeRepository.GetByIdAsync(product.StoreId);
            
            productDtos.Add(new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Slug = product.Slug,
                Price = product.Price,
                CompareAtPrice = product.CompareAtPrice,
                ImageUrl = null, // TODO: Get from ProductImage
                IsActive = product.IsActive,
                IsFeatured = product.IsFeatured,
                DisplayOrder = product.DisplayOrder,
                CreatedAt = product.CreatedAt,
                ModifiedAt = product.ModifiedAt,
                CategoryId = product.CategoryId,
                CategoryName = category?.Name ?? "Unknown",
                StoreId = product.StoreId,
                StoreName = store?.Name ?? "Unknown"
            });
        }
        
        return productDtos;
    }
    
    #endregion
}
