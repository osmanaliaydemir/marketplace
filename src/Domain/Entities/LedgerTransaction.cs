namespace Domain.Entities;

public sealed class LedgerTransaction : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public string RefType { get; set; } = string.Empty;
    public long RefId { get; set; }
    public DateTime Ts { get; init; }
}


