namespace Domain.Entities;

public sealed class Refund : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long PaymentId { get; set; }
    public long? OrderGroupId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string? Reason { get; set; }
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; init; }
}


