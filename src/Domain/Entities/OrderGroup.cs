namespace Domain.Entities;

public sealed class OrderGroup : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long OrderId { get; set; }
    public long SellerId { get; set; }
    public long StoreId { get; set; }
    public decimal ItemsTotal { get; set; }
    public decimal ShippingTotal { get; set; }
    public decimal GroupTotal { get; set; }
    public string Status { get; set; } = string.Empty;
}


