using System.Net;
using System.Text.Json;
using FluentValidation;
using Application.Exceptions;
using Application.Abstractions;

namespace Api.Middlewares;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            
            // Exception'ı veritabanına logla (lazy loading)
            try
            {
                var exceptionLogService = context.RequestServices.GetService<IExceptionLogService>();
                if (exceptionLogService != null)
                {
                    var contextData = new
                    {
                        RequestPath = context.Request.Path.ToString(),
                        RequestMethod = context.Request.Method,
                        IpAddress = GetClientIpAddress(context),
                        UserAgent = context.Request.Headers.UserAgent.ToString(),
                        UserId = GetUserIdFromContext(context),
                        QueryString = context.Request.QueryString.ToString(),
                        CorrelationId = context.TraceIdentifier
                    };
                    
                    await exceptionLogService.LogExceptionAsync(ex, contextData);
                }
            }
            catch (Exception logEx)
            {
                _logger.LogError(logEx, "Failed to log exception to database");
            }
            
            await HandleExceptionAsync(context, ex);
        }
    }

    private static string GetClientIpAddress(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString() ?? 
               context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? 
               context.Request.Headers["X-Real-IP"].FirstOrDefault() ?? 
               "Unknown";
    }

    private static string? GetUserIdFromContext(HttpContext context)
    {
        return context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ApiErrorResponse();

        switch (exception)
        {
            case ValidationException validationEx:
                response.Message = "Validation failed";
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Errors = validationEx.Errors.Select(e => new ApiError
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                }).ToList();
                break;

            case ArgumentNullException argNullEx:
                response.Message = "Required parameter is missing";
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Errors = new List<ApiError>
                {
                    new() { Field = argNullEx.ParamName ?? "unknown", Message = argNullEx.Message }
                };
                break;

            case ArgumentException argEx:
                response.Message = argEx.Message;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case BusinessRuleViolationException businessEx:
                response.Message = businessEx.Message;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Errors = new List<ApiError>
                {
                    new() { Field = businessEx.RuleName, Message = businessEx.Message }
                };
                break;

            case UnauthorizedAccessException:
                response.Message = "Unauthorized access";
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;

            case KeyNotFoundException:
                response.Message = "Resource not found";
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            case InvalidOperationException invalidOpEx:
                response.Message = invalidOpEx.Message;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case TimeoutException:
                response.Message = "Request timeout";
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                break;

            default:
                response.Message = "An error occurred while processing your request";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        context.Response.StatusCode = response.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

public sealed class ApiErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public List<ApiError> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string TraceId { get; set; } = Guid.NewGuid().ToString();
}

public sealed class ApiError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
