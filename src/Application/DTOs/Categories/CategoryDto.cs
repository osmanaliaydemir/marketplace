namespace Application.DTOs.Categories;

public sealed record CategoryDto
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? Description { get; init; }
    public long? ParentId { get; init; }
    public string? ParentName { get; init; }
    public string? ImageUrl { get; init; }
    public string? IconClass { get; init; }
    public bool IsActive { get; init; }
    public bool IsFeatured { get; init; }
    public int DisplayOrder { get; init; }
    public string? MetaTitle { get; init; }
    public string? MetaDescription { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public int ProductCount { get; init; }
    public int SubCategoryCount { get; init; }
    public int ChildrenCount { get; init; }
    public List<CategoryDto> Children { get; init; } = new();
} 