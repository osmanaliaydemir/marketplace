namespace Application.DTOs.Products;

/// <summary>
/// Ürün varyantı DTO'su
/// </summary>
public class ProductVariantDto
{
    /// <summary>
    /// Varyant ID'si
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Ürün ID'si
    /// </summary>
    public long ProductId { get; set; }

    /// <summary>
    /// Varyant adı
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Varyant adı (alias)
    /// </summary>
    public string VariantName { get; set; } = string.Empty;

    /// <summary>
    /// Varyant SKU'su
    /// </summary>
    public string? Sku { get; set; }

    /// <summary>
    /// Barkod
    /// </summary>
    public string? Barcode { get; set; }

    /// <summary>
    /// Varyant fiyatı
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Varyant eski fiyatı
    /// </summary>
    public decimal? CompareAtPrice { get; set; }

    /// <summary>
    /// Varyant stok miktarı
    /// </summary>
    public int StockQty { get; set; }

    /// <summary>
    /// Rezerve edilmiş stok miktarı
    /// </summary>
    public int ReservedQty { get; set; }

    /// <summary>
    /// Minimum sipariş miktarı
    /// </summary>
    public int? MinOrderQty { get; set; }

    /// <summary>
    /// Maksimum sipariş miktarı
    /// </summary>
    public int? MaxOrderQty { get; set; }

    /// <summary>
    /// Görüntülenme sırası
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Varsayılan varyant mı?
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Varyant ağırlığı
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// Varyant aktif mi?
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Varyant seçenekleri
    /// </summary>
    public Dictionary<string, string> Options { get; set; } = new();

    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Güncellenme tarihi
    /// </summary>
    public DateTime? ModifiedAt { get; set; }
}
