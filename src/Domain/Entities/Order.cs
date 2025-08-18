using Domain.Models;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Order : AuditableEntity
{
    public long CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}

public enum OrderStatus
{
    Pending = 1,
    Confirmed = 2,
    Processing = 3,
    Shipped = 4,
    Delivered = 5,
    Completed = 6,
    Cancelled = 7,
    Refunded = 8
}
