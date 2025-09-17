using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Services;
using Application.DTOs.Products;
using Application.DTOs.Stores;

namespace Web.Pages.Seller.Products;

[Authorize(Roles = "Seller")]
public sealed class CreateModel : PageModel
{
    private readonly ApiClient _api;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(ApiClient api, ILogger<CreateModel> logger)
    {
        _api = api;
        _logger = logger;
    }

    [BindProperty]
    public ProductCreateRequest Input { get; set; } = new()
    {
        Currency = "TRY",
        IsActive = true,
        IsPublished = false,
        StockQty = 0,
        Weight = 0
    };

    public List<CategoryOption> Categories { get; private set; } = new();

    public async Task OnGet()
    {
        try
        {
            // Kategorileri getir
            var categories = await _api.GetAsync<List<CategoryOption>>("/api/categories");
            Categories = categories ?? new List<CategoryOption>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading categories");
            Categories = new List<CategoryOption>();
        }
    }

    public sealed class CategoryOption
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long? ParentId { get; set; }
        public bool IsMainCategory { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Store bilgisi al (seller mağazası)
        var myStore = await _api.GetAsync<StoreDetailDto>("/api/stores/mine");
        if (myStore is null)
        {
            ModelState.AddModelError(string.Empty, "Mağaza bulunamadı");
            return Page();
        }

        // Validator gereği StoreId/SellerId doldurulmalı, API tarafı zaten claims'ten doğrulayacak
        Input = Input with
        {
            StoreId = myStore.Id,
            SellerId = myStore.SellerId
        };

        await _api.PostAsync<ProductCreateRequest, object>("/api/products", Input);
        return Redirect("/satici/urunler");
    }
}
