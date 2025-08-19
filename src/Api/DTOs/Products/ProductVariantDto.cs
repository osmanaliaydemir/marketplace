namespace Api.DTOs.Products;

// Ürün Varyantı için DTO
public sealed class ProductVariantDto
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public string? Sku { get; set; }
    public string? Barcode { get; set; }
    public string? VariantName { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int StockQty { get; set; }
    public int? ReservedQty { get; set; }
    public int? MinOrderQty { get; set; }
    public int? MaxOrderQty { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public int Weight { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    
    // Hesaplanan alanlar
    public decimal? DiscountPercentage => CompareAtPrice > 0 ? Math.Round(((CompareAtPrice.Value - Price) / CompareAtPrice.Value) * 100, 0) : null;
    public bool HasDiscount => CompareAtPrice > 0 && CompareAtPrice > Price;
    public bool InStock => StockQty > 0;
    public int AvailableQty => StockQty - (ReservedQty ?? 0);
}

// Varyant Oluşturma için DTO
public sealed class CreateProductVariantRequest
{
    public string? Sku { get; set; }
    public string? Barcode { get; set; }
    public string? VariantName { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int StockQty { get; set; }
    public int? MinOrderQty { get; set; } = 1;
    public int? MaxOrderQty { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsDefault { get; set; } = false;
    public int Weight { get; set; } = 0;
}

// Varyant Güncelleme için DTO
public sealed class UpdateProductVariantRequest
{
    public string? Sku { get; set; }
    public string? Barcode { get; set; }
    public string? VariantName { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int StockQty { get; set; }
    public int? MinOrderQty { get; set; }
    public int? MaxOrderQty { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public int Weight { get; set; }
}
