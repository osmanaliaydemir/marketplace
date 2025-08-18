using Api.DTOs.Categories;

namespace Api.DTOs.Stores;

public class StoreListDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class StoreDetailDto
{
    public long Id { get; set; }
    public long SellerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    
    // Navigation properties
    public SellerInfoDto Seller { get; set; } = new();
    public List<StoreCategoryListDto> Categories { get; set; } = new();
}

public class CreateStoreDto
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
}

public class UpdateStoreDto
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? LogoUrl { get; set; }
    public bool? IsActive { get; set; }
}

public class SellerInfoDto
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal CommissionRate { get; set; }
    public bool IsActive { get; set; }
}
