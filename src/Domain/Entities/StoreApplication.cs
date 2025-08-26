using Domain.Enums;

namespace Domain.Entities;

public sealed class StoreApplication : Domain.Models.AuditableEntity
{
    public long UserId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public string? TaxNumber { get; set; }
    public string? BusinessLicense { get; set; }
    public string? BusinessDescription { get; set; }
    public string? Website { get; set; }
    public string? SocialMedia { get; set; }
    public string PrimaryCategory { get; set; } = string.Empty;
    public string? SecondaryCategory { get; set; }
    public int ProductCount { get; set; }
    public decimal ExpectedRevenue { get; set; }
    public string? Experience { get; set; }
    public string? ProductDescription { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool TermsAccepted { get; set; }
    public bool Newsletter { get; set; }
    public StoreApplicationStatus Status { get; set; } = StoreApplicationStatus.Pending;
    public string? RejectionReason { get; set; }
    public string? AdminNotes { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public long? ApprovedByUserId { get; set; }
    public long? RejectedByUserId { get; set; }
    public string? ApprovalNotes { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public AppUser User { get; set; } = null!;
    
    // StoreName property'si BusinessName ile aynı değeri döndürür
    public string StoreName => BusinessName;
}
