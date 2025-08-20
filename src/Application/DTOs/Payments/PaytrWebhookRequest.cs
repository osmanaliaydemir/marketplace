namespace Application.DTOs.Payments;

public sealed record PaytrWebhookRequest
{
    public string PaymentId { get; init; } = string.Empty;
    public string EventType { get; init; } = string.Empty;
    public string? TransactionId { get; init; }
    public string? Signature { get; init; }
    public string? Payload { get; init; }
}
