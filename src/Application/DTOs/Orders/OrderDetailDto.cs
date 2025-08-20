namespace Application.DTOs.Orders;

public record OrderDetailDto
{
    public long Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public long CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public long StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal SubTotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal ShippingAmount { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public string? ShippingAddress { get; init; }
    public string? BillingAddress { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? TrackingNumber { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public DateTime? ShippedAt { get; init; }
    public DateTime? DeliveredAt { get; init; }
    public DateTime? CancelledAt { get; init; }
    
    // Navigation properties
    public IEnumerable<OrderItemDto> Items { get; init; } = Enumerable.Empty<OrderItemDto>();
}
