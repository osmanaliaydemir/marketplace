using Application.DTOs.Categories;
using Application.DTOs.Stores;
using Application.DTOs.Sellers;

namespace Application.DTOs.Products;

/// <summary>
/// Ürün detay bilgilerini içeren DTO
/// </summary>
/// <remarks>
/// Bu DTO ürünün tam detaylarını, kategori bilgilerini, mağaza bilgilerini ve 
/// satıcı bilgilerini içerir. Ürün listesi ve detay sayfalarında kullanılır.
/// </remarks>
public class ProductDetailDto
{
    /// <summary>
    /// Ürün benzersiz kimliği
    /// </summary>
    /// <example>1</example>
    public long Id { get; set; }

    /// <summary>
    /// Ürün adı
    /// </summary>
    /// <example>iPhone 15 Pro</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Ürün detaylı açıklaması
    /// </summary>
    /// <example>Apple iPhone 15 Pro - En yeni teknoloji ile donatılmış akıllı telefon</example>
    public string? Description { get; set; }

    /// <summary>
    /// Ürün kısa açıklaması
    /// </summary>
    /// <example>256GB, Titanium, 5G</example>
    public string? ShortDescription { get; set; }

    /// <summary>
    /// Ürün SEO dostu URL parçası
    /// </summary>
    /// <example>iphone-15-pro</example>
    public string? Slug { get; set; }

    /// <summary>
    /// Ürün fiyatı
    /// </summary>
    /// <example>29999.99</example>
    public decimal Price { get; set; }

    /// <summary>
    /// Ürünün eski fiyatı (indirim göstermek için)
    /// </summary>
    /// <example>32999.99</example>
    public decimal? CompareAtPrice { get; set; }

    /// <summary>
    /// Para birimi
    /// </summary>
    /// <example>TRY</example>
    public string Currency { get; set; } = "TRY";

    /// <summary>
    /// Stok miktarı
    /// </summary>
    /// <example>50</example>
    public int StockQty { get; set; }

    /// <summary>
    /// Ürün ağırlığı (gram)
    /// </summary>
    /// <example>187.0</example>
    public decimal? Weight { get; set; }

    /// <summary>
    /// Minimum sipariş miktarı
    /// </summary>
    /// <example>1</example>
    public int? MinOrderQty { get; set; }

    /// <summary>
    /// Maksimum sipariş miktarı
    /// </summary>
    /// <example>5</example>
    public int? MaxOrderQty { get; set; }

    /// <summary>
    /// Ürün aktif mi?
    /// </summary>
    /// <example>true</example>
    public bool IsActive { get; set; }

    /// <summary>
    /// Ürün yayınlanmış mı?
    /// </summary>
    /// <example>true</example>
    public bool IsPublished { get; set; }

    /// <summary>
    /// Ürün öne çıkan ürün mü?
    /// </summary>
    /// <example>true</example>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Görüntülenme sırası
    /// </summary>
    /// <example>1</example>
    public int? DisplayOrder { get; set; }

    /// <summary>
    /// Kategori ID'si
    /// </summary>
    /// <example>1</example>
    public long CategoryId { get; set; }

    /// <summary>
    /// Kategori adı
    /// </summary>
    /// <example>Elektronik</example>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Mağaza ID'si
    /// </summary>
    /// <example>1</example>
    public long StoreId { get; set; }

    /// <summary>
    /// Mağaza adı
    /// </summary>
    /// <example>TechStore</example>
    public string StoreName { get; set; } = string.Empty;

    /// <summary>
    /// Satıcı ID'si
    /// </summary>
    /// <example>1</example>
    public long SellerId { get; set; }

    /// <summary>
    /// Satıcı adı
    /// </summary>
    /// <example>TechStore Seller</example>
    public string SellerName { get; set; } = string.Empty;

    /// <summary>
    /// SEO başlığı
    /// </summary>
    /// <example>iPhone 15 Pro - TechStore</example>
    public string? MetaTitle { get; set; }

    /// <summary>
    /// SEO açıklaması
    /// </summary>
    /// <example>Apple iPhone 15 Pro 256GB Titanium modeli</example>
    public string? MetaDescription { get; set; }

    /// <summary>
    /// SEO anahtar kelimeleri
    /// </summary>
    /// <example>iphone, apple, smartphone, 5g</example>
    public string? MetaKeywords { get; set; }

    /// <summary>
    /// Yayınlanma tarihi
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Son güncellenme tarihi
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Güncellenme tarihi (alias)
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Ürün markası
    /// </summary>
    /// <example>Apple</example>
    public string? Brand { get; set; }

    /// <summary>
    /// Ürün modeli
    /// </summary>
    /// <example>iPhone 15 Pro</example>
    public string? Model { get; set; }

    /// <summary>
    /// Stok kodu
    /// </summary>
    /// <example>IP15PRO256</example>
    public string? Sku { get; set; }

    /// <summary>
    /// Barkod
    /// </summary>
    /// <example>1234567890123</example>
    public string? Barcode { get; set; }

    /// <summary>
    /// Ürün puanı
    /// </summary>
    /// <example>4.5</example>
    public decimal? Rating { get; set; }

    /// <summary>
    /// Değerlendirme sayısı
    /// </summary>
    /// <example>125</example>
    public int? ReviewCount { get; set; }

    /// <summary>
    /// Kategori bilgisi
    /// </summary>
    public CategoryDto? Category { get; set; }

    /// <summary>
    /// Mağaza bilgisi
    /// </summary>
    public Application.DTOs.Stores.StoreDto? Store { get; set; }

    /// <summary>
    /// Satıcı bilgisi
    /// </summary>
    public Application.DTOs.Sellers.SellerDto? Seller { get; set; }

    /// <summary>
    /// Ürün varyantları
    /// </summary>
    public IEnumerable<ProductVariantDto> Variants { get; set; } = Enumerable.Empty<ProductVariantDto>();

    /// <summary>
    /// Ürün resimleri
    /// </summary>
    public IEnumerable<ProductImageDto> Images { get; set; } = Enumerable.Empty<ProductImageDto>();

    /// <summary>
    /// Ürün değerlendirmeleri
    /// </summary>
    public IEnumerable<ProductReviewDto> Reviews { get; set; } = Enumerable.Empty<ProductReviewDto>();

    /// <summary>
    /// İndirim yüzdesi
    /// </summary>
    public decimal? DiscountPercentage => CompareAtPrice.HasValue && CompareAtPrice > Price 
        ? Math.Round(((CompareAtPrice.Value - Price) / CompareAtPrice.Value) * 100, 2) 
        : null;

    /// <summary>
    /// İndirim var mı?
    /// </summary>
    public bool HasDiscount => CompareAtPrice.HasValue && CompareAtPrice > Price;

    /// <summary>
    /// Stokta var mı?
    /// </summary>
    public bool IsInStock => StockQty > 0;

    /// <summary>
    /// Düşük stok mu?
    /// </summary>
    public bool IsLowStock => StockQty > 0 && StockQty <= 10;
}

// Product Review DTO
public record ProductReviewDto
{
    public long Id { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public int Rating { get; init; }
    public string Comment { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
} 
