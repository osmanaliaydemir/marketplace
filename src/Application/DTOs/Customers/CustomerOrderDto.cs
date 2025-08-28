namespace Application.DTOs.Customers;

public sealed record CustomerOrderDto
{
    public long Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public DateTime CreatedAt { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerEmail { get; init; } = string.Empty;
    public List<CustomerOrderItemDto> Items { get; init; } = new();
}
