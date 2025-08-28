namespace Application.DTOs.Stores;

public sealed record StoreUpdateRequest
{
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? LogoUrl { get; init; }
    public string? BannerUrl { get; init; }
    public string? Description { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Website { get; init; }
    public string? Address { get; init; }
    public string Currency { get; init; } = "TRY";
    public string Language { get; init; } = "tr";
    public bool IsActive { get; init; }
}
