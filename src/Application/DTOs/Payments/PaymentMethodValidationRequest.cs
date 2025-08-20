using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Payments;

public sealed record PaymentMethodValidationRequest
{
    public long MethodId { get; init; }
    public decimal? Amount { get; init; }
    public string? Currency { get; init; }
}
