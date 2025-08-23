namespace Domain.Entities;

public sealed class Category : Domain.Models.AuditableEntity
{
    public long? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? IconClass { get; set; } // Bootstrap icon class
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false; // Ana sayfada göster
    public int DisplayOrder { get; set; } = 0; // Görüntüleme sırası
    public string? MetaTitle { get; set; } // SEO
    public string? MetaDescription { get; set; } // SEO
    
    // Navigation properties
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}


