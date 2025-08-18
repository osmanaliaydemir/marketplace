namespace Domain.Entities;

public sealed class Category : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public bool IsActive { get; set; } = true;
}


