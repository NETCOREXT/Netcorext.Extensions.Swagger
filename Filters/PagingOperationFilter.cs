using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Netcorext.Contracts;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Netcorext.Extensions.Swagger.Filters;

public class PagingOperationFilter : IOperationFilter, ISchemaFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null || !operation.Parameters.Any())
            return;

        operation.Parameters = (from o in operation.Parameters
                                join i in context.ApiDescription.ParameterDescriptions on o.Name.ToLower() equals i.Name.ToLower() into g
                                from i in g.DefaultIfEmpty()
                                where !(i.ParameterDescriptor.BindingInfo.BindingSource == BindingSource.Query && i.ParameterDescriptor.ParameterType == typeof(Paging) && string.Equals(i.Name, nameof(Paging.Count), StringComparison.CurrentCultureIgnoreCase))
                                select o).ToList();
    }

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(Paging))
            return;

        if (!schema.Properties.ContainsKey("count")) return;

        schema.Properties.Remove("count");
    }
}