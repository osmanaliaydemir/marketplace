namespace Application.DTOs.Stores;

public sealed class StoreApplicationCreateRequest
{
	public string StoreName { get; set; } = string.Empty;
	public string Slug { get; set; } = string.Empty;
	public string ContactEmail { get; set; } = string.Empty;
	public string? ContactPhone { get; set; }
	public string? Description { get; set; }
	public string? BusinessAddress { get; set; }
	public string? TaxNumber { get; set; }
}

public sealed class StoreApplicationListDto
{
	public long Id { get; set; }
	public string StoreName { get; set; } = string.Empty;
	public string Slug { get; set; } = string.Empty;
	public string ContactEmail { get; set; } = string.Empty;
	public string Status { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
}

public sealed class StoreApplicationDetailDto
{
	public long Id { get; set; }
	public string StoreName { get; set; } = string.Empty;
	public string Slug { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string ContactEmail { get; set; } = string.Empty;
	public string? ContactPhone { get; set; }
	public string? BusinessAddress { get; set; }
	public string? TaxNumber { get; set; }
	public string Status { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public DateTime? ApprovedAt { get; set; }
	public string? RejectionReason { get; set; }
}

// Dashboard için yeni DTO'lar
public sealed class StoreApplicationSearchRequest
{
	public string? SearchTerm { get; set; }
	public string? Status { get; set; }
	public DateTime? DateFrom { get; set; }
	public DateTime? DateTo { get; set; }
	public int Page { get; set; } = 1;
	public int PageSize { get; set; } = 20;
	public string? SortBy { get; set; } = "CreatedAt";
	public bool SortDescending { get; set; } = true;
}

public sealed class StoreApplicationSearchResponse
{
	public IEnumerable<StoreApplicationListDto> Items { get; set; } = Enumerable.Empty<StoreApplicationListDto>();
	public int TotalCount { get; set; }
	public int Page { get; set; }
	public int PageSize { get; set; }
	public int TotalPages { get; set; }
	public bool HasNextPage { get; set; }
	public bool HasPreviousPage { get; set; }
}

public sealed class StoreApplicationStatsDto
{
	public int TotalApplications { get; set; }
	public int PendingApplications { get; set; }
	public int ApprovedApplications { get; set; }
	public int RejectedApplications { get; set; }
	public int CancelledApplications { get; set; }
	public decimal ApprovalRate { get; set; }
	public int ApplicationsThisMonth { get; set; }
	public int ApplicationsLastMonth { get; set; }
}
