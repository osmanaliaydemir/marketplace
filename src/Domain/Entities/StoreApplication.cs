namespace Domain.Entities;

public enum StoreApplicationStatus
{
	Pending = 1,
	Approved = 2,
	Rejected = 3,
	Cancelled = 4
}

public sealed class StoreApplication : Domain.Models.AuditableEntity
{
	public long Id { get; set; }
	public string StoreName { get; set; } = string.Empty;
	public string Slug { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string ContactEmail { get; set; } = string.Empty;
	public string? ContactPhone { get; set; }
	public string? BusinessAddress { get; set; }
	public string? TaxNumber { get; set; }
	public StoreApplicationStatus Status { get; set; } = StoreApplicationStatus.Pending;
	public string? RejectionReason { get; set; }
	public DateTime? ApprovedAt { get; set; }
	public long? ApprovedByUserId { get; set; }
	public long? SellerId { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime? ModifiedAt { get; set; }
}
