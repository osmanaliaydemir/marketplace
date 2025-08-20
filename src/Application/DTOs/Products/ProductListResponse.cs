namespace Application.DTOs.Products;

public sealed record ProductListResponse
{
    public IEnumerable<ProductDto> Products { get; init; } = Enumerable.Empty<ProductDto>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
}


