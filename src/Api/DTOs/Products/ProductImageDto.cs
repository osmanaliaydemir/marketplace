namespace Api.DTOs.Products;

// Ürün Resmi için DTO
public sealed class ProductImageDto
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? AltText { get; set; }
    public string? Title { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}

// Resim Oluşturma için DTO
public sealed class CreateProductImageRequest
{
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? AltText { get; set; }
    public string? Title { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsPrimary { get; set; } = false;
}

// Resim Güncelleme için DTO
public sealed class UpdateProductImageRequest
{
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? AltText { get; set; }
    public string? Title { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; }
}

// Resim Sıralama için DTO
public sealed class UpdateProductImageOrderRequest
{
    public List<long> ImageIds { get; set; } = new(); // Sıralanmış ID listesi
}
