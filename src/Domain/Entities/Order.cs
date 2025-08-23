using Domain.Models;
using Domain.ValueObjects;
using Domain.Enums;

namespace Domain.Entities;

public sealed class Order : Domain.Models.AuditableEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public long CustomerId { get; set; }
    public long StoreId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string? Notes { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public Store Store { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public Payment? Payment { get; set; }
    public Shipment? Shipment { get; set; }
}
