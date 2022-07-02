using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Netcorext.Extensions.Swagger.Filters;

public class SwaggerExcludePropertyFilter : ISchemaFilter
{
    private readonly IEnumerable<PropertyInfo> _properties;

    public SwaggerExcludePropertyFilter(IEnumerable<PropertyInfo> properties)
    {
        _properties = properties;
    }

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema?.Properties == null || !_properties.Any())
            return;

        var excludedProperties = _properties.Where(t => t.DeclaringType == context.Type)
                                            .ToArray();
        
        if (excludedProperties.Length == 0)
            return;
        
        foreach (var excludedProperty in excludedProperties)
        {
            var propertyToRemove = schema.Properties.Keys.SingleOrDefault(t => t.Equals(excludedProperty.Name, StringComparison.CurrentCultureIgnoreCase));
            
            if (propertyToRemove != null)
                schema.Properties.Remove(propertyToRemove);
        }
    }
}