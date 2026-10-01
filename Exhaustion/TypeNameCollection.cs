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
    /// <param name="budget">The shared analysis budget.</param>
    /// <param name="depth">The current hierarchy nesting depth.</param>
    public static void GetAllTypeNames(
        INamedTypeSymbol type,
        HashSet<string> result,
        AnalysisBudget? budget = null,
        int depth = 0
    )
    {
        budget ??= new AnalysisBudget();
        budget.Visit();
        budget.CheckDepth(depth);
        WriteLine($"GetAllTypeNames: {type.Name}, IsRecord={type.IsRecord}");

        // Check if this type itself is a closed hierarchy
        var nestedDerived = TypeHierarchyAnalysis.GetImmediateDerivedTypes(type, budget);

        if (nestedDerived.Count > 0 && TypeHierarchyAnalysis.IsClosedHierarchy(type, budget))
        {
            WriteLine(
                $"  -> Type is itself a closed hierarchy with {nestedDerived.Count} children"
            );
            // This type is a closed hierarchy, so recursively flatten its children
            foreach (var child in nestedDerived)
            {
                GetAllTypeNames(child, result, budget, depth + 1);
            }
        }
        else
        {
            // This is a leaf type - check if it has constructor parameters with closed hierarchies
            var paramHierarchies = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(
                type,
                budget
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
                    result,
                    budget
                );
            }
            else
            {
                budget.ReserveCombinations(1);
                // Just add the basic display name
                var displayName = DisplayNameGeneration.GetDisplayName(type, budget);
                WriteLine($"  -> Adding leaf type: {displayName}");
                _ = result.Add(displayName);
            }
        }
    }

    /// <summary>
    /// Gets all leaf types from a type hierarchy (types with no further derived types).
    /// </summary>
    /// <param name="type">The type to collect leaf types from.</param>
    /// <param name="budget">The shared analysis budget.</param>
    /// <returns>List of leaf types.</returns>
    public static List<INamedTypeSymbol> GetAllLeafTypes(INamedTypeSymbol type, AnalysisBudget? budget = null)
    {
        budget ??= new AnalysisBudget();
        var result = new List<INamedTypeSymbol>();
        CollectLeafTypes(type, result, budget);
        return result;
    }

    /// <summary>
    /// Recursively collects leaf types (types with no further derived types) from a hierarchy.
    /// </summary>
    /// <param name="type">The type to collect from.</param>
    /// <param name="result">The list to accumulate leaf types into.</param>
    /// <param name="budget">The shared analysis budget.</param>
    /// <param name="depth">The current hierarchy nesting depth.</param>
    public static void CollectLeafTypes(
        INamedTypeSymbol type,
        List<INamedTypeSymbol> result,
        AnalysisBudget? budget = null,
        int depth = 0
    )
    {
        budget ??= new AnalysisBudget();
        budget.Visit();
        budget.CheckDepth(depth);
        var derived = TypeHierarchyAnalysis.GetImmediateDerivedTypes(type, budget);

        if (derived.Count > 0 && TypeHierarchyAnalysis.IsClosedHierarchy(type, budget))
        {
            foreach (var child in derived)
            {
                CollectLeafTypes(child, result, budget, depth + 1);
            }
        }
        else
        {
            result.Add(type);
        }
    }
}
