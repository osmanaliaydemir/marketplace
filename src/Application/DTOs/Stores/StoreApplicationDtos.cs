namespace Application.DTOs.Stores;

public sealed class StoreApplicationCreateRequest
{
	public string BusinessName { get; set; } = string.Empty;
	public string BusinessType { get; set; } = string.Empty;
	public string? TaxNumber { get; set; }
	public string? BusinessLicense { get; set; }
	public string BusinessDescription { get; set; } = string.Empty;
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
}

public sealed class StoreApplicationUpdateRequest
{
	public long Id { get; set; }
	public string BusinessName { get; set; } = string.Empty;
	public string BusinessType { get; set; } = string.Empty;
	public string? TaxNumber { get; set; }
	public string? BusinessLicense { get; set; }
	public string BusinessDescription { get; set; } = string.Empty;
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
}

public sealed class StoreApplicationListDto
{
	public long Id { get; set; }
	public string BusinessName { get; set; } = string.Empty;
	public string BusinessType { get; set; } = string.Empty;
	public string Status { get; set; } = string.Empty;
	public DateTime SubmittedAt { get; set; }
	public string ContactName { get; set; } = string.Empty;
	public string PhoneNumber { get; set; } = string.Empty;
	public string City { get; set; } = string.Empty;
}

public sealed class StoreApplicationDetailDto
{
	public long Id { get; set; }
	public string BusinessName { get; set; } = string.Empty;
	public string BusinessType { get; set; } = string.Empty;
	public string? TaxNumber { get; set; }
	public string? BusinessLicense { get; set; }
	public string BusinessDescription { get; set; } = string.Empty;
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
	public string Status { get; set; } = string.Empty;
	public DateTime SubmittedAt { get; set; }
	public DateTime? ApprovedAt { get; set; }
	public DateTime? RejectedAt { get; set; }
	public string? RejectionReason { get; set; }
	
	// Controller uyumluluğu için eklenen property'ler
	public bool IsSuccess { get; set; } = true;
	public StoreApplicationDetailDto? Data { get; set; }
	public string? ErrorMessage { get; set; }
}

public sealed class StoreApplicationApprovalRequest
{
	public long ApplicationId { get; set; }
	public long ApprovedByUserId { get; set; }
	public string ApprovalNotes { get; set; } = string.Empty;
}

public sealed class StoreApplicationRejectionRequest
{
    public long ApplicationId { get; set; }
    public long RejectedByUserId { get; set; }
    public string RejectionReason { get; set; } = string.Empty;
}

public sealed class StoreApplicationSearchRequest
{
    public string? Status { get; set; }
    public string? BusinessType { get; set; }
    public string? PrimaryCategory { get; set; }
    public string? City { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public sealed class StoreApplicationSearchResponse
{
    public List<StoreApplicationListDto> Applications { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public sealed class StoreApplicationStatsDto
{
    public int PendingCount { get; set; }
    public int ApprovedCount { get; set; }
    public int RejectedCount { get; set; }
    public int TotalCount { get; set; }
}
