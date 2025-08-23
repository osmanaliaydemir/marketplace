namespace Application.DTOs.Payments;

public sealed record PaymentRefundRequest
{
    public decimal Amount { get; init; }
    public string Reason { get; init; } = string.Empty;
}
