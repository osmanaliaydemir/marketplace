namespace Application.DTOs.Payments;

public sealed record PaymentProcessResult
{
    public bool Success { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? TransactionId { get; init; }
    public string? ErrorMessage { get; init; }
}
