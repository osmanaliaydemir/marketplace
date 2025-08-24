using Application.DTOs.Users;

namespace Application.DTOs.Sellers;

public sealed record SellerDto
{
    public long Id { get; init; }
    public long UserId { get; init; }
    public decimal CommissionRate { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public AppUserDto User { get; init; } = null!;
}
