using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Persistence.Repositories;
using Domain.Entities;
using Api.DTOs.Products;
using System.Text.RegularExpressions;

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
	private readonly IStoreUnitOfWork _unitOfWork;
	private readonly ILogger<CategoriesController> _logger;

	public CategoriesController(IStoreUnitOfWork unitOfWork, ILogger<CategoriesController> logger)
	{
		_unitOfWork = unitOfWork;
		_logger = logger;
	}

	/// <summary>
	/// Kategorileri listele ve filtrele
	/// </summary>
	/// <param name="request">Arama ve filtreleme parametreleri</param>
	/// <returns>Sayfalanmış kategori listesi</returns>
	/// <response code="200">Kategoriler başarıyla alındı</response>
	/// <response code="500">Sunucu hatası</response>
	[HttpGet]
	[AllowAnonymous]
	[ProducesResponseType(typeof(CategorySearchResponse), StatusCodes.Status200OK)]
	public async Task<ActionResult<CategorySearchResponse>> GetAll([FromQuery] CategorySearchRequest request)
	{
		try
		{
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1 || request.PageSize > 200) request.PageSize = 50;

			var categories = await _unitOfWork.Categories.GetAllAsync();

			var query = categories.AsQueryable();

			if (!string.IsNullOrWhiteSpace(request.SearchTerm))
			{
				var term = request.SearchTerm.ToLowerInvariant();
				query = query.Where(c =>
					c.Name.ToLowerInvariant().Contains(term) ||
					(c.Slug ?? string.Empty).ToLowerInvariant().Contains(term));
			}

			if (request.ParentId.HasValue)
			{
				query = query.Where(c => c.ParentId == request.ParentId.Value);
			}

			if (request.IsActive.HasValue)
			{
				query = query.Where(c => c.IsActive == request.IsActive.Value);
			}

			if (request.IsFeatured.HasValue)
			{
				query = query.Where(c => c.IsFeatured == request.IsFeatured.Value);
			}

			query = request.SortBy?.ToLowerInvariant() switch
			{
				"name" => request.SortOrder?.ToLowerInvariant() == "desc" ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
				"createdat" => request.SortOrder?.ToLowerInvariant() == "desc" ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
				"displayorder" => request.SortOrder?.ToLowerInvariant() == "desc" ? query.OrderByDescending(c => c.DisplayOrder) : query.OrderBy(c => c.DisplayOrder),
				_ => query.OrderBy(c => c.DisplayOrder)
			};

			var totalCount = query.Count();
			var pageItems = query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();

			var result = new CategorySearchResponse
			{
				Categories = pageItems.Select(c => new CategoryDto
				{
					Id = c.Id,
					ParentId = c.ParentId,
					Name = c.Name,
					Slug = c.Slug,
					Description = c.Description,
					ImageUrl = c.ImageUrl,
					IconClass = c.IconClass,
					IsActive = c.IsActive,
					IsFeatured = c.IsFeatured,
					DisplayOrder = c.DisplayOrder,
					MetaTitle = c.MetaTitle,
					MetaDescription = c.MetaDescription,
					CreatedAt = c.CreatedAt,
					ModifiedAt = c.ModifiedAt,
					ProductCount = 0
				}).ToList(),
				TotalCount = totalCount,
				Page = request.Page,
				PageSize = request.PageSize
			};

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
	[ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<CategoryDto>> GetById(long id)
	{
		try
		{
			var category = await _unitOfWork.Categories.GetByIdAsync(id);
			if (category == null) return NotFound(new { Message = "Kategori bulunamadı" });

			var dto = new CategoryDto
			{
				Id = category.Id,
				ParentId = category.ParentId,
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
				ModifiedAt = category.ModifiedAt,
				ProductCount = 0
			};

			return Ok(dto);
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
			var categories = (await _unitOfWork.Categories.GetAllAsync()).ToList();
			var map = categories.ToDictionary(c => c.Id, c => new CategoryDto
			{
				Id = c.Id,
				ParentId = c.ParentId,
				Name = c.Name,
				Slug = c.Slug,
				Description = c.Description,
				ImageUrl = c.ImageUrl,
				IconClass = c.IconClass,
				IsActive = c.IsActive,
				IsFeatured = c.IsFeatured,
				DisplayOrder = c.DisplayOrder,
				MetaTitle = c.MetaTitle,
				MetaDescription = c.MetaDescription,
				CreatedAt = c.CreatedAt,
				ModifiedAt = c.ModifiedAt,
				Children = new List<CategoryDto>()
			});

			List<CategoryDto> roots = new();
			foreach (var cat in categories)
			{
				if (cat.ParentId.HasValue && map.ContainsKey(cat.ParentId.Value))
				{
					map[cat.ParentId.Value].Children.Add(map[cat.Id]);
				}
				else
				{
					roots.Add(map[cat.Id]);
				}
			}

			return Ok(roots.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name));
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
	[ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryRequest request)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(request.Name))
				return BadRequest(new { Message = "Kategori adı gereklidir" });

			var slug = string.IsNullOrWhiteSpace(request.Slug) ? GenerateSlug(request.Name) : GenerateSlug(request.Slug);

			var category = new Category
			{
				ParentId = request.ParentId,
				Name = request.Name,
				Slug = slug,
				Description = request.Description,
				ImageUrl = request.ImageUrl,
				IconClass = request.IconClass,
				IsActive = request.IsActive,
				IsFeatured = request.IsFeatured,
				DisplayOrder = request.DisplayOrder,
				MetaTitle = request.MetaTitle,
				MetaDescription = request.MetaDescription,
				CreatedAt = DateTime.UtcNow
			};

			category = await _unitOfWork.Categories.AddAsync(category);
			await _unitOfWork.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = category.Id }, await GetById(category.Id));
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
	[ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<CategoryDto>> Update(long id, [FromBody] UpdateCategoryRequest request)
	{
		try
		{
			var category = await _unitOfWork.Categories.GetByIdAsync(id);
			if (category == null) return NotFound(new { Message = "Kategori bulunamadı" });

			category.ParentId = request.ParentId;
			category.Name = request.Name;
			category.Slug = string.IsNullOrWhiteSpace(request.Slug) ? GenerateSlug(request.Name) : GenerateSlug(request.Slug);
			category.Description = request.Description;
			category.ImageUrl = request.ImageUrl;
			category.IconClass = request.IconClass;
			category.IsActive = request.IsActive;
			category.IsFeatured = request.IsFeatured;
			category.DisplayOrder = request.DisplayOrder;
			category.MetaTitle = request.MetaTitle;
			category.MetaDescription = request.MetaDescription;
			category.ModifiedAt = DateTime.UtcNow;

			await _unitOfWork.Categories.UpdateAsync(category);
			await _unitOfWork.SaveChangesAsync();

			return Ok((await GetById(id)).Value);
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
			var category = await _unitOfWork.Categories.GetByIdAsync(id);
			if (category == null) return NotFound(new { Message = "Kategori bulunamadı" });

			await _unitOfWork.Categories.DeleteAsync(id);
			await _unitOfWork.SaveChangesAsync();

			return Ok(new { Message = "Kategori silindi" });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting category {Id}", id);
			return StatusCode(500, new { Message = "Kategori silinirken bir hata oluştu" });
		}
	}

	/// <summary>
	/// Metin için SEO dostu slug oluştur
	/// </summary>
	/// <param name="name">Slug oluşturulacak metin</param>
	/// <returns>SEO dostu slug</returns>
	private static string GenerateSlug(string name)
	{
		var slug = name.ToLowerInvariant()
			.Replace("ç", "c").Replace("ğ", "g").Replace("ı", "i")
			.Replace("ö", "o").Replace("ş", "s").Replace("ü", "u");
		slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
		slug = Regex.Replace(slug, @"\s+", "-");
		slug = Regex.Replace(slug, @"-+", "-");
		return slug.Trim('-');
	}
}
