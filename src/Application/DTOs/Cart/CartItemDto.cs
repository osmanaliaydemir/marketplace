using Domain.ValueObjects;

namespace Application.DTOs.Cart;

public class CartItemDto
{
    public long Id { get; init; }
    public long CartId { get; init; }
    public long ProductId { get; init; }
    public long? ProductVariantId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductSku { get; init; } = string.Empty;
    public string? ProductVariantName { get; init; }
    public int Quantity { get; init; }
    public Money UnitPrice { get; init; }
    public Money TotalPrice { get; init; }
    public string Currency { get; init; } = "TRY";
    public DateTime AddedAt { get; init; }
    public string? ImageUrl { get; init; }
    public bool IsAvailable { get; init; }
    public int AvailableStock { get; init; }
    public long StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;
}
