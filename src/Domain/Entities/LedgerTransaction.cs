namespace Domain.Entities;

public sealed class LedgerTransaction : Domain.Models.BaseEntity
{
    public string TransactionNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string Type { get; set; } = string.Empty; // Credit, Debit
    public DateTime TransactionDate { get; set; }
    public string? Reference { get; set; }
    
    // Navigation properties
    public ICollection<LedgerPosting> Postings { get; set; } = new List<LedgerPosting>();
}


