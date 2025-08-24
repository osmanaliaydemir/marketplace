using Dashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.DTOs.Products;

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
		var created = await _api.PostAsync<ProductCreateRequest, object>("/api/products", Input);
		TempData["Success"] = "Ürün oluşturuldu.";
		return RedirectToPage("/Products/Index");
	}
}

// DTO sınıfları Application.DTOs.Products namespace'inden kullanılıyor
