namespace Domain.Entities;

public sealed class OrderGroup : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long CustomerId { get; set; }
    public long OrderId { get; set; }
    public long SellerId { get; set; }
    public long StoreId { get; set; }
    public decimal ItemsTotal { get; set; }
    public decimal ShippingTotal { get; set; }
    public decimal GroupTotal { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; init; }
}


