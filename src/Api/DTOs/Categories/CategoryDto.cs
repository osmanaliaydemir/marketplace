namespace Api.DTOs.Categories;

public class CategoryListDto
{
    public long Id { get; set; }
    public long? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
}

public class CategoryDetailDto
{
    public long Id { get; set; }
    public long? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public bool IsActive { get; set; }
    
    // Navigation properties
    public CategoryDetailDto? Parent { get; set; }
    public List<CategoryDetailDto> Children { get; set; } = new();
}

public class CreateCategoryDto
{
    public long? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
}

public class StoreCategoryListDto
{
    public long Id { get; set; }
    public long StoreId { get; set; }
    public long? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public long? GlobalCategoryId { get; set; }
    public int ProductCount { get; set; }
}

public class CreateStoreCategoryDto
{
    public long StoreId { get; set; }
    public long? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public long? GlobalCategoryId { get; set; }
}
