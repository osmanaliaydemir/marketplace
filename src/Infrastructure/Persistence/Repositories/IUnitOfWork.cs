using System.Data;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity> Repository<TEntity>() where TEntity : class, Domain.Models.IEntity;
    IAuditableRepository<TEntity> AuditableRepository<TEntity>() where TEntity : class, Domain.Models.IAuditableEntity;
    ISoftDeleteRepository<TEntity> SoftDeleteRepository<TEntity>() where TEntity : class, Domain.Models.IAuditableEntity, Domain.Models.ISoftDeleteEntity;
    
    Task<int> SaveChangesAsync();
    Task<IDbTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

public interface IStoreUnitOfWork : IUnitOfWork
{
    IRepository<Store> Stores { get; }
    IRepository<Seller> Sellers { get; }
    IRepository<AppUser> Users { get; }
    IRepository<StoreApplication> StoreApplications { get; }
    IRepository<Order> Orders { get; }
    IRepository<Product> Products { get; }
    IRepository<Category> Categories { get; }
    IRepository<ProductVariant> ProductVariants { get; }
    IRepository<ProductImage> ProductImages { get; }
}
