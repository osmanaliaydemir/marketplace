using Domain.Models;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Cart : BaseEntity
{
    public long CustomerId { get; set; }
    public long StoreId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    
    // Navigation properties
    public virtual Store Store { get; set; } = null!;
    public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
