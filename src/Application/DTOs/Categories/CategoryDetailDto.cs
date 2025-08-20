namespace Application.DTOs.Categories;

public sealed record CategoryDetailDto
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
    
    // Related Data
    public IEnumerable<CategoryDto> SubCategories { get; init; } = Enumerable.Empty<CategoryDto>();
    public IEnumerable<CategoryProductDto> Products { get; init; } = Enumerable.Empty<CategoryProductDto>();
    
    // Statistics
    public int ProductCount { get; init; }
    public int SubCategoryCount { get; init; }
    public int TotalProductCount { get; init; }
    
    // Hierarchy
    public IEnumerable<CategoryDto> Breadcrumb { get; init; } = Enumerable.Empty<CategoryDto>();
    public int Level { get; init; }
}

public sealed record CategoryProductDto
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Currency { get; init; } = "TRY";
    public bool IsActive { get; init; }
    public bool IsPublished { get; init; }
    public string? PrimaryImageUrl { get; init; }
    public DateTime CreatedAt { get; init; }
}
