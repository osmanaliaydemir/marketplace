namespace Application.DTOs.Products;

public sealed record ProductListRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SortBy { get; init; } = "CreatedAt";
    public string? SortOrder { get; init; } = "Desc";
    public long? CategoryId { get; init; }
    public long? StoreId { get; init; }
    public bool? IsActive { get; init; }
    public bool? IsPublished { get; init; }
    public bool? IsFeatured { get; init; }
}
