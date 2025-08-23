namespace Application.DTOs.Payments;

public sealed record RefundDto
{
    public long Id { get; init; }
    public long PaymentId { get; init; }
    public long OrderId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "TRY";
    public string Reason { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? ProviderRefundId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
}
