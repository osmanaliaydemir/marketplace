namespace Application.DTOs.Inventory;

public sealed record StockHistoryDto
{
    public long Id { get; init; }
    public long ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public StockOperationType OperationType { get; init; }
    public int Quantity { get; init; }
    public int PreviousStock { get; init; }
    public int NewStock { get; init; }
    public string? Reason { get; init; }
    public string? Reference { get; init; }
    public long? OrderId { get; init; }
    public long? UserId { get; init; }
    public string? UserName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}
