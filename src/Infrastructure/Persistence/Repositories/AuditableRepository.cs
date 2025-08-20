using Infrastructure.Persistence.Context;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Dapper;
using Infrastructure.Persistence.Naming;
using Application.Abstractions;

namespace Infrastructure.Persistence.Repositories;

public class AuditableRepository<TEntity> : Repository<TEntity>, IAuditableRepository<TEntity> 
    where TEntity : class, IAuditableEntity
{
    public AuditableRepository(IDbContext dbContext, ILogger logger, ITableNameResolver tableNameResolver, IColumnNameResolver columnNameResolver) 
        : base(dbContext, logger, tableNameResolver, columnNameResolver)
    {
    }

    public override async Task<TEntity> AddAsync(TEntity entity)
    {
        // Set audit fields
        entity.CreatedAt = DateTime.UtcNow;
        entity.ModifiedAt = null;
        
        return await base.AddAsync(entity);
    }

    public override async Task<TEntity> UpdateAsync(TEntity entity)
    {
        // Set audit fields
        entity.ModifiedAt = DateTime.UtcNow;
        
        return await base.UpdateAsync(entity);
    }

    public async Task<IEnumerable<TEntity>> GetModifiedSinceAsync(DateTime since)
    {
        try
        {
            var sql = $"SELECT * FROM {_tableName} WHERE modified_at >= @Since ORDER BY modified_at DESC";
            var connection = await _dbContext.GetConnectionAsync();
            
            return await connection.QueryAsync<TEntity>(sql, new { Since = since });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entities modified since {Since} from {Table}", since, _tableName);
            throw;
        }
    }

    public async Task<IEnumerable<TEntity>> GetCreatedBetweenAsync(DateTime start, DateTime end)
    {
        try
        {
            var sql = $"SELECT * FROM {_tableName} WHERE created_at BETWEEN @Start AND @End ORDER BY created_at DESC";
            var connection = await _dbContext.GetConnectionAsync();
            
            return await connection.QueryAsync<TEntity>(sql, new { Start = start, End = end });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entities created between {Start} and {End} from {Table}", start, end, _tableName);
            throw;
        }
    }
}
