namespace Application.DTOs.Categories;

public sealed record UpdateCategoryRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public long? ParentId { get; init; }
    public string? Slug { get; init; }
    public string? ImageUrl { get; init; }
    public string? IconClass { get; init; }
    public bool IsActive { get; init; }
    public bool IsFeatured { get; init; }
    public int DisplayOrder { get; init; }
    public string? MetaTitle { get; init; }
    public string? MetaDescription { get; init; }
}
