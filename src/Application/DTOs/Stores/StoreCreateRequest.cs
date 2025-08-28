namespace Application.DTOs.Stores;

public sealed record StoreCreateRequest
{
    public long SellerId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? LogoUrl { get; init; }
    public string? BannerUrl { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
}
