using Domain.Models;
using Domain.ValueObjects;
using Domain.Enums;

namespace Domain.Entities;

public sealed class Customer : Domain.Models.AuditableEntity
{
    public long UserId { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public AppUser User { get; set; } = null!;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
