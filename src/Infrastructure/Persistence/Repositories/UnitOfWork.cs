using System.Collections.Concurrent;
using System.Data;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Naming;
using Microsoft.Extensions.Logging;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbContext _dbContext;
    private readonly ILogger _logger;
    private readonly ITableNameResolver _tableNameResolver;
    private readonly IColumnNameResolver _columnNameResolver;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public UnitOfWork(IDbContext dbContext, ILogger<UnitOfWork> logger, ITableNameResolver tableNameResolver, IColumnNameResolver columnNameResolver)
    {
        _dbContext = dbContext;
        _logger = logger;
        _tableNameResolver = tableNameResolver;
        _columnNameResolver = columnNameResolver;
    }

    public IRepository<TEntity> Repository<TEntity>() where TEntity : class, Domain.Models.IEntity
    {
        var entityType = typeof(TEntity);
        if (!_repositories.TryGetValue(entityType, out var repository))
        {
            repository = Activator.CreateInstance(typeof(Repository<TEntity>), _dbContext, _logger, _tableNameResolver, _columnNameResolver)!;
            _repositories[entityType] = repository;
        }
        return (IRepository<TEntity>)repository;
    }

    public IAuditableRepository<TEntity> AuditableRepository<TEntity>() where TEntity : class, Domain.Models.IAuditableEntity
    {
        var entityType = typeof(TEntity);
        if (!_repositories.TryGetValue(entityType, out var repository))
        {
            repository = Activator.CreateInstance(typeof(AuditableRepository<TEntity>), _dbContext, _logger, _tableNameResolver, _columnNameResolver)!;
            _repositories[entityType] = repository;
        }
        return (IAuditableRepository<TEntity>)repository;
    }

    public ISoftDeleteRepository<TEntity> SoftDeleteRepository<TEntity>() where TEntity : class, Domain.Models.IAuditableEntity, Domain.Models.ISoftDeleteEntity
    {
        var entityType = typeof(TEntity);
        if (!_repositories.TryGetValue(entityType, out var repository))
        {
            repository = Activator.CreateInstance(typeof(SoftDeleteRepository<TEntity>), _dbContext, _logger, _tableNameResolver, _columnNameResolver)!;
            _repositories[entityType] = repository;
        }
        return (ISoftDeleteRepository<TEntity>)repository;
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
        foreach (var repo in _repositories.Values)
        {
            if (repo is IDisposable disposable)
                disposable.Dispose();
        }
        _repositories.Clear();
    }
}

public sealed class StoreUnitOfWork : UnitOfWork, IStoreUnitOfWork
{
    public StoreUnitOfWork(IDbContext dbContext, ILogger<StoreUnitOfWork> logger, ITableNameResolver tableNameResolver, IColumnNameResolver columnNameResolver)
        : base(dbContext, logger, tableNameResolver, columnNameResolver)
    {
    }

    public IRepository<Store> Stores => AuditableRepository<Store>();
    public IRepository<Seller> Sellers => AuditableRepository<Seller>();
    public IRepository<AppUser> Users => AuditableRepository<AppUser>();
    public IRepository<StoreApplication> StoreApplications => AuditableRepository<StoreApplication>();
    public IRepository<Order> Orders => AuditableRepository<Order>();
}
