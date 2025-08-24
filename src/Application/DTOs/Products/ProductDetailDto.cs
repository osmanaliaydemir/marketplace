using Application.DTOs.Categories;
using Application.DTOs.Stores;
using Application.DTOs.Sellers;

namespace Application.DTOs.Products;

public sealed record ProductDetailDto
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? ShortDescription { get; init; }
    public long CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public long StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;
    public long SellerId { get; init; }
    public string SellerName { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal? CompareAtPrice { get; init; }
    public string Currency { get; init; } = "TRY";
    public int StockQty { get; init; }
    public bool IsActive { get; init; }
    public bool IsFeatured { get; init; }
    public bool IsPublished { get; init; }
    public int DisplayOrder { get; init; }
    public decimal Weight { get; init; }
    public int? MinOrderQty { get; init; }
    public int? MaxOrderQty { get; init; }
    public string? MetaTitle { get; init; }
    public string? MetaDescription { get; init; }
    public string? MetaKeywords { get; init; }
    public DateTime? PublishedAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
    
    // Related Data
    public CategoryDto? Category { get; init; }
    public StoreDto? Store { get; init; }
    public SellerDto? Seller { get; init; }
    public IEnumerable<ProductVariantDto> Variants { get; init; } = Enumerable.Empty<ProductVariantDto>();
    public IEnumerable<ProductImageDto> Images { get; init; } = Enumerable.Empty<ProductImageDto>();
    
    // Calculated Properties
    public decimal? DiscountPercentage { get; init; }
    public bool HasDiscount => CompareAtPrice.HasValue && CompareAtPrice > Price;
    public bool IsInStock => StockQty > 0;
    public bool IsLowStock => StockQty > 0 && StockQty <= 10;
} 
