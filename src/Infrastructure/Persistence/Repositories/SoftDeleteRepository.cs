using Infrastructure.Persistence.Context;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Dapper;
using Infrastructure.Persistence.Naming;

namespace Infrastructure.Persistence.Repositories;

public class SoftDeleteRepository<TEntity> : AuditableRepository<TEntity>, ISoftDeleteRepository<TEntity> 
	where TEntity : class, ISoftDeleteEntity, IAuditableEntity
{
	public SoftDeleteRepository(IDbContext dbContext, ILogger logger, ITableNameResolver tableNameResolver, IColumnNameResolver columnNameResolver) 
		: base(dbContext, logger, tableNameResolver, columnNameResolver)
	{
	}

	public override async Task<TEntity> AddAsync(TEntity entity)
	{
		entity.IsDeleted = false;
		entity.DeletedAt = null;
		
		return await base.AddAsync(entity);
	}

	public override async Task<IEnumerable<TEntity>> GetAllAsync()
	{
		try
		{
			var sql = $"SELECT * FROM {_tableName} WHERE is_deleted = 0";
			var connection = await _dbContext.GetConnectionAsync();
			
			return await connection.QueryAsync<TEntity>(sql);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting all non-deleted entities from {Table}", _tableName);
			throw;
		}
	}

	public override async Task<TEntity?> GetByIdAsync(long id)
	{
		try
		{
			var sql = $"SELECT * FROM {_tableName} WHERE {_idColumn} = @Id AND is_deleted = 0";
			var connection = await _dbContext.GetConnectionAsync();
			
			return await connection.QueryFirstOrDefaultAsync<TEntity>(sql, new { Id = id });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting non-deleted entity by ID {Id} from {Table}", id, _tableName);
			throw;
		}
	}

	public override async Task<bool> DeleteAsync(long id)
	{
		try
		{
			var sql = $"UPDATE {_tableName} SET is_deleted = 1, deleted_at = @DeletedAt WHERE {_idColumn} = @Id";
			var connection = await _dbContext.GetConnectionAsync();
			
			var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, DeletedAt = DateTime.UtcNow });
			
			if (rowsAffected > 0)
			{
				_logger.LogInformation("Soft deleted entity with ID {Id} from {Table}", id, _tableName);
				return true;
			}
			
			return false;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error soft deleting entity with ID {Id} from {Table}", id, _tableName);
			throw;
		}
	}

	public override async Task<bool> DeleteAsync(TEntity entity)
	{
		return await DeleteAsync(entity.Id);
	}

	public async Task<IEnumerable<TEntity>> GetDeletedAsync()
	{
		try
		{
			var sql = $"SELECT * FROM {_tableName} WHERE is_deleted = 1 ORDER BY deleted_at DESC";
			var connection = await _dbContext.GetConnectionAsync();
			
			return await connection.QueryAsync<TEntity>(sql);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting deleted entities from {Table}", _tableName);
			throw;
		}
	}

	public async Task<bool> RestoreAsync(long id)
	{
		try
		{
			var sql = $"UPDATE {_tableName} SET is_deleted = 0, deleted_at = NULL, modified_at = @ModifiedAt WHERE {_idColumn} = @Id";
			var connection = await _dbContext.GetConnectionAsync();
			
			var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, ModifiedAt = DateTime.UtcNow });
			
			if (rowsAffected > 0)
			{
				_logger.LogInformation("Restored entity with ID {Id} in {Table}", id, _tableName);
				return true;
			}
			
			return false;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error restoring entity with ID {Id} in {Table}", id, _tableName);
			throw;
		}
	}

	public async Task<bool> HardDeleteAsync(long id)
	{
		try
		{
			var sql = $"DELETE FROM {_tableName} WHERE {_idColumn} = @Id";
			var connection = await _dbContext.GetConnectionAsync();
			
			var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
			
			if (rowsAffected > 0)
			{
				_logger.LogInformation("Hard deleted entity with ID {Id} from {Table}", id, _tableName);
				return true;
			}
			
			return false;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error hard deleting entity with ID {Id} from {Table}", id, _tableName);
			throw;
		}
	}

	public async Task<int> HardDeleteRangeAsync(Func<TEntity, bool> predicate)
	{
		try
		{
			var entitiesToDelete = await GetAsync(predicate);
			var count = 0;
			
			foreach (var entity in entitiesToDelete)
			{
				if (await HardDeleteAsync(entity.Id))
					count++;
			}
			
			_logger.LogInformation("Hard deleted {Count} entities from {Table}", count, _tableName);
			return count;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error hard deleting multiple entities from {Table}", _tableName);
			throw;
		}
	}
}
