namespace Application.DTOs.Payments;

public sealed record PaymentProcessRequest
{
    public string PaymentId { get; init; } = string.Empty;
    public string? TransactionId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? ErrorMessage { get; init; }
}
