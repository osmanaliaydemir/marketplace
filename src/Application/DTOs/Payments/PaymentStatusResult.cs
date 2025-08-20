namespace Application.DTOs.Payments;

public sealed record PaymentStatusResult
{
    public bool Success { get; init; }
    public string? Status { get; init; }
    public string? TransactionId { get; init; }
    public string? ErrorMessage { get; init; }
}
