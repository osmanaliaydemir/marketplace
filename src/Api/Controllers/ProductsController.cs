using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Products;
using Application.Abstractions;
using ApiProductDtos = Api.DTOs.Products;

namespace Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
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

            // Convert API DTO to Application DTO
            var searchRequest = new ProductSearchRequest
            {
                SearchTerm = request.SearchTerm,
                CategoryId = request.CategoryId,
                StoreId = request.StoreId,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                InStock = request.InStock,
                IsFeatured = request.IsFeatured,
                Page = request.Page,
                PageSize = request.PageSize,
                SortBy = request.SortBy,
                SortOrder = request.SortOrder
            };

            var result = await _productService.SearchAsync(searchRequest);

            // Convert Application DTO to API DTO
            var response = new ApiProductDtos.ProductSearchResponse
            {
                Products = result.Products.Select(p => new ApiProductDtos.ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug,
                    ShortDescription = p.ShortDescription,
                    Price = p.Price,
                    CompareAtPrice = p.CompareAtPrice,
                    Currency = p.Currency,
                    StockQty = p.StockQty,
                    IsActive = p.IsActive,
                    IsFeatured = p.IsFeatured,
                    PrimaryImageUrl = p.PrimaryImageUrl,
                    CategoryName = p.CategoryName,
                    StoreName = p.StoreName,
                    CreatedAt = p.CreatedAt
                }).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };

            _logger.LogInformation("Retrieved {Count} products out of {TotalCount}", response.Products.Count, response.TotalCount);
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

            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { Message = "Ürün bulunamadı" });
            }

            _logger.LogInformation("Successfully retrieved product details for ID: {ProductId}", id);
            return Ok(product);
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
            var product = await _productService.GetBySlugAsync(slug);
            if (product == null)
            {
                return NotFound(new { Message = "Ürün bulunamadı" });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product by slug: {Slug}", slug);
            return StatusCode(500, new { Message = "Ürün alınırken bir hata oluştu" });
        }
    }

    #endregion

    #region Ürün Yönetimi (Seller ve Admin)

    [HttpGet("mine")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<ActionResult<IEnumerable<ApiProductDtos.ProductListDto>>> GetMyProducts()
    {
        try
        {
            // Create a request to get seller's products
            var request = new ProductListRequest
            {
                Page = 1,
                PageSize = 1000, // Get all products for now
                IsActive = null, // Include both active and inactive
                IsPublished = null // Include both published and unpublished
            };

            var result = await _productService.ListAsync(request);

            // Convert to API DTO
            var productDtos = result.Products.Select(p => new ApiProductDtos.ProductListDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                ShortDescription = p.ShortDescription,
                Price = p.Price,
                CompareAtPrice = p.CompareAtPrice,
                Currency = p.Currency,
                StockQty = p.StockQty,
                IsActive = p.IsActive,
                IsFeatured = p.IsFeatured,
                PrimaryImageUrl = p.PrimaryImageUrl,
                CategoryName = p.CategoryName,
                StoreName = p.StoreName,
                CreatedAt = p.CreatedAt
            }).ToList();

            return Ok(productDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting seller products");
            return StatusCode(500, new { Message = "Ürünler alınırken bir hata oluştu" });
        }
    }

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

            var product = await _productService.CreateAsync(request);

            _logger.LogInformation("Successfully created product with ID: {ProductId}", product.Id);
            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product: {ProductName}", request.Name);
            return StatusCode(500, new { Message = "Ürün oluşturulurken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Ürün güncelle (Seller ve Admin)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<ActionResult<ProductDetailDto>> UpdateProduct(long id, [FromBody] Application.DTOs.Products.ProductUpdateRequest request)
    {
        try
        {
            _logger.LogInformation("Updating product ID: {ProductId}", id);

            var product = await _productService.UpdateAsync(id, request);

            _logger.LogInformation("Successfully updated product ID: {ProductId}", id);
            return Ok(product);
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

            var success = await _productService.SetActiveAsync(id, request.IsActive);
            if (!success)
            {
                return BadRequest(new { Message = "Ürün durumu güncellenemedi" });
            }

            if (request.IsPublished)
            {
                await _productService.PublishAsync(id);
            }
            else
            {
                await _productService.UnpublishAsync(id);
            }

            if (request.IsFeatured)
            {
                await _productService.SetFeaturedAsync(id, request.IsFeatured);
            }

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

            var success = await _productService.DeleteAsync(id);
            if (!success)
            {
                return BadRequest(new { Message = "Ürün silinemedi" });
            }

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
}
