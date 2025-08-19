namespace Domain.Entities;

public sealed class ProductVariant : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long ProductId { get; set; }
    public string? Sku { get; set; } // Stock Keeping Unit
    public string? Barcode { get; set; } // Barkod
    public string? VariantName { get; set; } // Varyant adı (örn: "Kırmızı - XL")
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; } // Eski fiyat
    public int StockQty { get; set; }
    public int? ReservedQty { get; set; } = 0; // Rezerve edilen miktar
    public int? MinOrderQty { get; set; } = 1;
    public int? MaxOrderQty { get; set; }
    public int DisplayOrder { get; set; } = 0; // Görüntüleme sırası
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; } = false; // Varsayılan varyant mı?
    public int Weight { get; set; } = 0; // Gram cinsinden
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; set; }
    
    // Navigation property (removed virtual for sealed class)
    // Product will be loaded separately in service layer
}


