using Dashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Application.DTOs.Categories;

namespace Dashboard.Pages.Categories;

public class NewModel : PageModel
{
	private readonly ApiClient _api;

	public NewModel(ApiClient api)
	{
		_api = api;
	}

	[BindProperty]
	public CreateCategoryRequest Input { get; set; } = new();

	public List<SelectListItem> ParentCategories { get; set; } = new();

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
			// Tüm kategorileri al (parent seçimi için)
			var categories = await _api.GetAsync<List<CategoryDto>>("/api/categories") ?? new();
			
			// Parent kategorileri hazırla
			ParentCategories = new List<SelectListItem>
			{
				new SelectListItem { Value = "", Text = "Ana Kategori (Parent Yok)" }
			};

			// Sadece ana kategorileri ekle (parent_id = null)
			var mainCategories = categories.Where(c => c.ParentId == null).OrderBy(c => c.DisplayOrder);
			foreach (var category in mainCategories)
			{
				ParentCategories.Add(new SelectListItem 
				{ 
					Value = category.Id.ToString(), 
					Text = category.Name 
				});
			}
		}
		catch
		{
			ParentCategories = new List<SelectListItem>
			{
				new SelectListItem { Value = "", Text = "Ana Kategori (Parent Yok)" }
			};
		}
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
		{
			TempData["Error"] = "Form hatalı.";
			await LoadParentCategories();
			return Page();
		}

		var token = HttpContext.Session.GetString("AuthToken");
		if (string.IsNullOrEmpty(token))
		{
			TempData["Error"] = "Oturum süresi dolmuş. Lütfen giriş yapın.";
			return RedirectToPage("/Login");
		}

		_api.SetBearer(token);

		try
		{
			// ParentId boşsa null olarak ayarla
			if (string.IsNullOrEmpty(Input.ParentId))
			{
				Input.ParentId = null;
			}

			var createdCategory = await _api.PostAsync<CreateCategoryRequest, object>("/api/categories", Input);

			if (createdCategory != null)
			{
				TempData["Success"] = "Kategori başarıyla oluşturuldu!";
				return RedirectToPage("./Index");
			}
			else
			{
				TempData["Error"] = "Kategori oluşturulurken bir hata oluştu.";
				await LoadParentCategories();
				return Page();
			}
		}
		catch (Exception ex)
		{
			TempData["Error"] = "Kategori oluşturulurken beklenmeyen bir hata oluştu: " + ex.Message;
			await LoadParentCategories();
			return Page();
		}
	}

	private async Task LoadParentCategories()
	{
		try
		{
			var categories = await _api.GetAsync<List<CategoryDto>>("/api/categories") ?? new();
			
			ParentCategories = new List<SelectListItem>
			{
				new SelectListItem { Value = "", Text = "Ana Kategori (Parent Yok)" }
			};

			var mainCategories = categories.Where(c => c.ParentId == null).OrderBy(c => c.DisplayOrder);
			foreach (var category in mainCategories)
			{
				ParentCategories.Add(new SelectListItem 
				{ 
					Value = category.Id.ToString(), 
					Text = category.Name 
				});
			}
		}
		catch
		{
			ParentCategories = new List<SelectListItem>
			{
				new SelectListItem { Value = "", Text = "Ana Kategori (Parent Yok)" }
			};
		}
	}
}

public sealed class CreateCategoryRequest
{
	[Required(ErrorMessage = "Kategori adı gereklidir.")]
	[StringLength(255, ErrorMessage = "Kategori adı en fazla 255 karakter olabilir.")]
	public string Name { get; set; } = string.Empty;

	[StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
	public string? Description { get; set; }

	[StringLength(500, ErrorMessage = "Resim URL'i en fazla 500 karakter olabilir.")]
	[Url(ErrorMessage = "Geçerli bir URL giriniz.")]
	public string? ImageUrl { get; set; }

	[StringLength(100, ErrorMessage = "İkon sınıfı en fazla 100 karakter olabilir.")]
	public string? IconClass { get; set; }

	public bool IsActive { get; set; } = true;
	public bool IsFeatured { get; set; } = false;

	[Range(0, 999, ErrorMessage = "Görünüm sırası 0-999 arasında olmalıdır.")]
	public int DisplayOrder { get; set; } = 0;

	[StringLength(255, ErrorMessage = "Meta başlık en fazla 255 karakter olabilir.")]
	public string? MetaTitle { get; set; }

	[StringLength(500, ErrorMessage = "Meta açıklama en fazla 500 karakter olabilir.")]
	public string? MetaDescription { get; set; }

	public string? ParentId { get; set; }
}
