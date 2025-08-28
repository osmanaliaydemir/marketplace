using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Services;
using Application.DTOs.Products;

namespace Web.Pages.Seller.Products;

[Authorize(Roles = "Seller")]
public sealed class EditModel : PageModel
{
    private readonly ApiClient _api;

    public EditModel(ApiClient api)
    {
        _api = api;
    }

    [BindProperty]
    public UpdateProductRequest Input { get; set; } = new();

    [FromRoute]
    public long Id { get; set; }

    public ProductDetailDto? Product { get; private set; }

    public async Task<IActionResult> OnGetAsync(long id)
    {
        Product = await _api.GetAsync<ProductDetailDto>($"/api/products/{id}");
        if (Product == null)
        {
            return NotFound();
        }

        Input = new UpdateProductRequest
        {
            CategoryId = Product.CategoryId,
            Name = Product.Name,
            Description = Product.Description,
            ShortDescription = Product.ShortDescription,
            Price = Product.Price,
            CompareAtPrice = Product.CompareAtPrice,
            Currency = Product.Currency,
            StockQty = Product.StockQty,
            IsActive = Product.IsActive,
            IsFeatured = Product.IsFeatured,
            IsPublished = Product.IsPublished,
            Weight = Product.Weight,
            MinOrderQty = Product.MinOrderQty,
            MaxOrderQty = Product.MaxOrderQty,
            MetaTitle = Product.MetaTitle,
            MetaDescription = Product.MetaDescription,
            MetaKeywords = Product.MetaKeywords
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(long id)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _api.PutAsync<UpdateProductRequest, ProductDetailDto>($"/api/products/{id}", Input);
        return Redirect("/satici/urunler");
    }
}
