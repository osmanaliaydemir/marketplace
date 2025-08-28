namespace Application.DTOs.Products;

/// <summary>
/// Ürün resmi DTO'su
/// </summary>
public class ProductImageDto
{
    /// <summary>
    /// Resim ID'si
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Ürün ID'si
    /// </summary>
    public long ProductId { get; set; }

    /// <summary>
    /// Resim URL'i
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Küçük resim URL'i
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Resim başlığı
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Resim açıklaması
    /// </summary>
    public string? AltText { get; set; }

    /// <summary>
    /// Resim sırası
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Ana resim mi?
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Resim aktif mi?
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Güncellenme tarihi
    /// </summary>
    public DateTime? ModifiedAt { get; set; }
}
