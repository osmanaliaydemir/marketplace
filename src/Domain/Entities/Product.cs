namespace Domain.Entities;

public sealed class Product : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long SellerId { get; set; }
    public long CategoryId { get; set; }
    public long StoreId { get; set; }
    public long? StoreCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "TRY";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; set; }
}
