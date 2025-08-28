namespace Application.DTOs.Customers;

public sealed record CustomerProfileDto
{
    public long Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public DateTime? BirthDate { get; init; }
    public string? Gender { get; init; }
    public DateTime MemberSince { get; init; }
    public int TotalOrders { get; init; }
    public int TotalWishlist { get; init; }
    public DateTime LastLogin { get; init; }
}
