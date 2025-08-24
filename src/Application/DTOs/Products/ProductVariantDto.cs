namespace Application.DTOs.Products;

public sealed record ProductVariantDto
{
    public long Id { get; init; }
    public long ProductId { get; init; }
    public string? Sku { get; init; }
    public string? Barcode { get; init; }
    public string? VariantName { get; init; }
    public decimal Price { get; init; }
    public decimal? CompareAtPrice { get; init; }
    public int StockQty { get; init; }
    public int ReservedQty { get; init; }
    public int? MinOrderQty { get; init; }
    public int? MaxOrderQty { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsDefault { get; init; }
    public decimal Weight { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}
