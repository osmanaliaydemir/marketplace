using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Any;

namespace Api.Swagger;

public class AddApiVersionParameter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();

        // API Version parametresi ekle
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "api-version",
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString("1.0")
            },
            Description = "API version for backward compatibility"
        });

        // Request ID parametresi ekle
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Request-ID",
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string"
            },
            Description = "Unique request identifier for tracing"
        });
    }
}
