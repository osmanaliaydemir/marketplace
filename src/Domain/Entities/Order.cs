using Domain.Models;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Order : AuditableEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public long CustomerId { get; set; }
    public long StoreId { get; set; }
    public OrderStatus Status { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string? Notes { get; set; }
    public string? ShippingAddress { get; set; }
    public string? BillingAddress { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public Store Store { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
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
