namespace Api.DTOs.Products;

public class ProductListDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "TRY";
    public string? ImageUrl { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string StoreSlug { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProductDetailDto
{
    public long Id { get; set; }
    public long SellerId { get; set; }
    public long CategoryId { get; set; }
    public long StoreId { get; set; }
    public long? StoreCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "TRY";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    
    // Navigation properties
    public StoreInfoDto Store { get; set; } = new();
    public CategoryInfoDto Category { get; set; } = new();
    public List<ProductVariantDto> Variants { get; set; } = new();
}

public class CreateProductDto
{
    public long StoreId { get; set; }
    public long CategoryId { get; set; }
    public long? StoreCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "TRY";
}

public class UpdateProductDto
{
    public long? CategoryId { get; set; }
    public long? StoreCategoryId { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? Currency { get; set; }
    public bool? IsActive { get; set; }
}

// Helper DTOs
public class StoreInfoDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
}

public class CategoryInfoDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
}

public class ProductVariantDto
{
    public long Id { get; set; }
    public string? Sku { get; set; }
    public decimal Price { get; set; }
    public int StockQty { get; set; }
    public bool IsActive { get; set; }
}
