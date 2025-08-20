using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Payments;

public sealed record PaymentInitiationRequest
{
    public long OrderId { get; init; }
    public long CustomerId { get; init; }
    public decimal Amount { get; init; }
    public string? Currency { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
}
