namespace Application.DTOs.Cart;

public sealed record CartDto
{
    public long Id { get; init; }
    public long CustomerId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
    
    // Cart Items
    public IEnumerable<CartItemDto> Items { get; init; } = Enumerable.Empty<CartItemDto>();
    
    // Summary
    public int TotalItemCount { get; init; }
    public int UniqueProductCount { get; init; }
    public decimal SubTotal { get; init; }
    public decimal Total { get; init; }
    public string Currency { get; init; } = "TRY";
    
    // Status
    public bool IsEmpty => !Items.Any();
    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
    public bool HasItems => Items.Any();
}


