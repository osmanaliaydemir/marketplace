namespace Domain.Entities;

public sealed class Product : Domain.Models.AuditableEntity
{
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
    public int? StockQty { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false; // Öne çıkan ürün
    public bool IsPublished { get; set; } = true; // Yayınlanmış mı
    public int DisplayOrder { get; set; } = 0; // Görüntüleme sırası
    public decimal Weight { get; set; } = 0; // Gram cinsinden (decimal for precision)
    public int MinOrderQty { get; set; } = 1; // Minimum sipariş miktarı
    public int MaxOrderQty { get; set; } = 999; // Maksimum sipariş miktarı
    public string? MetaTitle { get; set; } // SEO
    public string? MetaDescription { get; set; } // SEO
    public string? MetaKeywords { get; set; } // SEO
    public DateTime? PublishedAt { get; set; } // Yayınlanma tarihi
    
    // Navigation properties
    public Category Category { get; set; } = null!;
    public Store Store { get; set; } = null!;
    public Seller Seller { get; set; } = null!;
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}
