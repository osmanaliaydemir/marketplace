namespace Domain.Entities;

public sealed class StoreCategory : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long StoreId { get; set; }
    public long? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public long? GlobalCategoryId { get; set; }
}


