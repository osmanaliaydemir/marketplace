namespace Application.DTOs.Payments;

public sealed record PaymentProcessRequest
{
    public string PaymentToken { get; init; } = string.Empty;
    public Dictionary<string, string>? AdditionalData { get; init; }
}
