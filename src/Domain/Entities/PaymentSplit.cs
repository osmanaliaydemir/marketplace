namespace Domain.Entities;

public sealed class PaymentSplit : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long PaymentId { get; set; }
    public long OrderGroupId { get; set; }
    public long SellerId { get; set; }
    public decimal SellerAmount { get; set; }
    public decimal PlatformCommission { get; set; }
    public string Currency { get; set; } = "TRY";
}


