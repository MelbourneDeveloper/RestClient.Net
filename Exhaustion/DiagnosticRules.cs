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
    /// Gets the collection of supported diagnostic descriptors.
    /// </summary>
    public static ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(ExhaustionRule);
}
