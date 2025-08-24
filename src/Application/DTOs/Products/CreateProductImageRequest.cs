namespace Application.DTOs.Products;

public sealed record CreateProductImageRequest
{
    public string ImageUrl { get; init; } = string.Empty;
    public string? ThumbnailUrl { get; init; }
    public string? AltText { get; init; }
    public string? Title { get; init; }
    public int DisplayOrder { get; init; } = 0;
    public bool IsPrimary { get; init; } = false;
}
