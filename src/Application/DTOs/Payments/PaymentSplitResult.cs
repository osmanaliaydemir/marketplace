namespace Application.DTOs.Payments;

public sealed record PaymentSplitResult
{
    public bool Success { get; init; }
    public string? PaymentId { get; init; }
    public long StoreId { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal CommissionAmount { get; init; }
    public decimal NetAmount { get; init; }
    // CommissionRate is not supported
    public string? Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? ErrorMessage { get; init; }
}

public sealed record PaymentSplitDetailResultDto
{
    public long StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? ProcessedAt { get; init; }
    public string? Reference { get; init; }
}
