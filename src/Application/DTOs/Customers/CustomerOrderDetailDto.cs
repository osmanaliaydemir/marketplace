namespace Application.DTOs.Customers;

public sealed record CustomerOrderDetailDto
{
    public long Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public DateTime CreatedAt { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerEmail { get; init; } = string.Empty;
    public string? CustomerPhone { get; init; }
    public CustomerAddressDto ShippingAddress { get; init; } = null!;
    public CustomerAddressDto BillingAddress { get; init; } = null!;
    public List<CustomerOrderItemDto> Items { get; init; } = new();
    public string PaymentMethod { get; init; } = string.Empty;
    public string PaymentStatus { get; init; } = string.Empty;
    public string ShippingMethod { get; init; } = string.Empty;
    public string? TrackingNumber { get; init; }
    public DateTime? EstimatedDelivery { get; init; }
}
