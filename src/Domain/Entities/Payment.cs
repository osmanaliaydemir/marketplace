namespace Domain.Entities;

public sealed class Payment : Domain.Models.AuditableEntity
{
    public long OrderId { get; set; }
    public long CustomerId { get; set; }
    public string ProviderPaymentId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Failed, Refunded
    public string? ErrorMessage { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public DateTime? RefundedAt { get; set; }
    
    // Navigation properties
    public Order Order { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public ICollection<Refund> Refunds { get; set; } = new List<Refund>();
    public ICollection<PaymentSplit> Splits { get; set; } = new List<PaymentSplit>();
}
