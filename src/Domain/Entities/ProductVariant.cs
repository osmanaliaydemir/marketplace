namespace Domain.Entities;

public sealed class ProductVariant : Domain.Models.AuditableEntity
{
    public long ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string? Barcode { get; set; }
    public string VariantName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int? StockQty { get; set; }
    public int ReservedQty { get; set; } = 0;
    public int MinOrderQty { get; set; } = 1;
    public int MaxOrderQty { get; set; } = 999;
    public int DisplayOrder { get; set; } = 0;
    public bool IsDefault { get; set; } = false;
    public decimal Weight { get; set; } = 0;
    public string? Sku { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public Product Product { get; set; } = null!;
}


