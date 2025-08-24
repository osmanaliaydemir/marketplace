using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Persistence.Repositories;
using Domain.Entities;
using Application.DTOs.Products;

namespace Api.Controllers;

/// <summary>
/// Ürün resimleri yönetimi için API endpoint'leri
/// </summary>
[ApiController]
[Route("api/products/{productId}/images")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public sealed class ProductImagesController : ControllerBase
{
	private readonly IStoreUnitOfWork _unitOfWork;
	private readonly ILogger<ProductImagesController> _logger;

	public ProductImagesController(IStoreUnitOfWork unitOfWork, ILogger<ProductImagesController> logger)
	{
		_unitOfWork = unitOfWork;
		_logger = logger;
	}

	/// <summary>
	/// Ürün resimlerini listele
	/// </summary>
	/// <param name="productId">Ürün ID'si</param>
	/// <returns>Ürün resimleri listesi</returns>
	/// <response code="200">Resimler başarıyla alındı</response>
	/// <response code="404">Ürün bulunamadı</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpGet]
	[AllowAnonymous]
	[ProducesResponseType(typeof(IEnumerable<ProductImageDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<IEnumerable<ProductImageDto>>> GetProductImages(long productId)
	{
		try
		{
			// Ürünün var olup olmadığını kontrol et
			var products = await _unitOfWork.Products.GetAllAsync();
			var product = products.FirstOrDefault(p => p.Id == productId);
			if (product == null)
				return NotFound(new { Message = "Ürün bulunamadı" });

			var images = await _unitOfWork.ProductImages.GetAllAsync();
			var productImages = images.Where(img => img.ProductId == productId && img.IsActive)
				.OrderBy(img => img.DisplayOrder)
				.ToList();

			var dtos = productImages.Select(img => new ProductImageDto
			{
				Id = img.Id,
				ProductId = img.ProductId,
				ImageUrl = img.ImageUrl,
				ThumbnailUrl = img.ThumbnailUrl,
				AltText = img.AltText,
				Title = img.Title,
				DisplayOrder = img.DisplayOrder,
				IsPrimary = img.IsPrimary,
				IsActive = img.IsActive,
				CreatedAt = img.CreatedAt,
				ModifiedAt = img.ModifiedAt
			});

			return Ok(dtos);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting product images for product {ProductId}", productId);
			return StatusCode(500, new { Message = "Ürün resimleri alınırken bir hata oluştu" });
		}
	}

	/// <summary>
	/// Ürün resmi detayını getir
	/// </summary>
	/// <param name="productId">Ürün ID'si</param>
	/// <param name="imageId">Resim ID'si</param>
	/// <returns>Resim detayı</returns>
	/// <response code="200">Resim başarıyla alındı</response>
	/// <response code="404">Resim bulunamadı</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpGet("{imageId}")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(ProductImageDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<ProductImageDto>> GetProductImage(long productId, long imageId)
	{
		try
		{
			var images = await _unitOfWork.ProductImages.GetAllAsync();
			var image = images.FirstOrDefault(img => img.Id == imageId && img.ProductId == productId && img.IsActive);

			if (image == null)
				return NotFound(new { Message = "Resim bulunamadı" });

			var dto = new ProductImageDto
			{
				Id = image.Id,
				ProductId = image.ProductId,
				ImageUrl = image.ImageUrl,
				ThumbnailUrl = image.ThumbnailUrl,
				AltText = image.AltText,
				Title = image.Title,
				DisplayOrder = image.DisplayOrder,
				IsPrimary = image.IsPrimary,
				IsActive = image.IsActive,
				CreatedAt = image.CreatedAt,
				ModifiedAt = image.ModifiedAt
			};

			return Ok(dto);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting product image {ImageId} for product {ProductId}", imageId, productId);
			return StatusCode(500, new { Message = "Resim alınırken bir hata oluştu" });
		}
	}

	/// <summary>
	/// Yeni ürün resmi ekle
	/// </summary>
	/// <param name="productId">Ürün ID'si</param>
	/// <param name="request">Resim ekleme bilgileri</param>
	/// <returns>Eklenen resim</returns>
	/// <response code="201">Resim başarıyla eklendi</response>
	/// <response code="400">Geçersiz veri</response>
	/// <response code="401">Yetkisiz erişim</response>
	/// <response code="404">Ürün bulunamadı</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpPost]
	[Authorize(Roles = "Seller,Admin")]
	[ProducesResponseType(typeof(ProductImageDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<ProductImageDto>> AddProductImage(long productId, [FromBody] CreateProductImageRequest request)
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

			// Eğer bu resim primary olarak işaretleniyorsa, diğer resimleri primary olmaktan çıkar
			if (request.IsPrimary)
			{
				var existingImages = await _unitOfWork.ProductImages.GetAllAsync();
				var primaryImages = existingImages.Where(img => img.ProductId == productId && img.IsPrimary).ToList();
				foreach (var img in primaryImages)
				{
					img.IsPrimary = false;
					img.ModifiedAt = DateTime.UtcNow;
					await _unitOfWork.ProductImages.UpdateAsync(img);
				}
			}

			var image = new ProductImage
			{
				ProductId = productId,
				ImageUrl = request.ImageUrl,
				ThumbnailUrl = request.ThumbnailUrl,
				AltText = request.AltText,
				Title = request.Title,
				DisplayOrder = request.DisplayOrder,
				IsPrimary = request.IsPrimary,
				IsActive = true,
				CreatedAt = DateTime.UtcNow
			};

			image = await _unitOfWork.ProductImages.AddAsync(image);
			await _unitOfWork.SaveChangesAsync();

			var dto = new ProductImageDto
			{
				Id = image.Id,
				ProductId = image.ProductId,
				ImageUrl = image.ImageUrl,
				ThumbnailUrl = image.ThumbnailUrl,
				AltText = image.AltText,
				Title = image.Title,
				DisplayOrder = image.DisplayOrder,
				IsPrimary = image.IsPrimary,
				IsActive = image.IsActive,
				CreatedAt = image.CreatedAt,
				ModifiedAt = image.ModifiedAt
			};

			return CreatedAtAction(nameof(GetProductImage), new { productId, imageId = image.Id }, dto);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error adding product image for product {ProductId}", productId);
			return StatusCode(500, new { Message = "Resim eklenirken bir hata oluştu" });
		}
	}

	/// <summary>
	/// Ürün resmini güncelle
	/// </summary>
	/// <param name="productId">Ürün ID'si</param>
	/// <param name="imageId">Resim ID'si</param>
	/// <param name="request">Güncellenecek resim bilgileri</param>
	/// <returns>Güncellenmiş resim</returns>
	/// <response code="200">Resim başarıyla güncellendi</response>
	/// <response code="400">Geçersiz veri</response>
	/// <response code="401">Yetkisiz erişim</response>
	/// <response code="404">Resim bulunamadı</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpPut("{imageId}")]
	[Authorize(Roles = "Seller,Admin")]
	[ProducesResponseType(typeof(ProductImageDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<ProductImageDto>> UpdateProductImage(long productId, long imageId, [FromBody] UpdateProductImageRequest request)
	{
		try
		{
			var images = await _unitOfWork.ProductImages.GetAllAsync();
			var image = images.FirstOrDefault(img => img.Id == imageId && img.ProductId == productId);

			if (image == null)
				return NotFound(new { Message = "Resim bulunamadı" });

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

			// Eğer bu resim primary olarak işaretleniyorsa, diğer resimleri primary olmaktan çıkar
			if (request.IsPrimary && !image.IsPrimary)
			{
				var existingImages = await _unitOfWork.ProductImages.GetAllAsync();
				var primaryImages = existingImages.Where(img => img.ProductId == productId && img.IsPrimary).ToList();
				foreach (var img in primaryImages)
				{
					img.IsPrimary = false;
					img.ModifiedAt = DateTime.UtcNow;
					await _unitOfWork.ProductImages.UpdateAsync(img);
				}
			}

			image.ImageUrl = request.ImageUrl;
			image.ThumbnailUrl = request.ThumbnailUrl;
			image.AltText = request.AltText;
			image.Title = request.Title;
			image.DisplayOrder = request.DisplayOrder;
			image.IsPrimary = request.IsPrimary;
			image.IsActive = request.IsActive;
			image.ModifiedAt = DateTime.UtcNow;

			await _unitOfWork.ProductImages.UpdateAsync(image);
			await _unitOfWork.SaveChangesAsync();

			var dto = new ProductImageDto
			{
				Id = image.Id,
				ProductId = image.ProductId,
				ImageUrl = image.ImageUrl,
				ThumbnailUrl = image.ThumbnailUrl,
				AltText = image.AltText,
				Title = image.Title,
				DisplayOrder = image.DisplayOrder,
				IsPrimary = image.IsPrimary,
				IsActive = image.IsActive,
				CreatedAt = image.CreatedAt,
				ModifiedAt = image.ModifiedAt
			};

			return Ok(dto);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating product image {ImageId} for product {ProductId}", imageId, productId);
			return StatusCode(500, new { Message = "Resim güncellenirken bir hata oluştu" });
		}
	}

	/// <summary>
	/// Resim sıralamasını güncelle
	/// </summary>
	/// <param name="productId">Ürün ID'si</param>
	/// <param name="request">Sıralanmış resim ID'leri</param>
	/// <returns>Güncelleme sonucu</returns>
	/// <response code="200">Sıralama başarıyla güncellendi</response>
	/// <response code="400">Geçersiz veri</response>
	/// <response code="401">Yetkisiz erişim</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpPut("order")]
	[Authorize(Roles = "Seller,Admin")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> UpdateImageOrder(long productId, [FromBody] UpdateProductImageOrderRequest request)
	{
		try
		{
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

			var images = await _unitOfWork.ProductImages.GetAllAsync();
			var productImages = images.Where(img => img.ProductId == productId).ToList();

			// Sıralama güncelle
			for (int i = 0; i < request.ImageIds.Count; i++)
			{
				var image = productImages.FirstOrDefault(img => img.Id == request.ImageIds[i]);
				if (image != null)
				{
					image.DisplayOrder = i + 1;
					image.ModifiedAt = DateTime.UtcNow;
					await _unitOfWork.ProductImages.UpdateAsync(image);
				}
			}

			await _unitOfWork.SaveChangesAsync();

			return Ok(new { Message = "Resim sıralaması güncellendi" });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating image order for product {ProductId}", productId);
			return StatusCode(500, new { Message = "Resim sıralaması güncellenirken bir hata oluştu" });
		}
	}

	/// <summary>
	/// Ürün resmini sil (soft delete)
	/// </summary>
	/// <param name="productId">Ürün ID'si</param>
	/// <param name="imageId">Resim ID'si</param>
	/// <returns>Silme işlemi sonucu</returns>
	/// <response code="200">Resim başarıyla silindi</response>
	/// <response code="401">Yetkisiz erişim</response>
	/// <response code="404">Resim bulunamadı</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpDelete("{imageId}")]
	[Authorize(Roles = "Seller,Admin")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteProductImage(long productId, long imageId)
	{
		try
		{
			var images = await _unitOfWork.ProductImages.GetAllAsync();
			var image = images.FirstOrDefault(img => img.Id == imageId && img.ProductId == productId);

			if (image == null)
				return NotFound(new { Message = "Resim bulunamadı" });

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

			// Eğer silinecek resim primary ise, başka bir resmi primary yap
			if (image.IsPrimary)
			{
				var otherImages = images.Where(img => img.ProductId == productId && img.Id != imageId && img.IsActive).ToList();
				if (otherImages.Any())
				{
					var newPrimary = otherImages.OrderBy(img => img.DisplayOrder).First();
					newPrimary.IsPrimary = true;
					newPrimary.ModifiedAt = DateTime.UtcNow;
					await _unitOfWork.ProductImages.UpdateAsync(newPrimary);
				}
			}

			await _unitOfWork.ProductImages.DeleteAsync(imageId);
			await _unitOfWork.SaveChangesAsync();

			return Ok(new { Message = "Resim silindi" });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting product image {ImageId} for product {ProductId}", imageId, productId);
			return StatusCode(500, new { Message = "Resim silinirken bir hata oluştu" });
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
