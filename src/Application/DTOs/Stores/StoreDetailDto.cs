using Application.DTOs.Sellers;

namespace Application.DTOs.Stores;

public sealed record StoreDetailDto
{
    public long Id { get; init; }
    public long SellerId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? LogoUrl { get; init; }
    public string? BannerUrl { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public SellerInfoDto Seller { get; init; } = null!;
    public List<Application.DTOs.Categories.StoreCategoryListDto> Categories { get; init; } = new();
}

public sealed record SellerInfoDto
{
    public long Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public decimal CommissionRate { get; init; }
    public bool IsActive { get; init; }
}
