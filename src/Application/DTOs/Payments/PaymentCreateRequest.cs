namespace Application.DTOs.Payments;

public sealed record PaymentCreateRequest
{
    public long OrderId { get; init; }
    public long CustomerId { get; init; }
    public long StoreId { get; init; }
    public string Provider { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "TRY";
    public DateTime? ExpiresAt { get; init; }
}
