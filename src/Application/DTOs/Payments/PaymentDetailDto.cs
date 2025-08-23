namespace Application.DTOs.Payments;

public sealed record PaymentDetailDto
{
    public long Id { get; init; }
    public long OrderId { get; init; }
    public long CustomerId { get; init; }
    public long StoreId { get; init; }
    public string Provider { get; init; } = string.Empty;
    public string ProviderPaymentId { get; init; } = string.Empty;
    public string TransactionId { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "TRY";
    public string Status { get; init; } = string.Empty;
    public string? ErrorMessage { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
}
