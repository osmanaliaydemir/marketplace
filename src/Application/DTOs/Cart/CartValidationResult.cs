namespace Application.DTOs.Cart;

public sealed record CartValidationResult
{
    public bool IsValid { get; init; }
    public IEnumerable<CartValidationError> Errors { get; init; } = Enumerable.Empty<CartValidationError>();
    public IEnumerable<CartValidationWarning> Warnings { get; init; } = Enumerable.Empty<CartValidationWarning>();
    public int ValidItemCount { get; init; }
    public int InvalidItemCount { get; init; }
    public decimal TotalValidAmount { get; init; }
    public string Currency { get; init; } = "TRY";
}

public sealed record CartValidationError
{
    public long ItemId { get; init; }
    public long ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ErrorCode { get; init; } = string.Empty;
    public string ErrorMessage { get; init; } = string.Empty;
    public CartValidationErrorType ErrorType { get; init; }
}

public sealed record CartValidationWarning
{
    public long ItemId { get; init; }
    public long ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string WarningCode { get; init; } = string.Empty;
    public string WarningMessage { get; init; } = string.Empty;
    public CartValidationWarningType WarningType { get; init; }
}

public enum CartValidationErrorType
{
    OutOfStock,
    InsufficientStock,
    ProductInactive,
    ProductUnpublished,
    StoreInactive,
    PriceChanged,
    ProductNotFound,
    InvalidQuantity,
    MaximumQuantityExceeded
}

public enum CartValidationWarningType
{
    LowStock,
    PriceIncrease,
    ProductDiscontinued,
    ShippingRestriction
}
