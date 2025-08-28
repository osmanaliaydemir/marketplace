namespace Application.DTOs.Stores;

public sealed record StoreSearchRequest
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
    public long? SellerId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
