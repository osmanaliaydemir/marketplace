namespace Application.DTOs.Products;

public sealed record UpdateProductImageRequest
{
    public string ImageUrl { get; init; } = string.Empty;
    public string? ThumbnailUrl { get; init; }
    public string? AltText { get; init; }
    public string? Title { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsPrimary { get; init; }
    public bool IsActive { get; init; }
}
