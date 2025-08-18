namespace Domain.Entities;

public sealed class Shipment : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long OrderGroupId { get; set; }
    public string Carrier { get; set; } = string.Empty;
    public string? Service { get; set; }
    public string? TrackingNumber { get; set; }
    public string? LabelUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}


