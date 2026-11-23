using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Exhaustion.Tests;

#pragma warning disable CA1515
#pragma warning disable SA1600
#pragma warning disable CA1506

[TestClass]
public sealed class BoundedAnalysisRegressionTests
{
    private static readonly int[] ThreeParameterIndices = [0, 1, 2];
    private static readonly string[] ChoiceVariantNames = ["A", "B"];
    private static readonly string[] FirstParameterCombinations =
    [
        "Branch with A",
        "Branch with B",
    ];

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public async Task BroadRecursiveHierarchy_ReportsBoundedDiagnostic(
        bool useMetadata,
        bool useSwitchStatement
    )
    {
        // Exactly 25 leaf variants and three hierarchy-valued parameters: 25^3 combinations.
        // This is large enough to expose issue #146 without an uncontrolled OOM reproduction.
        var hierarchy = BuildHierarchy();
        var references = GetPlatformReferences();
        var consumer = BuildConsumer(useSwitchStatement);
        var source = hierarchy + consumer;

        if (useMetadata)
        {
            var hierarchyCompilation = CreateCompilation(
                "Issue146HierarchyMetadata",
                hierarchy,
                references
            );
            AssertCompiles(hierarchyCompilation);

            using var stream = new MemoryStream();
            var emitResult = hierarchyCompilation.Emit(stream);
            Assert.IsTrue(
                emitResult.Success,
                string.Join(
                    Environment.NewLine,
                    emitResult.Diagnostics.Select(diagnostic => diagnostic.ToString())
                )
            );
            references.Add(MetadataReference.CreateFromImage(stream.ToArray()));
            source = consumer;
        }

        var compilation = CreateCompilation("Issue146BoundedRegression", source, references);
        AssertCompiles(compilation);

        var diagnostics = await compilation
            .WithAnalyzers([new ExhaustionAnalyzer()])
            .GetAnalyzerDiagnosticsAsync()
            .ConfigureAwait(false);

        Assert.IsFalse(
            diagnostics.Any(diagnostic => diagnostic.Id == "AD0001"),
            "Issue #146: a valid recursive AST switch must not throw an analyzer exception. "
                + string.Join(
                    Environment.NewLine,
                    diagnostics.Where(diagnostic => diagnostic.Id == "AD0001")
                )
        );
        Assert.AreEqual(
            1,
            diagnostics.Length,
            "The AST switch must produce one controlled diagnostic."
        );
        var diagnostic = diagnostics[0];
        Assert.AreEqual("EXHAUSTION002", diagnostic.Id);
        Assert.AreEqual(DiagnosticSeverity.Warning, diagnostic.Severity);
        Assert.IsTrue(
            diagnostic.Location.IsInSource,
            "The diagnostic must identify the affected switch."
        );
        var message = diagnostic.GetMessage(CultureInfo.InvariantCulture);
        Assert.IsTrue(
            message.Length < 4096,
            $"Issue #146: the analyzer materialized a {message.Length}-character diagnostic from "
                + $"25^3 constructor combinations (metadata={useMetadata}, statement={useSwitchStatement}); "
                + "large-hierarchy diagnostics must remain bounded."
        );
        Assert.AreEqual(DiagnosticRules.AnalysisLimitRule, diagnostic.Descriptor);
        StringAssert.Contains(
            message,
            "could not be determined",
            "A bounded diagnostic must disclose incomplete analysis.",
            StringComparison.Ordinal
        );
        await AnalyzerInteractionAssertions
            .AssertEditCycleAsync(compilation, diagnostics)
            .ConfigureAwait(false);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void CancellationAfterHierarchyTraversal_StopsEveryAnalysisEntryPoint(
        bool useSwitchStatement
    )
    {
        var compilation = CreateCompilation(
            "CancelledAnalysis",
            BuildHierarchy() + BuildConsumer(useSwitchStatement),
            GetPlatformReferences()
        );
        AssertCompiles(compilation);
        var type = compilation.GetTypeByMetadataName("Issue146.Types.Expression")!;
        var branch = type.GetTypeMembers("Branch").Single();
        using var cancellation = new CancellationTokenSource();
        var budget = new AnalysisBudget(cancellation.Token);
        var leaves = TypeNameCollection.GetAllLeafTypes(type, budget);
        Assert.AreEqual(25, leaves.Count);
        Assert.AreEqual(25, leaves.Distinct(SymbolEqualityComparer.Default).Count());
        Assert.IsTrue(
            leaves.All(leaf => SymbolEqualityComparer.Default.Equals(type, leaf.BaseType))
        );
        var parameterHierarchies = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(
            branch,
            budget
        );
        CollectionAssert.AreEqual(
            ThreeParameterIndices,
            parameterHierarchies.Select(parameter => parameter.Index).ToArray()
        );
        Assert.IsTrue(
            parameterHierarchies.All(parameter => parameter.Variants.Count == leaves.Count)
        );
        Assert.IsTrue(
            parameterHierarchies.All(parameter =>
                parameter.Variants.All(variant =>
                    leaves.Contains(variant, SymbolEqualityComparer.Default)
                )
            )
        );
        cancellation.Cancel();

        AssertCancellation(() => TypeHierarchyAnalysis.GetRequiredTypeNames(type, budget), budget);
        AssertCancellation(() => TypeNameCollection.GetAllLeafTypes(type, budget), budget);
        AssertCancellation(() => TypeNameCollection.GetAllTypeNames(type, [], budget), budget);
        AssertCancellation(
            () => ConstructorParameterAnalysis.GetConstructorParameterHierarchies(branch, budget),
            budget
        );
        AssertCancellation(() => DisplayNameGeneration.GetDisplayName(type, budget), budget);
        AssertCancellation(
            () => DisplayNameGeneration.GetDisplayNameWithParameters(branch, [], budget),
            budget
        );
        var result = new HashSet<string>();
        AssertCancellation(
            () =>
                ConstructorParameterAnalysis.ExpandParameterCombinations(
                    branch,
                    parameterHierarchies,
                    0,
                    [],
                    result,
                    budget
                ),
            budget
        );
        Assert.AreEqual(
            0,
            result.Count,
            "Cancellation must be observed before any constructor product is materialized."
        );

        var tree = compilation.SyntaxTrees.Single();
        var model = compilation.GetSemanticModel(tree);
        if (useSwitchStatement)
        {
            var statement = tree.GetRoot()
                .DescendantNodes()
                .OfType<SwitchStatementSyntax>()
                .Single();
            AssertCancellation(
                () => PatternAnalysis.GetMatchedTypesFromStatement(statement, model, budget),
                budget
            );
        }
        else
        {
            var expression = tree.GetRoot()
                .DescendantNodes()
                .OfType<SwitchExpressionSyntax>()
                .Single();
            AssertCancellation(
                () => PatternAnalysis.GetMatchedTypes(expression, model, budget),
                budget
            );
        }

        var resumedLeaves = TypeNameCollection.GetAllLeafTypes(type, new AnalysisBudget());
        CollectionAssert.AreEqual(
            leaves,
            resumedLeaves,
            "A canceled analysis must not poison later analysis of the same symbols."
        );
        Assert.AreNotSame(leaves, resumedLeaves);
        Assert.AreEqual(
            3,
            ConstructorParameterAnalysis.GetConstructorParameterHierarchies(branch).Count
        );
        AssertCompiles(compilation);
    }

    [TestMethod]
    public async Task OversizedSwitch_DoesNotSuppressOrdinarySwitchInSameCompilation()
    {
        var source =
            BuildHierarchy()
            + BuildConsumer(false)
            + """
                public abstract record Tiny
                {
                    private Tiny() { }
                    public sealed record One : Tiny;
                    public sealed record Two : Tiny;
                }
                public static class TinyConsumer
                {
                    public static int Evaluate(Tiny value) => value switch
                    {
                        Tiny.One => 1,
                        _ => 0,
                    };
                }
                """;
        var compilation = CreateCompilation(
            "IndependentSwitchBudgets",
            source,
            GetPlatformReferences()
        );
        AssertCompiles(compilation);
        var diagnostics = await compilation
            .WithAnalyzers([new ExhaustionAnalyzer()])
            .GetAnalyzerDiagnosticsAsync()
            .ConfigureAwait(false);

        Assert.AreEqual(2, diagnostics.Length);
        Assert.AreEqual(1, diagnostics.Count(diagnostic => diagnostic.Id == "EXHAUSTION002"));
        var ordinary = diagnostics.Single(diagnostic => diagnostic.Id == "EXHAUSTION001");
        StringAssert.Contains(
            ordinary.GetMessage(CultureInfo.InvariantCulture),
            "Missing: Two",
            "The small switch must retain its missing-case diagnostic.",
            StringComparison.Ordinal
        );
        Assert.AreEqual(
            "Switch on Tiny is not exhaustive; Matched: One; Missing: Two",
            ordinary.GetMessage(CultureInfo.InvariantCulture)
        );
        Assert.AreEqual(DiagnosticSeverity.Warning, ordinary.Severity);
        Assert.AreEqual(
            2,
            diagnostics.Select(diagnostic => diagnostic.Location.SourceSpan).Distinct().Count()
        );
        Assert.IsTrue(
            diagnostics.All(diagnostic =>
                diagnostic.Location.SourceTree == compilation.SyntaxTrees.Single()
            )
        );
        await AnalyzerInteractionAssertions
            .AssertEditCycleAsync(compilation, diagnostics)
            .ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ManySmallProducts_ExceedAggregateSwitchBudget()
    {
        var source = new StringBuilder(
            """
            public abstract record Choice
            {
                private Choice() { }
                public sealed record A : Choice;
                public sealed record B : Choice;
            }
            public abstract record Root
            {
                private Root() { }
            """
        );
        var parameters = string.Join(
            ", ",
            Enumerable.Range(0, 8).Select(index => $"Choice P{index}")
        );
        // Five individually small products of 256 combinations exceed the per-switch limit.
        for (var index = 0; index < 5; index++)
        {
            _ = source.AppendLine(
                CultureInfo.InvariantCulture,
                $"public sealed record Branch{index}({parameters}) : Root;"
            );
        }
        _ = source.AppendLine(
            "} public static class Consumer { public static int Evaluate(Root root) => root switch { _ => 0 }; }"
        );
        await AssertLimitDiagnosticAsync(source.ToString()).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task OversizedSwitchWithoutFallback_ReportsIncompleteAnalysis()
    {
        var source =
            BuildHierarchy()
            + """
                public static class Consumer
                {
                    public static int Evaluate(Issue146.Types.Expression expression) => expression switch
                    {
                        Issue146.Types.Expression.Leaf0 => 0,
                    };
                }
                """;
        await AssertLimitDiagnosticAsync(source).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task DeepClosedHierarchy_ReportsControlledDiagnostic()
    {
        var source = new StringBuilder("public abstract record Level0 { private Level0() { }");
        for (var index = 1; index <= 70; index++)
        {
            _ = source.AppendLine(
                CultureInfo.InvariantCulture,
                $"public abstract record Level{index} : Level{index - 1} {{ private Level{index}() {{ }}"
            );
        }
        _ = source.AppendLine("public sealed record Leaf : Level70;");
        _ = source.Append('}', 71);
        _ = source.AppendLine(
            "public static class Consumer { public static int Evaluate(Level0 value) => value switch { _ => 0 }; }"
        );
        await AssertLimitDiagnosticAsync(source.ToString()).ConfigureAwait(false);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task DeepGenericArguments_RespectDepthLimitDuringTypeNameFormatting(
        bool includeTypedPattern
    )
    {
        var argument = "int";
        for (var index = 0; index < 70; index++)
        {
            argument = $"Box<{argument}>";
        }
        var typedPattern = includeTypedPattern ? $"Root<{argument}>.Leaf => 1," : string.Empty;
        var source = $$"""
            public sealed record Box<T>(T Value);
            public abstract record Root<T>
            {
                private Root() { }
                public sealed record Leaf : Root<T>;
            }
            public static class Consumer
            {
                public static int Evaluate(Root<{{argument}}> value) => value switch
                {
                    {{typedPattern}}
                    _ => 0,
                };
            }
            """;
        await AssertLimitDiagnosticAsync(source).ConfigureAwait(false);
    }

    [TestMethod]
    [DataRow(10, 1024)]
    [DataRow(11, 0)]
    public void ConstructorProduct_ExpandsAtLimitAndRejectsLargerProductBeforeAllocation(
        int parameterCount,
        int expectedCount
    )
    {
        var parameters = string.Join(
            ", ",
            Enumerable.Range(0, parameterCount).Select(index => $"Choice P{index}")
        );
        var source = $$"""
            public abstract record Choice
            {
                private Choice() { }
                public sealed record A : Choice;
                public sealed record B : Choice;
            }
            public sealed record Branch({{parameters}});
            """;
        var compilation = CreateCompilation("ProductBoundary", source, GetPlatformReferences());
        AssertCompiles(compilation);
        var branch = compilation.GetTypeByMetadataName("Branch")!;
        var budget = new AnalysisBudget();
        var parametersToExpand = ConstructorParameterAnalysis.GetConstructorParameterHierarchies(
            branch,
            budget
        );
        Assert.AreEqual(parameterCount, parametersToExpand.Count);
        Assert.IsTrue(parametersToExpand.All(parameter => parameter.Variants.Count == 2));
        Assert.IsTrue(
            parametersToExpand.All(parameter =>
                parameter.Variants.Select(variant => variant.Name).SequenceEqual(ChoiceVariantNames)
            )
        );
        var result = new HashSet<string>();
        var selections = new Dictionary<int, INamedTypeSymbol>();

        if (expectedCount == 0)
        {
            _ = Assert.ThrowsException<AnalysisLimitExceededException>(
                () =>
                    ConstructorParameterAnalysis.ExpandParameterCombinations(
                        branch,
                        parametersToExpand,
                        0,
                        selections,
                        result,
                        budget
                    )
            );
        }
        else
        {
            ConstructorParameterAnalysis.ExpandParameterCombinations(
                branch,
                parametersToExpand,
                0,
                selections,
                result,
                budget
            );
        }

        Assert.AreEqual(
            expectedCount,
            result.Count,
            "Oversized products must be rejected before adding any partial coverage."
        );
        Assert.AreEqual(0, selections.Count, "Expansion must preserve the caller's selections.");
        Assert.AreEqual(
            AnalysisBudget.MaximumCombinations - expectedCount,
            budget.RemainingCombinations
        );

        if (expectedCount == 0)
        {
            ConstructorParameterAnalysis.ExpandParameterCombinations(
                branch,
                [parametersToExpand[0]],
                0,
                selections,
                result,
                budget
            );
            CollectionAssert.AreEquivalent(FirstParameterCombinations, result.ToArray());
            Assert.AreEqual(AnalysisBudget.MaximumCombinations - 2, budget.RemainingCombinations);
        }
        else
        {
            var completed = result.ToArray();
            _ = Assert.ThrowsException<AnalysisLimitExceededException>(
                () =>
                    ConstructorParameterAnalysis.ExpandParameterCombinations(
                        branch,
                        parametersToExpand,
                        0,
                        selections,
                        result,
                        budget
                    )
            );
            CollectionAssert.AreEquivalent(
                completed,
                result.ToArray(),
                "An exhausted budget must not corrupt completed coverage."
            );
            var repeated = new HashSet<string>();
            ConstructorParameterAnalysis.ExpandParameterCombinations(
                branch,
                parametersToExpand,
                0,
                selections,
                repeated,
                new AnalysisBudget()
            );
            CollectionAssert.AreEquivalent(
                completed,
                repeated.ToArray(),
                "A fresh analysis budget must produce the same complete coverage."
            );
        }

        Assert.AreEqual(0, selections.Count);
        Assert.AreEqual(parameterCount, parametersToExpand.Count);
        AssertCompiles(compilation);
    }

    private static async Task AssertLimitDiagnosticAsync(string source)
    {
        var compilation = CreateCompilation("BoundedAnalysis", source, GetPlatformReferences());
        AssertCompiles(compilation);
        var diagnostics = await compilation
            .WithAnalyzers([new ExhaustionAnalyzer()])
            .GetAnalyzerDiagnosticsAsync()
            .ConfigureAwait(false);

        Assert.AreEqual(1, diagnostics.Length);
        Assert.AreEqual("EXHAUSTION002", diagnostics[0].Id);
        Assert.AreEqual(DiagnosticSeverity.Warning, diagnostics[0].Severity);
        Assert.IsTrue(diagnostics[0].GetMessage(CultureInfo.InvariantCulture).Length < 4096);
        StringAssert.Contains(
            diagnostics[0].GetMessage(CultureInfo.InvariantCulture),
            "could not be determined",
            "The diagnostic must explicitly report incomplete analysis.",
            StringComparison.Ordinal
        );
        Assert.IsTrue(diagnostics[0].Location.IsInSource);
        Assert.AreEqual(DiagnosticRules.AnalysisLimitRule, diagnostics[0].Descriptor);
        Assert.IsFalse(diagnostics[0].IsSuppressed);
        await AnalyzerInteractionAssertions
            .AssertEditCycleAsync(compilation, diagnostics)
            .ConfigureAwait(false);
    }

    private static void AssertCancellation(Action action, AnalysisBudget budget)
    {
        var remaining = budget.RemainingCombinations;
        var exception = Assert.ThrowsException<OperationCanceledException>(action);
        Assert.AreEqual(budget.CancellationToken, exception.CancellationToken);
        Assert.IsTrue(exception.CancellationToken.IsCancellationRequested);
        Assert.AreEqual(
            remaining,
            budget.RemainingCombinations,
            "Canceled actions must not consume coverage capacity."
        );
    }

    private static string BuildHierarchy()
    {
        var source = new StringBuilder(
            """
            namespace Issue146.Types
            {
                public abstract record Expression
                {
                    private Expression() { }
            """
        );
        for (var index = 0; index < 24; index++)
        {
            _ = source
                .Append("public sealed record Leaf")
                .Append(index)
                .AppendLine(" : Expression;");
        }

        _ = source.AppendLine(
            "public sealed record Branch(Expression Left, Expression Middle, Expression Right) : Expression;"
        );
        _ = source.AppendLine("} }");
        return source.ToString();
    }

    private static string BuildConsumer(bool useSwitchStatement) =>
        useSwitchStatement
            ? """
                namespace Issue146.Consumer
                {
                    using Issue146.Types;

                    public static class PredicateReader
                    {
                        public static Expression StripAlias(Expression expression)
                        {
                            switch (expression)
                            {
                                case Expression.Branch branch:
                                    return branch.Left;
                                default:
                                    return expression;
                            }
                        }
                    }
                }
                """
            : """
                namespace Issue146.Consumer
                {
                    using Issue146.Types;

                    public static class PredicateReader
                    {
                        public static Expression StripAlias(Expression expression) => expression switch
                        {
                            Expression.Branch branch => branch.Left,
                            _ => expression,
                        };
                    }
                }
                """;

    private static List<MetadataReference> GetPlatformReferences()
    {
        var platformAssemblies =
            (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
            ?? throw new InvalidOperationException("Runtime assembly references are unavailable.");
        return
        [
            .. platformAssemblies
                .Split(Path.PathSeparator)
                .Distinct(StringComparer.Ordinal)
                .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path)),
        ];
    }

    private static CSharpCompilation CreateCompilation(
        string assemblyName,
        string source,
        List<MetadataReference> references
    ) =>
        CSharpCompilation.Create(
            assemblyName,
            [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp12))],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

    private static void AssertCompiles(CSharpCompilation compilation)
    {
        var errors = compilation
            .GetDiagnostics()
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .ToArray();
        Assert.AreEqual(
            0,
            errors.Length,
            "The fixture must compile before analyzer failures can establish issue #146: "
                + string.Join(
                    Environment.NewLine,
                    errors.Select(diagnostic => diagnostic.ToString())
                )
        );
    }
}
