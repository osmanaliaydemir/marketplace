namespace Application.DTOs.Payments;

public sealed record RefundStatusResult
{
    public bool Success { get; init; }
    public string? RefundId { get; init; }
    public string? Status { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "TRY";
    public DateTime CreatedAt { get; init; }
    public string? ErrorMessage { get; init; }
}
