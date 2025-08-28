namespace Application.DTOs.Stores;

public sealed record StoreListRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public bool? IsActive { get; init; }
    public long? SellerId { get; init; }
}
