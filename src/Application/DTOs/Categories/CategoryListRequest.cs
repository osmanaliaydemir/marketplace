namespace Application.DTOs.Categories;

public sealed record CategoryListRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SearchTerm { get; init; }
    public long? ParentId { get; init; }
    public bool? IsActive { get; init; }
    public bool? IsFeatured { get; init; }
    public bool IncludeInactive { get; init; } = false;
    public bool IncludeProductCount { get; init; } = false;
    public string? SortBy { get; init; } = "DisplayOrder";
    public string? SortOrder { get; init; } = "Asc";
}
