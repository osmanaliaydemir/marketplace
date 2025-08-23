namespace Domain.Entities;

public sealed class LedgerPosting : Domain.Models.BaseEntity
{
    public long TransactionId { get; set; }
    public string Account { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? Description { get; set; }
    
    // Navigation properties
    public LedgerTransaction Transaction { get; set; } = null!;
}


