namespace Application.DTOs.Inventory;

public sealed record StockReservationDto
{
    public long Id { get; init; }
    public long ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public string Reason { get; init; } = string.Empty;
    public string? Reference { get; init; }
    public long? OrderId { get; init; }
    public long? CustomerId { get; init; }
    public string? CustomerName { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }
    public bool IsExpired { get; init; }
    public bool IsConfirmed { get; init; }
    public DateTime? ConfirmedAt { get; init; }
    public DateTime? CancelledAt { get; init; }
    public string? CancellationReason { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}
