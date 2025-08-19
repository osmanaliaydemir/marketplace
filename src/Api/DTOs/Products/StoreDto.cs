namespace Api.DTOs.Products;

// Mağaza için DTO
public sealed class StoreDto
{
    public long Id { get; set; }
    public long SellerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    
    // İlişkili veriler
    public SellerDto Seller { get; set; } = null!;
    public int ProductCount { get; set; } = 0;
    public int CategoryCount { get; set; } = 0;
}

// Mağaza Oluşturma için DTO
public sealed class CreateStoreRequest
{
    public long SellerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

// Mağaza Güncelleme için DTO
public sealed class UpdateStoreRequest
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
