using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Payments;

public sealed record PaytrTokenRequest
{
    public long OrderId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "TRY";
    public string CustomerEmail { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
}
