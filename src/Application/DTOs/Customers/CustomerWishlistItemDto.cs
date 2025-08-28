namespace Application.DTOs.Customers;

public sealed record CustomerWishlistItemDto
{
    public long Id { get; init; }
    public long ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? ProductImage { get; init; }
    public decimal Price { get; init; }
    public bool IsInStock { get; init; }
    public int StockQuantity { get; init; }
    public DateTime AddedAt { get; init; }
}
