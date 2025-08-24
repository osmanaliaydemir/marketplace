using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Naming;
using Microsoft.Extensions.Logging;
using Dapper;
using System.Data;

namespace Infrastructure.Persistence.Repositories;

public sealed class ExceptionLogRepository : Repository<ExceptionLog>, IExceptionLogRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ExceptionLogRepository(
        IDbContext context, 
        ILogger<ExceptionLogRepository> logger, 
        ITableNameResolver tableNameResolver, 
        IColumnNameResolver columnNameResolver,
        ISqlConnectionFactory connectionFactory) 
        : base(context, logger, tableNameResolver, columnNameResolver)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<ExceptionLog>> GetBySeverityAsync(ExceptionSeverity severity)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            SELECT * FROM exception_logs 
            WHERE severity = @Severity 
            ORDER BY occurred_at DESC";
        
        return await connection.QueryAsync<ExceptionLog>(sql, new { Severity = (int)severity });
    }

    public async Task<IEnumerable<ExceptionLog>> GetByStatusAsync(ExceptionStatus status)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            SELECT * FROM exception_logs 
            WHERE status = @Status 
            ORDER BY occurred_at DESC";
        
        return await connection.QueryAsync<ExceptionLog>(sql, new { Status = (int)status });
    }

    public async Task<IEnumerable<ExceptionLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            SELECT * FROM exception_logs 
            WHERE occurred_at BETWEEN @StartDate AND @EndDate 
            ORDER BY occurred_at DESC";
        
        return await connection.QueryAsync<ExceptionLog>(sql, new { StartDate = startDate, EndDate = endDate });
    }

    public async Task<IEnumerable<ExceptionLog>> GetByExceptionTypeAsync(string exceptionType)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            SELECT * FROM exception_logs 
            WHERE exception_type = @ExceptionType 
            ORDER BY occurred_at DESC";
        
        return await connection.QueryAsync<ExceptionLog>(sql, new { ExceptionType = exceptionType });
    }

    public async Task<IEnumerable<ExceptionLog>> GetUnresolvedAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            SELECT * FROM exception_logs 
            WHERE status IN (1, 2) -- New or Investigating
            ORDER BY severity DESC, occurred_at DESC";
        
        return await connection.QueryAsync<ExceptionLog>(sql);
    }

    public async Task<ExceptionLog?> GetByCorrelationIdAsync(string correlationId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            SELECT TOP 1 * FROM exception_logs 
            WHERE correlation_id = @CorrelationId 
            ORDER BY occurred_at DESC";
        
        return await connection.QueryFirstOrDefaultAsync<ExceptionLog>(sql, new { CorrelationId = correlationId });
    }

    public async Task<int> GetCountBySeverityAsync(ExceptionSeverity severity)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            SELECT COUNT(*) FROM exception_logs 
            WHERE severity = @Severity";
        
        return await connection.QuerySingleAsync<int>(sql, new { Severity = (int)severity });
    }

    public async Task<int> GetCountByStatusAsync(ExceptionStatus status)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            SELECT COUNT(*) FROM exception_logs 
            WHERE status = @Status";
        
        return await connection.QuerySingleAsync<int>(sql, new { Status = (int)status });
    }

    public async Task<Dictionary<string, int>> GetExceptionTypeStatsAsync(DateTime startDate, DateTime endDate)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            SELECT exception_type, COUNT(*) as Count
            FROM exception_logs 
            WHERE occurred_at BETWEEN @StartDate AND @EndDate
            GROUP BY exception_type
            ORDER BY Count DESC";
        
        var results = await connection.QueryAsync<(string ExceptionType, int Count)>(sql, 
            new { StartDate = startDate, EndDate = endDate });
        
        return results.ToDictionary(x => x.ExceptionType, x => x.Count);
    }

    public async Task<Dictionary<ExceptionSeverity, int>> GetSeverityStatsAsync(DateTime startDate, DateTime endDate)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            SELECT severity, COUNT(*) as Count
            FROM exception_logs 
            WHERE occurred_at BETWEEN @StartDate AND @EndDate
            GROUP BY severity
            ORDER BY severity";
        
        var results = await connection.QueryAsync<(int Severity, int Count)>(sql, 
            new { StartDate = startDate, EndDate = endDate });
        
        return results.ToDictionary(x => (ExceptionSeverity)x.Severity, x => x.Count);
    }

    public async Task UpdateStatusAsync(long id, ExceptionStatus status, string? resolvedBy = null, string? notes = null)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            UPDATE exception_logs 
            SET status = @Status,
                resolved_by = @ResolvedBy,
                resolution_notes = @Notes,
                resolved_at = CASE WHEN @Status = 3 THEN GETUTCDATE() ELSE resolved_at END,
                updated_at = GETUTCDATE()
            WHERE id = @Id";
        
        await connection.ExecuteAsync(sql, new 
        { 
            Id = id, 
            Status = (int)status, 
            ResolvedBy = resolvedBy, 
            Notes = notes 
        });
    }

    public async Task IncrementOccurrenceCountAsync(long id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            UPDATE exception_logs 
            SET occurrence_count = occurrence_count + 1,
                last_occurrence = GETUTCDATE(),
                updated_at = GETUTCDATE()
            WHERE id = @Id";
        
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<IEnumerable<ExceptionLog>> GetSimilarExceptionsAsync(string exceptionType, string message, int withinHours = 24)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            SELECT * FROM exception_logs 
            WHERE exception_type = @ExceptionType 
                AND message = @Message
                AND occurred_at >= DATEADD(HOUR, -@WithinHours, GETUTCDATE())
            ORDER BY occurred_at DESC";
        
        return await connection.QueryAsync<ExceptionLog>(sql, new 
        { 
            ExceptionType = exceptionType, 
            Message = message, 
            WithinHours = withinHours 
        });
    }

    // Override AddAsync to handle snake_case column mapping
    public override async Task<ExceptionLog> AddAsync(ExceptionLog entity)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            INSERT INTO exception_logs (
                exception_type, message, stack_trace, source, user_agent, user_id,
                request_path, request_method, request_body, query_string, ip_address,
                correlation_id, environment, application_version, severity, status,
                occurred_at, resolved_at, resolved_by, resolution_notes, occurrence_count,
                last_occurrence, tags
            ) 
            VALUES (
                @ExceptionType, @Message, @StackTrace, @Source, @UserAgent, @UserId,
                @RequestPath, @RequestMethod, @RequestBody, @QueryString, @IpAddress,
                @CorrelationId, @Environment, @ApplicationVersion, @Severity, @Status,
                @OccurredAt, @ResolvedAt, @ResolvedBy, @ResolutionNotes, @OccurrenceCount,
                @LastOccurrence, @Tags
            );
            SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

        var id = await connection.QuerySingleAsync<long>(sql, new
        {
            ExceptionType = entity.ExceptionType,
            Message = entity.Message,
            StackTrace = entity.StackTrace,
            Source = entity.Source,
            UserAgent = entity.UserAgent,
            UserId = entity.UserId,
            RequestPath = entity.RequestPath,
            RequestMethod = entity.RequestMethod,
            RequestBody = entity.RequestBody,
            QueryString = entity.QueryString,
            IpAddress = entity.IpAddress,
            CorrelationId = entity.CorrelationId,
            Environment = entity.Environment,
            ApplicationVersion = entity.ApplicationVersion,
            Severity = (int)entity.Severity,
            Status = (int)entity.Status,
            OccurredAt = entity.OccurredAt,
            ResolvedAt = entity.ResolvedAt,
            ResolvedBy = entity.ResolvedBy,
            ResolutionNotes = entity.ResolutionNotes,
            OccurrenceCount = entity.OccurrenceCount,
            LastOccurrence = entity.LastOccurrence,
            Tags = entity.Tags
        });

        entity.Id = id;
        return entity;
    }

    // Override GetByIdAsync to handle snake_case column mapping
    public override async Task<ExceptionLog?> GetByIdAsync(long id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = "SELECT * FROM exception_logs WHERE id = @Id";
        
        return await connection.QueryFirstOrDefaultAsync<ExceptionLog>(sql, new { Id = id });
    }
}
