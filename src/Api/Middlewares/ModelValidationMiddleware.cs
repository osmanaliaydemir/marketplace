using FluentValidation;
using System.Text.Json;

namespace Api.Middlewares;

public sealed class ModelValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ModelValidationMiddleware> _logger;

    public ModelValidationMiddleware(RequestDelegate next, IServiceProvider serviceProvider, ILogger<ModelValidationMiddleware> logger)
    {
        _next = next;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Sadece POST, PUT, PATCH istekleri için validation yap
        if (context.Request.Method == HttpMethods.Post || 
            context.Request.Method == HttpMethods.Put || 
            context.Request.Method == HttpMethods.Patch)
        {
            // Request body'yi oku ve deserialize et
            await ValidateRequestAsync(context);
        }

        await _next(context);
    }

    private async Task ValidateRequestAsync(HttpContext context)
    {
        try
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null) return;

            var actionDescriptor = endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();
            if (actionDescriptor == null) return;

            // Request body'sinden model tipini çıkar
            var parameters = actionDescriptor.MethodInfo.GetParameters();
            var bodyParameter = parameters.FirstOrDefault(p => 
                p.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.FromBodyAttribute), false).Any());

            if (bodyParameter == null) return;

            var modelType = bodyParameter.ParameterType;
            var validatorType = typeof(IValidator<>).MakeGenericType(modelType);
            
            // Validator'ı DI'dan al
            var validator = _serviceProvider.GetService(validatorType) as IValidator;
            if (validator == null) return;

            // Request body'yi oku
            context.Request.EnableBuffering();
            context.Request.Body.Position = 0;
            
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (string.IsNullOrEmpty(requestBody)) return;

            // Model'i deserialize et
            var model = JsonSerializer.Deserialize(requestBody, modelType, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });

            if (model == null) return;

            // Validation yap
            var validationContext = new ValidationContext<object>(model);
            var validationResult = await validator.ValidateAsync(validationContext);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new ApiError
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                }).ToList();

                var errorResponse = new ApiErrorResponse
                {
                    Message = "Validation failed",
                    StatusCode = 400,
                    Errors = errors
                };

                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(jsonResponse);
                return;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error during model validation");
            // Validation hatası olursa devam et, controller'da handle edilir
        }
    }
}
