using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Netcorext.Extensions.Swagger.Helpers;

public static class CustomIdHelper
{
    private static Dictionary<string, string> Naming = new();
    private static readonly Regex RegexIdMatch = new(@"^[a-zA-Z0-9\.\-_]+$", RegexOptions.Compiled);
    private static readonly Regex RegexIdReplace = new(@"[^a-zA-Z0-9\.\-_]", RegexOptions.Compiled);

    public static string ReplaceCustomId(Type type)
    {
        var id = type.ToString();

        if (RegexIdMatch.IsMatch(id)) return id;

        id = RegexIdReplace.Replace(id, "_").Trim('_');

        return id;
    }

    public static string? FriendlyCustomId(Type type)
    {
        if (type.IsAssignableTo(typeof(ITuple))) return null;

        var id = type.Name;

        if (type.IsGenericType)
        {
            id = type.BaseType?.Name ?? id[..^2];

            var args = type.GetGenericArguments()
                           .Select(t => GetName(t));

            id = id + "Of" + args.Aggregate((c, n) => $"{c}_{n}");
        }

        var i = 1;

        var name = id;

        while (!Naming.TryAdd(name, type.ToString()))
        {
            if (Naming[name] == type.ToString()) break;

            i++;

            name = $"{id}{i}";
        }

        id = name;

        return id;
    }

    private static string? GetName(Type type, string? lastName = null)
    {
        var name = type.Name;

        if (type.IsArray) return lastName == null ? $"{name[..^2]}Array" : $"{lastName}Of{name[..^2]}Array";

        if (!type.IsGenericType || type.IsValueType || type == typeof(string)) return lastName == null ? name : $"{lastName}Of{name}";

        var args = type.GetGenericArguments()
                       .Select(t => GetName(t, lastName))
                       .ToArray();

        name = args.Aggregate((c, n) => $"{c}_{n}");

        if (type.IsAssignableTo(typeof(IEnumerable))) name += GetCollectionName(type);

        return lastName == null ? name : $"{lastName}Of{name}";
    }

    private static string GetCollectionName(Type type)
    {
        if (!type.IsAssignableTo(typeof(IEnumerable))) return "";
        if (type.GetGenericTypeDefinition() == typeof(IDictionary<,>) || type.GetGenericTypeDefinition() == typeof(Dictionary<,>)) return "_Map";
        if (type.GetGenericTypeDefinition() == typeof(IList<>) || type.GetGenericTypeDefinition() == typeof(List<>)) return "List";
        if (type.GetGenericTypeDefinition() == typeof(ICollection<>)) return "Collection";
        if (type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) return "Enumerable";

        return "Array";
    }
}