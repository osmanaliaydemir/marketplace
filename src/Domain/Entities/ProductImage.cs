namespace Domain.Entities;

public sealed class ProductImage : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; } // Küçük resim
    public string? AltText { get; set; } // Alt text (SEO)
    public string? Title { get; set; } // Resim başlığı
    public int DisplayOrder { get; set; } = 0; // Görüntüleme sırası
    public bool IsPrimary { get; set; } = false; // Ana resim mi?
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; set; }
    
    // Navigation property (removed virtual for sealed class)
    // Product will be loaded separately in service layer
}
