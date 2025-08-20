namespace Application.DTOs.Orders;

public record OrderGroupDto
{
    public long Id { get; init; }
    public string GroupNumber { get; init; } = string.Empty;
    public long CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public int OrderCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    
    // Navigation properties
    public IEnumerable<OrderDto> Orders { get; init; } = Enumerable.Empty<OrderDto>();
}
