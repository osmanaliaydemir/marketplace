namespace Application.DTOs.Stores;

public sealed record StoreStatsDto
{
    public int TotalStores { get; init; }
    public int ActiveStores { get; init; }
    public int InactiveStores { get; init; }
    public int TotalSellers { get; init; }
    public int ActiveSellers { get; init; }
    public int AverageProductsPerStore { get; init; }
    public int NewStoresThisMonth { get; init; }
    public int NewStoresThisWeek { get; init; }
}
