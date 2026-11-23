using Microsoft.CodeAnalysis;
using static System.Diagnostics.Debug;

namespace Exhaustion;

/// <summary>
/// Functions for analyzing type hierarchies and determining closed hierarchies.
/// </summary>
internal static class TypeHierarchyAnalysis
{
    /// <summary>
    /// Checks if a type represents a closed hierarchy (sealed set of subtypes).
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is a closed hierarchy, false otherwise.</returns>
    public static bool IsClosedHierarchy(INamedTypeSymbol type)
    {
        WriteLine($"IsClosedHierarchy check for: {type.Name}");

        // Must be abstract or sealed record (records use sealed for their nested types)
        if (!type.IsAbstract && !type.IsRecord)
        {
            WriteLine(
                $"  -> Not abstract and not record: IsAbstract={type.IsAbstract}, IsRecord={type.IsRecord}"
            );
            return false;
        }

        // Check for private constructors only (or protected for records - copy constructor)
        var constructors = type.Constructors.Where(c => !c.IsStatic).ToList();

        var hasPublicConstructor = constructors.Any(c =>
            c.DeclaredAccessibility == Accessibility.Public
        );
        if (hasPublicConstructor)
        {
            WriteLine($"  -> Has public constructor");
            return false;
        }

        // Must have at least one derived type
        var derivedCount = GetImmediateDerivedTypes(type).Count;
        WriteLine($"  -> Derived types count: {derivedCount}");
        return derivedCount > 0;
    }

    /// <summary>
    /// Gets the immediate derived types (direct children) of a base type.
    /// Only looks for nested types within the base type itself.
    /// </summary>
    /// <param name="baseType">The base type to find derived types for.</param>
    /// <returns>List of immediate derived types.</returns>
    public static List<INamedTypeSymbol> GetImmediateDerivedTypes(INamedTypeSymbol baseType)
    {
        var result = new List<INamedTypeSymbol>();

        // Look for nested types within this type itself
        var members = baseType.GetTypeMembers();

        foreach (var member in members)
        {
            if (member.BaseType is INamedTypeSymbol memberBase)
            {
                // Compare the original definitions (unbounded generics)
                var memberBaseOriginal = memberBase.OriginalDefinition;
                var baseTypeOriginal = baseType.OriginalDefinition;

                if (SymbolEqualityComparer.Default.Equals(memberBaseOriginal, baseTypeOriginal))
                {
                    result.Add(member);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Gets the set of type names required for exhaustive matching on a closed hierarchy.
    /// Returns empty set if the type is not a closed hierarchy.
    /// </summary>
    /// <param name="type">The type to analyze.</param>
    /// <returns>Set of required type names for exhaustive pattern matching.</returns>
    public static HashSet<string> GetRequiredTypeNames(ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
        {
            return [];
        }

        // Check if this is a closed hierarchy
        if (!IsClosedHierarchy(namedType))
        {
            return [];
        }

        // Get immediate derived types
        var derivedTypes = GetImmediateDerivedTypes(namedType);

        // Collect type names from derived types
        var result = new HashSet<string>();
        foreach (var derived in derivedTypes)
        {
            // If the parent is a constructed generic type, construct the derived type with the same type arguments
            var instantiatedDerived = derived;
            if (namedType.IsGenericType && !namedType.IsUnboundGenericType)
            {
                // Only try to construct if the derived type is also generic
                if (derived.IsGenericType && derived.Arity > 0)
                {
                    instantiatedDerived = derived.ConstructedFrom.Construct(
                        [.. namedType.TypeArguments]
                    );
                    WriteLine(
                        $"Constructing {derived.Name} with type args from {namedType} -> {instantiatedDerived}"
                    );
                }
                else
                {
                    WriteLine($"Derived type {derived.Name} is not generic, using as-is");
                }
            }

            // Recursively get type names
            TypeNameCollection.GetAllTypeNames(instantiatedDerived, result);
        }

        return result;
    }
}
