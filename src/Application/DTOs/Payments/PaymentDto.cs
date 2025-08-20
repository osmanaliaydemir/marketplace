namespace Application.DTOs.Payments;

public sealed record PaymentDto
{
    public long Id { get; init; }
    public long OrderId { get; init; }
    public long CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public long StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string PaymentMethod { get; init; } = string.Empty;
    public string? ProviderPaymentId { get; init; }
    public string? TransactionId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
}
