namespace Application.DTOs.Categories;

public sealed record CategoryListResponse
{
    public IEnumerable<CategoryDto> Categories { get; init; } = Enumerable.Empty<CategoryDto>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
}

// CategoryDto ayrı dosyada tanımlandı
