namespace Application.DTOs.Payments;

public sealed record RefundRequest
{
    public decimal Amount { get; init; }
    public string? Reason { get; init; }
}
