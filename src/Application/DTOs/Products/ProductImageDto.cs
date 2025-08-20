namespace Application.DTOs.Products;

public sealed record ProductImageDto
{
    public long Id { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string? AltText { get; init; }
    public string? Description { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsPrimary { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}
