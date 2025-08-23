namespace Domain.Entities;

public sealed class OrderItem : Domain.Models.AuditableEntity
{
    public long OrderId { get; set; }
    public long ProductId { get; set; }
    public long StoreId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? ProductName { get; set; }
    public string? ProductSku { get; set; }
    
    // Navigation properties
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public Store Store { get; set; } = null!;
}


