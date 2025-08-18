namespace Domain.Entities;

public sealed class AppUser : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string Role { get; set; } = string.Empty; // Customer, Seller, Admin
    public DateTime CreatedAt { get; init; }
    public bool IsActive { get; set; } = true;
}


