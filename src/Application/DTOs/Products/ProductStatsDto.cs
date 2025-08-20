namespace Application.DTOs.Products;

public sealed record ProductStatsDto
{
    public int TotalProducts { get; init; }
    public int ActiveProducts { get; init; }
    public int InactiveProducts { get; init; }
    public int PublishedProducts { get; init; }
    public int UnpublishedProducts { get; init; }
    public int FeaturedProducts { get; init; }
    public int OutOfStockProducts { get; init; }
    public int LowStockProducts { get; init; }
    
    public decimal TotalInventoryValue { get; init; }
    public string Currency { get; init; } = "TRY";
    public decimal AverageProductPrice { get; init; }
    public decimal TotalRevenue { get; init; }
    
    public int TotalVariants { get; init; }
    public int TotalImages { get; init; }
    public int ProductsWithVariants { get; init; }
    public int ProductsWithImages { get; init; }
    
    public DateTime? FirstProductDate { get; init; }
    public DateTime? LastProductDate { get; init; }
    public int ProductsAddedThisMonth { get; init; }
    public int ProductsUpdatedThisMonth { get; init; }
    
    public IEnumerable<ProductCategoryStatsDto> CategoryStats { get; init; } = Enumerable.Empty<ProductCategoryStatsDto>();
    public IEnumerable<ProductStoreStatsDto> StoreStats { get; init; } = Enumerable.Empty<ProductStoreStatsDto>();
}

public sealed record ProductCategoryStatsDto
{
    public long CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public int ProductCount { get; init; }
    public decimal TotalValue { get; init; }
    public decimal AveragePrice { get; init; }
}

public sealed record ProductStoreStatsDto
{
    public long StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;
    public int ProductCount { get; init; }
    public decimal TotalValue { get; init; }
    public decimal AveragePrice { get; init; }
}
