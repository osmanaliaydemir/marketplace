namespace Domain.Entities;

public sealed class Category : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? IconClass { get; set; } // Bootstrap icon class
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false; // Ana sayfada göster
    public int DisplayOrder { get; set; } = 0; // Görüntüleme sırası
    public string? MetaTitle { get; set; } // SEO
    public string? MetaDescription { get; set; } // SEO
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; set; }
    
    // Navigation properties (removed virtual for sealed class)
    // Parent, Children, Products will be loaded separately in service layer
}


