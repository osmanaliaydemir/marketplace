using Dashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.DTOs.Products;

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
			Input.Weight = Product.Weight ?? 0;
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

// DTO'lar Application layer'dan geliyor, burada tekrar tanımlanmamalı
