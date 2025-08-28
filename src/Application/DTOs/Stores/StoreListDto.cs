using Application.DTOs.Sellers;

namespace Application.DTOs.Stores;

public sealed record StoreListDto
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? LogoUrl { get; init; }
    public bool IsActive { get; init; }
    public string SellerName { get; init; } = string.Empty;
    public int ProductCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public SellerInfoDto? Seller { get; init; }
}
