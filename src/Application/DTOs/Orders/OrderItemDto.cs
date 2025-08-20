namespace Application.DTOs.Orders;

public record OrderItemDto
{
    public long Id { get; init; }
    public long OrderId { get; init; }
    public long ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? ProductSku { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string? Notes { get; init; }
}
