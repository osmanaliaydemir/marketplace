using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Services;

namespace Web.Pages.Seller.Products;

[Authorize(Roles = "Seller")]
public sealed class IndexModel : PageModel
{
    private readonly ApiClient _apiClient;

    public IndexModel(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public List<ProductListItem> Products { get; private set; } = new();

    public async Task OnGet()
    {
        var result = await _apiClient.GetAsync<List<ProductListItem>>("/api/products/mine");
        Products = result ?? new List<ProductListItem>();
    }

    public sealed class ProductListItem
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int StockQty { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public string? PrimaryImageUrl { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
