using Domain.Models;

namespace Application.Abstractions;

public interface IRepository<TEntity> where TEntity : class, IEntity
{
    // Query operations
    Task<TEntity?> GetByIdAsync(long id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> GetAsync(Func<TEntity, bool> predicate);
    Task<TEntity?> GetFirstAsync(Func<TEntity, bool> predicate);
    Task<bool> ExistsAsync(long id);
    Task<int> CountAsync(Func<TEntity, bool>? predicate = null);
    
    // Paged operations
    Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        Func<TEntity, bool>? predicate = null,
        Func<TEntity, object>? orderBy = null,
        bool ascending = true);
    
    // Command operations
    Task<TEntity> AddAsync(TEntity entity);
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(long id);
    Task<bool> DeleteAsync(TEntity entity);
    Task<int> DeleteRangeAsync(Func<TEntity, bool> predicate);
    
    // Bulk operations
    Task<int> ExecuteAsync(string sql, object? param = null);
    Task<IEnumerable<TResult>> QueryAsync<TResult>(string sql, object? param = null);
    Task<TResult?> QueryFirstOrDefaultAsync<TResult>(string sql, object? param = null);
    Task<TResult> QuerySingleAsync<TResult>(string sql, object? param = null);
}

// Specialized interfaces for specific entity types
public interface IAuditableRepository<TEntity> : IRepository<TEntity> 
    where TEntity : class, IAuditableEntity
{
    Task<IEnumerable<TEntity>> GetModifiedSinceAsync(DateTime since);
    Task<IEnumerable<TEntity>> GetCreatedBetweenAsync(DateTime start, DateTime end);
}

public interface ISoftDeleteRepository<TEntity> : IRepository<TEntity> 
    where TEntity : class, ISoftDeleteEntity
{
    Task<IEnumerable<TEntity>> GetDeletedAsync();
    Task<bool> RestoreAsync(long id);
    Task<bool> HardDeleteAsync(long id);
    Task<int> HardDeleteRangeAsync(Func<TEntity, bool> predicate);
}
