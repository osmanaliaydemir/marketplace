namespace Domain.Entities;

public sealed class Seller : Domain.Models.AuditableEntity
{
    public long UserId { get; set; }
    public decimal CommissionRate { get; set; } = 10.00m;
    public bool IsActive { get; set; } = true;
    public string? CompanyName { get; set; }
    public string? TaxNumber { get; set; }
    public string? BankAccountInfo { get; set; }
    
    // Navigation properties
    public AppUser User { get; set; } = null!;
    public ICollection<Store> Stores { get; set; } = new List<Store>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
