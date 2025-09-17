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
        try
        {
            var result = await _apiClient.GetAsync<ApiResponse>("/api/products/mine");
            if (result?.Products != null)
            {
                Products = result.Products.Select(p => new ProductListItem
                {
                    Id = p.Id,
                    Name = p.Name,
                    ShortDescription = p.ShortDescription,
                    Price = p.Price,
                    CompareAtPrice = p.CompareAtPrice,
                    Currency = p.Currency,
                    StockQty = p.StockQty,
                    IsActive = p.IsActive,
                    IsFeatured = p.IsFeatured,
                    PrimaryImageUrl = p.PrimaryImageUrl,
                    CategoryName = p.CategoryName,
                    StoreName = p.StoreName,
                    CreatedAt = p.CreatedAt
                }).ToList();
            }
            else
            {
                Products = new List<ProductListItem>();
            }
        }
        catch (Exception ex)
        {
            // Log error or handle gracefully
            Products = new List<ProductListItem>();
        }
    }

    public sealed class ApiResponse
    {
        public List<ApiProductListItem> Products { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class ApiProductListItem
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
