namespace Domain.Models;

public abstract class BaseEntity : IEntity
{
    public long Id { get; set; }
}

public abstract class AuditableEntity : BaseEntity, IAuditableEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
}

public abstract class SoftDeleteEntity : AuditableEntity, ISoftDeleteEntity
{
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}
