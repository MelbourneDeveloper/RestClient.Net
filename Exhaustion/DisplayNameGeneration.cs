using Microsoft.CodeAnalysis;
using static System.Diagnostics.Debug;

namespace Exhaustion;

/// <summary>
/// Pure functions for generating display names for types.
/// </summary>
internal static class DisplayNameGeneration
{
    /// <summary>
    /// Gets the display name for a named type symbol, handling generic types and nested types.
    /// </summary>
    /// <param name="type">The type symbol to get the display name for.</param>
    /// <returns>The display name of the type.</returns>
    public static string GetDisplayName(INamedTypeSymbol type)
    {
        WriteLine(
            $"GetDisplayName: {type.Name}, IsGenericType={type.IsGenericType}, Arity={type.Arity}, TypeArguments.Length={type.TypeArguments.Length}, ContainingType={type.ContainingType?.Name}"
        );

        // For nested types in generic parents, we need to check the containing type
        if (type.ContainingType != null && type.ContainingType.IsGenericType)
        {
            WriteLine(
                $"  ContainingType.IsGenericType={type.ContainingType.IsGenericType}, TypeArguments.Length={type.ContainingType.TypeArguments.Length}"
            );
            // This is a nested type like ErrorResponseError in HttpError<TError>
            // Get the type arguments from the containing type
            var typeArgs = string.Join(
                ", ",
                type.ContainingType.TypeArguments.Select(GetTypeArgumentName)
            );
            var displayName = $"{type.Name}<{typeArgs}>";
            WriteLine($"  -> Nested type display name: {displayName}");
            return displayName;
        }

        if (type.IsGenericType)
        {
            var name = type.Name;
            var typeArgs = string.Join(", ", type.TypeArguments.Select(GetTypeArgumentName));
            var displayName = $"{name}<{typeArgs}>";
            WriteLine($"  -> Generic type display name: {displayName}");
            return displayName;
        }

        WriteLine($"  -> Simple type display name: {type.Name}");
        return type.Name;
    }

    /// <summary>
    /// Gets the display name for a type with specific parameter variants.
    /// </summary>
    /// <param name="type">The type to get the display name for.</param>
    /// <param name="selectedVariants">Dictionary mapping parameter indices to selected variant types.</param>
    /// <returns>The display name with parameter variants included.</returns>
    public static string GetDisplayNameWithParameters(
        INamedTypeSymbol type,
        Dictionary<int, INamedTypeSymbol> selectedVariants
    )
    {
        var baseName = GetDisplayName(type);

        if (selectedVariants.Count == 0)
        {
            return baseName;
        }

        var paramNames = selectedVariants.Values.Select(GetDisplayName);
        return $"{baseName} with {string.Join(", ", paramNames)}";
    }

    /// <summary>
    /// Gets the unbound (non-generic) type name, removing generic arity suffixes.
    /// </summary>
    /// <param name="type">The type to get the unbound name for.</param>
    /// <returns>The unbound type name without generic arity suffix.</returns>
    public static string GetUnboundTypeName(ITypeSymbol type)
    {
        if (type is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            var unboundName = namedType.ConstructUnboundGenericType().Name;
            // Remove generic arity suffix like `2
            var backtickIndex = unboundName.IndexOf('`');
            return backtickIndex >= 0 ? unboundName.Substring(0, backtickIndex) : unboundName;
        }
        return type.Name;
    }

    private static string GetTypeArgumentName(ITypeSymbol typeArg)
    {
        if (typeArg is INamedTypeSymbol namedTypeArg && namedTypeArg.IsGenericType)
        {
            var name = namedTypeArg.Name;
            var typeArgs = string.Join(
                ", ",
                namedTypeArg.TypeArguments.Select(GetTypeArgumentName)
            );
            return $"{name}<{typeArgs}>";
        }
        return typeArg.Name;
    }
}
