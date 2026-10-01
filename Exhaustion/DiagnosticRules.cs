using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Exhaustion;

/// <summary>
/// Contains diagnostic rule definitions for the exhaustion analyzer.
/// </summary>
internal static class DiagnosticRules
{
    /// <summary>
    /// Diagnostic ID for exhaustive pattern matching warnings.
    /// </summary>
    public const string DiagnosticId = "EXHAUSTION001";

    /// <summary>
    /// Diagnostic ID when exhaustive coverage cannot be determined within the analysis budget.
    /// </summary>
    public const string AnalysisLimitDiagnosticId = "EXHAUSTION002";

    private const string Title = "Switch expression must be exhaustive for closed type hierarchies";
    private const string MessageFormat = "{0}; {1}";
    private const string Category = "Design";

    /// <summary>
    /// The diagnostic rule for non-exhaustive pattern matching.
    /// </summary>
    public static readonly DiagnosticDescriptor ExhaustionRule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    /// <summary>
    /// The diagnostic rule for a hierarchy that exceeds the supported analysis complexity.
    /// </summary>
    public static readonly DiagnosticDescriptor AnalysisLimitRule = new(
        AnalysisLimitDiagnosticId,
        "Switch exhaustiveness analysis exceeded its complexity limit",
        "Switch exhaustiveness could not be determined because the analysis complexity limit was exceeded",
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    /// <summary>
    /// Gets the collection of supported diagnostic descriptors.
    /// </summary>
    public static ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(ExhaustionRule, AnalysisLimitRule);
}
