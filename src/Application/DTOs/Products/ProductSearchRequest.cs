namespace Application.DTOs.Products;

public sealed record ProductSearchRequest
{
    public string? SearchTerm { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public long? CategoryId { get; init; }
    public long? StoreId { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public bool? IsActive { get; init; } = true;
    public bool? IsPublished { get; init; } = true;
    public bool? InStock { get; init; }
    public bool? IsFeatured { get; init; }
    public string? SortBy { get; init; } = "Relevance";
    public string? SortOrder { get; init; } = "Desc";
    public bool IncludeVariants { get; init; } = false;
    public bool IncludeImages { get; init; } = true;
    public string? Brand { get; init; }
    public string? Tags { get; init; }
}
