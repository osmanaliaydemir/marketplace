namespace Domain.Entities;

public sealed class Product : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long SellerId { get; set; }
    public long CategoryId { get; set; }
    public long StoreId { get; set; }
    public long? StoreCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; } // Eski fiyat (indirim için)
    public string Currency { get; set; } = "TRY";
    public int StockQty { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false; // Öne çıkan ürün
    public bool IsPublished { get; set; } = true; // Yayınlanmış mı
    public int DisplayOrder { get; set; } = 0; // Görüntüleme sırası
    public int Weight { get; set; } = 0; // Gram cinsinden
    public int? MinOrderQty { get; set; } = 1; // Minimum sipariş miktarı
    public int? MaxOrderQty { get; set; } // Maksimum sipariş miktarı
    public string? MetaTitle { get; set; } // SEO
    public string? MetaDescription { get; set; } // SEO
    public string? MetaKeywords { get; set; } // SEO
    public DateTime? PublishedAt { get; set; } // Yayınlanma tarihi
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; set; }
    
    // Navigation properties (removed virtual for sealed class)
    // Category, Store, Seller, Variants, Images will be loaded separately in service layer
}
