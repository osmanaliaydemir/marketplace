using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Any;

namespace Api.Swagger;

public class AddRequiredHeaderParameter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Tüm endpoint'ler için gerekli header'ları ekle
        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();

        // Content-Type header'ı ekle
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Content-Type",
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString("application/json")
            },
            Description = "Content type of the request"
        });

        // Accept header'ı ekle
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Accept",
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString("application/json")
            },
            Description = "Accept header for response format"
        });

        // Rate limiting header'ı ekle
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-RateLimit-Limit",
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "integer"
            },
            Description = "Rate limit for this endpoint"
        });
    }
}
