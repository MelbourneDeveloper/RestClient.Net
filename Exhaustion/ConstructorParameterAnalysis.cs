using Microsoft.CodeAnalysis;
using static System.Diagnostics.Debug;

namespace Exhaustion;

/// <summary>
/// Pure functions for analyzing constructor parameters and their type hierarchies.
/// </summary>
internal static class ConstructorParameterAnalysis
{
    /// <summary>
    /// Gets information about constructor parameters that have closed type hierarchies.
    /// Returns a list of (parameter index, variants) tuples for parameters with multiple variants.
    /// </summary>
    /// <param name="type">The type to analyze.</param>
    /// <returns>List of parameter indices and their type variants.</returns>
    public static List<(
        int Index,
        List<INamedTypeSymbol> Variants
    )> GetConstructorParameterHierarchies(INamedTypeSymbol type)
    {
        var result = new List<(int, List<INamedTypeSymbol>)>();

        if (!type.IsRecord)
        {
            return result;
        }

        // Get the primary constructor
        var primaryCtor = type
            .Constructors.Where(c => !c.IsStatic)
            .OrderByDescending(c => c.Parameters.Length)
            .First();

        for (var i = 0; i < primaryCtor.Parameters.Length; i++)
        {
            var param = primaryCtor.Parameters[i];
            var paramType = param.Type;

            if (paramType is INamedTypeSymbol namedParamType)
            {
                var variants = TypeNameCollection.GetAllLeafTypes(namedParamType);

                // Only care if there are multiple variants (closed hierarchy)
                if (variants.Count > 1)
                {
                    WriteLine($"    Param {i} ({param.Name}) has {variants.Count} variants");
                    result.Add((i, variants));
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Recursively expands all combinations of parameter variants and adds them to the result set.
    /// This generates the cartesian product of all parameter variant combinations.
    /// </summary>
    /// <param name="type">The type being analyzed.</param>
    /// <param name="paramHierarchies">The parameter hierarchies to expand.</param>
    /// <param name="currentIndex">The current index in the parameter hierarchies list.</param>
    /// <param name="selectedVariants">The currently selected variants for each parameter.</param>
    /// <param name="result">The set to accumulate expanded type names into.</param>
    public static void ExpandParameterCombinations(
        INamedTypeSymbol type,
        List<(int Index, List<INamedTypeSymbol> Variants)> paramHierarchies,
        int currentIndex,
        Dictionary<int, INamedTypeSymbol> selectedVariants,
        HashSet<string> result
    )
    {
        if (currentIndex >= paramHierarchies.Count)
        {
            // Base case: all parameters assigned - create display name
            var displayName = DisplayNameGeneration.GetDisplayNameWithParameters(
                type,
                selectedVariants
            );
            WriteLine($"    -> Adding expanded type: {displayName}");
            _ = result.Add(displayName);
            return;
        }

        var (paramIndex, variants) = paramHierarchies[currentIndex];

        foreach (var variant in variants)
        {
            var newSelectedVariants = new Dictionary<int, INamedTypeSymbol>(selectedVariants)
            {
                [paramIndex] = variant,
            };
            ExpandParameterCombinations(
                type,
                paramHierarchies,
                currentIndex + 1,
                newSelectedVariants,
                result
            );
        }
    }
}
