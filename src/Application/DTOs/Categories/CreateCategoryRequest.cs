namespace Application.DTOs.Categories;

public sealed record CreateCategoryRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public long? ParentId { get; init; }
    public string? Slug { get; init; }
    public string? ImageUrl { get; init; }
    public string? IconClass { get; init; }
    public bool IsActive { get; init; } = true;
    public bool IsFeatured { get; init; } = false;
    public int DisplayOrder { get; init; } = 0;
    public string? MetaTitle { get; init; }
    public string? MetaDescription { get; init; }
}
