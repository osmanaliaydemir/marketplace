using System.Data;
using Dapper;
using Infrastructure.Persistence.Context;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Infrastructure.Persistence.Naming;
using Application.Abstractions;
using Application.Exceptions;

namespace Infrastructure.Persistence.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
	protected readonly IDbContext _dbContext;
	protected readonly ILogger _logger;
	protected readonly string _tableName;
	protected readonly string _idColumn;
	private readonly IColumnNameResolver _columnNameResolver;

	public Repository(IDbContext dbContext, ILogger logger, ITableNameResolver? tableNameResolver, IColumnNameResolver? columnNameResolver)
	{
		_dbContext = dbContext;
		_logger = logger;
		
		// Null-safe fallback for table name resolution
		if (tableNameResolver != null)
		{
			_tableName = tableNameResolver.ResolveTableName(typeof(TEntity));
		}
		else
		{
			// Fallback: Use entity name with 's' suffix
			_tableName = typeof(TEntity).Name + "s";
		}
		
		// Null-safe fallback for column name resolution  
		if (columnNameResolver != null)
		{
			_idColumn = columnNameResolver.ResolveIdColumnName(typeof(TEntity));
			_columnNameResolver = columnNameResolver;
		}
		else
		{
			// Fallback: Use 'Id' as default
			_idColumn = "Id";
			_columnNameResolver = new DefaultColumnNameResolver();
		}
	}

	#region Query Operations

	public virtual async Task<TEntity?> GetByIdAsync(long id)
	{
		try
		{
			var sql = $"SELECT * FROM {_tableName} WHERE {_idColumn} = @Id";
			var connection = await _dbContext.GetConnectionAsync();
			
			var entity = await connection.QueryFirstOrDefaultAsync<TEntity>(sql, new { Id = id }, _dbContext.Transaction);
			
			if (entity == null)
			{
				_logger.LogWarning("Entity with ID {Id} not found in {Table}", id, _tableName);
			}
			
			return entity;
		}
		catch (Exception ex) when (ex is not RepositoryException && ex is not EntityNotFoundException)
		{
			var errorMessage = $"Error getting entity by ID {id} from {_tableName}";
			_logger.LogError(ex, errorMessage);
			
			if (ex is InvalidOperationException || ex is TimeoutException)
			{
				throw new DatabaseConnectionException(errorMessage, "GetById", null, null, null, ex);
			}
			
			throw new RepositoryException(errorMessage, "GetById", typeof(TEntity).Name, id, _tableName, ex);
		}
	}

	public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
	{
		try
		{
			var sql = $"SELECT * FROM {_tableName}";
			var connection = await _dbContext.GetConnectionAsync();
			
			var entities = await connection.QueryAsync<TEntity>(sql, transaction: _dbContext.Transaction);
			
			_logger.LogDebug("Retrieved {Count} entities from {Table}", entities.Count(), _tableName);
			return entities;
		}
		catch (Exception ex) when (ex is not RepositoryException)
		{
			var errorMessage = $"Error getting all entities from {_tableName}";
			_logger.LogError(ex, errorMessage);
			
			if (ex is InvalidOperationException || ex is TimeoutException)
			{
				throw new DatabaseConnectionException(errorMessage, "GetAll", null, null, null, ex);
			}
			
			throw new RepositoryException(errorMessage, "GetAll", typeof(TEntity).Name, null, _tableName, ex);
		}
	}

	public virtual async Task<IEnumerable<TEntity>> GetAsync(Func<TEntity, bool> predicate)
	{
		try
		{
			var allEntities = await GetAllAsync();
			var filteredEntities = allEntities.Where(predicate);
			
			_logger.LogDebug("Filtered entities from {Table}: {Count} found", _tableName, filteredEntities.Count());
			return filteredEntities;
		}
		catch (Exception ex) when (ex is not RepositoryException)
		{
			var errorMessage = $"Error getting filtered entities from {_tableName}";
			_logger.LogError(ex, errorMessage);
			
			if (ex is InvalidOperationException || ex is TimeoutException)
			{
				throw new DatabaseConnectionException(errorMessage, "GetFiltered", null, null, null, ex);
			}
			
			throw new RepositoryException(errorMessage, "GetFiltered", typeof(TEntity).Name, null, _tableName, ex);
		}
	}

	public virtual async Task<TEntity?> GetFirstAsync(Func<TEntity, bool> predicate)
	{
		try
		{
			var allEntities = await GetAllAsync();
			var firstEntity = allEntities.FirstOrDefault(predicate);
			
			_logger.LogDebug("GetFirst from {Table}: Entity found: {Found}", _tableName, firstEntity != null);
			return firstEntity;
		}
		catch (Exception ex) when (ex is not RepositoryException)
		{
			var errorMessage = $"Error getting first entity from {_tableName}";
			_logger.LogError(ex, errorMessage);
			
			if (ex is InvalidOperationException || ex is TimeoutException)
			{
				throw new DatabaseConnectionException(errorMessage, "GetFirst", null, null, null, ex);
			}
			
			throw new RepositoryException(errorMessage, "GetFirst", typeof(TEntity).Name, null, _tableName, ex);
		}
	}

	public virtual async Task<bool> ExistsAsync(long id)
	{
		try
		{
			var sql = $"SELECT COUNT(1) FROM {_tableName} WHERE {_idColumn} = @Id";
			var connection = await _dbContext.GetConnectionAsync();
			
			var count = await connection.ExecuteScalarAsync<int>(sql, new { Id = id }, _dbContext.Transaction);
			var exists = count > 0;
			
			_logger.LogDebug("Entity with ID {Id} exists in {Table}: {Exists}", id, _tableName, exists);
			return exists;
		}
		catch (Exception ex) when (ex is not RepositoryException)
		{
			var errorMessage = $"Error checking existence of entity with ID {id} in {_tableName}";
			_logger.LogError(ex, errorMessage);
			
			if (ex is InvalidOperationException || ex is TimeoutException)
			{
				throw new DatabaseConnectionException(errorMessage, "Exists", null, null, null, ex);
			}
			
			throw new RepositoryException(errorMessage, "Exists", typeof(TEntity).Name, id, _tableName, ex);
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
				var count = await connection.ExecuteScalarAsync<int>(sql, null, _dbContext.Transaction);
				
				_logger.LogDebug("Total count in {Table}: {Count}", _tableName, count);
				return count;
			}

			var allEntities = await GetAllAsync();
			var filteredCount = allEntities.Count(predicate);
			
			_logger.LogDebug("Filtered count in {Table}: {Count}", _tableName, filteredCount);
			return filteredCount;
		}
		catch (Exception ex) when (ex is not RepositoryException)
		{
			var errorMessage = $"Error counting entities in {_tableName}";
			_logger.LogError(ex, errorMessage);
			
			if (ex is InvalidOperationException || ex is TimeoutException)
			{
				throw new DatabaseConnectionException(errorMessage, "Count", null, null, null, ex);
			}
			
			throw new RepositoryException(errorMessage, "Count", typeof(TEntity).Name, null, _tableName, ex);
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
			
			_logger.LogDebug("Retrieved {Count} entities from {Table} (Page: {Page}, Size: {Size}, Total: {Total})", 
				items.Count(), _tableName, pageNumber, pageSize, totalCount);
			
			return (items, totalCount);
		}
		catch (Exception ex) when (ex is not RepositoryException)
		{
			var errorMessage = $"Error getting paged entities from {_tableName}";
			_logger.LogError(ex, errorMessage);
			
			if (ex is InvalidOperationException || ex is TimeoutException)
			{
				throw new DatabaseConnectionException(errorMessage, "GetPaged", null, null, null, ex);
			}
			
			throw new RepositoryException(errorMessage, "GetPaged", typeof(TEntity).Name, null, _tableName, ex);
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
		catch (Exception ex) when (ex is not RepositoryException)
		{
			var errorMessage = $"Error adding entity to {_tableName}";
			_logger.LogError(ex, errorMessage);
			
			if (ex is InvalidOperationException || ex is TimeoutException)
			{
				throw new DatabaseConnectionException(errorMessage, "Add", null, null, null, ex);
			}
			
			throw new RepositoryException(errorMessage, "Add", typeof(TEntity).Name, null, _tableName, ex);
		}
	}

	public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
	{
		try
		{
			var results = new List<TEntity>();
			var entityList = entities.ToList();
			
			_logger.LogDebug("Starting to add {Count} entities to {Table}", entityList.Count, _tableName);
			
			foreach (var entity in entityList)
			{
				var result = await AddAsync(entity);
				results.Add(result);
			}
			
			_logger.LogInformation("Successfully added {Count} entities to {Table}", results.Count, _tableName);
			return results;
		}
		catch (Exception ex) when (ex is not RepositoryException)
		{
			var errorMessage = $"Error adding multiple entities to {_tableName}";
			_logger.LogError(ex, errorMessage);
			
			if (ex is InvalidOperationException || ex is TimeoutException)
			{
				throw new DatabaseConnectionException(errorMessage, "AddRange", null, null, null, ex);
			}
			
			throw new RepositoryException(errorMessage, "AddRange", typeof(TEntity).Name, null, _tableName, ex);
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
			{
				var errorMessage = $"Entity with ID {entity.Id} not found in {_tableName}";
				_logger.LogWarning(errorMessage);
				throw new EntityNotFoundException(typeof(TEntity).Name, entity.Id, errorMessage, _tableName);
			}
			
			_logger.LogInformation("Updated entity with ID {Id} in {Table}", entity.Id, _tableName);
			return entity;
		}
		catch (Exception ex) when (ex is not RepositoryException && ex is not EntityNotFoundException)
		{
			var errorMessage = $"Error updating entity with ID {entity.Id} in {_tableName}";
			_logger.LogError(ex, errorMessage);
			
			if (ex is InvalidOperationException || ex is TimeoutException)
			{
				throw new DatabaseConnectionException(errorMessage, "Update", null, null, null, ex);
			}
			
			throw new RepositoryException(errorMessage, "Update", typeof(TEntity).Name, entity.Id, _tableName, ex);
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
			
			_logger.LogWarning("Entity with ID {Id} not found for deletion in {Table}", id, _tableName);
			return false;
		}
		catch (Exception ex) when (ex is not RepositoryException)
		{
			var errorMessage = $"Error deleting entity with ID {id} from {_tableName}";
			_logger.LogError(ex, errorMessage);
			
			if (ex is InvalidOperationException || ex is TimeoutException)
			{
				throw new DatabaseConnectionException(errorMessage, "Delete", null, null, null, ex);
			}
			
			throw new RepositoryException(errorMessage, "Delete", typeof(TEntity).Name, id, _tableName, ex);
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
			var entityList = entitiesToDelete.ToList();
			
			_logger.LogDebug("Starting to delete {Count} entities from {Table}", entityList.Count, _tableName);
			
			var count = 0;
			foreach (var entity in entityList)
			{
				if (await DeleteAsync(entity.Id))
					count++;
			}
			
			_logger.LogInformation("Successfully deleted {Count} entities from {Table}", count, _tableName);
			return count;
		}
		catch (Exception ex) when (ex is not RepositoryException)
		{
			var errorMessage = $"Error deleting multiple entities from {_tableName}";
			_logger.LogError(ex, errorMessage);
			
			if (ex is InvalidOperationException || ex is TimeoutException)
			{
				throw new DatabaseConnectionException(errorMessage, "DeleteRange", null, null, null, ex);
			}
			
			throw new RepositoryException(errorMessage, "DeleteRange", typeof(TEntity).Name, null, _tableName, ex);
		}
	}

	#endregion

	#region Bulk Operations

	public virtual async Task<int> ExecuteAsync(string sql, object? param = null)
	{
		try
		{
			var connection = await _dbContext.GetConnectionAsync();
			var rowsAffected = await connection.ExecuteAsync(sql, param, _dbContext.Transaction);
			
			_logger.LogDebug("Executed SQL: {Sql}, Rows affected: {RowsAffected}", sql, rowsAffected);
			return rowsAffected;
		}
		catch (Exception ex) when (ex is not RepositoryException)
		{
			var errorMessage = $"Error executing SQL: {sql}";
			_logger.LogError(ex, errorMessage);
			
			if (ex is InvalidOperationException || ex is TimeoutException)
			{
				throw new DatabaseConnectionException(errorMessage, "Execute", null, null, null, ex);
			}
			
			throw new RepositoryException(errorMessage, "Execute", typeof(TEntity).Name, null, _tableName, ex);
		}
	}

	public virtual async Task<IEnumerable<TResult>> QueryAsync<TResult>(string sql, object? param = null)
	{
		try
		{
			var connection = await _dbContext.GetConnectionAsync();
			var results = await connection.QueryAsync<TResult>(sql, param, _dbContext.Transaction);
			
			_logger.LogDebug("Query executed: {Sql}, Results count: {Count}", sql, results.Count());
			return results;
		}
		catch (Exception ex) when (ex is not RepositoryException)
		{
			var errorMessage = $"Error querying SQL: {sql}";
			_logger.LogError(ex, errorMessage);
			
			if (ex is InvalidOperationException || ex is TimeoutException)
			{
				throw new DatabaseConnectionException(errorMessage, "Query", null, null, null, ex);
			}
			
			throw new RepositoryException(errorMessage, "Query", typeof(TEntity).Name, null, _tableName, ex);
		}
	}

	public virtual async Task<TResult?> QueryFirstOrDefaultAsync<TResult>(string sql, object? param = null)
	{
		try
		{
			var connection = await _dbContext.GetConnectionAsync();
			var result = await connection.QueryFirstOrDefaultAsync<TResult>(sql, param, _dbContext.Transaction);
			
			_logger.LogDebug("QueryFirstOrDefault executed: {Sql}, Result found: {Found}", sql, result != null);
			return result;
		}
		catch (Exception ex) when (ex is not RepositoryException)
		{
			var errorMessage = $"Error querying first or default: {sql}";
			_logger.LogError(ex, errorMessage);
			
			if (ex is InvalidOperationException || ex is TimeoutException)
			{
				throw new DatabaseConnectionException(errorMessage, "QueryFirstOrDefault", null, null, null, ex);
			}
			
			throw new RepositoryException(errorMessage, "QueryFirstOrDefault", typeof(TEntity).Name, null, _tableName, ex);
		}
	}

	public virtual async Task<TResult> QuerySingleAsync<TResult>(string sql, object? param = null)
	{
		try
		{
			var connection = await _dbContext.GetConnectionAsync();
			var result = await connection.QuerySingleAsync<TResult>(sql, param, _dbContext.Transaction);
			
			_logger.LogDebug("QuerySingle executed: {Sql}, Result found: {Found}", sql, result != null);
			return result;
		}
		catch (Exception ex) when (ex is not RepositoryException)
		{
			var errorMessage = $"Error querying single: {sql}";
			_logger.LogError(ex, errorMessage);
			
			if (ex is InvalidOperationException || ex is TimeoutException)
			{
				throw new DatabaseConnectionException(errorMessage, "QuerySingle", null, null, null, ex);
			}
			
			throw new RepositoryException(errorMessage, "QuerySingle", typeof(TEntity).Name, null, _tableName, ex);
		}
	}

	#endregion
}
