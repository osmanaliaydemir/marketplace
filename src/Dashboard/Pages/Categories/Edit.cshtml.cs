using Dashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Dashboard.Pages.Categories;

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
	public UpdateCategoryRequest Input { get; set; } = new();

	public CategoryDto? Category { get; set; }
	public List<SelectListItem> ParentCategories { get; set; } = new();

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
			Category = await _api.GetAsync<CategoryDto>($"/api/categories/{Id}");
			if (Category == null)
			{
				TempData["Error"] = "Kategori bulunamadı.";
				return RedirectToPage("/Categories/Index");
			}

			// Parent kategorileri yükle
			await LoadParentCategories();

			// Form'u doldur
			Input.Name = Category.Name;
			Input.Description = Category.Description;
			Input.ImageUrl = Category.ImageUrl;
			Input.IconClass = Category.IconClass;
			Input.IsActive = Category.IsActive;
			Input.IsFeatured = Category.IsFeatured;
			Input.DisplayOrder = Category.DisplayOrder;
			Input.MetaTitle = Category.MetaTitle;
			Input.MetaDescription = Category.MetaDescription;
			Input.ParentId = Category.ParentId?.ToString();
		}
		catch (Exception ex)
		{
			TempData["Error"] = "Kategori bilgileri alınırken hata oluştu.";
			return RedirectToPage("/Categories/Index");
		}

		return Page();
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

			// Kendisini parent olarak seçemez
			if (Input.ParentId == Id.ToString())
			{
				ModelState.AddModelError("Input.ParentId", "Kategori kendisini üst kategori olarak seçemez.");
				await LoadParentCategories();
				return Page();
			}

			await _api.PutAsync<UpdateCategoryRequest, object>($"/api/categories/{Id}", Input);
			TempData["Success"] = "Kategori başarıyla güncellendi.";
			return RedirectToPage("./Index");
		}
		catch (Exception ex)
		{
			TempData["Error"] = "Kategori güncellenirken beklenmeyen bir hata oluştu: " + ex.Message;
			await LoadParentCategories();
			return Page();
		}
	}

	private async Task LoadParentCategories()
	{
		try
		{
			// Tüm kategorileri al
			var categories = await _api.GetAsync<List<CategoryDto>>("/api/categories") ?? new();
			
			// Parent kategorileri hazırla (kendisi hariç)
			ParentCategories = new List<SelectListItem>
			{
				new SelectListItem { Value = "", Text = "Ana Kategori (Parent Yok)" }
			};

			// Sadece ana kategorileri ekle (parent_id = null ve kendisi hariç)
			var mainCategories = categories.Where(c => c.ParentId == null && c.Id != Id).OrderBy(c => c.DisplayOrder);
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

public sealed class UpdateCategoryRequest
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

	public bool IsActive { get; set; }
	public bool IsFeatured { get; set; }

	[Range(0, 999, ErrorMessage = "Görünüm sırası 0-999 arasında olmalıdır.")]
	public int DisplayOrder { get; set; }

	[StringLength(255, ErrorMessage = "Meta başlık en fazla 255 karakter olabilir.")]
	public string? MetaTitle { get; set; }

	[StringLength(500, ErrorMessage = "Meta açıklama en fazla 500 karakter olabilir.")]
	public string? MetaDescription { get; set; }

	public string? ParentId { get; set; }
}
