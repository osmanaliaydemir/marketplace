namespace Application.DTOs.Orders;

public sealed record OrderStatsDto
{
    public int TotalOrders { get; init; }
    public int PendingOrders { get; init; }
    public int ConfirmedOrders { get; init; }
    public int ProcessingOrders { get; init; }
    public int ShippedOrders { get; init; }
    public int DeliveredOrders { get; init; }
    public int CancelledOrders { get; init; }
    public int RefundedOrders { get; init; }
    
    public decimal TotalRevenue { get; init; }
    public decimal PendingRevenue { get; init; }
    public decimal ConfirmedRevenue { get; init; }
    public decimal ProcessingRevenue { get; init; }
    public decimal ShippedRevenue { get; init; }
    public decimal DeliveredRevenue { get; init; }
    public decimal CancelledRevenue { get; init; }
    public decimal RefundedRevenue { get; init; }
    
    public string Currency { get; init; } = "TRY";
    public decimal AverageOrderValue { get; init; }
    public int TotalItems { get; init; }
    public decimal AverageItemsPerOrder { get; init; }
    
    public DateTime? FirstOrderDate { get; init; }
    public DateTime? LastOrderDate { get; init; }
    public TimeSpan AverageProcessingTime { get; init; }
    public TimeSpan AverageShippingTime { get; init; }
    
    public IEnumerable<OrderStatusTrendDto> StatusTrends { get; init; } = Enumerable.Empty<OrderStatusTrendDto>();
    public IEnumerable<OrderRevenueTrendDto> RevenueTrends { get; init; } = Enumerable.Empty<OrderRevenueTrendDto>();
}

public sealed record OrderStatusTrendDto
{
    public string Status { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public int Count { get; init; }
    public decimal Revenue { get; init; }
}

public sealed record OrderRevenueTrendDto
{
    public DateTime Date { get; init; }
    public int OrderCount { get; init; }
    public decimal Revenue { get; init; }
    public decimal AverageOrderValue { get; init; }
}
