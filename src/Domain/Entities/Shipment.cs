using Domain.Enums;

namespace Domain.Entities;

public sealed class Shipment : Domain.Models.BaseEntity
{
    public long OrderId { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public string Carrier { get; set; } = string.Empty;
    public ShipmentStatus Status { get; set; } = ShipmentStatus.Pending;
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public Order Order { get; set; } = null!;
}


