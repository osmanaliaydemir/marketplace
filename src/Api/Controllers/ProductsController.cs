using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Products;
using Application.DTOs.Categories;
using Application.DTOs.Stores;
using Application.DTOs.Sellers;
using Application.DTOs.Users;
using Infrastructure.Persistence.Repositories;
using Domain.Entities;
using System.Text.RegularExpressions;
using ApiProductDtos = Api.DTOs.Products;

namespace Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IStoreUnitOfWork _unitOfWork;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IStoreUnitOfWork unitOfWork, ILogger<ProductsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    #region Ürün Listeleme ve Arama

    /// <summary>
    /// Ürünleri listele (public endpoint)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiProductDtos.ProductSearchResponse>> GetProducts([FromQuery] ApiProductDtos.ProductSearchRequest request)
    {
        try
        {
            _logger.LogInformation("Getting products with search criteria: {SearchTerm}, CategoryId: {CategoryId}, Page: {Page}", 
                request.SearchTerm, request.CategoryId, request.Page);

            var products = await _unitOfWork.Products.GetAllAsync();
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var stores = await _unitOfWork.Stores.GetAllAsync();
            var sellers = await _unitOfWork.Sellers.GetAllAsync();
            var users = await _unitOfWork.Users.GetAllAsync();

            // Filtreleme
            var filteredProducts = products.Where(p => p.IsActive && p.IsPublished);

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLowerInvariant();
                filteredProducts = filteredProducts.Where(p => 
                    p.Name.ToLowerInvariant().Contains(searchTerm) ||
                    p.Description?.ToLowerInvariant().Contains(searchTerm) == true ||
                    p.ShortDescription?.ToLowerInvariant().Contains(searchTerm) == true);
            }

            if (request.CategoryId.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.CategoryId == request.CategoryId.Value);
            }

            if (request.StoreId.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.StoreId == request.StoreId.Value);
            }

            if (request.MinPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price <= request.MaxPrice.Value);
            }

            if (request.InStock.HasValue)
            {
                if (request.InStock.Value)
                    filteredProducts = filteredProducts.Where(p => p.StockQty > 0);
                else
                    filteredProducts = filteredProducts.Where(p => p.StockQty <= 0);
            }

            if (request.IsFeatured.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.IsFeatured == request.IsFeatured.Value);
            }

            // Sıralama
            var sortedProducts = request.SortBy?.ToLowerInvariant() switch
            {
                "name" => request.SortOrder?.ToLowerInvariant() == "desc" 
                    ? filteredProducts.OrderByDescending(p => p.Name)
                    : filteredProducts.OrderBy(p => p.Name),
                "price" => request.SortOrder?.ToLowerInvariant() == "desc"
                    ? filteredProducts.OrderByDescending(p => p.Price)
                    : filteredProducts.OrderBy(p => p.Price),
                _ => request.SortOrder?.ToLowerInvariant() == "desc"
                    ? filteredProducts.OrderByDescending(p => p.CreatedAt)
                    : filteredProducts.OrderBy(p => p.CreatedAt)
            };

            var totalCount = sortedProducts.Count();
            var pagedProducts = sortedProducts
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // DTO'ya dönüştür
            var productDtos = pagedProducts.Select(p => new ApiProductDtos.ProductListDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                ShortDescription = p.ShortDescription,
                Price = p.Price,
                CompareAtPrice = p.CompareAtPrice,
                Currency = p.Currency,
                StockQty = p.StockQty ?? 0,
                IsActive = p.IsActive,
                IsFeatured = p.IsFeatured,
                PrimaryImageUrl = null, // TODO: ProductImage'dan al
                CategoryName = categories.FirstOrDefault(c => c.Id == p.CategoryId)?.Name ?? "Unknown",
                StoreName = stores.FirstOrDefault(s => s.Id == p.StoreId)?.Name ?? "Unknown",
                CreatedAt = p.CreatedAt
            }).ToList();

            var response = new ApiProductDtos.ProductSearchResponse
            {
                Products = productDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            _logger.LogInformation("Retrieved {Count} products out of {TotalCount}", productDtos.Count, totalCount);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
            return StatusCode(500, new { Message = "Ürünler alınırken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Ürün detayını getir (public endpoint)
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDetailDto>> GetProduct(long id)
    {
        try
        {
            _logger.LogInformation("Getting product details for ID: {ProductId}", id);

            var products = await _unitOfWork.Products.GetAllAsync();
            var product = products.FirstOrDefault(p => p.Id == id && p.IsActive && p.IsPublished);

            if (product == null)
            {
                return NotFound(new { Message = "Ürün bulunamadı" });
            }

            var categories = await _unitOfWork.Categories.GetAllAsync();
            var stores = await _unitOfWork.Stores.GetAllAsync();
            var sellers = await _unitOfWork.Sellers.GetAllAsync();
            var users = await _unitOfWork.Users.GetAllAsync();

            var category = categories.FirstOrDefault(c => c.Id == product.CategoryId);
            var store = stores.FirstOrDefault(s => s.Id == product.StoreId);
            var seller = sellers.FirstOrDefault(s => s.Id == product.SellerId);
            var user = users.FirstOrDefault(u => u.Id == seller?.UserId);

            var productDto = new ProductDetailDto
            {
                Id = product.Id,
                SellerId = product.SellerId,
                CategoryId = product.CategoryId,
                StoreId = product.StoreId,
                Name = product.Name,
                Slug = product.Slug,
                Description = product.Description,
                ShortDescription = product.ShortDescription,
                Price = product.Price,
                CompareAtPrice = product.CompareAtPrice,
                Currency = product.Currency,
                StockQty = product.StockQty ?? 0,
                IsActive = product.IsActive,
                IsFeatured = product.IsFeatured,
                IsPublished = product.IsPublished,
                Weight = product.Weight,
                MinOrderQty = product.MinOrderQty,
                MaxOrderQty = product.MaxOrderQty,
                MetaTitle = product.MetaTitle,
                MetaDescription = product.MetaDescription,
                MetaKeywords = product.MetaKeywords,
                PublishedAt = product.PublishedAt,
                CreatedAt = product.CreatedAt,
                ModifiedAt = product.ModifiedAt,
                Category = category != null ? new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Slug = category.Slug,
                    Description = category.Description,
                    ImageUrl = category.ImageUrl,
                    IconClass = category.IconClass,
                    IsActive = category.IsActive,
                    IsFeatured = category.IsFeatured,
                    DisplayOrder = category.DisplayOrder,
                    MetaTitle = category.MetaTitle,
                    MetaDescription = category.MetaDescription,
                    CreatedAt = category.CreatedAt,
                    ModifiedAt = category.ModifiedAt
                } : null!,
                Store = store != null ? new StoreDto
                {
                    Id = store.Id,
                    SellerId = store.SellerId,
                    Name = store.Name,
                    Slug = store.Slug,
                    LogoUrl = store.LogoUrl,
                    BannerUrl = store.BannerUrl,
                    Description = store.Description,
                    IsActive = store.IsActive,
                    CreatedAt = store.CreatedAt,
                    ModifiedAt = store.ModifiedAt
                } : null!,
                Seller = seller != null && user != null ? new SellerDto
                {
                    Id = seller.Id,
                    UserId = seller.UserId,
                    CommissionRate = seller.CommissionRate,
                    IsActive = seller.IsActive,
                    CreatedAt = seller.CreatedAt,
                    ModifiedAt = seller.ModifiedAt,
                    User = new AppUserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        Role = user.Role.ToString(),
                        IsActive = user.IsActive,
                        CreatedAt = user.CreatedAt,
                        ModifiedAt = user.ModifiedAt
                    }
                } : null!,
                Variants = new List<ProductVariantDto>(), // TODO: ProductVariant'dan al
                Images = new List<ProductImageDto>() // TODO: ProductImage'dan al
            };

            _logger.LogInformation("Successfully retrieved product details for ID: {ProductId}", id);
            return Ok(productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product details for ID: {ProductId}", id);
            return StatusCode(500, new { Message = "Ürün detayları alınırken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Slug ile ürün getir (public endpoint)
    /// </summary>
    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDetailDto>> GetProductBySlug(string slug)
    {
        try
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            var product = products.FirstOrDefault(p => p.Slug == slug && p.IsActive && p.IsPublished);

            if (product == null)
            {
                return NotFound(new { Message = "Ürün bulunamadı" });
            }

            return await GetProduct(product.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product by slug: {Slug}", slug);
            return StatusCode(500, new { Message = "Ürün alınırken bir hata oluştu" });
        }
    }

    #endregion

    #region Ürün Yönetimi (Seller ve Admin)

    /// <summary>
    /// Yeni ürün oluştur (Seller ve Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<ActionResult<ProductDetailDto>> CreateProduct([FromBody] ProductCreateRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new product: {ProductName}", request.Name);

            // Slug oluştur
            var slug = GenerateSlug(request.Name);

            // Kullanıcı ID'sini al
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });
            }

            // Seller'ı bul
            var sellers = await _unitOfWork.Sellers.GetAllAsync();
            var seller = sellers.FirstOrDefault(s => s.UserId == userId);
            if (seller == null)
            {
                return BadRequest(new { Message = "Satıcı hesabı bulunamadı" });
            }

            // Store'u bul
            var stores = await _unitOfWork.Stores.GetAllAsync();
            var store = stores.FirstOrDefault(s => s.SellerId == seller.Id);
            if (store == null)
            {
                return BadRequest(new { Message = "Mağaza bulunamadı" });
            }

            // Ürün oluştur
            var product = new Product
            {
                SellerId = seller.Id,
                CategoryId = request.CategoryId,
                StoreId = store.Id,
                Name = request.Name,
                Slug = slug,
                Description = request.Description,
                ShortDescription = request.ShortDescription,
                Price = request.Price,
                CompareAtPrice = request.CompareAtPrice,
                Currency = request.Currency,
                StockQty = request.StockQty,
                IsActive = request.IsActive,
                IsFeatured = request.IsFeatured,
                IsPublished = request.IsPublished,
                Weight = (decimal)request.Weight,
                MinOrderQty = request.MinOrderQty ?? 1,
                MaxOrderQty = request.MaxOrderQty ?? 999,
                MetaTitle = request.MetaTitle,
                MetaDescription = request.MetaDescription,
                MetaKeywords = request.MetaKeywords,
                PublishedAt = request.IsPublished ? DateTime.UtcNow : null,
                CreatedAt = DateTime.UtcNow
            };

            product = await _unitOfWork.Products.AddAsync(product);

            // Variant'ları ekle
            if (request.Variants.Any())
            {
                foreach (var variantRequest in request.Variants)
                {
                    var variant = new ProductVariant
                    {
                        ProductId = product.Id,
                        Sku = variantRequest.Sku,
                        Barcode = variantRequest.Barcode,
                        VariantName = variantRequest.VariantName,
                        Price = variantRequest.Price,
                        CompareAtPrice = variantRequest.CompareAtPrice,
                        StockQty = variantRequest.StockQty,
                        MinOrderQty = variantRequest.MinOrderQty ?? 1,
                        MaxOrderQty = variantRequest.MaxOrderQty ?? 999,
                        IsDefault = variantRequest.IsDefault,
                        Weight = variantRequest.Weight,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.ProductVariants.AddAsync(variant);
                }
            }

            // Resimleri ekle
            if (request.Images.Any())
            {
                for (int i = 0; i < request.Images.Count; i++)
                {
                    var imageRequest = request.Images[i];
                    var image = new ProductImage
                    {
                        ProductId = product.Id,
                        ImageUrl = imageRequest.ImageUrl,
                        ThumbnailUrl = imageRequest.ThumbnailUrl,
                        AltText = imageRequest.AltText,
                        Title = imageRequest.Title,
                        DisplayOrder = imageRequest.DisplayOrder,
                        IsPrimary = imageRequest.IsPrimary,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.ProductImages.AddAsync(image);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Successfully created product with ID: {ProductId}", product.Id);

            // Oluşturulan ürünü döndür
            return await GetProduct(product.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product: {ProductName}", request.Name);
            throw; // Global middleware handle edecek
        }
    }

    /// <summary>
    /// Ürün güncelle (Seller ve Admin)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<ActionResult<ProductDetailDto>> UpdateProduct(long id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            _logger.LogInformation("Updating product ID: {ProductId}", id);

            var products = await _unitOfWork.Products.GetAllAsync();
            var product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound(new { Message = "Ürün bulunamadı" });
            }

            // Yetki kontrolü
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });
            }

            var sellers = await _unitOfWork.Sellers.GetAllAsync();
            var seller = sellers.FirstOrDefault(s => s.UserId == userId);
            if (seller == null || (product.SellerId != seller.Id && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            // Güncelleme
            product.CategoryId = request.CategoryId;
            product.Name = request.Name;
            product.Description = request.Description;
            product.ShortDescription = request.ShortDescription;
            product.Price = request.Price;
            product.CompareAtPrice = request.CompareAtPrice;
            product.Currency = request.Currency;
            product.StockQty = request.StockQty;
            product.IsActive = request.IsActive;
            product.IsFeatured = request.IsFeatured;
            product.IsPublished = request.IsPublished;
            product.Weight = (decimal)request.Weight;
            product.MinOrderQty = request.MinOrderQty ?? 1;
            product.MaxOrderQty = request.MaxOrderQty ?? 999;
            product.MetaTitle = request.MetaTitle;
            product.MetaDescription = request.MetaDescription;
            product.MetaKeywords = request.MetaKeywords;
            product.ModifiedAt = DateTime.UtcNow;

            if (request.IsPublished && !product.PublishedAt.HasValue)
            {
                product.PublishedAt = DateTime.UtcNow;
            }

            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Successfully updated product ID: {ProductId}", id);

            return await GetProduct(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product ID: {ProductId}", id);
            return StatusCode(500, new { Message = "Ürün güncellenirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Ürün durumunu güncelle (Seller ve Admin)
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<ActionResult> UpdateProductStatus(long id, [FromBody] ApiProductDtos.UpdateProductStatusRequest request)
    {
        try
        {
            _logger.LogInformation("Updating product status ID: {ProductId}", id);

            var products = await _unitOfWork.Products.GetAllAsync();
            var product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound(new { Message = "Ürün bulunamadı" });
            }

            // Yetki kontrolü
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });
            }

            var sellers = await _unitOfWork.Sellers.GetAllAsync();
            var seller = sellers.FirstOrDefault(s => s.UserId == userId);
            if (seller == null || (product.SellerId != seller.Id && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            // Durum güncelleme
            product.IsActive = request.IsActive;
            product.IsPublished = request.IsPublished;
            product.IsFeatured = request.IsFeatured;
            product.ModifiedAt = DateTime.UtcNow;

            if (request.IsPublished && !product.PublishedAt.HasValue)
            {
                product.PublishedAt = DateTime.UtcNow;
            }

            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Successfully updated product status ID: {ProductId}", id);

            return Ok(new { Message = "Ürün durumu başarıyla güncellendi" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product status ID: {ProductId}", id);
            return StatusCode(500, new { Message = "Ürün durumu güncellenirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Ürün sil (soft delete - Seller ve Admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<ActionResult> DeleteProduct(long id)
    {
        try
        {
            _logger.LogInformation("Deleting product ID: {ProductId}", id);

            var products = await _unitOfWork.Products.GetAllAsync();
            var product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound(new { Message = "Ürün bulunamadı" });
            }

            // Yetki kontrolü
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });
            }

            var sellers = await _unitOfWork.Sellers.GetAllAsync();
            var seller = sellers.FirstOrDefault(s => s.UserId == userId);
            if (seller == null || (product.SellerId != seller.Id && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            // Soft delete
            product.IsActive = false;
            product.IsPublished = false;
            product.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Successfully deleted product ID: {ProductId}", id);

            return Ok(new { Message = "Ürün başarıyla silindi" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product ID: {ProductId}", id);
            return StatusCode(500, new { Message = "Ürün silinirken bir hata oluştu" });
        }
    }

    #endregion

    #region Yardımcı Metodlar

    private long? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }

    private static string GenerateSlug(string name)
    {
        // Türkçe karakterleri değiştir
        var slug = name.ToLowerInvariant()
            .Replace("ç", "c")
            .Replace("ğ", "g")
            .Replace("ı", "i")
            .Replace("ö", "o")
            .Replace("ş", "s")
            .Replace("ü", "u");

        // Sadece harf, rakam ve tire bırak
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        
        // Boşlukları tire ile değiştir
        slug = Regex.Replace(slug, @"\s+", "-");
        
        // Birden fazla tireyi tek tire yap
        slug = Regex.Replace(slug, @"-+", "-");
        
        // Başta ve sonda tire varsa kaldır
        slug = slug.Trim('-');
        
        return slug;
    }

    #endregion
}
