namespace Domain.Entities;

public sealed class ProductVariant : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long ProductId { get; set; }
    public string? Sku { get; set; }
    public decimal Price { get; set; }
    public int StockQty { get; set; }
    public bool IsActive { get; set; }
}


