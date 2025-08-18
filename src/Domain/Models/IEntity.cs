namespace Domain.Models;

public interface IEntity
{
    long Id { get; set; }
}

public interface IAuditableEntity : IEntity
{
    DateTime CreatedAt { get; set; }
    DateTime? ModifiedAt { get; set; }
}

public interface ISoftDeleteEntity : IEntity
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
}
