namespace Application.DTOs.Cart;

public sealed record CartStoreGroupDto
{
    public long StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;
    public string StoreSlug { get; init; } = string.Empty;
    public bool StoreIsActive { get; init; }
    public IEnumerable<CartItemDto> Items { get; init; } = Enumerable.Empty<CartItemDto>();
    public int ItemCount { get; init; }
    public decimal SubTotal { get; init; }
    public decimal ShippingCost { get; init; }
    public decimal ShippingTotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal Total { get; init; }
    public string Currency { get; init; } = "TRY";
    public bool IsValid { get; init; }
    public string? ValidationMessage { get; init; }
}
