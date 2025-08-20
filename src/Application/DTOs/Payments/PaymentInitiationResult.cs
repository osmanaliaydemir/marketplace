namespace Application.DTOs.Payments;

public sealed record PaymentInitiationResult
{
    public bool Success { get; init; }
    public string? PaymentId { get; init; }
    public string? ProviderPaymentId { get; init; }
    public string? RedirectUrl { get; init; }
    public string? ErrorMessage { get; init; }
}
