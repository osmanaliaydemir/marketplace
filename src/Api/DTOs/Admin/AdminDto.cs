namespace Api.DTOs.Admin;

public class SellerApprovalDto
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PaytrSubmerchantId { get; set; }
    public byte KycStatus { get; set; }
    public decimal CommissionRate { get; set; }
    public string? Iban { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    
    // Store information
    public List<StoreApprovalDto> Stores { get; set; } = new();
}

public class StoreApprovalDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ApproveSellerDto
{
    public long SellerId { get; set; }
    public bool IsApproved { get; set; }
    public string? RejectionReason { get; set; }
    public decimal? CommissionRate { get; set; }
}

public class CommissionUpdateDto
{
    public long SellerId { get; set; }
    public decimal CommissionRate { get; set; }
    public string? Reason { get; set; }
}

public class RefundRequestDto
{
    public long OrderGroupId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public List<RefundItemRequestDto> Items { get; set; } = new();
}

public class RefundItemRequestDto
{
    public long OrderItemId { get; set; }
    public decimal Amount { get; set; }
}

public class SettlementReportDto
{
    public DateTime Date { get; set; }
    public decimal TotalGross { get; set; }
    public decimal TotalPlatformCommission { get; set; }
    public decimal TotalPspFees { get; set; }
    public decimal TotalNetToSellers { get; set; }
    public string Currency { get; set; } = "TRY";
    public int TransactionCount { get; set; }
    
    // Breakdown by seller
    public List<SellerSettlementDto> SellerSettlements { get; set; } = new();
}

public class SellerSettlementDto
{
    public long SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    public decimal Gross { get; set; }
    public decimal PlatformCommission { get; set; }
    public decimal NetAmount { get; set; }
    public int TransactionCount { get; set; }
}
