namespace Application.DTOs.Products;

public sealed record ProductImageDto
{
    public long Id { get; init; }
    public long ProductId { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string? ThumbnailUrl { get; init; }
    public string? AltText { get; init; }
    public string? Title { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsPrimary { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}
