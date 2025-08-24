using Dashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.DTOs.Categories;

namespace Dashboard.Pages.Categories;

public class IndexModel : PageModel
{
	private readonly ApiClient _api;

	public IndexModel(ApiClient api)
	{
		_api = api;
	}

	[BindProperty(SupportsGet = true)]
	public CategorySearchRequest SearchRequest { get; set; } = new();

	public CategorySearchResponse SearchResponse { get; set; } = new();

	public async Task OnGetAsync()
	{
		var token = HttpContext.Session.GetString("AuthToken");
		if (string.IsNullOrEmpty(token))
		{
			SearchResponse = new CategorySearchResponse();
			return;
		}

		_api.SetBearer(token);

		var query = new Dictionary<string, string?>
		{
			["searchTerm"] = SearchRequest.SearchTerm,
			["parentId"] = SearchRequest.ParentId?.ToString(),
			["isActive"] = SearchRequest.IsActive?.ToString(),
			["isFeatured"] = SearchRequest.IsFeatured?.ToString(),
			["sortBy"] = SearchRequest.SortBy,
			["sortOrder"] = SearchRequest.SortOrder,
			["page"] = SearchRequest.Page.ToString(),
			["pageSize"] = SearchRequest.PageSize.ToString()
		};

		var queryString = string.Join("&", query.Where(kv => !string.IsNullOrEmpty(kv.Value)).Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value!)}"));
		var url = $"/api/categories?{queryString}";

		var res = await _api.GetAsync<CategorySearchResponse>(url);
		if (res != null)
		{
			SearchResponse = res;
		}
	}

	public async Task<IActionResult> OnPostToggleStatusAsync(long id, bool isActive, bool isFeatured)
	{
		var token = HttpContext.Session.GetString("AuthToken");
		if (string.IsNullOrEmpty(token))
		{
			TempData["Error"] = "Oturum süresi dolmuş. Lütfen giriş yapın.";
			return RedirectToPage("/Login");
		}

		_api.SetBearer(token);
		await _api.PatchAsync<object, object>($"/api/categories/{id}/status", new { isActive, isFeatured });
		TempData["Success"] = "Kategori durumu güncellendi.";
		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostDeleteAsync(long id)
	{
		var token = HttpContext.Session.GetString("AuthToken");
		if (string.IsNullOrEmpty(token))
		{
			TempData["Error"] = "Oturum süresi dolmuş. Lütfen giriş yapın.";
			return RedirectToPage("/Login");
		}

		_api.SetBearer(token);
		await _api.DeleteAsync($"/api/categories/{id}");
		TempData["Success"] = "Kategori silindi.";
		return RedirectToPage();
	}
}

public sealed class CategorySearchRequest
{
	public string? SearchTerm { get; set; }
	public long? ParentId { get; set; }
	public bool? IsActive { get; set; }
	public bool? IsFeatured { get; set; }
	public string? SortBy { get; set; } = "DisplayOrder";
	public string? SortOrder { get; set; } = "Asc";
	public int Page { get; set; } = 1;
	public int PageSize { get; set; } = 20;
}

public sealed class CategorySearchResponse
{
	public List<CategoryDto> Categories { get; set; } = new();
	public int TotalCount { get; set; }
	public int Page { get; set; }
	public int PageSize { get; set; }
	public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

// DTO'lar Application layer'dan geliyor, burada tekrar tanımlanmamalı
