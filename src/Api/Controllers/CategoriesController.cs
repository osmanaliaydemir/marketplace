using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Abstractions;
using Application.DTOs.Categories;

namespace Api.Controllers;

/// <summary>
/// Kategori yönetimi için API endpoint'leri
/// </summary>
[ApiController]
[Route("api/categories")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public sealed class CategoriesController : ControllerBase
{
	private readonly ICategoryService _categoryService;
	private readonly ILogger<CategoriesController> _logger;

	public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
	{
		_categoryService = categoryService;
		_logger = logger;
	}

	/// <summary>
	/// Basit kategori listesi (dropdown için)
	/// </summary>
	/// <returns>Aktif kategorilerin basit listesi</returns>
	/// <response code="200">Kategoriler başarıyla alındı</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpGet]
	[AllowAnonymous]
	[ProducesResponseType(typeof(IEnumerable<CategoryOptionDto>), StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<CategoryOptionDto>>> GetAll()
	{
		try
		{
			var categories = await _categoryService.GetCategoryOptionsAsync();
			return Ok(categories);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting category options");
			return StatusCode(500, new { Message = "Kategoriler alınırken bir hata oluştu" });
		}
	}

	/// <summary>
	/// Kategorileri listele ve filtrele (detaylı arama)
	/// </summary>
	/// <param name="request">Arama ve filtreleme parametreleri</param>
	/// <returns>Sayfalanmış kategori listesi</returns>
	/// <response code="200">Kategoriler başarıyla alındı</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpGet("search")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(CategoryListResponse), StatusCodes.Status200OK)]
	public async Task<ActionResult<CategoryListResponse>> Search([FromQuery] CategorySearchRequest request)
	{
		try
		{
			// Convert API request to Application request
			var listRequest = new CategoryListRequest
			{
				SearchTerm = request.SearchTerm,
				ParentId = request.ParentId,
				IsActive = request.IsActive,
				IsFeatured = request.IsFeatured,
				Page = request.Page < 1 ? 1 : request.Page,
				PageSize = request.PageSize < 1 || request.PageSize > 200 ? 50 : request.PageSize,
				SortBy = request.SortBy,
				SortOrder = request.SortOrder
			};

			var result = await _categoryService.ListAsync(listRequest);
			return Ok(result);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error listing categories");
			return StatusCode(500, new { Message = "Kategoriler alınırken bir hata oluştu" });
		}
	}

	/// <summary>
	/// ID ile kategori detayını getir
	/// </summary>
	/// <param name="id">Kategori ID'si</param>
	/// <returns>Kategori detayı</returns>
	/// <response code="200">Kategori başarıyla alındı</response>
	/// <response code="404">Kategori bulunamadı</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpGet("{id}")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(CategoryDetailDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<CategoryDetailDto>> GetById(long id)
	{
		try
		{
			var category = await _categoryService.GetByIdAsync(id);
			if (category == null) return NotFound(new { Message = "Kategori bulunamadı" });

			return Ok(category);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting category by id {Id}", id);
			return StatusCode(500, new { Message = "Kategori alınırken bir hata oluştu" });
		}
	}

	/// <summary>
	/// Hiyerarşik kategori ağacını getir
	/// </summary>
	/// <returns>Kategori ağacı (parent-child ilişkisi ile)</returns>
	/// <response code="200">Kategori ağacı başarıyla alındı</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpGet("tree")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<CategoryDto>>> GetTree()
	{
		try
		{
			var categories = await _categoryService.GetCategoryTreeAsync();
			return Ok(categories);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting category tree");
			return StatusCode(500, new { Message = "Kategori ağacı alınırken bir hata oluştu" });
		}
	}

	/// <summary>
	/// Yeni kategori oluştur
	/// </summary>
	/// <param name="request">Kategori oluşturma bilgileri</param>
	/// <returns>Oluşturulan kategori</returns>
	/// <response code="201">Kategori başarıyla oluşturuldu</response>
	/// <response code="400">Geçersiz veri</response>
	/// <response code="401">Yetkisiz erişim</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpPost]
	[Authorize(Roles = "Admin")]
	[ProducesResponseType(typeof(CategoryDetailDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<CategoryDetailDto>> Create([FromBody] CategoryCreateRequest request)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(request.Name))
				return BadRequest(new { Message = "Kategori adı gereklidir" });

			var category = await _categoryService.CreateAsync(request);
			return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error creating category");
			return StatusCode(500, new { Message = "Kategori oluşturulurken bir hata oluştu" });
		}
	}

	/// <summary>
	/// Kategori güncelle
	/// </summary>
	/// <param name="id">Kategori ID'si</param>
	/// <param name="request">Güncellenecek kategori bilgileri</param>
	/// <returns>Güncellenmiş kategori</returns>
	/// <response code="200">Kategori başarıyla güncellendi</response>
	/// <response code="400">Geçersiz veri</response>
	/// <response code="401">Yetkisiz erişim</response>
	/// <response code="404">Kategori bulunamadı</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpPut("{id}")]
	[Authorize(Roles = "Admin")]
	[ProducesResponseType(typeof(CategoryDetailDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<CategoryDetailDto>> Update(long id, [FromBody] CategoryUpdateRequest request)
	{
		try
		{
			var category = await _categoryService.UpdateAsync(id, request);
			return Ok(category);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating category {Id}", id);
			return StatusCode(500, new { Message = "Kategori güncellenirken bir hata oluştu" });
		}
	}

	/// <summary>
	/// Kategori sil (soft delete)
	/// </summary>
	/// <param name="id">Kategori ID'si</param>
	/// <returns>Silme işlemi sonucu</returns>
	/// <response code="200">Kategori başarıyla silindi</response>
	/// <response code="401">Yetkisiz erişim</response>
	/// <response code="404">Kategori bulunamadı</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpDelete("{id}")]
	[Authorize(Roles = "Admin")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Delete(long id)
	{
		try
		{
			var result = await _categoryService.DeleteAsync(id);
			if (!result) return NotFound(new { Message = "Kategori bulunamadı" });

			return Ok(new { Message = "Kategori silindi" });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting category {Id}", id);
			return StatusCode(500, new { Message = "Kategori silinirken bir hata oluştu" });
		}
	}

}
