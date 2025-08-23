namespace Application.DTOs.Payments;

public sealed record RefundResult
{
    public bool Success { get; init; }
    public string? RefundId { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "TRY";
    public string? ErrorMessage { get; init; }
}
