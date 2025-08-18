namespace Domain.Entities;

public sealed class Inventory : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long VariantId { get; set; }
    public int OnHand { get; set; }
    public int Reserved { get; set; }
    public DateTime UpdatedAt { get; init; }
}


