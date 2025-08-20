namespace Application.DTOs.Payments;

public sealed record PaymentFraudCheckRequest
{
    public string PaymentId { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    public string? DeviceId { get; init; }
}
