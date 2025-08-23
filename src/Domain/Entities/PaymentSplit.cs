namespace Domain.Entities;

public sealed class PaymentSplit : Domain.Models.AuditableEntity
{
    public long PaymentId { get; set; }
    public long StoreId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal NetAmount { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Processed, Failed
    public DateTime? ProcessedAt { get; set; }
    
    // Navigation properties
    public Payment Payment { get; set; } = null!;
    public Store Store { get; set; } = null!;
}


