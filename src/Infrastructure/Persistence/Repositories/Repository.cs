using System.Data;
using Dapper;
using Infrastructure.Persistence.Context;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Infrastructure.Persistence.Naming;
using Application.Abstractions;

namespace Infrastructure.Persistence.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
	protected readonly IDbContext _dbContext;
	protected readonly ILogger _logger;
	protected readonly string _tableName;
	protected readonly string _idColumn;
	private readonly IColumnNameResolver _columnNameResolver;

	public Repository(IDbContext dbContext, ILogger logger, ITableNameResolver tableNameResolver, IColumnNameResolver columnNameResolver)
	{
		_dbContext = dbContext;
		_logger = logger;
		_tableName = tableNameResolver.ResolveTableName(typeof(TEntity));
		_idColumn = columnNameResolver.ResolveIdColumnName(typeof(TEntity));
		_columnNameResolver = columnNameResolver;
	}

	#region Query Operations

	public virtual async Task<TEntity?> GetByIdAsync(long id)
	{
		try
		{
			var sql = $"SELECT * FROM {_tableName} WHERE {_idColumn} = @Id";
			var connection = await _dbContext.GetConnectionAsync();
			
			return await connection.QueryFirstOrDefaultAsync<TEntity>(sql, new { Id = id }, _dbContext.Transaction);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting entity by ID {Id} from {Table}", id, _tableName);
			throw;
		}
	}

	public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
	{
		try
		{
			var sql = $"SELECT * FROM {_tableName}";
			var connection = await _dbContext.GetConnectionAsync();
			
			return await connection.QueryAsync<TEntity>(sql, transaction: _dbContext.Transaction);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting all entities from {Table}", _tableName);
			throw;
		}
	}

	public virtual async Task<IEnumerable<TEntity>> GetAsync(Func<TEntity, bool> predicate)
	{
		try
		{
			var allEntities = await GetAllAsync();
			return allEntities.Where(predicate);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting filtered entities from {Table}", _tableName);
			throw;
		}
	}

	public virtual async Task<TEntity?> GetFirstAsync(Func<TEntity, bool> predicate)
	{
		try
		{
			var allEntities = await GetAllAsync();
			return allEntities.FirstOrDefault(predicate);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting first entity from {Table}", _tableName);
			throw;
		}
	}

	public virtual async Task<bool> ExistsAsync(long id)
	{
		try
		{
			var sql = $"SELECT COUNT(1) FROM {_tableName} WHERE {_idColumn} = @Id";
			var connection = await _dbContext.GetConnectionAsync();
			
			var count = await connection.ExecuteScalarAsync<int>(sql, new { Id = id }, _dbContext.Transaction);
			return count > 0;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error checking existence of entity with ID {Id} in {Table}", id, _tableName);
			throw;
		}
	}

	public virtual async Task<int> CountAsync(Func<TEntity, bool>? predicate = null)
	{
		try
		{
			if (predicate == null)
			{
				var sql = $"SELECT COUNT(1) FROM {_tableName}";
				var connection = await _dbContext.GetConnectionAsync();
				return await connection.ExecuteScalarAsync<int>(sql, null, _dbContext.Transaction);
			}

			var allEntities = await GetAllAsync();
			return allEntities.Count(predicate);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error counting entities in {Table}", _tableName);
			throw;
		}
	}

	public virtual async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
		int pageNumber, 
		int pageSize, 
		Func<TEntity, bool>? predicate = null,
		Func<TEntity, object>? orderBy = null,
		bool ascending = true)
	{
		try
		{
			var allEntities = await GetAllAsync();
			
			if (predicate != null)
				allEntities = allEntities.Where(predicate);
			
			if (orderBy != null)
				allEntities = ascending ? allEntities.OrderBy(orderBy) : allEntities.OrderByDescending(orderBy);
			
			var totalCount = allEntities.Count();
			var items = allEntities.Skip((pageNumber - 1) * pageSize).Take(pageSize);
			
			return (items, totalCount);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting paged entities from {Table}", _tableName);
			throw;
		}
	}

	#endregion

	#region Command Operations

	public virtual async Task<TEntity> AddAsync(TEntity entity)
	{
		try
		{
			var connection = await _dbContext.GetConnectionAsync();
			
			var properties = typeof(TEntity).GetProperties()
				.Where(p => p.Name != nameof(IEntity.Id))
				.ToList();
			
			var columns = string.Join(", ", properties.Select(p => _columnNameResolver.ResolveColumnName(p.Name)));
			var parameters = string.Join(", ", properties.Select(p => "@" + p.Name));
			
			var sql = $"INSERT INTO {_tableName} ({columns}) VALUES ({parameters}); SELECT CAST(SCOPE_IDENTITY() as bigint)";
			
			var id = await connection.ExecuteScalarAsync<long>(sql, entity, _dbContext.Transaction);
			
			var idProperty = typeof(TEntity).GetProperty(nameof(IEntity.Id));
			idProperty?.SetValue(entity, id);
			
			_logger.LogInformation("Added entity with ID {Id} to {Table}", id, _tableName);
			return entity;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error adding entity to {Table}", _tableName);
			throw;
		}
	}

	public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
	{
		try
		{
			var results = new List<TEntity>();
			foreach (var entity in entities)
			{
				var result = await AddAsync(entity);
				results.Add(result);
			}
			
			_logger.LogInformation("Added {Count} entities to {Table}", results.Count, _tableName);
			return results;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error adding multiple entities to {Table}", _tableName);
			throw;
		}
	}

	public virtual async Task<TEntity> UpdateAsync(TEntity entity)
	{
		try
		{
			var connection = await _dbContext.GetConnectionAsync();
			
			var properties = typeof(TEntity).GetProperties()
				.Where(p => p.Name != nameof(IEntity.Id))
				.ToList();
			
			var setClause = string.Join(", ", properties.Select(p => $"{_columnNameResolver.ResolveColumnName(p.Name)} = @{p.Name}"));
			var sql = $"UPDATE {_tableName} SET {setClause} WHERE {_idColumn} = @Id";
			
			var rowsAffected = await connection.ExecuteAsync(sql, entity, _dbContext.Transaction);
			
			if (rowsAffected == 0)
				throw new InvalidOperationException($"Entity with ID {entity.Id} not found in {_tableName}");
			
			_logger.LogInformation("Updated entity with ID {Id} in {Table}", entity.Id, _tableName);
			return entity;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating entity with ID {Id} in {Table}", entity.Id, _tableName);
			throw;
		}
	}

	public virtual async Task<bool> DeleteAsync(long id)
	{
		try
		{
			var sql = $"DELETE FROM {_tableName} WHERE {_idColumn} = @Id";
			var connection = await _dbContext.GetConnectionAsync();
			
			var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id }, _dbContext.Transaction);
			
			if (rowsAffected > 0)
			{
				_logger.LogInformation("Deleted entity with ID {Id} from {Table}", id, _tableName);
				return true;
			}
			
			return false;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting entity with ID {Id} from {Table}", id, _tableName);
			throw;
		}
	}

	public virtual async Task<bool> DeleteAsync(TEntity entity)
	{
		return await DeleteAsync(entity.Id);
	}

	public virtual async Task<int> DeleteRangeAsync(Func<TEntity, bool> predicate)
	{
		try
		{
			var entitiesToDelete = await GetAsync(predicate);
			var count = 0;
			
			foreach (var entity in entitiesToDelete)
			{
				if (await DeleteAsync(entity.Id))
					count++;
			}
			
			_logger.LogInformation("Deleted {Count} entities from {Table}", count, _tableName);
			return count;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting multiple entities from {Table}", _tableName);
			throw;
		}
	}

	#endregion

	#region Bulk Operations

	public virtual async Task<int> ExecuteAsync(string sql, object? param = null)
	{
		try
		{
			var connection = await _dbContext.GetConnectionAsync();
			return await connection.ExecuteAsync(sql, param, _dbContext.Transaction);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error executing SQL: {Sql}", sql);
			throw;
		}
	}

	public virtual async Task<IEnumerable<TResult>> QueryAsync<TResult>(string sql, object? param = null)
	{
		try
		{
			var connection = await _dbContext.GetConnectionAsync();
			return await connection.QueryAsync<TResult>(sql, param, _dbContext.Transaction);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error querying SQL: {Sql}", sql);
			throw;
		}
	}

	public virtual async Task<TResult?> QueryFirstOrDefaultAsync<TResult>(string sql, object? param = null)
	{
		try
		{
			var connection = await _dbContext.GetConnectionAsync();
			return await connection.QueryFirstOrDefaultAsync<TResult>(sql, param, _dbContext.Transaction);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error querying first or default: {Sql}", sql);
			throw;
		}
	}

	public virtual async Task<TResult> QuerySingleAsync<TResult>(string sql, object? param = null)
	{
		try
		{
			var connection = await _dbContext.GetConnectionAsync();
			return await connection.QuerySingleAsync<TResult>(sql, param, _dbContext.Transaction);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error querying single: {Sql}", sql);
			throw;
		}
	}

	#endregion
}
