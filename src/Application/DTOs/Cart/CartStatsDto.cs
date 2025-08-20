namespace Application.DTOs.Cart;

public sealed record CartStatsDto
{
    public long CustomerId { get; init; }
    public int TotalItems { get; init; }
    public int UniqueProducts { get; init; }
    public int UniqueStores { get; init; }
    public decimal TotalValue { get; init; }
    public string Currency { get; init; } = "TRY";
    public DateTime LastActivity { get; init; }
    public TimeSpan CartAge { get; init; }
    public TimeSpan AverageSessionDuration { get; init; }
    public int AbandonedCartCount { get; init; }
    public decimal AbandonedCartValue { get; init; }
    public decimal ConversionRate { get; init; }
    public IEnumerable<CartItemStatsDto> TopProducts { get; init; } = Enumerable.Empty<CartItemStatsDto>();
    public IEnumerable<CartStoreStatsDto> TopStores { get; init; } = Enumerable.Empty<CartStoreStatsDto>();
}

public sealed record CartItemStatsDto
{
    public long ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int AddToCartCount { get; init; }
    public int PurchaseCount { get; init; }
    public decimal TotalRevenue { get; init; }
    public decimal ConversionRate { get; init; }
}

public sealed record CartStoreStatsDto
{
    public long StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;
    public int AddToCartCount { get; init; }
    public int PurchaseCount { get; init; }
    public decimal TotalRevenue { get; init; }
    public decimal ConversionRate { get; init; }
}
