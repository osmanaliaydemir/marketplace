namespace Api.DTOs.Products;

// Satıcı için DTO
public sealed class SellerDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public decimal CommissionRate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    
    // İlişkili veriler
    public AppUserDto User { get; set; } = null!;
    public int StoreCount { get; set; } = 0;
    public int ProductCount { get; set; } = 0;
}

// Satıcı Oluşturma için DTO
public sealed class CreateSellerRequest
{
    public long UserId { get; set; }
    public decimal CommissionRate { get; set; } = 0.10m; // Varsayılan %10
    public bool IsActive { get; set; } = true;
}

// Satıcı Güncelleme için DTO
public sealed class UpdateSellerRequest
{
    public decimal CommissionRate { get; set; }
    public bool IsActive { get; set; }
}

// AppUser için DTO (SellerDto'da kullanılıyor)
public sealed class AppUserDto
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
