using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Persistence.Repositories;
using Domain.Entities;
using Api.DTOs.Products;

namespace Api.Controllers;

/// <summary>
/// Ürün varyantları yönetimi için API endpoint'leri
/// </summary>
[ApiController]
[Route("api/products/{productId}/variants")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public sealed class ProductVariantsController : ControllerBase
{
	private readonly IStoreUnitOfWork _unitOfWork;
	private readonly ILogger<ProductVariantsController> _logger;

	public ProductVariantsController(IStoreUnitOfWork unitOfWork, ILogger<ProductVariantsController> logger)
	{
		_unitOfWork = unitOfWork;
		_logger = logger;
	}

	/// <summary>
	/// Ürün varyantlarını listele
	/// </summary>
	/// <param name="productId">Ürün ID'si</param>
	/// <returns>Ürün varyantları listesi</returns>
	/// <response code="200">Varyantlar başarıyla alındı</response>
	/// <response code="404">Ürün bulunamadı</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpGet]
	[AllowAnonymous]
	[ProducesResponseType(typeof(IEnumerable<ProductVariantDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<IEnumerable<ProductVariantDto>>> GetProductVariants(long productId)
	{
		try
		{
			// Ürünün var olup olmadığını kontrol et
			var products = await _unitOfWork.Products.GetAllAsync();
			var product = products.FirstOrDefault(p => p.Id == productId);
			if (product == null)
				return NotFound(new { Message = "Ürün bulunamadı" });

			var variants = await _unitOfWork.ProductVariants.GetAllAsync();
			var productVariants = variants.Where(v => v.ProductId == productId && v.IsActive)
				.OrderBy(v => v.DisplayOrder)
				.ThenBy(v => v.VariantName)
				.ToList();

			var dtos = productVariants.Select(v => new ProductVariantDto
			{
				Id = v.Id,
				ProductId = v.ProductId,
				Sku = v.Sku,
				Barcode = v.Barcode,
				VariantName = v.VariantName,
				Price = v.Price,
				CompareAtPrice = v.CompareAtPrice,
				StockQty = v.StockQty,
				ReservedQty = v.ReservedQty,
				MinOrderQty = v.MinOrderQty,
				MaxOrderQty = v.MaxOrderQty,
				DisplayOrder = v.DisplayOrder,
				IsActive = v.IsActive,
				IsDefault = v.IsDefault,
				Weight = v.Weight,
				CreatedAt = v.CreatedAt,
				ModifiedAt = v.ModifiedAt
			});

			return Ok(dtos);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting product variants for product {ProductId}", productId);
			return StatusCode(500, new { Message = "Ürün varyantları alınırken bir hata oluştu" });
		}
	}

	/// <summary>
	/// Ürün varyantı detayını getir
	/// </summary>
	/// <param name="productId">Ürün ID'si</param>
	/// <param name="variantId">Varyant ID'si</param>
	/// <returns>Varyant detayı</returns>
	/// <response code="200">Varyant başarıyla alındı</response>
	/// <response code="404">Varyant bulunamadı</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpGet("{variantId}")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(ProductVariantDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<ProductVariantDto>> GetProductVariant(long productId, long variantId)
	{
		try
		{
			var variants = await _unitOfWork.ProductVariants.GetAllAsync();
			var variant = variants.FirstOrDefault(v => v.Id == variantId && v.ProductId == productId && v.IsActive);

			if (variant == null)
				return NotFound(new { Message = "Varyant bulunamadı" });

			var dto = new ProductVariantDto
			{
				Id = variant.Id,
				ProductId = variant.ProductId,
				Sku = variant.Sku,
				Barcode = variant.Barcode,
				VariantName = variant.VariantName,
				Price = variant.Price,
				CompareAtPrice = variant.CompareAtPrice,
				StockQty = variant.StockQty,
				ReservedQty = variant.ReservedQty,
				MinOrderQty = variant.MinOrderQty,
				MaxOrderQty = variant.MaxOrderQty,
				DisplayOrder = variant.DisplayOrder,
				IsActive = variant.IsActive,
				IsDefault = variant.IsDefault,
				Weight = variant.Weight,
				CreatedAt = variant.CreatedAt,
				ModifiedAt = variant.ModifiedAt
			};

			return Ok(dto);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting product variant {VariantId} for product {ProductId}", variantId, productId);
			return StatusCode(500, new { Message = "Varyant alınırken bir hata oluştu" });
		}
	}

	/// <summary>
	/// Yeni ürün varyantı ekle
	/// </summary>
	/// <param name="productId">Ürün ID'si</param>
	/// <param name="request">Varyant ekleme bilgileri</param>
	/// <returns>Eklenen varyant</returns>
	/// <response code="201">Varyant başarıyla eklendi</response>
	/// <response code="400">Geçersiz veri</response>
	/// <response code="401">Yetkisiz erişim</response>
	/// <response code="404">Ürün bulunamadı</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpPost]
	[Authorize(Roles = "Seller,Admin")]
	[ProducesResponseType(typeof(ProductVariantDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<ProductVariantDto>> AddProductVariant(long productId, [FromBody] CreateProductVariantRequest request)
	{
		try
		{
			// Ürünün var olup olmadığını ve yetki kontrolü
			var products = await _unitOfWork.Products.GetAllAsync();
			var product = products.FirstOrDefault(p => p.Id == productId);
			if (product == null)
				return NotFound(new { Message = "Ürün bulunamadı" });

			// Yetki kontrolü
			var userId = GetCurrentUserId();
			if (userId == null)
				return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });

			var sellers = await _unitOfWork.Sellers.GetAllAsync();
			var seller = sellers.FirstOrDefault(s => s.UserId == userId);
			if (seller == null || (product.SellerId != seller.Id && !User.IsInRole("Admin")))
				return Forbid();

			// Eğer bu varyant default olarak işaretleniyorsa, diğer varyantları default olmaktan çıkar
			if (request.IsDefault)
			{
				var existingVariants = await _unitOfWork.ProductVariants.GetAllAsync();
				var defaultVariants = existingVariants.Where(v => v.ProductId == productId && v.IsDefault).ToList();
				foreach (var v in defaultVariants)
				{
					v.IsDefault = false;
					v.ModifiedAt = DateTime.UtcNow;
					await _unitOfWork.ProductVariants.UpdateAsync(v);
				}
			}

			var variant = new ProductVariant
			{
				ProductId = productId,
				Sku = request.Sku,
				Barcode = request.Barcode,
				VariantName = request.VariantName,
				Price = request.Price,
				CompareAtPrice = request.CompareAtPrice,
				StockQty = request.StockQty,
				MinOrderQty = request.MinOrderQty,
				MaxOrderQty = request.MaxOrderQty,
				DisplayOrder = request.DisplayOrder,
				IsDefault = request.IsDefault,
				Weight = request.Weight,
				IsActive = true,
				CreatedAt = DateTime.UtcNow
			};

			variant = await _unitOfWork.ProductVariants.AddAsync(variant);
			await _unitOfWork.SaveChangesAsync();

			var dto = new ProductVariantDto
			{
				Id = variant.Id,
				ProductId = variant.ProductId,
				Sku = variant.Sku,
				Barcode = variant.Barcode,
				VariantName = variant.VariantName,
				Price = variant.Price,
				CompareAtPrice = variant.CompareAtPrice,
				StockQty = variant.StockQty,
				ReservedQty = variant.ReservedQty,
				MinOrderQty = variant.MinOrderQty,
				MaxOrderQty = variant.MaxOrderQty,
				DisplayOrder = variant.DisplayOrder,
				IsActive = variant.IsActive,
				IsDefault = variant.IsDefault,
				Weight = variant.Weight,
				CreatedAt = variant.CreatedAt,
				ModifiedAt = variant.ModifiedAt
			};

			return CreatedAtAction(nameof(GetProductVariant), new { productId, variantId = variant.Id }, dto);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error adding product variant for product {ProductId}", productId);
			return StatusCode(500, new { Message = "Varyant eklenirken bir hata oluştu" });
		}
	}

	/// <summary>
	/// Ürün varyantını güncelle
	/// </summary>
	/// <param name="productId">Ürün ID'si</param>
	/// <param name="variantId">Varyant ID'si</param>
	/// <param name="request">Güncellenecek varyant bilgileri</param>
	/// <returns>Güncellenmiş varyant</returns>
	/// <response code="200">Varyant başarıyla güncellendi</response>
	/// <response code="400">Geçersiz veri</response>
	/// <response code="401">Yetkisiz erişim</response>
	/// <response code="404">Varyant bulunamadı</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpPut("{variantId}")]
	[Authorize(Roles = "Seller,Admin")]
	[ProducesResponseType(typeof(ProductVariantDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<ProductVariantDto>> UpdateProductVariant(long productId, long variantId, [FromBody] UpdateProductVariantRequest request)
	{
		try
		{
			var variants = await _unitOfWork.ProductVariants.GetAllAsync();
			var variant = variants.FirstOrDefault(v => v.Id == variantId && v.ProductId == productId);

			if (variant == null)
				return NotFound(new { Message = "Varyant bulunamadı" });

			// Yetki kontrolü
			var products = await _unitOfWork.Products.GetAllAsync();
			var product = products.FirstOrDefault(p => p.Id == productId);
			if (product == null)
				return NotFound(new { Message = "Ürün bulunamadı" });

			var userId = GetCurrentUserId();
			if (userId == null)
				return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });

			var sellers = await _unitOfWork.Sellers.GetAllAsync();
			var seller = sellers.FirstOrDefault(s => s.UserId == userId);
			if (seller == null || (product.SellerId != seller.Id && !User.IsInRole("Admin")))
				return Forbid();

			// Eğer bu varyant default olarak işaretleniyorsa, diğer varyantları default olmaktan çıkar
			if (request.IsDefault && !variant.IsDefault)
			{
				var existingVariants = await _unitOfWork.ProductVariants.GetAllAsync();
				var defaultVariants = existingVariants.Where(v => v.ProductId == productId && v.IsDefault).ToList();
				foreach (var v in defaultVariants)
				{
					v.IsDefault = false;
					v.ModifiedAt = DateTime.UtcNow;
					await _unitOfWork.ProductVariants.UpdateAsync(v);
				}
			}

			variant.Sku = request.Sku;
			variant.Barcode = request.Barcode;
			variant.VariantName = request.VariantName;
			variant.Price = request.Price;
			variant.CompareAtPrice = request.CompareAtPrice;
			variant.StockQty = request.StockQty;
			variant.MinOrderQty = request.MinOrderQty;
			variant.MaxOrderQty = request.MaxOrderQty;
			variant.DisplayOrder = request.DisplayOrder;
			variant.IsDefault = request.IsDefault;
			variant.IsActive = request.IsActive;
			variant.Weight = request.Weight;
			variant.ModifiedAt = DateTime.UtcNow;

			await _unitOfWork.ProductVariants.UpdateAsync(variant);
			await _unitOfWork.SaveChangesAsync();

			var dto = new ProductVariantDto
			{
				Id = variant.Id,
				ProductId = variant.ProductId,
				Sku = variant.Sku,
				Barcode = variant.Barcode,
				VariantName = variant.VariantName,
				Price = variant.Price,
				CompareAtPrice = variant.CompareAtPrice,
				StockQty = variant.StockQty,
				ReservedQty = variant.ReservedQty,
				MinOrderQty = variant.MinOrderQty,
				MaxOrderQty = variant.MaxOrderQty,
				DisplayOrder = variant.DisplayOrder,
				IsActive = variant.IsActive,
				IsDefault = variant.IsDefault,
				Weight = variant.Weight,
				CreatedAt = variant.CreatedAt,
				ModifiedAt = variant.ModifiedAt
			};

			return Ok(dto);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating product variant {VariantId} for product {ProductId}", variantId, productId);
			return StatusCode(500, new { Message = "Varyant güncellenirken bir hata oluştu" });
		}
	}

	/// <summary>
	/// Stok miktarını güncelle
	/// </summary>
	/// <param name="productId">Ürün ID'si</param>
	/// <param name="variantId">Varyant ID'si</param>
	/// <param name="stockQty">Yeni stok miktarı</param>
	/// <returns>Güncelleme sonucu</returns>
	/// <response code="200">Stok miktarı güncellendi</response>
	/// <response code="400">Geçersiz stok miktarı</response>
	/// <response code="401">Yetkisiz erişim</response>
	/// <response code="404">Varyant bulunamadı</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpPatch("{variantId}/stock")]
	[Authorize(Roles = "Seller,Admin")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> UpdateStock(long productId, long variantId, [FromBody] int stockQty)
	{
		try
		{
			if (stockQty < 0)
				return BadRequest(new { Message = "Stok miktarı negatif olamaz" });

			var variants = await _unitOfWork.ProductVariants.GetAllAsync();
			var variant = variants.FirstOrDefault(v => v.Id == variantId && v.ProductId == productId);

			if (variant == null)
				return NotFound(new { Message = "Varyant bulunamadı" });

			// Yetki kontrolü
			var products = await _unitOfWork.Products.GetAllAsync();
			var product = products.FirstOrDefault(p => p.Id == productId);
			if (product == null)
				return NotFound(new { Message = "Ürün bulunamadı" });

			var userId = GetCurrentUserId();
			if (userId == null)
				return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });

			var sellers = await _unitOfWork.Sellers.GetAllAsync();
			var seller = sellers.FirstOrDefault(s => s.UserId == userId);
			if (seller == null || (product.SellerId != seller.Id && !User.IsInRole("Admin")))
				return Forbid();

			variant.StockQty = stockQty;
			variant.ModifiedAt = DateTime.UtcNow;

			await _unitOfWork.ProductVariants.UpdateAsync(variant);
			await _unitOfWork.SaveChangesAsync();

			return Ok(new { Message = "Stok miktarı güncellendi", NewStockQty = stockQty });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating stock for variant {VariantId} of product {ProductId}", variantId, productId);
			return StatusCode(500, new { Message = "Stok miktarı güncellenirken bir hata oluştu" });
		}
	}

	/// <summary>
	/// Ürün varyantını sil (soft delete)
	/// </summary>
	/// <param name="productId">Ürün ID'si</param>
	/// <param name="variantId">Varyant ID'si</param>
	/// <returns>Silme işlemi sonucu</returns>
	/// <response code="200">Varyant başarıyla silindi</response>
	/// <response code="401">Yetkisiz erişim</response>
	/// <response code="404">Varyant bulunamadı</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpDelete("{variantId}")]
	[Authorize(Roles = "Seller,Admin")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteProductVariant(long productId, long variantId)
	{
		try
		{
			var variants = await _unitOfWork.ProductVariants.GetAllAsync();
			var variant = variants.FirstOrDefault(v => v.Id == variantId && v.ProductId == productId);

			if (variant == null)
				return NotFound(new { Message = "Varyant bulunamadı" });

			// Yetki kontrolü
			var products = await _unitOfWork.Products.GetAllAsync();
			var product = products.FirstOrDefault(p => p.Id == productId);
			if (product == null)
				return NotFound(new { Message = "Ürün bulunamadı" });

			var userId = GetCurrentUserId();
			if (userId == null)
				return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });

			var sellers = await _unitOfWork.Sellers.GetAllAsync();
			var seller = sellers.FirstOrDefault(s => s.UserId == userId);
			if (seller == null || (product.SellerId != seller.Id && !User.IsInRole("Admin")))
				return Forbid();

			// Eğer silinecek varyant default ise, başka bir varyantı default yap
			if (variant.IsDefault)
			{
				var otherVariants = variants.Where(v => v.ProductId == productId && v.Id != variantId && v.IsActive).ToList();
				if (otherVariants.Any())
				{
					var newDefault = otherVariants.OrderBy(v => v.VariantName).First();
					newDefault.IsDefault = true;
					newDefault.ModifiedAt = DateTime.UtcNow;
					await _unitOfWork.ProductVariants.UpdateAsync(newDefault);
				}
			}

			await _unitOfWork.ProductVariants.DeleteAsync(variantId);
			await _unitOfWork.SaveChangesAsync();

			return Ok(new { Message = "Varyant silindi" });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting product variant {VariantId} for product {ProductId}", variantId, productId);
			return StatusCode(500, new { Message = "Varyant silinirken bir hata oluştu" });
		}
	}

	/// <summary>
	/// Mevcut kullanıcının ID'sini al
	/// </summary>
	/// <returns>Kullanıcı ID'si veya null</returns>
	private long? GetCurrentUserId()
	{
		var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
		if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
		{
			return userId;
		}
		return null;
	}
}
