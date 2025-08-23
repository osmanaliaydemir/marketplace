using Domain.Enums;

namespace Domain.Entities;

public sealed class StoreApplication : Domain.Models.AuditableEntity
{
    public long UserId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }
    public string? BusinessAddress { get; set; }
    public string? TaxNumber { get; set; }
    public string? CompanyName { get; set; }
    public StoreApplicationStatus Status { get; set; } = StoreApplicationStatus.Pending;
    public string? RejectionReason { get; set; }
    public string? AdminNotes { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    
    // Navigation properties
    public AppUser User { get; set; } = null!;
}
