using Domain.Entities;

namespace Application.Abstractions;

public interface IExceptionLogRepository : IRepository<ExceptionLog>
{
    Task<IEnumerable<ExceptionLog>> GetBySeverityAsync(ExceptionSeverity severity);
    Task<IEnumerable<ExceptionLog>> GetByStatusAsync(ExceptionStatus status);
    Task<IEnumerable<ExceptionLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<ExceptionLog>> GetByExceptionTypeAsync(string exceptionType);
    Task<IEnumerable<ExceptionLog>> GetUnresolvedAsync();
    Task<ExceptionLog?> GetByCorrelationIdAsync(string correlationId);
    Task<int> GetCountBySeverityAsync(ExceptionSeverity severity);
    Task<int> GetCountByStatusAsync(ExceptionStatus status);
    Task<Dictionary<string, int>> GetExceptionTypeStatsAsync(DateTime startDate, DateTime endDate);
    Task<Dictionary<ExceptionSeverity, int>> GetSeverityStatsAsync(DateTime startDate, DateTime endDate);
    Task UpdateStatusAsync(long id, ExceptionStatus status, string? resolvedBy = null, string? notes = null);
    Task IncrementOccurrenceCountAsync(long id);
    Task<IEnumerable<ExceptionLog>> GetSimilarExceptionsAsync(string exceptionType, string message, int withinHours = 24);
}
