using Domain.Enums;

namespace Application.DTOs.Orders;

public record OrderDto
{
    public long Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public long CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public long StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;
    public OrderStatus Status { get; init; }
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public int ItemCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ShippedAt { get; init; }
    public DateTime? DeliveredAt { get; init; }
}
