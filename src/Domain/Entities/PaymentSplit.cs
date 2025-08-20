namespace Domain.Entities;

public sealed class PaymentSplit : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long PaymentId { get; set; }
    public long StoreId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal StoreAmount { get; set; }
    public decimal CommissionRate { get; set; }
    public string Status { get; set; } = "pending";
    public string Currency { get; set; } = "TRY";
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; set; }
}


