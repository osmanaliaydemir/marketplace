using Dashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.DTOs.Products;
using Application.DTOs.Categories;

namespace Dashboard.Pages.Products;

public class NewModel : PageModel
{
	private readonly ApiClient _api;

	public NewModel(ApiClient api)
	{
		_api = api;
	}

	[BindProperty]
	public ProductCreateRequest Input { get; set; } = new();

	public List<CategoryOptionDto> Categories { get; private set; } = new();

	public async Task OnGetAsync()
	{
		var token = HttpContext.Session.GetString("AuthToken");
		if (string.IsNullOrEmpty(token))
		{
			TempData["Error"] = "Oturum süresi dolmuş. Lütfen giriş yapın.";
			return;
		}

		_api.SetBearer(token);
		
		try
		{
			// Kategorileri getir
			var categories = await _api.GetAsync<List<CategoryOptionDto>>("/api/categories");
			Categories = categories ?? new List<CategoryOptionDto>();
		}
		catch (Exception ex)
		{
			TempData["Error"] = "Kategoriler yüklenirken hata oluştu.";
			Categories = new List<CategoryOptionDto>();
		}
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
		var created = await _api.PostAsync<ProductCreateRequest, object>("/api/products", Input);
		TempData["Success"] = "Ürün oluşturuldu.";
		return RedirectToPage("/Products/Index");
	}
}

// DTO sınıfları Application.DTOs.Products namespace'inden kullanılıyor
