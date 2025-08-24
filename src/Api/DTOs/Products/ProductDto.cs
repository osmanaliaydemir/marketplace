using Application.DTOs.Categories;
using Application.DTOs.Products;

namespace Api.DTOs.Products;

// Ürün Listesi için DTO
public sealed class ProductListDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public string Currency { get; set; } = "TRY";
    public int StockQty { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal? DiscountPercentage => CompareAtPrice > 0 ? Math.Round(((CompareAtPrice.Value - Price) / CompareAtPrice.Value) * 100, 0) : null;
}

// ProductDetailDto Application layer'dan geliyor

// Ürün Oluşturma için DTO - Application.DTOs.Products.ProductCreateRequest kullanılıyor

// UpdateProductRequest Application layer'dan geliyor

// Ürün Arama için DTO
public sealed class ProductSearchRequest
{
    public string? SearchTerm { get; set; }
    public long? CategoryId { get; set; }
    public long? StoreId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? InStock { get; set; }
    public bool? IsFeatured { get; set; }
    public string? SortBy { get; set; } = "CreatedAt"; // Name, Price, CreatedAt
    public string? SortOrder { get; set; } = "Desc"; // Asc, Desc
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// Ürün Arama Sonucu için DTO
public sealed class ProductSearchResponse
{
    public List<ProductListDto> Products { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

// Ürün Durumu Güncelleme için DTO
public sealed class UpdateProductStatusRequest
{
    public bool IsActive { get; set; }
    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }
}

// StoreDto, SellerDto, AppUserDto Application layer'da tanımlanacak
