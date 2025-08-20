using Domain.Models;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Customer : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsVerified { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
