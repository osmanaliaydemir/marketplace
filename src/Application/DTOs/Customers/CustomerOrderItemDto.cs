namespace Application.DTOs.Customers;

public sealed record CustomerOrderItemDto
{
    public long ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? ProductImage { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal? TotalPrice { get; init; }
}
