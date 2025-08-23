namespace Application.DTOs.Orders;

public sealed record CreateRefundRequest
{
    public decimal Amount { get; init; }
    public string Reason { get; init; } = string.Empty;
    public string? Notes { get; init; }
}
