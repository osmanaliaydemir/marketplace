using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Products;

public sealed record ProductUpdateRequest
{
    [Required(ErrorMessage = "Ürün adı zorunludur")]
    [StringLength(255, ErrorMessage = "Ürün adı en fazla 255 karakter olabilir")]
    public string Name { get; init; } = string.Empty;
    
    [Required(ErrorMessage = "Ürün açıklaması zorunludur")]
    public string Description { get; init; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "Kısa açıklama en fazla 500 karakter olabilir")]
    public string? ShortDescription { get; init; }
    
    [Required(ErrorMessage = "Kategori seçimi zorunludur")]
    public long CategoryId { get; init; }
    
    [Required(ErrorMessage = "Fiyat zorunludur")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır")]
    public decimal Price { get; init; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Karşılaştırma fiyatı 0'dan büyük olmalıdır")]
    public decimal? CompareAtPrice { get; init; }
    
    [Required(ErrorMessage = "Para birimi zorunludur")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Para birimi 3 karakter olmalıdır")]
    public string Currency { get; init; } = "TRY";
    
    [Required(ErrorMessage = "Stok miktarı zorunludur")]
    [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı 0'dan küçük olamaz")]
    public int StockQty { get; init; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Ağırlık 0'dan küçük olamaz")]
    public decimal Weight { get; init; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Minimum sipariş miktarı 1'den küçük olamaz")]
    public int? MinOrderQty { get; init; } = 1;
    
    [Range(1, int.MaxValue, ErrorMessage = "Maksimum sipariş miktarı 1'den küçük olamaz")]
    public int? MaxOrderQty { get; init; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Display order 0'dan küçük olamaz")]
    public int DisplayOrder { get; init; } = 0;
    
    public bool IsActive { get; init; }
    public bool IsFeatured { get; init; }
    public bool IsPublished { get; init; }
    
    [StringLength(255, ErrorMessage = "Meta başlık en fazla 255 karakter olabilir")]
    public string? MetaTitle { get; init; }
    
    [StringLength(500, ErrorMessage = "Meta açıklama en fazla 500 karakter olabilir")]
    public string? MetaDescription { get; init; }
    
    [StringLength(500, ErrorMessage = "Meta anahtar kelimeler en fazla 500 karakter olabilir")]
    public string? MetaKeywords { get; init; }
}
