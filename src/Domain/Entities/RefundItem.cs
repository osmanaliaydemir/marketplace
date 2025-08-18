namespace Domain.Entities;

public sealed class RefundItem : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long RefundId { get; set; }
    public long OrderItemId { get; set; }
    public decimal Amount { get; set; }
}


