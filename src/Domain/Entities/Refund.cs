namespace Domain.Entities;

public sealed class Refund : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long PaymentId { get; set; }
    public long? OrderGroupId { get; set; }
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; init; }
}


