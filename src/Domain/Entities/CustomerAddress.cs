using Domain.Models;

namespace Domain.Entities;

public class CustomerAddress : AuditableEntity
{
    public long CustomerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual AppUser Customer { get; set; } = null!;
}
