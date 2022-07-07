using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Netcorext.Extensions.Swagger.Extensions;

public static class ApiExplorerExtension
{
    public static bool HasParameter<T>(this ApiParameterDescription parameter, Expression<Func<T, object>> propertySelector) where T : class
    {
        if (parameter.ParameterDescriptor.ParameterType != typeof(T)) return false;
        
        var unaryExpression = propertySelector.Body as UnaryExpression;
        var operand = unaryExpression?.Operand as MemberExpression;

        if (operand == null) return false;

        var parameterPath = operand.ToString();

        parameterPath = propertySelector.Parameters
                                        .Aggregate(parameterPath, (current, parameterExpression) => $".{current}.".Replace($".{parameterExpression.Name}.", "", StringComparison.CurrentCultureIgnoreCase))
                                        .Trim('.');

        return parameterPath == parameter.Name;
    }
}