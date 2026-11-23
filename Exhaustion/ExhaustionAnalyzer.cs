using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Exhaustion.DiagnosticMessages;
using static Exhaustion.DiagnosticRules;
using static Exhaustion.DiscardDetection;
using static Exhaustion.DisplayNameGeneration;
using static Exhaustion.PatternAnalysis;
using static Exhaustion.TypeHierarchyAnalysis;

namespace Exhaustion;

/// <summary>
/// Analyzer that enforces exhaustive pattern matching for closed type hierarchies.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ExhaustionAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Diagnostic ID for exhaustive pattern matching warnings.
    /// </summary>
    public const string DiagnosticId = DiagnosticRules.DiagnosticId;

    /// <summary>
    /// Diagnostic ID when exhaustive coverage cannot be determined within the analysis budget.
    /// </summary>
    public const string AnalysisLimitDiagnosticId = DiagnosticRules.AnalysisLimitDiagnosticId;

    /// <summary>
    /// Gets the collection of diagnostic descriptors supported by this analyzer.
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        DiagnosticRules.SupportedDiagnostics;

    /// <summary>
    /// Initializes the analyzer by registering actions for switch expressions and statements.
    /// </summary>
    /// <param name="context">The analysis context.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(
            AnalyzeSwitch,
            SyntaxKind.SwitchExpression,
            SyntaxKind.SwitchStatement
        );
    }

    private static void AnalyzeSwitch(SyntaxNodeAnalysisContext context)
    {
        var budget = new AnalysisBudget(context.CancellationToken);
        try
        {
            if (context.Node is SwitchExpressionSyntax)
            {
                AnalyzeSwitchExpression(context, budget);
            }
            else
            {
                AnalyzeSwitchStatement(context, budget);
            }
        }
        catch (AnalysisLimitExceededException)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            context.ReportDiagnostic(
                Diagnostic.Create(AnalysisLimitRule, context.Node.GetLocation())
            );
        }
    }

    private static void AnalyzeSwitchExpression(
        SyntaxNodeAnalysisContext context,
        AnalysisBudget budget
    )
    {
        budget.Visit();
        var switchExpr = (SwitchExpressionSyntax)context.Node;
        var model = context.SemanticModel;

        var switchType = model
            .GetTypeInfo(switchExpr.GoverningExpression, budget.CancellationToken)
            .Type!;

        // Check if there's a TOP-LEVEL discard pattern (not nested subpattern discards)
        var hasDiscard = switchExpr.Arms.Any(arm => IsTopLevelDiscard(arm.Pattern));

        // Get types matched by the switch arms (needed for both closed and non-closed hierarchies)
        var matchedTypes = GetMatchedTypes(switchExpr, model, budget);

        // Check if type is closed hierarchy and get required type names
        var requiredTypeNames = GetRequiredTypeNames(switchType, budget);
        if (requiredTypeNames.Count == 0)
        {
            // Not a closed hierarchy, but check for redundant default arm on sealed types
            if (hasDiscard && switchType is INamedTypeSymbol namedSwitchType)
            {
                var switchTypeName = GetDisplayName(namedSwitchType, budget);
                // If the switch type itself is matched, the default is redundant
                if (matchedTypes.Contains(switchTypeName))
                {
                    var (mainMessage, detailMessage) = BuildDetailMessage(
                        switchTypeName,
                        matchedTypes,
                        []
                    );
                    var diagnostic = Diagnostic.Create(
                        ExhaustionRule,
                        switchExpr.GetLocation(),
                        mainMessage,
                        detailMessage
                    );
                    context.ReportDiagnostic(diagnostic);
                }
            }
            return;
        }

        // Find missing types (types in required but not in matched)
        var missingTypes = new HashSet<string>(requiredTypeNames.Except(matchedTypes));

        // Report if there's a discard OR if there are missing types
        if (hasDiscard || missingTypes.Count > 0)
        {
            var baseTypeName = GetUnboundTypeName(switchType);
            var (mainMessage, detailMessage) = BuildDetailMessage(
                baseTypeName,
                matchedTypes,
                missingTypes
            );
            var diagnostic = Diagnostic.Create(
                ExhaustionRule,
                switchExpr.GetLocation(),
                mainMessage,
                detailMessage
            );
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static void AnalyzeSwitchStatement(
        SyntaxNodeAnalysisContext context,
        AnalysisBudget budget
    )
    {
        budget.Visit();
        var switchStmt = (SwitchStatementSyntax)context.Node;
        var model = context.SemanticModel;

        var switchType = model.GetTypeInfo(switchStmt.Expression, budget.CancellationToken).Type!;

        // Check if there's a default or TOP-LEVEL discard case
        var hasDefault = switchStmt.Sections.Any(section =>
            section.Labels.Any(label =>
                label is DefaultSwitchLabelSyntax
                || (
                    label is CasePatternSwitchLabelSyntax casePattern
                    && IsTopLevelDiscard(casePattern.Pattern)
                )
            )
        );

        // Get types matched by the switch statement (needed for both closed and non-closed hierarchies)
        var matchedTypes = GetMatchedTypesFromStatement(switchStmt, model, budget);

        // Check if type is closed hierarchy and get required type names
        var requiredTypeNames = GetRequiredTypeNames(switchType, budget);
        if (requiredTypeNames.Count == 0)
        {
            // Not a closed hierarchy, but check for redundant default arm on sealed types
            if (hasDefault && switchType is INamedTypeSymbol namedSwitchType)
            {
                var switchTypeName = GetDisplayName(namedSwitchType, budget);
                // If the switch type itself is matched, the default is redundant
                if (matchedTypes.Contains(switchTypeName))
                {
                    var (mainMessage, detailMessage) = BuildDetailMessage(
                        switchTypeName,
                        matchedTypes,
                        []
                    );
                    var diagnostic = Diagnostic.Create(
                        ExhaustionRule,
                        switchStmt.GetLocation(),
                        mainMessage,
                        detailMessage
                    );
                    context.ReportDiagnostic(diagnostic);
                }
            }
            return;
        }

        // Find missing types (types in required but not in matched)
        var missingTypes = new HashSet<string>(requiredTypeNames.Except(matchedTypes));

        // Report if there's a default/discard OR if there are missing types
        if (hasDefault || missingTypes.Count > 0)
        {
            var baseTypeName = GetUnboundTypeName(switchType);
            var (mainMessage, detailMessage) = BuildDetailMessage(
                baseTypeName,
                matchedTypes,
                missingTypes
            );
            var diagnostic = Diagnostic.Create(
                ExhaustionRule,
                switchStmt.GetLocation(),
                mainMessage,
                detailMessage
            );
            context.ReportDiagnostic(diagnostic);
        }
    }
}
