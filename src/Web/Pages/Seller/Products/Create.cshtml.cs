using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Services;
using Application.DTOs.Products;

namespace Web.Pages.Seller.Products;

[Authorize(Roles = "Seller")]
public sealed class CreateModel : PageModel
{
    private readonly ApiClient _api;

    public CreateModel(ApiClient api)
    {
        _api = api;
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

    public async Task OnGet()
    {
        // No-op: form render
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Store bilgisi al (seller mağazası)
        var myStore = await _api.GetAsync<dynamic>("/api/stores/mine");
        if (myStore == null)
        {
            ModelState.AddModelError(string.Empty, "Mağaza bulunamadı");
            return Page();
        }

        // Validator gereği StoreId/SellerId doldurulmalı, API tarafı zaten claims'ten doğrulayacak
        Input = Input with
        {
            StoreId = (long)myStore.id,
            SellerId = (long)myStore.sellerId
        };

        await _api.PostAsync<ProductCreateRequest, object>("/api/products", Input);
        return Redirect("/satici/urunler");
    }
}
