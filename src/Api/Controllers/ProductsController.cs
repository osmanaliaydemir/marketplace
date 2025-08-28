using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Products;
using Application.Abstractions;
using ApiProductDtos = Api.DTOs.Products;

namespace Api.Controllers;

/// <summary>
/// Ürün yönetimi için API endpoint'leri
/// </summary>
/// <remarks>
/// Bu controller ürün listeleme, arama, detay görüntüleme ve yönetim işlemlerini sağlar.
/// Satıcılar kendi ürünlerini yönetebilir, müşteriler ise ürünleri görüntüleyebilir.
/// </remarks>
[ApiController]
[Route("api/products")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), 400)]
[ProducesResponseType(typeof(ProblemDetails), 401)]
[ProducesResponseType(typeof(ProblemDetails), 403)]
[ProducesResponseType(typeof(ProblemDetails), 404)]
[ProducesResponseType(typeof(ProblemDetails), 500)]
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
    /// Ürünleri arama kriterlerine göre listeler
    /// </summary>
    /// <param name="request">Arama ve filtreleme kriterleri</param>
    /// <returns>Sayfalanmış ürün listesi</returns>
    /// <response code="200">Ürünler başarıyla getirildi</response>
    /// <response code="400">Geçersiz arama kriterleri</response>
    /// <response code="500">Sunucu hatası</response>
    /// <example>
    /// GET /api/products?searchTerm=iphone&amp;categoryId=1&amp;page=1&amp;pageSize=10
    /// </example>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiProductDtos.ProductSearchResponse), 200)]
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
    /// ID'ye göre ürün detaylarını getirir
    /// </summary>
    /// <param name="id">Ürün ID'si</param>
    /// <returns>Ürün detayları</returns>
    /// <response code="200">Ürün başarıyla getirildi</response>
    /// <response code="404">Ürün bulunamadı</response>
    /// <response code="500">Sunucu hatası</response>
    /// <example>
    /// GET /api/products/1
    /// </example>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductDetailDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<ProductDetailDto>> GetProduct(long id)
    {
        try
        {
            _logger.LogInformation("Getting product details for ID: {ProductId}", id);

            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found: {ProductId}", id);
                return NotFound(new { Message = $"ID {id} olan ürün bulunamadı" });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product with ID: {ProductId}", id);
            return StatusCode(500, new { Message = "Ürün alınırken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Slug'a göre ürün detaylarını getirir
    /// </summary>
    /// <param name="slug">Ürün slug'ı</param>
    /// <returns>Ürün detayları</returns>
    /// <response code="200">Ürün başarıyla getirildi</response>
    /// <response code="404">Ürün bulunamadı</response>
    /// <response code="500">Sunucu hatası</response>
    /// <example>
    /// GET /api/products/by-slug/iphone-15-pro
    /// </example>
    [HttpGet("by-slug/{slug}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductDetailDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<ProductDetailDto>> GetProductBySlug(string slug)
    {
        try
        {
            _logger.LogInformation("Getting product details for slug: {Slug}", slug);

            var product = await _productService.GetBySlugAsync(slug);
            if (product == null)
            {
                _logger.LogWarning("Product not found with slug: {Slug}", slug);
                return NotFound(new { Message = $"Slug '{slug}' olan ürün bulunamadı" });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product with slug: {Slug}", slug);
            return StatusCode(500, new { Message = "Ürün alınırken bir hata oluştu" });
        }
    }

    #endregion

    #region Ürün Yönetimi (Satıcı işlemleri)

    /// <summary>
    /// Yeni ürün oluşturur (Sadece satıcılar)
    /// </summary>
    /// <param name="request">Ürün oluşturma bilgileri</param>
    /// <returns>Oluşturulan ürün detayları</returns>
    /// <response code="201">Ürün başarıyla oluşturuldu</response>
    /// <response code="400">Geçersiz ürün bilgileri</response>
    /// <response code="401">Yetkilendirme hatası</response>
    /// <response code="403">Satıcı yetkisi gerekli</response>
    /// <response code="500">Sunucu hatası</response>
    /// <example>
    /// POST /api/products
    /// {
    ///   "name": "iPhone 15 Pro",
    ///   "description": "Apple iPhone 15 Pro",
    ///   "price": 29999.99,
    ///   "categoryId": 1,
    ///   "storeId": 1
    /// }
    /// </example>
    [HttpPost]
    [Authorize(Roles = "Seller")]
    [ProducesResponseType(typeof(ProductDetailDto), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<ActionResult<ProductDetailDto>> CreateProduct([FromBody] ProductCreateRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new product: {ProductName}", request.Name);

            var createdProduct = await _productService.CreateAsync(request);
            _logger.LogInformation("Product created successfully: {ProductId}", createdProduct.Id);

            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid product data: {ProductName}", request.Name);
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product: {ProductName}", request.Name);
            return StatusCode(500, new { Message = "Ürün oluşturulurken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Ürün bilgilerini günceller (Sadece satıcılar)
    /// </summary>
    /// <param name="id">Ürün ID'si</param>
    /// <param name="request">Güncellenecek ürün bilgileri</param>
    /// <returns>Güncellenen ürün detayları</returns>
    /// <response code="200">Ürün başarıyla güncellendi</response>
    /// <response code="400">Geçersiz ürün bilgileri</response>
    /// <response code="404">Ürün bulunamadı</response>
    /// <response code="401">Yetkilendirme hatası</response>
    /// <response code="403">Satıcı yetkisi gerekli</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Seller")]
    [ProducesResponseType(typeof(ProductDetailDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<ProductDetailDto>> UpdateProduct(long id, [FromBody] ProductUpdateRequest request)
    {
        try
        {
            _logger.LogInformation("Updating product: {ProductId}", id);

            var updatedProduct = await _productService.UpdateAsync(id, request);
            _logger.LogInformation("Product updated successfully: {ProductId}", id);

            return Ok(updatedProduct);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid product data for update: {ProductId}", id);
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product: {ProductId}", id);
            return StatusCode(500, new { Message = "Ürün güncellenirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Ürünü yayınlar (Sadece satıcılar)
    /// </summary>
    /// <param name="id">Ürün ID'si</param>
    /// <returns>İşlem sonucu</returns>
    /// <response code="200">Ürün başarıyla yayınlandı</response>
    /// <response code="404">Ürün bulunamadı</response>
    /// <response code="401">Yetkilendirme hatası</response>
    /// <response code="403">Satıcı yetkisi gerekli</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpPost("{id}/publish")]
    [Authorize(Roles = "Seller")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult> PublishProduct(long id)
    {
        try
        {
            _logger.LogInformation("Publishing product: {ProductId}", id);

            var success = await _productService.PublishAsync(id);
            if (!success)
            {
                _logger.LogWarning("Failed to publish product: {ProductId}", id);
                return NotFound(new { Message = $"ID {id} olan ürün bulunamadı" });
            }

            _logger.LogInformation("Product published successfully: {ProductId}", id);
            return Ok(new { Message = "Ürün başarıyla yayınlandı" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing product: {ProductId}", id);
            return StatusCode(500, new { Message = "Ürün yayınlanırken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Ürünü yayından kaldırır (Sadece satıcılar)
    /// </summary>
    /// <param name="id">Ürün ID'si</param>
    /// <returns>İşlem sonucu</returns>
    /// <response code="200">Ürün başarıyla yayından kaldırıldı</response>
    /// <response code="404">Ürün bulunamadı</response>
    /// <response code="401">Yetkilendirme hatası</response>
    /// <response code="403">Satıcı yetkisi gerekli</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpPost("{id}/unpublish")]
    [Authorize(Roles = "Seller")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult> UnpublishProduct(long id)
    {
        try
        {
            _logger.LogInformation("Unpublishing product: {ProductId}", id);

            var success = await _productService.UnpublishAsync(id);
            if (!success)
            {
                _logger.LogWarning("Failed to unpublish product: {ProductId}", id);
                return NotFound(new { Message = $"ID {id} olan ürün bulunamadı" });
            }

            _logger.LogInformation("Product unpublished successfully: {ProductId}", id);
            return Ok(new { Message = "Ürün yayından kaldırıldı" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unpublishing product: {ProductId}", id);
            return StatusCode(500, new { Message = "Ürün yayından kaldırılırken bir hata oluştu" });
        }
    }

    #endregion
}
