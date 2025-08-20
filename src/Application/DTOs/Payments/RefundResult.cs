namespace Application.DTOs.Payments;

public sealed record RefundResult
{
    public bool Success { get; init; }
    public string? RefundId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "TRY";
    public string? ErrorMessage { get; init; }
}
