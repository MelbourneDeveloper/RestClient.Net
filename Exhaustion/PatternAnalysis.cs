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
    /// <param name="budget">The shared analysis budget.</param>
    /// <returns>Set of matched type names.</returns>
    public static HashSet<string> GetMatchedTypes(
        SwitchExpressionSyntax switchExpr,
        SemanticModel model,
        AnalysisBudget? budget = null
    )
    {
        budget ??= new AnalysisBudget();
        budget.Visit();
        var matchedTypes = new HashSet<string>();

        foreach (var arm in switchExpr.Arms)
        {
            budget.Visit();
            // Skip only TOP-LEVEL discards (like `_ => ...`), not patterns with nested discards
            if (IsTopLevelDiscard(arm.Pattern))
            {
                continue;
            }

            var typeName = GetPatternTypeName(arm.Pattern, model, budget);
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
    /// <param name="budget">The shared analysis budget.</param>
    /// <returns>Set of matched type names.</returns>
    public static HashSet<string> GetMatchedTypesFromStatement(
        SwitchStatementSyntax switchStmt,
        SemanticModel model,
        AnalysisBudget? budget = null
    )
    {
        budget ??= new AnalysisBudget();
        budget.Visit();
        var matchedTypes = new HashSet<string>();

        foreach (var section in switchStmt.Sections)
        {
            budget.Visit();
            foreach (var label in section.Labels)
            {
                budget.Visit();
                if (label is CasePatternSwitchLabelSyntax casePattern)
                {
                    // Skip only TOP-LEVEL discards (like `case _:`), not patterns with nested discards
                    if (IsTopLevelDiscard(casePattern.Pattern))
                    {
                        continue;
                    }

                    var typeName = GetPatternTypeName(casePattern.Pattern, model, budget);
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
    /// <param name="budget">The shared analysis budget.</param>
    /// <returns>The type name, or null if the pattern doesn't match a specific type.</returns>
    public static string? GetPatternTypeName(PatternSyntax pattern, SemanticModel model, AnalysisBudget? budget = null)
    {
        budget ??= new AnalysisBudget();
        budget.Visit();
        WriteLine($"GetPatternTypeName: pattern type = {pattern.GetType().Name}");

        // For constant patterns like type aliases without parentheses (e.g., "OkUnit")
        if (pattern is ConstantPatternSyntax constantPattern)
        {
            WriteLine($"  ConstantPattern: {constantPattern.Expression}");
            var typeInfo = model.GetTypeInfo(constantPattern.Expression, budget.CancellationToken);
            var symbolInfo = model.GetSymbolInfo(constantPattern.Expression, budget.CancellationToken);
            WriteLine($"  TypeInfo.Type: {typeInfo.Type}, ConvertedType: {typeInfo.ConvertedType}");
            WriteLine($"  SymbolInfo.Symbol: {symbolInfo.Symbol}");

            // Check if this is a type alias (using statement creates a named type symbol)
            if (symbolInfo.Symbol is INamedTypeSymbol aliasType)
            {
                var displayName = GetDisplayName(aliasType, budget);
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
            var typeInfo = model.GetTypeInfo(declPattern.Type, budget.CancellationToken);
            WriteLine($"  TypeInfo.Type: {typeInfo.Type}");
            if (typeInfo.Type is INamedTypeSymbol namedType)
            {
                var displayName = GetDisplayName(namedType, budget);
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
            var typeInfo = model.GetTypeInfo(recursivePattern, budget.CancellationToken);
            WriteLine($"  TypeInfo.Type: {typeInfo.Type}, ConvertedType: {typeInfo.ConvertedType}");

            WriteLine($"  Has explicit Type syntax: {recursivePattern.Type}");
            var explicitTypeInfo = model.GetTypeInfo(recursivePattern.Type, budget.CancellationToken);
            var symbolInfo = model.GetSymbolInfo(recursivePattern.Type, budget.CancellationToken);
            WriteLine($"  Explicit TypeInfo: {explicitTypeInfo.Type}");
            WriteLine($"  Explicit SymbolInfo: {symbolInfo.Symbol}");

            if (symbolInfo.Symbol is not INamedTypeSymbol outerType)
            {
                WriteLine($"  -> Not a named type symbol");
                return null;
            }

            // Check if there are nested patterns matching specific variants
            var nestedVariants = GetNestedVariants(recursivePattern, model, outerType, budget);

            if (nestedVariants.Count > 0)
            {
                var baseName = GetDisplayName(outerType, budget);
                var variantNames = nestedVariants.Select(variant => GetDisplayName(variant, budget));
                var displayName = $"{baseName} with {string.Join(", ", variantNames)}";
                WriteLine($"  -> Matched with nested: {displayName}");
                return displayName;
            }

            var simpleDisplayName = GetDisplayName(outerType, budget);
            WriteLine($"  -> Matched: {simpleDisplayName}");
            return simpleDisplayName;
        }

        // For type patterns
        if (pattern is TypePatternSyntax typePattern)
        {
            WriteLine($"  TypePattern: {typePattern.Type}");
            var typeInfo = model.GetTypeInfo(typePattern.Type, budget.CancellationToken);
            WriteLine($"  TypeInfo.Type: {typeInfo.Type}");
            if (typeInfo.Type is INamedTypeSymbol namedType)
            {
                var displayName = GetDisplayName(namedType, budget);
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
    /// <param name="budget">The shared analysis budget.</param>
    /// <returns>List of nested type variants found in the pattern.</returns>
    public static List<INamedTypeSymbol> GetNestedVariants(
        RecursivePatternSyntax recursivePattern,
        SemanticModel model,
        INamedTypeSymbol outerType,
        AnalysisBudget? budget = null
    )
    {
        budget ??= new AnalysisBudget();
        budget.Visit();
        var result = new List<INamedTypeSymbol>();

        if (recursivePattern.PositionalPatternClause == null)
        {
            return result;
        }

        // Get the constructor parameters for the outer type
        var paramHierarchies = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(
            outerType,
            budget
        );
        if (paramHierarchies.Count == 0)
        {
            return result;
        }

        // Check each subpattern
        foreach (var subpattern in recursivePattern.PositionalPatternClause.Subpatterns)
        {
            budget.Visit();
            INamedTypeSymbol? nestedType = null;

            // Check if this subpattern matches a specific type (not a discard)
            if (subpattern.Pattern is RecursivePatternSyntax nestedRecursive)
            {
                if (nestedRecursive.Type != null)
                {
                    var nestedSymbolInfo = model.GetSymbolInfo(nestedRecursive.Type, budget.CancellationToken);

                    if (nestedSymbolInfo.Symbol is INamedTypeSymbol aliasType)
                    {
                        nestedType = aliasType;
                    }
                }
            }
            // Handle DeclarationPatternSyntax (e.g., "ApiErrorResponse errorResponse")
            else if (subpattern.Pattern is DeclarationPatternSyntax declPattern)
            {
                var declSymbolInfo = model.GetSymbolInfo(declPattern.Type, budget.CancellationToken);

                if (declSymbolInfo.Symbol is INamedTypeSymbol aliasType)
                {
                    nestedType = aliasType;
                }
            }
            // Handle ConstantPatternSyntax (e.g., type aliases without variables like "ResponseErrorString")
            else if (subpattern.Pattern is ConstantPatternSyntax constantPattern)
            {
                var symbolInfo = model.GetSymbolInfo(constantPattern.Expression, budget.CancellationToken);

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
