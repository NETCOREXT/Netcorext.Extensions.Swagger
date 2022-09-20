using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Netcorext.Extensions.Swagger.Extensions;

public static class SwaggerFilterExtension
{
    public static bool HasParameter<T>(this ApiParameterDescription parameter, Expression<Func<T, object?>> propertySelector) where T : class
    {
        if (parameter.ParameterDescriptor.ParameterType != typeof(T)) return false;
        
        var unaryExpression = propertySelector.Body as UnaryExpression;

        if (unaryExpression?.Operand is not MemberExpression operand) return false;

        var parameterPath = operand.ToString();

        parameterPath = propertySelector.Parameters
                                        .Aggregate(parameterPath, (current, parameterExpression) => $".{current}.".Replace($".{parameterExpression.Name}.", "", StringComparison.CurrentCultureIgnoreCase))
                                        .Trim('.');

        return parameter.Name.Equals(parameterPath, StringComparison.CurrentCultureIgnoreCase);
    }
    
    public static bool HasProperty<T>(this PropertyInfo property, Expression<Func<T, object?>> propertySelector) where T : class
    {
        if (property.DeclaringType != typeof(T)) return false;
        
        var unaryExpression = propertySelector.Body as UnaryExpression;

        if (unaryExpression?.Operand is not MemberExpression operand) return false;

        var parameterPath = operand.ToString();

        parameterPath = propertySelector.Parameters
                                        .Aggregate(parameterPath, (current, parameterExpression) => $".{current}.".Replace($".{parameterExpression.Name}.", "", StringComparison.CurrentCultureIgnoreCase))
                                        .Trim('.');

        return property.Name.Equals(parameterPath, StringComparison.CurrentCultureIgnoreCase);
    }
}