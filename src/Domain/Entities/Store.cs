namespace Domain.Entities;

public sealed class Store : Domain.Models.AuditableEntity
{
    public long Id { get; init; }
    public long SellerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; set; }
}


