namespace Application.DTOs.Inventory;

public sealed record InventoryStatsDto
{
    public int TotalProducts { get; init; }
    public int InStockProducts { get; init; }
    public int OutOfStockProducts { get; init; }
    public int LowStockProducts { get; init; }
    public int OverstockedProducts { get; init; }
    
    public int TotalStockQuantity { get; init; }
    public int ReservedStockQuantity { get; init; }
    public int AvailableStockQuantity { get; init; }
    public decimal TotalStockValue { get; init; }
    public string Currency { get; init; } = "TRY";
    
    public decimal AverageStockPerProduct { get; init; }
    public decimal StockTurnoverRate { get; init; }
    public int DaysOfInventory { get; init; }
    
    public int ActiveReservations { get; init; }
    public int ExpiredReservations { get; init; }
    public int ConfirmedReservations { get; init; }
    
    public DateTime LastStockUpdate { get; init; }
    public DateTime LastInventoryCount { get; init; }
    
    public IEnumerable<InventoryAlertDto> Alerts { get; init; } = Enumerable.Empty<InventoryAlertDto>();
    public IEnumerable<InventoryMovementDto> RecentMovements { get; init; } = Enumerable.Empty<InventoryMovementDto>();
}

public sealed record InventoryAlertDto
{
    public long ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string AlertType { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public bool IsResolved { get; init; }
    public DateTime? ResolvedAt { get; init; }
}
