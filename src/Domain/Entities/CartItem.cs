using Domain.Models;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class CartItem : Domain.Models.AuditableEntity
{
    public long CartId { get; set; }
    public long ProductId { get; set; }
    public long? ProductVariantId { get; set; }
    public long StoreId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    
    // Navigation properties
    public Cart Cart { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public Store Store { get; set; } = null!;
}
