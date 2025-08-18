namespace Domain.Entities;

public sealed class LedgerPosting : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long TransactionId { get; set; }
    public string Account { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}


