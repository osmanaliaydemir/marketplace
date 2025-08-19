using Dashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard.Pages.Products;

public class EditModel : PageModel
{
	private readonly ApiClient _api;

	public EditModel(ApiClient api)
	{
		_api = api;
	}

	[BindProperty(SupportsGet = true)]
	public long Id { get; set; }

	[BindProperty]
	public UpdateProductRequest Input { get; set; } = new();

	public ProductDetailDto? Product { get; set; }
	public List<ProductVariantDto> Variants { get; set; } = new();
	public List<ProductImageDto> Images { get; set; } = new();

	public async Task<IActionResult> OnGetAsync()
	{
		var token = HttpContext.Session.GetString("AuthToken");
		if (string.IsNullOrEmpty(token))
		{
			TempData["Error"] = "Oturum süresi dolmuş. Lütfen giriş yapın.";
			return RedirectToPage("/Login");
		}

		_api.SetBearer(token);

		try
		{
			Product = await _api.GetAsync<ProductDetailDto>($"/api/products/{Id}");
			if (Product == null)
			{
				TempData["Error"] = "Ürün bulunamadı.";
				return RedirectToPage("/Products/Index");
			}

			// Varyantları al
			Variants = await _api.GetAsync<List<ProductVariantDto>>($"/api/products/{Id}/variants") ?? new();

			// Resimleri al
			Images = await _api.GetAsync<List<ProductImageDto>>($"/api/products/{Id}/images") ?? new();

			// Form'u doldur
			Input.CategoryId = Product.CategoryId;
			Input.Name = Product.Name;
			Input.Description = Product.Description;
			Input.ShortDescription = Product.ShortDescription;
			Input.Price = Product.Price;
			Input.CompareAtPrice = Product.CompareAtPrice;
			Input.Currency = Product.Currency;
			Input.StockQty = Product.StockQty;
			Input.IsActive = Product.IsActive;
			Input.IsFeatured = Product.IsFeatured;
			Input.IsPublished = Product.IsPublished;
			Input.MinOrderQty = Product.MinOrderQty;
			Input.MaxOrderQty = Product.MaxOrderQty;
			Input.MetaTitle = Product.MetaTitle;
			Input.MetaDescription = Product.MetaDescription;
			Input.MetaKeywords = Product.MetaKeywords;
			Input.Weight = Product.Weight;
		}
		catch (Exception ex)
		{
			TempData["Error"] = "Ürün bilgileri alınırken hata oluştu.";
			return RedirectToPage("/Products/Index");
		}

		return Page();
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
		await _api.PutAsync<UpdateProductRequest, object>($"/api/products/{Id}", Input);
		TempData["Success"] = "Ürün güncellendi.";
		return RedirectToPage();
	}
}

public sealed class UpdateProductRequest
{
	public long CategoryId { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string? ShortDescription { get; set; }
	public decimal Price { get; set; }
	public decimal? CompareAtPrice { get; set; }
	public string Currency { get; set; } = "TRY";
	public int StockQty { get; set; }
	public bool IsActive { get; set; }
	public bool IsFeatured { get; set; }
	public bool IsPublished { get; set; }
	public int? MinOrderQty { get; set; }
	public int? MaxOrderQty { get; set; }
	public string? MetaTitle { get; set; }
	public string? MetaDescription { get; set; }
	public string? MetaKeywords { get; set; }
	public int Weight { get; set; }
}

public sealed class ProductDetailDto
{
	public long Id { get; set; }
	public long SellerId { get; set; }
	public long CategoryId { get; set; }
	public long StoreId { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Slug { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string? ShortDescription { get; set; }
	public decimal Price { get; set; }
	public decimal? CompareAtPrice { get; set; }
	public string Currency { get; set; } = "TRY";
	public int StockQty { get; set; }
	public bool IsActive { get; set; }
	public bool IsFeatured { get; set; }
	public bool IsPublished { get; set; }
	public int Weight { get; set; }
	public int? MinOrderQty { get; set; }
	public int? MaxOrderQty { get; set; }
	public string? MetaTitle { get; set; }
	public string? MetaDescription { get; set; }
	public string? MetaKeywords { get; set; }
	public DateTime? PublishedAt { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? ModifiedAt { get; set; }
}

public sealed class ProductVariantDto
{
	public long Id { get; set; }
	public long ProductId { get; set; }
	public string? Sku { get; set; }
	public string? Barcode { get; set; }
	public string? VariantName { get; set; }
	public decimal Price { get; set; }
	public decimal? CompareAtPrice { get; set; }
	public int StockQty { get; set; }
	public int ReservedQty { get; set; }
	public int? MinOrderQty { get; set; }
	public int? MaxOrderQty { get; set; }
	public int DisplayOrder { get; set; }
	public bool IsActive { get; set; }
	public bool IsDefault { get; set; }
	public int Weight { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? ModifiedAt { get; set; }
}

public sealed class ProductImageDto
{
	public long Id { get; set; }
	public long ProductId { get; set; }
	public string ImageUrl { get; set; } = string.Empty;
	public string? ThumbnailUrl { get; set; }
	public string? AltText { get; set; }
	public string? Title { get; set; }
	public int DisplayOrder { get; set; }
	public bool IsPrimary { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? ModifiedAt { get; set; }
}
