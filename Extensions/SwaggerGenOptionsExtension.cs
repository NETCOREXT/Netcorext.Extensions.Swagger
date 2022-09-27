using Microsoft.OpenApi.Models;
using Netcorext.Extensions.Swagger.Filters;

namespace Swashbuckle.AspNetCore.SwaggerGen;

public static class SwaggerGenOptionsExtension
{
    public static SwaggerGenOptions AddParameter(this SwaggerGenOptions options, params OpenApiParameter[] parameters)
    {
        if (!parameters.Any()) return options;

        options.OperationFilter<CustomParameterFilter>(new object[] { parameters });

        return options;
    }
}