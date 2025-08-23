using Domain.Models;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Cart : Domain.Models.AuditableEntity
{
    public long CustomerId { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime? ExpiresAt { get; set; }
    
    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
