namespace Domain.Entities;

public sealed class Refund : Domain.Models.AuditableEntity
{
    public long PaymentId { get; set; }
    public long OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Completed
    public string? Reason { get; set; }
    public string? AdminNotes { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    // Navigation properties
    public Payment Payment { get; set; } = null!;
    public Order Order { get; set; } = null!;
    public ICollection<RefundItem> Items { get; set; } = new List<RefundItem>();
}


