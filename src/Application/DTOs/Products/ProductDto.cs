

namespace Application.DTOs.Products;

public class ProductDto
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Slug { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal? CompareAtPrice { get; init; }
    public string? ImageUrl { get; init; }
    public bool IsActive { get; init; }
    public bool IsFeatured { get; init; }
    public int DisplayOrder { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
    
    // Navigation properties
    public long CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public long StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;
}
