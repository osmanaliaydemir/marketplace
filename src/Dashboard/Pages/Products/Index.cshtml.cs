using Dashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.DTOs.Products;
using Application.DTOs.Categories;

namespace Dashboard.Pages.Products;

public class IndexModel : PageModel
{
	private readonly ApiClient _api;

	public IndexModel(ApiClient api)
	{
		_api = api;
	}

	[BindProperty(SupportsGet = true)]
	public ProductSearchRequest SearchRequest { get; set; } = new();

	public ProductSearchResponse SearchResponse { get; set; } = new();
	public List<CategoryDto> Categories { get; set; } = new();

	public async Task OnGetAsync()
	{
		var token = HttpContext.Session.GetString("AuthToken");
		if (string.IsNullOrEmpty(token))
		{
			SearchResponse = new ProductSearchResponse();
			return;
		}

		_api.SetBearer(token);

		// Kategorileri al
		try
		{
			Categories = await _api.GetAsync<List<CategoryDto>>("/api/categories") ?? new();
		}
		catch
		{
			Categories = new();
		}

		var query = new Dictionary<string, string?>
		{
			["searchTerm"] = SearchRequest.SearchTerm,
			["categoryId"] = SearchRequest.CategoryId?.ToString(),
			["storeId"] = SearchRequest.StoreId?.ToString(),
			["minPrice"] = SearchRequest.MinPrice?.ToString(System.Globalization.CultureInfo.InvariantCulture),
			["maxPrice"] = SearchRequest.MaxPrice?.ToString(System.Globalization.CultureInfo.InvariantCulture),
			["inStock"] = SearchRequest.InStock?.ToString(),
			["isFeatured"] = SearchRequest.IsFeatured?.ToString(),
			["sortBy"] = SearchRequest.SortBy,
			["sortOrder"] = SearchRequest.SortOrder,
			["page"] = SearchRequest.Page.ToString(),
			["pageSize"] = SearchRequest.PageSize.ToString()
		};

		var queryString = string.Join("&", query.Where(kv => !string.IsNullOrEmpty(kv.Value)).Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value!)}"));
		var url = $"/api/products?{queryString}";

		var res = await _api.GetAsync<ProductSearchResponse>(url);
		if (res != null)
		{
			SearchResponse = res;
		}
	}

	public async Task<IActionResult> OnPostToggleStatusAsync(long id, bool isActive, bool isPublished, bool isFeatured)
	{
		var token = HttpContext.Session.GetString("AuthToken");
		if (string.IsNullOrEmpty(token))
		{
			TempData["Error"] = "Oturum süresi dolmuş. Lütfen giriş yapın.";
			return RedirectToPage("/Login");
		}

		_api.SetBearer(token);
		await _api.PatchAsync<object, object>($"/api/products/{id}/status", new { isActive, isPublished, isFeatured });
		TempData["Success"] = "Ürün durumu güncellendi.";
		return RedirectToPage();
	}
}

public sealed class ProductSearchRequest
{
	public string? SearchTerm { get; set; }
	public long? CategoryId { get; set; }
	public long? StoreId { get; set; }
	public decimal? MinPrice { get; set; }
	public decimal? MaxPrice { get; set; }
	public bool? InStock { get; set; }
	public bool? IsFeatured { get; set; }
	public string? SortBy { get; set; } = "CreatedAt";
	public string? SortOrder { get; set; } = "Desc";
	public int Page { get; set; } = 1;
	public int PageSize { get; set; } = 20;
}

public sealed class ProductListDto
{
	public long Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Slug { get; set; } = string.Empty;
	public string? ShortDescription { get; set; }
	public decimal Price { get; set; }
	public decimal? CompareAtPrice { get; set; }
	public string Currency { get; set; } = "TRY";
	public int StockQty { get; set; }
	public bool IsActive { get; set; }
	public bool IsFeatured { get; set; }
	public string? PrimaryImageUrl { get; set; }
	public string CategoryName { get; set; } = string.Empty;
	public string StoreName { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
}

public sealed class ProductSearchResponse
{
	public List<ProductListDto> Products { get; set; } = new();
	public int TotalCount { get; set; }
	public int Page { get; set; }
	public int PageSize { get; set; }
	public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

// DTO'lar Application layer'dan geliyor, burada tekrar tanımlanmamalı
