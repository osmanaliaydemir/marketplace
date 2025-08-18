namespace Api.Middlewares;

public sealed class ProblemDetailsMiddleware : IMiddleware
{
    private readonly ILogger<ProblemDetailsMiddleware> _log;
    public ProblemDetailsMiddleware(ILogger<ProblemDetailsMiddleware> log) => _log = log;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync("{\"title\":\"Internal Server Error\"}");
        }
    }
}
