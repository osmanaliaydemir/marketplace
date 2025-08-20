namespace Application.DTOs.Payments;

public sealed record PaymentRequest
{
    public string PaymentId { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "TRY";
    public string CustomerEmail { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string OrderId { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
