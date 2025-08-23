namespace Domain.Entities;

public sealed class ProductImage : Domain.Models.AuditableEntity
{
    public long ProductId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsPrimary { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public string? ThumbnailUrl { get; set; }
    
    // Navigation properties
    public Product Product { get; set; } = null!;
}
