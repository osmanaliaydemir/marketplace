namespace Application.DTOs.Orders;

public sealed record PaymentStatusDto
{
    public long OrderId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public string PaymentStatus { get; init; } = string.Empty;
    public string PaymentMethod { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "TRY";
    public DateTime? PaymentDate { get; init; }
    public string? TransactionId { get; init; }
    public string? PaymentProviderResponse { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}
