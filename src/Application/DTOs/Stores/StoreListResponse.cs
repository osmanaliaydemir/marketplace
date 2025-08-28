namespace Application.DTOs.Stores;

public sealed record StoreListResponse
{
    public IEnumerable<StoreListDto> Items { get; init; } = Enumerable.Empty<StoreListDto>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}
