namespace Domain.Entities;

public sealed class StoreCategory : Domain.Models.BaseEntity
{
    public long StoreId { get; set; }
    public long CategoryId { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
    
    // Navigation properties
    public Store Store { get; set; } = null!;
    public Category Category { get; set; } = null!;
}


