namespace Application.DTOs.Products;

public sealed record ProductListRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SearchTerm { get; init; }
    public long? CategoryId { get; init; }
    public long? StoreId { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public bool? IsActive { get; init; }
    public bool? IsFeatured { get; init; }
    public bool? IsPublished { get; init; }
    public bool? InStock { get; init; }
    public string? SortBy { get; init; } = "CreatedAt";
    public string? SortOrder { get; init; } = "Desc";
}
