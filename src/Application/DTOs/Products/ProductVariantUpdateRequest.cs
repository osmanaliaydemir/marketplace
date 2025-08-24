using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Products;

public sealed record ProductVariantUpdateRequest
{
    [Required(ErrorMessage = "Varyant adı zorunludur")]
    [StringLength(255, ErrorMessage = "Varyant adı en fazla 255 karakter olabilir")]
    public string VariantName { get; init; } = string.Empty;
    
    [StringLength(100, ErrorMessage = "SKU en fazla 100 karakter olabilir")]
    public string? Sku { get; init; }
    
    [StringLength(50, ErrorMessage = "Barkod en fazla 50 karakter olabilir")]
    public string? Barcode { get; init; }
    
    [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
    public string? Description { get; init; }
    
    [Required(ErrorMessage = "Fiyat zorunludur")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır")]
    public decimal Price { get; init; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Karşılaştırma fiyatı 0'dan büyük olmalıdır")]
    public decimal? CompareAtPrice { get; init; }
    
    [Required(ErrorMessage = "Stok miktarı zorunludur")]
    [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı 0'dan küçük olamaz")]
    public int StockQty { get; init; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Minimum sipariş miktarı 1'den küçük olamaz")]
    public int? MinOrderQty { get; init; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Maksimum sipariş miktarı 1'den küçük olamaz")]
    public int? MaxOrderQty { get; init; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Ağırlık 0'dan küçük olamaz")]
    public decimal Weight { get; init; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Görüntüleme sırası 0'dan küçük olamaz")]
    public int DisplayOrder { get; init; } = 0;
    
    public bool IsDefault { get; init; }
    public bool IsActive { get; init; } = true;
    public Dictionary<string, string>? Attributes { get; init; }
}
