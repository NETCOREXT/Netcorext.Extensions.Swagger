using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Netcorext.Extensions.Swagger.Filters;

public class CustomParameterFilter : IOperationFilter
{
    private readonly IEnumerable<OpenApiParameter> _parameters;

    public CustomParameterFilter(IEnumerable<OpenApiParameter> parameters)
    {
        _parameters = parameters;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!_parameters.Any()) return;

        foreach (var parameter in _parameters)
        {
            operation.Parameters.Add(parameter);
        }
    }
}