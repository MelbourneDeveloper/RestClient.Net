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
    /// <param name="budget">The shared analysis budget.</param>
    /// <returns>List of parameter indices and their type variants.</returns>
    public static List<(
        int Index,
        List<INamedTypeSymbol> Variants
    )> GetConstructorParameterHierarchies(INamedTypeSymbol type, AnalysisBudget? budget = null)
    {
        budget ??= new AnalysisBudget();
        budget.Visit();
        var result = new List<(int, List<INamedTypeSymbol>)>();

        if (!type.IsRecord)
        {
            return result;
        }

        // Get the primary constructor
        IMethodSymbol? primaryCtor = null;
        foreach (var constructor in type.Constructors)
        {
            budget.Visit();
            if (
                !constructor.IsStatic
                && (
                    primaryCtor == null
                    || constructor.Parameters.Length > primaryCtor.Parameters.Length
                )
            )
            {
                primaryCtor = constructor;
            }
        }

        if (primaryCtor == null)
        {
            return result;
        }

        for (var i = 0; i < primaryCtor.Parameters.Length; i++)
        {
            budget.Visit();
            var param = primaryCtor.Parameters[i];
            var paramType = param.Type;

            if (paramType is INamedTypeSymbol namedParamType)
            {
                var variants = TypeNameCollection.GetAllLeafTypes(namedParamType, budget);

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
    /// <param name="budget">The shared analysis budget.</param>
    public static void ExpandParameterCombinations(
        INamedTypeSymbol type,
        List<(int Index, List<INamedTypeSymbol> Variants)> paramHierarchies,
        int currentIndex,
        Dictionary<int, INamedTypeSymbol> selectedVariants,
        HashSet<string> result,
        AnalysisBudget? budget = null
    )
    {
        budget ??= new AnalysisBudget();
        var combinationCount = 1;
        for (var index = currentIndex; index < paramHierarchies.Count; index++)
        {
            budget.Visit();
            var variantCount = paramHierarchies[index].Variants.Count;
            if (variantCount == 0)
            {
                return;
            }

            // Divide before multiplying so even enormous products cannot overflow or allocate.
            if (combinationCount > budget.RemainingCombinations / variantCount)
            {
                throw new AnalysisLimitExceededException();
            }

            combinationCount *= variantCount;
        }

        budget.ReserveCombinations(combinationCount);
        ExpandParameterCombinationsCore(
            type,
            paramHierarchies,
            currentIndex,
            new Dictionary<int, INamedTypeSymbol>(selectedVariants),
            result,
            budget
        );
    }

    private static void ExpandParameterCombinationsCore(
        INamedTypeSymbol type,
        List<(int Index, List<INamedTypeSymbol> Variants)> paramHierarchies,
        int currentIndex,
        Dictionary<int, INamedTypeSymbol> selectedVariants,
        HashSet<string> result,
        AnalysisBudget budget
    )
    {
        budget.Visit();
        budget.CheckDepth(currentIndex);
        if (currentIndex >= paramHierarchies.Count)
        {
            // Base case: all parameters assigned - create display name
            var displayName = DisplayNameGeneration.GetDisplayNameWithParameters(
                type,
                selectedVariants,
                budget
            );
            WriteLine($"    -> Adding expanded type: {displayName}");
            _ = result.Add(displayName);
            return;
        }

        var (paramIndex, variants) = paramHierarchies[currentIndex];

        foreach (var variant in variants)
        {
            selectedVariants[paramIndex] = variant;
            ExpandParameterCombinationsCore(
                type,
                paramHierarchies,
                currentIndex + 1,
                selectedVariants,
                result,
                budget
            );
        }

        _ = selectedVariants.Remove(paramIndex);
    }
}
