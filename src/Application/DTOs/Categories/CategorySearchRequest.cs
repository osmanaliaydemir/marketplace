namespace Application.DTOs.Categories;

public sealed record CategorySearchRequest
{
    public string? SearchTerm { get; init; }
    public long? ParentId { get; init; }
    public bool? IsActive { get; init; }
    public bool? IsFeatured { get; init; }
    public string? SortBy { get; init; } = "DisplayOrder"; // Name, DisplayOrder, CreatedAt
    public string? SortOrder { get; init; } = "Asc"; // Asc, Desc
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
