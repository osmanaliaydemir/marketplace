using Domain.Models;
using Domain.ValueObjects;

namespace Domain.Entities;

public class CartItem : BaseEntity
{
    public long CartId { get; set; }
    public long ProductId { get; set; }
    public long? ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public Money UnitPrice { get; set; }
    public Money TotalPrice { get; set; }
    public DateTime AddedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Cart Cart { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual ProductVariant? ProductVariant { get; set; }
}
