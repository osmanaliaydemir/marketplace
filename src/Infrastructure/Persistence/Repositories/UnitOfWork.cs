using System.Collections.Concurrent;
using System.Data;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Naming;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Domain.Entities;
using Application.Abstractions;

namespace Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbContext _dbContext;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWork(IDbContext dbContext, ILogger<UnitOfWork> logger, IServiceProvider serviceProvider)
    {
        _dbContext = dbContext;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public IRepository<TEntity> Repository<TEntity>() where TEntity : class, Domain.Models.IEntity
    {
        var repositoryType = typeof(IRepository<TEntity>);
        return (IRepository<TEntity>)_serviceProvider.GetRequiredService(repositoryType);
    }

    public IAuditableRepository<TEntity> AuditableRepository<TEntity>() where TEntity : class, Domain.Models.IAuditableEntity
    {
        var repositoryType = typeof(IAuditableRepository<TEntity>);
        return (IAuditableRepository<TEntity>)_serviceProvider.GetRequiredService(repositoryType);
    }

    public ISoftDeleteRepository<TEntity> SoftDeleteRepository<TEntity>() where TEntity : class, Domain.Models.IAuditableEntity, Domain.Models.ISoftDeleteEntity
    {
        var repositoryType = typeof(ISoftDeleteRepository<TEntity>);
        return (ISoftDeleteRepository<TEntity>)_serviceProvider.GetRequiredService(repositoryType);
    }

    public Task<int> SaveChangesAsync()
    {
        try
        {
            if (_dbContext.Transaction != null)
            {
                _dbContext.Transaction.Commit();
                _logger.LogInformation("Transaction committed successfully");
                return Task.FromResult(1);
            }
            return Task.FromResult(0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes");
            throw;
        }
    }

    public async Task<IDbTransaction> BeginTransactionAsync()
    {
        try
        {
            var connection = await _dbContext.GetConnectionAsync();
            var transaction = connection.BeginTransaction();
            _dbContext.SetTransaction(transaction);
            _logger.LogInformation("Transaction began successfully");
            return transaction;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error beginning transaction");
            throw;
        }
    }

    public Task CommitTransactionAsync()
    {
        try
        {
            if (_dbContext.Transaction != null)
            {
                _dbContext.Transaction.Commit();
                _logger.LogInformation("Transaction committed successfully");
            }
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing transaction");
            throw;
        }
    }

    public Task RollbackTransactionAsync()
    {
        try
        {
            if (_dbContext.Transaction != null)
            {
                _dbContext.Transaction.Rollback();
                _logger.LogInformation("Transaction rolled back successfully");
            }
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back transaction");
            throw;
        }
    }

    public virtual void Dispose()
    {
        _dbContext?.Dispose();
        // The following lines are removed as repositories are now managed by DI
        // foreach (var repo in _repositories.Values)
        // {
        //     if (repo is IDisposable disposable)
        //         disposable.Dispose();
        // }
        // _repositories.Clear();
    }
}

public sealed class StoreUnitOfWork : UnitOfWork, IStoreUnitOfWork
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IAppUserRepository _userRepository;

    public StoreUnitOfWork(
        IDbContext dbContext, 
        ILogger<StoreUnitOfWork> logger, 
        IServiceProvider serviceProvider,
        IInventoryRepository inventoryRepository,
        IAppUserRepository userRepository)
        : base(dbContext, logger, serviceProvider)
    {
        _inventoryRepository = inventoryRepository;
        _userRepository = userRepository;
    }

    public IRepository<Store> Stores => AuditableRepository<Store>();
    public IRepository<Seller> Sellers => AuditableRepository<Seller>();
    public IAppUserRepository Users => _userRepository;
    public IRepository<StoreApplication> StoreApplications => AuditableRepository<StoreApplication>();
    public IRepository<Order> Orders => AuditableRepository<Order>();
    public IRepository<Product> Products => AuditableRepository<Product>();
    public IRepository<Category> Categories => AuditableRepository<Category>();
    public IRepository<ProductVariant> ProductVariants => AuditableRepository<ProductVariant>();
    public IRepository<ProductImage> ProductImages => AuditableRepository<ProductImage>();
    public IInventoryRepository Inventory => _inventoryRepository; // EKLENDI!
    public IRepository<Customer> Customers => AuditableRepository<Customer>(); // BONUS: Bu da eksikti
    public IRepository<Payment> Payments => AuditableRepository<Payment>(); // BONUS: Bu da eksikti
}
