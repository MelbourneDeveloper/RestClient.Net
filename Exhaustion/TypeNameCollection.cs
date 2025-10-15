using Microsoft.CodeAnalysis;
using static System.Diagnostics.Debug;

namespace Exhaustion;

/// <summary>
/// Pure functions for collecting and flattening type names from hierarchies.
/// </summary>
internal static class TypeNameCollection
{
    /// <summary>
    /// Recursively collects all type names from a type hierarchy.
    /// If the type is itself a closed hierarchy, it recursively flattens its children.
    /// If the type is a leaf type, it adds the type name (or all parameter combinations if applicable).
    /// </summary>
    /// <param name="type">The type to collect names from.</param>
    /// <param name="result">The set to accumulate type names into.</param>
    public static void GetAllTypeNames(INamedTypeSymbol type, HashSet<string> result)
    {
        WriteLine($"GetAllTypeNames: {type.Name}, IsRecord={type.IsRecord}");

        // Check if this type itself is a closed hierarchy
        var nestedDerived = TypeHierarchyAnalysis.GetImmediateDerivedTypes(type);

        if (nestedDerived.Count > 0 && TypeHierarchyAnalysis.IsClosedHierarchy(type))
        {
            WriteLine(
                $"  -> Type is itself a closed hierarchy with {nestedDerived.Count} children"
            );
            // This type is a closed hierarchy, so recursively flatten its children
            foreach (var child in nestedDerived)
            {
                GetAllTypeNames(child, result);
            }
        }
        else
        {
            // This is a leaf type - check if it has constructor parameters with closed hierarchies
            var paramHierarchies = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(
                type
            );

            if (paramHierarchies.Count > 0)
            {
                WriteLine(
                    $"  -> Type {type.Name} has {paramHierarchies.Count} constructor params with closed hierarchies"
                );
                // Expand all combinations
                ConstructorParameterAnalysis.ExpandParameterCombinations(
                    type,
                    paramHierarchies,
                    0,
                    [],
                    result
                );
            }
            else
            {
                // Just add the basic display name
                var displayName = DisplayNameGeneration.GetDisplayName(type);
                WriteLine($"  -> Adding leaf type: {displayName}");
                _ = result.Add(displayName);
            }
        }
    }

    /// <summary>
    /// Gets all leaf types from a type hierarchy (types with no further derived types).
    /// </summary>
    /// <param name="type">The type to collect leaf types from.</param>
    /// <returns>List of leaf types.</returns>
    public static List<INamedTypeSymbol> GetAllLeafTypes(INamedTypeSymbol type)
    {
        var result = new List<INamedTypeSymbol>();
        CollectLeafTypes(type, result);
        return result;
    }

    /// <summary>
    /// Recursively collects leaf types (types with no further derived types) from a hierarchy.
    /// </summary>
    /// <param name="type">The type to collect from.</param>
    /// <param name="result">The list to accumulate leaf types into.</param>
    public static void CollectLeafTypes(INamedTypeSymbol type, List<INamedTypeSymbol> result)
    {
        var derived = TypeHierarchyAnalysis.GetImmediateDerivedTypes(type);

        if (derived.Count > 0 && TypeHierarchyAnalysis.IsClosedHierarchy(type))
        {
            foreach (var child in derived)
            {
                CollectLeafTypes(child, result);
            }
        }
        else
        {
            result.Add(type);
        }
    }
}
