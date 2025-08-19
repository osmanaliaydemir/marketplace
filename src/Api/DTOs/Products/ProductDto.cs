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

// Ürün Detayı için DTO
public sealed class ProductDetailDto
{
    public long Id { get; set; }
    public long SellerId { get; set; }
    public long CategoryId { get; set; }
    public long StoreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public string Currency { get; set; } = "TRY";
    public int StockQty { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }
    public int Weight { get; set; }
    public int? MinOrderQty { get; set; }
    public int? MaxOrderQty { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    
    // İlişkili veriler
    public CategoryDto Category { get; set; } = null!;
    public StoreDto Store { get; set; } = null!;
    public SellerDto Seller { get; set; } = null!;
    public List<ProductVariantDto> Variants { get; set; } = new();
    public List<ProductImageDto> Images { get; set; } = new();
    
    // Hesaplanan alanlar
    public decimal? DiscountPercentage => CompareAtPrice > 0 ? Math.Round(((CompareAtPrice.Value - Price) / CompareAtPrice.Value) * 100, 0) : null;
    public bool HasDiscount => CompareAtPrice > 0 && CompareAtPrice > Price;
    public bool InStock => StockQty > 0;
}

// Ürün Oluşturma için DTO
public sealed class CreateProductRequest
{
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public string Currency { get; set; } = "TRY";
    public int StockQty { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public bool IsPublished { get; set; } = true;
    public int Weight { get; set; } = 0;
    public int? MinOrderQty { get; set; } = 1;
    public int? MaxOrderQty { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public List<CreateProductVariantRequest> Variants { get; set; } = new();
    public List<CreateProductImageRequest> Images { get; set; } = new();
}

// Ürün Güncelleme için DTO
public sealed class UpdateProductRequest
{
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public string Currency { get; set; } = "TRY";
    public int StockQty { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }
    public int Weight { get; set; }
    public int? MinOrderQty { get; set; }
    public int? MaxOrderQty { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
}

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
