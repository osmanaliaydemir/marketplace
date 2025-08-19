using Dashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard.Pages.Products;

public class NewModel : PageModel
{
	private readonly ApiClient _api;

	public NewModel(ApiClient api)
	{
		_api = api;
	}

	[BindProperty]
	public CreateProductRequest Input { get; set; } = new();

	public async Task OnGetAsync()
	{
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
		{
			TempData["Error"] = "Form hatalı.";
			return Page();
		}

		var token = HttpContext.Session.GetString("AuthToken");
		if (string.IsNullOrEmpty(token))
		{
			TempData["Error"] = "Oturum süresi dolmuş. Lütfen giriş yapın.";
			return RedirectToPage("/Login");
		}

		_api.SetBearer(token);
		var created = await _api.PostAsync<CreateProductRequest, object>("/api/products", Input);
		TempData["Success"] = "Ürün oluşturuldu.";
		return RedirectToPage("/Products/Index");
	}
}

public sealed class CreateProductRequest
{
	public long CategoryId { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string? ShortDescription { get; set; }
	public decimal Price { get; set; }
	public decimal? CompareAtPrice { get; set; }
	public string Currency { get; set; } = "TRY";
	public int StockQty { get; set; }
	public bool IsActive { get; set; } = true;
	public bool IsFeatured { get; set; }
	public bool IsPublished { get; set; }
	public int? MinOrderQty { get; set; }
	public int? MaxOrderQty { get; set; }
	public string? MetaTitle { get; set; }
	public string? MetaDescription { get; set; }
	public string? MetaKeywords { get; set; }
	public int Weight { get; set; }
	public List<CreateProductVariantRequest> Variants { get; set; } = new();
	public List<CreateProductImageRequest> Images { get; set; } = new();
}

public sealed class CreateProductVariantRequest
{
	public string? Sku { get; set; }
	public string? Barcode { get; set; }
	public string? VariantName { get; set; }
	public decimal Price { get; set; }
	public decimal? CompareAtPrice { get; set; }
	public int StockQty { get; set; }
	public int? MinOrderQty { get; set; }
	public int? MaxOrderQty { get; set; }
	public bool IsDefault { get; set; }
	public int Weight { get; set; }
}

public sealed class CreateProductImageRequest
{
	public string ImageUrl { get; set; } = string.Empty;
	public string? ThumbnailUrl { get; set; }
	public string? AltText { get; set; }
	public string? Title { get; set; }
	public int DisplayOrder { get; set; }
	public bool IsPrimary { get; set; }
}
