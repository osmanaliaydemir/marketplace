using Application.Abstractions;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Application.Exceptions;

namespace Application.Services;

public sealed class ExceptionLogService : IExceptionLogService
{
    private readonly IExceptionLogRepository _exceptionLogRepository;
    private readonly ILogger<ExceptionLogService> _logger;

    public ExceptionLogService(IExceptionLogRepository exceptionLogRepository, ILogger<ExceptionLogService> logger)
    {
        _exceptionLogRepository = exceptionLogRepository;
        _logger = logger;
    }

    public async Task<ExceptionLog> LogExceptionAsync(Exception exception, object? httpContextData = null)
    {
        try
        {
            var exceptionLog = new ExceptionLog
            {
                ExceptionType = exception.GetType().Name,
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                Source = exception.Source,
                Severity = DetermineSeverity(exception),
                Status = ExceptionStatus.New,
                OccurredAt = DateTime.UtcNow,
                LastOccurrence = DateTime.UtcNow,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                ApplicationVersion = GetApplicationVersion(),
                CorrelationId = null, // CorrelationId will be set from httpContextData if available
                Tags = JsonSerializer.Serialize(ExtractExceptionTags(exception))
            };

            // HTTP context bilgilerini ekle
            if (httpContextData != null)
            {
                // HttpContext data'sı varsa, reflection ile property'leri al
                try
                {
                    var contextType = httpContextData.GetType();
                    
                    // Request path
                    var pathProperty = contextType.GetProperty("RequestPath");
                    if (pathProperty != null)
                        exceptionLog.RequestPath = pathProperty.GetValue(httpContextData)?.ToString();
                    
                    // Request method
                    var methodProperty = contextType.GetProperty("RequestMethod");
                    if (methodProperty != null)
                        exceptionLog.RequestMethod = methodProperty.GetValue(httpContextData)?.ToString();
                    
                    // IP Address
                    var ipProperty = contextType.GetProperty("IpAddress");
                    if (ipProperty != null)
                        exceptionLog.IpAddress = ipProperty.GetValue(httpContextData)?.ToString();
                    
                    // User Agent
                    var userAgentProperty = contextType.GetProperty("UserAgent");
                    if (userAgentProperty != null)
                        exceptionLog.UserAgent = userAgentProperty.GetValue(httpContextData)?.ToString();
                    
                    // User ID
                    var userIdProperty = contextType.GetProperty("UserId");
                    if (userIdProperty != null)
                        exceptionLog.UserId = userIdProperty.GetValue(httpContextData)?.ToString();
                    
                    // Query String
                    var queryStringProperty = contextType.GetProperty("QueryString");
                    if (queryStringProperty != null)
                        exceptionLog.QueryString = queryStringProperty.GetValue(httpContextData)?.ToString();
                    
                    // Request Body
                    var requestBodyProperty = contextType.GetProperty("RequestBody");
                    if (requestBodyProperty != null)
                        exceptionLog.RequestBody = requestBodyProperty.GetValue(httpContextData)?.ToString();
                    
                    // Correlation ID
                    var correlationIdProperty = contextType.GetProperty("CorrelationId");
                    if (correlationIdProperty != null)
                        exceptionLog.CorrelationId = correlationIdProperty.GetValue(httpContextData)?.ToString();
                }
                catch
                {
                    // Reflection hatası olursa devam et
                }
            }

            // Benzer exception var mı kontrol et
            var similarException = await _exceptionLogRepository.GetSimilarExceptionsAsync(
                exceptionLog.ExceptionType, 
                exceptionLog.Message);

            if (similarException.Any())
            {
                var existingException = similarException.First();
                // Mevcut exception'ı güncelle
                await _exceptionLogRepository.IncrementOccurrenceCountAsync(existingException.Id);
                exceptionLog = existingException;
                exceptionLog.OccurrenceCount++;
                exceptionLog.LastOccurrence = DateTime.UtcNow;
            }

            var savedException = await _exceptionLogRepository.AddAsync(exceptionLog);
            
            _logger.LogInformation("Exception logged: {ExceptionType} - {Message}", 
                exceptionLog.ExceptionType, exceptionLog.Message);

            return savedException;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging exception");
            throw;
        }
    }

    public async Task<ExceptionLog?> GetByIdAsync(long id)
    {
        return await _exceptionLogRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<ExceptionLog>> GetRecentExceptionsAsync(int count = 100)
    {
        var exceptions = await _exceptionLogRepository.GetAsync(e => true);
        return exceptions.OrderByDescending(e => e.LastOccurrence).Take(count);
    }

    public async Task<IEnumerable<ExceptionLog>> GetBySeverityAsync(ExceptionSeverity severity)
    {
        return await _exceptionLogRepository.GetBySeverityAsync(severity);
    }

    public async Task<IEnumerable<ExceptionLog>> GetByStatusAsync(ExceptionStatus status)
    {
        return await _exceptionLogRepository.GetByStatusAsync(status);
    }

    public async Task<IEnumerable<ExceptionLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _exceptionLogRepository.GetByDateRangeAsync(startDate, endDate);
    }

    public async Task<IEnumerable<ExceptionLog>> GetUnresolvedAsync()
    {
        return await _exceptionLogRepository.GetUnresolvedAsync();
    }

    public async Task<ExceptionLog?> GetByCorrelationIdAsync(string correlationId)
    {
        return await _exceptionLogRepository.GetByCorrelationIdAsync(correlationId);
    }

    public async Task UpdateStatusAsync(long id, ExceptionStatus status, string? resolvedBy = null, string? notes = null)
    {
        await _exceptionLogRepository.UpdateStatusAsync(id, status, resolvedBy, notes);
    }

    public async Task<ExceptionAnalytics> GetAnalyticsAsync(DateTime startDate, DateTime endDate)
    {
        var exceptions = await _exceptionLogRepository.GetByDateRangeAsync(startDate, endDate);
        var analytics = new ExceptionAnalytics
        {
            TotalExceptions = exceptions.Count(),
            UnresolvedExceptions = exceptions.Count(e => e.Status == ExceptionStatus.New || e.Status == ExceptionStatus.Investigating),
            CriticalExceptions = exceptions.Count(e => e.Severity == ExceptionSeverity.Critical),
            HighSeverityExceptions = exceptions.Count(e => e.Severity == ExceptionSeverity.High)
        };

        // Exception type distribution
        analytics.ExceptionTypeDistribution = exceptions
            .GroupBy(e => e.ExceptionType)
            .ToDictionary(g => g.Key, g => g.Count());

        // Severity distribution
        analytics.SeverityDistribution = exceptions
            .GroupBy(e => e.Severity)
            .ToDictionary(g => g.Key, g => g.Count());

        // Status distribution
        analytics.StatusDistribution = exceptions
            .GroupBy(e => e.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        // Top exception types
        analytics.TopExceptionTypes = analytics.ExceptionTypeDistribution
            .OrderByDescending(kvp => kvp.Value)
            .Take(10)
            .Select(kvp => kvp.Key)
            .ToList();

        // Top affected endpoints
        analytics.TopAffectedEndpoints = exceptions
            .Where(e => !string.IsNullOrEmpty(e.RequestPath))
            .GroupBy(e => e.RequestPath)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .Select(g => g.Key)
            .ToList();

        return analytics;
    }

    public async Task<IEnumerable<ExceptionTrend>> GetTrendsAsync(DateTime startDate, DateTime endDate, string groupBy = "day")
    {
        var exceptions = await _exceptionLogRepository.GetByDateRangeAsync(startDate, endDate);
        var trends = new List<ExceptionTrend>();

        var groupedExceptions = groupBy.ToLower() switch
        {
            "hour" => exceptions.GroupBy(e => new DateTime(e.OccurredAt.Year, e.OccurredAt.Month, e.OccurredAt.Day, e.OccurredAt.Hour, 0, 0)),
            "day" => exceptions.GroupBy(e => e.OccurredAt.Date),
            "week" => exceptions.GroupBy(e => e.OccurredAt.Date.AddDays(-(int)e.OccurredAt.DayOfWeek)),
            "month" => exceptions.GroupBy(e => new DateTime(e.OccurredAt.Year, e.OccurredAt.Month, 1)),
            _ => exceptions.GroupBy(e => e.OccurredAt.Date)
        };

        foreach (var group in groupedExceptions)
        {
            trends.Add(new ExceptionTrend
            {
                Date = group.Key,
                Group = groupBy,
                Count = group.Count(),
                Severity = group.OrderByDescending(e => e.Severity).First().Severity,
                ExceptionType = group.GroupBy(e => e.ExceptionType).OrderByDescending(g => g.Count()).First().Key
            });
        }

        return trends.OrderBy(t => t.Date);
    }

    public async Task<IEnumerable<ExceptionLog>> GetSimilarExceptionsAsync(string exceptionType, string message, int withinHours = 24)
    {
        return await _exceptionLogRepository.GetSimilarExceptionsAsync(exceptionType, message, withinHours);
    }

    public async Task<bool> MarkAsResolvedAsync(long id, string resolvedBy, string? notes = null)
    {
        try
        {
            await _exceptionLogRepository.UpdateStatusAsync(id, ExceptionStatus.Resolved, resolvedBy, notes);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> MarkAsIgnoredAsync(long id, string ignoredBy, string? notes = null)
    {
        try
        {
            await _exceptionLogRepository.UpdateStatusAsync(id, ExceptionStatus.Ignored, ignoredBy, notes);
            return true;
        }
        catch
        {
            return false;
        }
    }

    #region Private Helper Methods

    private static ExceptionSeverity DetermineSeverity(Exception exception)
    {
        return exception switch
        {
            BusinessRuleViolationException => ExceptionSeverity.Medium,
            ArgumentNullException => ExceptionSeverity.Low,
            ArgumentException => ExceptionSeverity.Low,
            UnauthorizedAccessException => ExceptionSeverity.Medium,
            KeyNotFoundException => ExceptionSeverity.Low,
            InvalidOperationException => ExceptionSeverity.Medium,
            TimeoutException => ExceptionSeverity.High,
            _ => ExceptionSeverity.Medium
        };
    }

    private static string GetApplicationVersion()
    {
        return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
    }



    private static Dictionary<string, object> ExtractExceptionTags(Exception exception)
    {
        var tags = new Dictionary<string, object>
        {
            ["InnerExceptionType"] = exception.InnerException?.GetType().Name ?? "None",
            ["InnerExceptionMessage"] = exception.InnerException?.Message ?? "None",
            ["TargetSite"] = exception.TargetSite?.Name ?? "None",
            ["HelpLink"] = exception.HelpLink ?? "None"
        };

        if (exception is BusinessRuleViolationException businessEx)
        {
            tags["RuleName"] = businessEx.RuleName;
            tags["RuleData"] = businessEx.RuleData ?? "None";
        }

        return tags;
    }

    #endregion
}
