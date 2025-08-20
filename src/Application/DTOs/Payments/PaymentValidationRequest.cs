using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Payments;

public sealed record PaymentValidationRequest
{
    public long OrderId { get; init; }
    public long PaymentMethodId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "TRY";
}
