namespace Application.DTOs.Products;

public sealed record ProductVariantDto
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Sku { get; init; }
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public decimal? CompareAtPrice { get; init; }
    public int StockQty { get; init; }
    public int Weight { get; init; }
    public bool IsActive { get; init; }
    public Dictionary<string, string>? Attributes { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}
