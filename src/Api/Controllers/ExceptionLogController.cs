using Application.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Support")]
public class ExceptionLogController : ControllerBase
{
    private readonly IExceptionLogService _exceptionLogService;
    private readonly ILogger<ExceptionLogController> _logger;

    public ExceptionLogController(IExceptionLogService exceptionLogService, ILogger<ExceptionLogController> logger)
    {
        _exceptionLogService = exceptionLogService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetRecentExceptions([FromQuery] int count = 100)
    {
        try
        {
            var exceptions = await _exceptionLogService.GetRecentExceptionsAsync(count);
            return Ok(exceptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent exceptions");
            throw;
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetExceptionById(long id)
    {
        try
        {
            var exception = await _exceptionLogService.GetByIdAsync(id);
            if (exception == null)
                return NotFound();

            return Ok(exception);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving exception {ExceptionId}", id);
            throw;
        }
    }

    [HttpGet("severity/{severity}")]
    public async Task<IActionResult> GetBySeverity(ExceptionSeverity severity)
    {
        try
        {
            var exceptions = await _exceptionLogService.GetBySeverityAsync(severity);
            return Ok(exceptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving exceptions by severity {Severity}", severity);
            throw;
        }
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(ExceptionStatus status)
    {
        try
        {
            var exceptions = await _exceptionLogService.GetByStatusAsync(status);
            return Ok(exceptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving exceptions by status {Status}", status);
            throw;
        }
    }

    [HttpGet("unresolved")]
    public async Task<IActionResult> GetUnresolved()
    {
        try
        {
            var exceptions = await _exceptionLogService.GetUnresolvedAsync();
            return Ok(exceptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving unresolved exceptions");
            throw;
        }
    }

    [HttpGet("correlation/{correlationId}")]
    public async Task<IActionResult> GetByCorrelationId(string correlationId)
    {
        try
        {
            var exception = await _exceptionLogService.GetByCorrelationIdAsync(correlationId);
            if (exception == null)
                return NotFound();

            return Ok(exception);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving exception by correlation ID {CorrelationId}", correlationId);
            throw;
        }
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            var analytics = await _exceptionLogService.GetAnalyticsAsync(startDate, endDate);
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving exception analytics");
            throw;
        }
    }

    [HttpGet("trends")]
    public async Task<IActionResult> GetTrends([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string groupBy = "day")
    {
        try
        {
            var trends = await _exceptionLogService.GetTrendsAsync(startDate, endDate, groupBy);
            return Ok(trends);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving exception trends");
            throw;
        }
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateExceptionStatusRequest request)
    {
        try
        {
            await _exceptionLogService.UpdateStatusAsync(id, request.Status, request.UpdatedBy, request.Notes);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating exception {ExceptionId} status", id);
            throw;
        }
    }

    [HttpPut("{id}/resolve")]
    public async Task<IActionResult> MarkAsResolved(long id, [FromBody] ResolveExceptionRequest request)
    {
        try
        {
            var success = await _exceptionLogService.MarkAsResolvedAsync(id, request.ResolvedBy, request.Notes);
            if (!success)
                return BadRequest("Failed to mark exception as resolved");

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking exception {ExceptionId} as resolved", id);
            throw;
        }
    }

    [HttpPut("{id}/ignore")]
    public async Task<IActionResult> MarkAsIgnored(long id, [FromBody] IgnoreExceptionRequest request)
    {
        try
        {
            var success = await _exceptionLogService.MarkAsIgnoredAsync(id, request.IgnoredBy, request.Notes);
            if (!success)
                return BadRequest("Failed to mark exception as ignored");

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking exception {ExceptionId} as ignored", id);
            throw;
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchExceptions([FromQuery] string exceptionType, [FromQuery] string message, [FromQuery] int withinHours = 24)
    {
        try
        {
            var exceptions = await _exceptionLogService.GetSimilarExceptionsAsync(exceptionType, message, withinHours);
            return Ok(exceptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching exceptions");
            throw;
        }
    }
}

public sealed class UpdateExceptionStatusRequest
{
    public ExceptionStatus Status { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public sealed class ResolveExceptionRequest
{
    public string ResolvedBy { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public sealed class IgnoreExceptionRequest
{
    public string IgnoredBy { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
