using Domain.Models;

namespace Domain.Entities;

public sealed class ExceptionLog : BaseEntity
{
    public string ExceptionType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? Source { get; set; }
    public string? UserAgent { get; set; }
    public string? UserId { get; set; }
    public string? RequestPath { get; set; }
    public string? RequestMethod { get; set; }
    public string? RequestBody { get; set; }
    public string? QueryString { get; set; }
    public string? IpAddress { get; set; }
    public string? CorrelationId { get; set; }
    public string? Environment { get; set; }
    public string? ApplicationVersion { get; set; }
    public ExceptionSeverity Severity { get; set; }
    public ExceptionStatus Status { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public string? ResolutionNotes { get; set; }
    public int OccurrenceCount { get; set; } = 1;
    public DateTime LastOccurrence { get; set; }
    public string? Tags { get; set; } // JSON formatında ek bilgiler
}

public enum ExceptionSeverity
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum ExceptionStatus
{
    New = 1,
    Investigating = 2,
    Resolved = 3,
    Ignored = 4
}
