using Domain.Entities;

namespace Application.Abstractions;

public interface IExceptionLogService
{
    Task<ExceptionLog> LogExceptionAsync(Exception exception, object? httpContextData = null);
    Task<ExceptionLog?> GetByIdAsync(long id);
    Task<IEnumerable<ExceptionLog>> GetRecentExceptionsAsync(int count = 100);
    Task<IEnumerable<ExceptionLog>> GetBySeverityAsync(ExceptionSeverity severity);
    Task<IEnumerable<ExceptionLog>> GetByStatusAsync(ExceptionStatus status);
    Task<IEnumerable<ExceptionLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<ExceptionLog>> GetUnresolvedAsync();
    Task<ExceptionLog?> GetByCorrelationIdAsync(string correlationId);
    Task UpdateStatusAsync(long id, ExceptionStatus status, string? resolvedBy = null, string? notes = null);
    Task<ExceptionAnalytics> GetAnalyticsAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<ExceptionTrend>> GetTrendsAsync(DateTime startDate, DateTime endDate, string groupBy = "day");
    Task<IEnumerable<ExceptionLog>> GetSimilarExceptionsAsync(string exceptionType, string message, int withinHours = 24);
    Task<bool> MarkAsResolvedAsync(long id, string resolvedBy, string? notes = null);
    Task<bool> MarkAsIgnoredAsync(long id, string ignoredBy, string? notes = null);
}

public sealed class ExceptionAnalytics
{
    public int TotalExceptions { get; set; }
    public int UnresolvedExceptions { get; set; }
    public int CriticalExceptions { get; set; }
    public int HighSeverityExceptions { get; set; }
    public Dictionary<string, int> ExceptionTypeDistribution { get; set; } = new();
    public Dictionary<ExceptionSeverity, int> SeverityDistribution { get; set; } = new();
    public Dictionary<ExceptionStatus, int> StatusDistribution { get; set; } = new();
    public double AverageResolutionTimeHours { get; set; }
    public List<string> TopExceptionTypes { get; set; } = new();
    public List<string> TopAffectedEndpoints { get; set; } = new();
}

public sealed class ExceptionTrend
{
    public DateTime Date { get; set; }
    public string Group { get; set; } = string.Empty;
    public int Count { get; set; }
    public ExceptionSeverity Severity { get; set; }
    public string ExceptionType { get; set; } = string.Empty;
}
