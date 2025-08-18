namespace Domain.Entities;

public sealed class OrderItem : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long OrderGroupId { get; set; }
    public long ProductId { get; set; }
    public long? VariantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}


