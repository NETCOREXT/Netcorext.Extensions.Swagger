using Microsoft.OpenApi.Models;
using Netcorext.Extensions.Swagger.Filters;
using Netcorext.Extensions.Swagger.Helpers;

namespace Swashbuckle.AspNetCore.SwaggerGen;

public static class SwaggerGenOptionsExtension
{
    public static SwaggerGenOptions AddParameter(this SwaggerGenOptions options, params OpenApiParameter[] parameters)
    {
        if (!parameters.Any()) return options;

        options.OperationFilter<CustomParameterFilter>(new object[] { parameters });

        return options;
    }

    public static SwaggerGenOptions CustomSafetySchemaIds(this SwaggerGenOptions options)
    {
        options.CustomSchemaIds(CustomIdHelper.ReplaceCustomId);
        
        return options;
    }
    
    public static SwaggerGenOptions CustomFriendlySchemaIds(this SwaggerGenOptions options)
    {
        options.CustomSchemaIds(CustomIdHelper.FriendlyCustomId);
        
        return options;
    }
}