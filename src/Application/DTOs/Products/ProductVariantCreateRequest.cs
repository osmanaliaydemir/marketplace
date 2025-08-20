using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Products;

public sealed record ProductVariantCreateRequest
{
    [Required(ErrorMessage = "Varyant adı zorunludur")]
    [StringLength(255, ErrorMessage = "Varyant adı en fazla 255 karakter olabilir")]
    public string Name { get; init; } = string.Empty;
    
    [StringLength(100, ErrorMessage = "SKU en fazla 100 karakter olabilir")]
    public string? Sku { get; init; }
    
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
    
    [Range(0, int.MaxValue, ErrorMessage = "Ağırlık 0'dan küçük olamaz")]
    public int Weight { get; init; }
    
    public bool IsActive { get; init; } = true;
    public Dictionary<string, string>? Attributes { get; init; }
}
