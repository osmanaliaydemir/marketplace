namespace Domain.Entities;

public sealed class Store : Domain.Models.AuditableEntity
{
    public long SellerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }
    public string? WorkingHours { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string Currency { get; set; } = "TRY";
    public string Language { get; set; } = "tr";
    
    // Navigation properties
    public Seller Seller { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<StoreCategory> StoreCategories { get; set; } = new List<StoreCategory>();
}


