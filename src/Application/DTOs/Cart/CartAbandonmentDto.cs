namespace Application.DTOs.Cart;

public sealed record CartAbandonmentDto
{
    public long CartId { get; init; }
    public long CustomerId { get; init; }
    public string CustomerEmail { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? LastActivity { get; init; }
    public TimeSpan TimeSinceLastActivity { get; init; }
    public int ItemCount { get; init; }
    public decimal TotalValue { get; init; }
    public string Currency { get; init; } = "TRY";
    public bool IsRecovered { get; init; }
    public DateTime? RecoveredAt { get; init; }
    public decimal? RecoveredValue { get; init; }
    public string? AbandonmentReason { get; init; }
    public IEnumerable<CartAbandonmentItemDto> Items { get; init; } = Enumerable.Empty<CartAbandonmentItemDto>();
}

public sealed record CartAbandonmentItemDto
{
    public long ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductSlug { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice { get; init; }
    public string? ProductImageUrl { get; init; }
    public long StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;
}
