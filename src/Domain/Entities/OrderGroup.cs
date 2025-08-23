namespace Domain.Entities;

public sealed class OrderGroup : Domain.Models.AuditableEntity
{
    public long CustomerId { get; set; }
    public string GroupNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Cancelled
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "TRY";
    public DateTime? CompletedAt { get; set; }
    
    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}


