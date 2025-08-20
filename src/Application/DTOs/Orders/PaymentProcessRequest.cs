namespace Application.DTOs.Orders;

public sealed record PaymentProcessRequest
{
    public string PaymentToken { get; init; } = string.Empty;
    public Dictionary<string, string>? AdditionalData { get; init; }
}
