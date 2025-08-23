namespace Application.DTOs.Payments;

public record PaymentCallbackRequest
{
    public string PaymentId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? TransactionId { get; init; }
    public string? Signature { get; init; }
    public string? RawData { get; init; }
}
