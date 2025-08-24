namespace Application.DTOs.Products;

public sealed record UpdateProductVariantRequest
{
    public string Sku { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public string VariantName { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal? CompareAtPrice { get; init; }
    public int StockQty { get; init; }
    public int? MinOrderQty { get; init; }
    public int? MaxOrderQty { get; init; }
    public bool IsDefault { get; init; }
    public decimal Weight { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
}
