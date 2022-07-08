using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Netcorext.Extensions.Swagger.Filters;

public class ExcludeSchemaPropertyFilter : ISchemaFilter
{
    private readonly Func<PropertyInfo, bool>? _handler;

    public ExcludeSchemaPropertyFilter(Func<PropertyInfo, bool>? handler)
    {
        _handler = handler;
    }

    public void Apply(OpenApiSchema? schema, SchemaFilterContext context)
    {
        if (schema?.Properties == null || !schema.Properties.Any() || _handler == null)
            return;

        var properties = context.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var excludedProperties = properties.Where(t => !_handler.Invoke(t)).ToArray();

        if (excludedProperties.Length == 0)
            return;

        schema.Properties = (from o in schema.Properties
                             join i in excludedProperties on o.Key.ToLower() equals i.Name.ToLower() into g
                             from i in g.DefaultIfEmpty()
                             where i != null
                             select o).ToDictionary(t => t.Key, t => t.Value);
    }
}