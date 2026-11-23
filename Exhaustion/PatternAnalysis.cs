using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static System.Diagnostics.Debug;
using static Exhaustion.DiscardDetection;
using static Exhaustion.DisplayNameGeneration;

namespace Exhaustion;

/// <summary>
/// Pure functions for analyzing patterns in switch expressions and statements.
/// </summary>
internal static class PatternAnalysis
{
    /// <summary>
    /// Gets the set of types matched by a switch expression.
    /// </summary>
    /// <param name="switchExpr">The switch expression to analyze.</param>
    /// <param name="model">The semantic model for type resolution.</param>
    /// <returns>Set of matched type names.</returns>
    public static HashSet<string> GetMatchedTypes(
        SwitchExpressionSyntax switchExpr,
        SemanticModel model
    )
    {
        var matchedTypes = new HashSet<string>();

        foreach (var arm in switchExpr.Arms)
        {
            // Skip only TOP-LEVEL discards (like `_ => ...`), not patterns with nested discards
            if (IsTopLevelDiscard(arm.Pattern))
            {
                continue;
            }

            var typeName = GetPatternTypeName(arm.Pattern, model);
            if (typeName != null)
            {
                _ = matchedTypes.Add(typeName);
            }
        }

        return matchedTypes;
    }

    /// <summary>
    /// Gets the set of types matched by a switch statement.
    /// </summary>
    /// <param name="switchStmt">The switch statement to analyze.</param>
    /// <param name="model">The semantic model for type resolution.</param>
    /// <returns>Set of matched type names.</returns>
    public static HashSet<string> GetMatchedTypesFromStatement(
        SwitchStatementSyntax switchStmt,
        SemanticModel model
    )
    {
        var matchedTypes = new HashSet<string>();

        foreach (var section in switchStmt.Sections)
        {
            foreach (var label in section.Labels)
            {
                if (label is CasePatternSwitchLabelSyntax casePattern)
                {
                    // Skip only TOP-LEVEL discards (like `case _:`), not patterns with nested discards
                    if (IsTopLevelDiscard(casePattern.Pattern))
                    {
                        continue;
                    }

                    var typeName = GetPatternTypeName(casePattern.Pattern, model);
                    if (typeName != null)
                    {
                        _ = matchedTypes.Add(typeName);
                    }
                }
            }
        }

        return matchedTypes;
    }

    /// <summary>
    /// Extracts the type name from a pattern, handling various pattern types.
    /// </summary>
    /// <param name="pattern">The pattern to analyze.</param>
    /// <param name="model">The semantic model for type resolution.</param>
    /// <returns>The type name, or null if the pattern doesn't match a specific type.</returns>
    public static string? GetPatternTypeName(PatternSyntax pattern, SemanticModel model)
    {
        WriteLine($"GetPatternTypeName: pattern type = {pattern.GetType().Name}");

        // For constant patterns like type aliases without parentheses (e.g., "OkUnit")
        if (pattern is ConstantPatternSyntax constantPattern)
        {
            WriteLine($"  ConstantPattern: {constantPattern.Expression}");
            var typeInfo = model.GetTypeInfo(constantPattern.Expression);
            var symbolInfo = model.GetSymbolInfo(constantPattern.Expression);
            WriteLine($"  TypeInfo.Type: {typeInfo.Type}, ConvertedType: {typeInfo.ConvertedType}");
            WriteLine($"  SymbolInfo.Symbol: {symbolInfo.Symbol}");

            // Check if this is a type alias (using statement creates a named type symbol)
            if (symbolInfo.Symbol is INamedTypeSymbol aliasType)
            {
                var displayName = GetDisplayName(aliasType);
                WriteLine($"  -> Matched (from Symbol): {displayName}");
                return displayName;
            }

            WriteLine($"  -> Not a named type symbol");
            return null;
        }

        // For declaration patterns like "Ok ok" or "Error error"
        if (pattern is DeclarationPatternSyntax declPattern)
        {
            WriteLine($"  DeclarationPattern: {declPattern.Type}");
            var typeInfo = model.GetTypeInfo(declPattern.Type);
            WriteLine($"  TypeInfo.Type: {typeInfo.Type}");
            if (typeInfo.Type is INamedTypeSymbol namedType)
            {
                var displayName = GetDisplayName(namedType);
                WriteLine($"  -> Matched: {displayName}");
                return displayName;
            }

            WriteLine($"  -> Not a named type");
            return null;
        }

        // For recursive patterns like "Ok(var value)" or "Error(_)" or "ResponseError(var b, var sc, _)"
        if (pattern is RecursivePatternSyntax recursivePattern && recursivePattern.Type != null)
        {
            WriteLine($"  RecursivePattern, Type property: {recursivePattern.Type}");
            var typeInfo = model.GetTypeInfo(recursivePattern);
            WriteLine($"  TypeInfo.Type: {typeInfo.Type}, ConvertedType: {typeInfo.ConvertedType}");

            WriteLine($"  Has explicit Type syntax: {recursivePattern.Type}");
            var explicitTypeInfo = model.GetTypeInfo(recursivePattern.Type);
            var symbolInfo = model.GetSymbolInfo(recursivePattern.Type);
            WriteLine($"  Explicit TypeInfo: {explicitTypeInfo.Type}");
            WriteLine($"  Explicit SymbolInfo: {symbolInfo.Symbol}");

            if (symbolInfo.Symbol is not INamedTypeSymbol outerType)
            {
                WriteLine($"  -> Not a named type symbol");
                return null;
            }

            // Check if there are nested patterns matching specific variants
            var nestedVariants = GetNestedVariants(recursivePattern, model, outerType);

            if (nestedVariants.Count > 0)
            {
                var baseName = GetDisplayName(outerType);
                var variantNames = nestedVariants.Select(GetDisplayName);
                var displayName = $"{baseName} with {string.Join(", ", variantNames)}";
                WriteLine($"  -> Matched with nested: {displayName}");
                return displayName;
            }

            var simpleDisplayName = GetDisplayName(outerType);
            WriteLine($"  -> Matched: {simpleDisplayName}");
            return simpleDisplayName;
        }

        // For type patterns
        if (pattern is TypePatternSyntax typePattern)
        {
            WriteLine($"  TypePattern: {typePattern.Type}");
            var typeInfo = model.GetTypeInfo(typePattern.Type);
            WriteLine($"  TypeInfo.Type: {typeInfo.Type}");
            if (typeInfo.Type is INamedTypeSymbol namedType)
            {
                var displayName = GetDisplayName(namedType);
                WriteLine($"  -> Matched: {displayName}");
                return displayName;
            }

            WriteLine($"  -> Not a named type");
            return null;
        }

        WriteLine($"  -> No match");
        return null;
    }

    /// <summary>
    /// Extracts nested type variants from a recursive pattern's subpatterns.
    /// </summary>
    /// <param name="recursivePattern">The recursive pattern to analyze.</param>
    /// <param name="model">The semantic model for type resolution.</param>
    /// <param name="outerType">The outer type being matched.</param>
    /// <returns>List of nested type variants found in the pattern.</returns>
    public static List<INamedTypeSymbol> GetNestedVariants(
        RecursivePatternSyntax recursivePattern,
        SemanticModel model,
        INamedTypeSymbol outerType
    )
    {
        var result = new List<INamedTypeSymbol>();

        if (recursivePattern.PositionalPatternClause == null)
        {
            return result;
        }

        // Get the constructor parameters for the outer type
        var paramHierarchies = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(
            outerType
        );
        if (paramHierarchies.Count == 0)
        {
            return result;
        }

        // Check each subpattern
        foreach (var subpattern in recursivePattern.PositionalPatternClause.Subpatterns)
        {
            INamedTypeSymbol? nestedType = null;

            // Check if this subpattern matches a specific type (not a discard)
            if (subpattern.Pattern is RecursivePatternSyntax nestedRecursive)
            {
                if (nestedRecursive.Type != null)
                {
                    var nestedSymbolInfo = model.GetSymbolInfo(nestedRecursive.Type);

                    if (nestedSymbolInfo.Symbol is INamedTypeSymbol aliasType)
                    {
                        nestedType = aliasType;
                    }
                }
            }
            // Handle DeclarationPatternSyntax (e.g., "ApiErrorResponse errorResponse")
            else if (subpattern.Pattern is DeclarationPatternSyntax declPattern)
            {
                var declSymbolInfo = model.GetSymbolInfo(declPattern.Type);

                if (declSymbolInfo.Symbol is INamedTypeSymbol aliasType)
                {
                    nestedType = aliasType;
                }
            }
            // Handle ConstantPatternSyntax (e.g., type aliases without variables like "ResponseErrorString")
            else if (subpattern.Pattern is ConstantPatternSyntax constantPattern)
            {
                var symbolInfo = model.GetSymbolInfo(constantPattern.Expression);

                if (symbolInfo.Symbol is INamedTypeSymbol aliasType)
                {
                    nestedType = aliasType;
                }
            }

            if (nestedType != null)
            {
                result.Add(nestedType);
            }
        }

        return result;
    }
}
