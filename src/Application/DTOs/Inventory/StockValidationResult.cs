namespace Application.DTOs.Inventory;

public sealed record StockValidationResult
{
    public long ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public bool IsValid { get; init; }
    public IEnumerable<StockValidationError> Errors { get; init; } = Enumerable.Empty<StockValidationError>();
    public IEnumerable<StockValidationWarning> Warnings { get; init; } = Enumerable.Empty<StockValidationWarning>();
    
    // Current stock information
    public int CurrentStock { get; init; }
    public int ReservedStock { get; init; }
    public int AvailableStock { get; init; }
    public int MinimumStock { get; init; }
    public int MaximumStock { get; init; }
    
    // Stock health indicators
    public bool IsLowStock { get; init; }
    public bool IsOutOfStock { get; init; }
    public bool IsOverstocked { get; init; }
    public decimal StockTurnoverRate { get; init; }
    public int DaysOfInventory { get; init; }
}

public sealed record StockValidationError
{
    public string ErrorCode { get; init; } = string.Empty;
    public string ErrorMessage { get; init; } = string.Empty;
    public StockValidationErrorType ErrorType { get; init; }
    public string? Suggestion { get; init; }
}

public sealed record StockValidationWarning
{
    public string WarningCode { get; init; } = string.Empty;
    public string WarningMessage { get; init; } = string.Empty;
    public StockValidationWarningType WarningType { get; init; }
    public string? Suggestion { get; init; }
}

public enum StockValidationErrorType
{
    InsufficientStock,
    StockBelowMinimum,
    InvalidStockOperation,
    StockReservationConflict,
    StockOperationNotAllowed,
    StockDataInconsistency
}

public enum StockValidationWarningType
{
    LowStock,
    Overstocked,
    SlowMovingStock,
    ExpiringStock,
    StockReservationExpiring,
    StockValueFluctuation
}
