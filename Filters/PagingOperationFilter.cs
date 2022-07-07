using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Netcorext.Contracts;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Netcorext.Extensions.Swagger.Filters;

public class PagingOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        while (context.ApiDescription.ParameterDescriptions.FirstOrDefault(t => t.Type == typeof(Paging)) is { } parameterDescription)
        {
            if (parameterDescription.Type != typeof(Paging)) continue;

            var parameter = operation.Parameters.FirstOrDefault(t => t.Name == parameterDescription.Name);

            if (parameter == null) continue;

            operation.Parameters.Remove(parameter);

            operation.Parameters.Add(new OpenApiParameter
                                     {
                                         Name = nameof(Paging.Offset).ToLower(),
                                         In = ParameterLocation.Query,
                                         Schema = new OpenApiSchema
                                                  {
                                                      Type = "integer",
                                                      Format = "int32"
                                                  }
                                     });

            operation.Parameters.Add(new OpenApiParameter
                                     {
                                         Name = nameof(Paging.Limit).ToLower(),
                                         In = ParameterLocation.Query,
                                         Schema = new OpenApiSchema
                                                  {
                                                      Type = "integer",
                                                      Format = "int32"
                                                  }
                                     });

            context.ApiDescription.ParameterDescriptions.Remove(parameterDescription);

            context.ApiDescription.ParameterDescriptions.Add(new ApiParameterDescription
                                                             {
                                                                 Name = nameof(Paging.Offset).ToLower(),
                                                                 Source = BindingSource.Query,
                                                                 Type = typeof(int?),
                                                                 ParameterDescriptor = new ParameterDescriptor
                                                                                       {
                                                                                           Name = nameof(Paging.Offset).ToLower(),
                                                                                           ParameterType = typeof(int?)
                                                                                       }
                                                             });

            context.ApiDescription.ParameterDescriptions.Add(new ApiParameterDescription
                                                             {
                                                                 Name = nameof(Paging.Limit).ToLower(),
                                                                 Source = BindingSource.Query,
                                                                 Type = typeof(int?),
                                                                 ParameterDescriptor = new ParameterDescriptor
                                                                                       {
                                                                                           Name = nameof(Paging.Limit).ToLower(),
                                                                                           ParameterType = typeof(int?)
                                                                                       }
                                                             });
        }
    }
}