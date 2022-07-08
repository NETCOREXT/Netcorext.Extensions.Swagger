using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Netcorext.Extensions.Swagger.Filters;

public class ExcludeParameterFilter : IOperationFilter
{
    private readonly Func<ApiParameterDescription?, bool>? _handler;

    public ExcludeParameterFilter(Func<ApiParameterDescription?, bool>? handler)
    {
        _handler = handler;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null || !operation.Parameters.Any() || _handler == null)
            return;

        operation.Parameters = (from o in operation.Parameters
                                join i in context.ApiDescription.ParameterDescriptions on o.Name.ToLower() equals i.Name.ToLower() into g
                                from i in g.DefaultIfEmpty()
                                where !_handler.Invoke(i)
                                select o).ToList();
    }
}