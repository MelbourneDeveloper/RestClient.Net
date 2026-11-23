using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<Exhaustion.ExhaustionAnalyzer>;

namespace Exhaustion.Tests;

#pragma warning disable CA1506 // Exercises Roslyn syntax editing, compilation, and diagnostic contracts together.
#pragma warning disable SA1600

internal static class AnalyzerInteractionAssertions
{
    private static readonly ImmutableArray<MetadataReference> RuntimeReferences =
    [
        .. (
            (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
            ?? throw new InvalidOperationException("Runtime assembly references are unavailable.")
        )
            .Split(Path.PathSeparator)
            .Distinct(StringComparer.Ordinal)
            .Select(path => MetadataReference.CreateFromFile(path)),
    ];

    internal static async Task VerifyAsync(string source, params DiagnosticResult[] expected)
    {
        // Preserve the original verifier's exact diagnostics, markup spans, and compiler-error expectations.
        await VerifyCS.VerifyAnalyzerAsync(source, expected).ConfigureAwait(false);
        var unmarkedSource = source
            .Replace("{|#0:", string.Empty, StringComparison.Ordinal)
            .Replace("|}", string.Empty, StringComparison.Ordinal);
        var compilation = CSharpCompilation.Create(
            "AnalyzerInteractionFixture",
            [
                CSharpSyntaxTree.ParseText(
                    unmarkedSource,
                    new CSharpParseOptions(LanguageVersion.CSharp12)
                ),
            ],
            RuntimeReferences,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        var diagnostics = await compilation
            .WithAnalyzers([new ExhaustionAnalyzer()])
            .GetAnalyzerDiagnosticsAsync()
            .ConfigureAwait(false);
        var expectedAnalyzerIds = expected
            .Where(result => result.Id.StartsWith("EXHAUSTION", StringComparison.Ordinal))
            .Select(result => result.Id)
            .Order(StringComparer.Ordinal)
            .ToArray();
        CollectionAssert.AreEqual(
            expectedAnalyzerIds,
            diagnostics.Select(diagnostic => diagnostic.Id).Order(StringComparer.Ordinal).ToArray()
        );
        await AssertEditCycleAsync(compilation, diagnostics).ConfigureAwait(false);
    }

    internal static async Task AssertEditCycleAsync(
        CSharpCompilation compilation,
        IReadOnlyList<Diagnostic> expectedDiagnostics
    )
    {
        var analyzer = new ExhaustionAnalyzer();
        var originalTrees = compilation.SyntaxTrees.ToArray();
        var originalSources = originalTrees.Select(tree => tree.GetText().ToString()).ToArray();
        var originalDiagnostics = await compilation
            .WithAnalyzers([analyzer])
            .GetAnalyzerDiagnosticsAsync()
            .ConfigureAwait(false);
        AssertDiagnostics(compilation, expectedDiagnostics, originalDiagnostics);

        var editedCompilation = compilation;
        var switchesEdited = 0;
        foreach (var tree in originalTrees)
        {
            var root = await tree.GetRootAsync().ConfigureAwait(false);
            var switches = root.DescendantNodes()
                .Where(node => node is SwitchExpressionSyntax or SwitchStatementSyntax)
                .ToArray();
            switchesEdited += switches.Length;
            var editedRoot = root.ReplaceNodes(
                switches,
                (original, _) =>
                    original is SwitchExpressionSyntax
                        ? SyntaxFactory.ParseExpression("default!").WithTriviaFrom(original)
                        : SyntaxFactory
                            .ParseStatement("throw new System.InvalidOperationException();")
                            .WithTriviaFrom(original)
            );
            var editedTree = tree.WithRootAndOptions(editedRoot, tree.Options);
            editedCompilation = editedCompilation.ReplaceSyntaxTree(tree, editedTree);
        }

        Assert.IsTrue(
            switchesEdited > 0,
            "The edit cycle must actually replace at least one switch."
        );
        Assert.IsFalse(
            editedCompilation.SyntaxTrees.Any(tree =>
                tree.GetRoot()
                    .DescendantNodes()
                    .Any(node => node is SwitchExpressionSyntax or SwitchStatementSyntax)
            ),
            "The edited document must contain no switches."
        );
        var editedDiagnostics = await editedCompilation
            .WithAnalyzers([analyzer])
            .GetAnalyzerDiagnosticsAsync()
            .ConfigureAwait(false);
        AssertDiagnostics(editedCompilation, [], editedDiagnostics);

        var restoredCompilation = editedCompilation
            .RemoveAllSyntaxTrees()
            .AddSyntaxTrees(originalTrees);
        var restoredDiagnostics = await restoredCompilation
            .WithAnalyzers([analyzer])
            .GetAnalyzerDiagnosticsAsync()
            .ConfigureAwait(false);
        AssertDiagnostics(restoredCompilation, expectedDiagnostics, restoredDiagnostics);
        CollectionAssert.AreEqual(
            originalTrees,
            compilation.SyntaxTrees.ToArray(),
            "Editing must preserve the original compilation's trees."
        );
        CollectionAssert.AreEqual(
            originalSources,
            compilation.SyntaxTrees.Select(tree => tree.GetText().ToString()).ToArray(),
            "Editing must preserve the original source."
        );
        CollectionAssert.AreEqual(
            originalSources,
            restoredCompilation.SyntaxTrees.Select(tree => tree.GetText().ToString()).ToArray(),
            "Restoring must reproduce the original source exactly."
        );
    }

    private static void AssertDiagnostics(
        CSharpCompilation compilation,
        IReadOnlyList<Diagnostic> expected,
        ImmutableArray<Diagnostic> actual
    )
    {
        Assert.IsFalse(
            actual.Any(diagnostic => diagnostic.Id == "AD0001"),
            string.Join(Environment.NewLine, actual)
        );
        Assert.AreEqual(
            expected.Count,
            actual.Length,
            "Each edit must produce exactly the expected analyzer diagnostics."
        );
        var orderedExpected = expected
            .OrderBy(diagnostic => diagnostic.Location.SourceSpan.Start)
            .ThenBy(diagnostic => diagnostic.Id, StringComparer.Ordinal)
            .ToArray();
        var orderedActual = actual
            .OrderBy(diagnostic => diagnostic.Location.SourceSpan.Start)
            .ThenBy(diagnostic => diagnostic.Id, StringComparer.Ordinal)
            .ToArray();
        for (var index = 0; index < orderedExpected.Length; index++)
        {
            var wanted = orderedExpected[index];
            var diagnostic = orderedActual[index];
            Assert.AreEqual(wanted.Id, diagnostic.Id);
            Assert.AreEqual(DiagnosticSeverity.Warning, diagnostic.Severity);
            Assert.AreEqual(wanted.Severity, diagnostic.Severity);
            Assert.AreEqual(wanted.WarningLevel, diagnostic.WarningLevel);
            Assert.AreEqual(wanted.IsSuppressed, diagnostic.IsSuppressed);
            Assert.AreEqual(
                wanted.GetMessage(CultureInfo.InvariantCulture),
                diagnostic.GetMessage(CultureInfo.InvariantCulture)
            );
            Assert.AreEqual(wanted.Location.SourceSpan, diagnostic.Location.SourceSpan);
            Assert.AreEqual(wanted.Location.GetLineSpan(), diagnostic.Location.GetLineSpan());
            Assert.IsTrue(diagnostic.Location.IsInSource);
            Assert.IsTrue(compilation.SyntaxTrees.Contains(diagnostic.Location.SourceTree));
            var diagnosedNode = diagnostic
                .Location.SourceTree.GetRoot()
                .FindNode(diagnostic.Location.SourceSpan);
            Assert.IsTrue(
                diagnosedNode is SwitchExpressionSyntax or SwitchStatementSyntax,
                "Diagnostics must identify an entire switch."
            );
            Assert.AreEqual(diagnosedNode.Span, diagnostic.Location.SourceSpan);
            Assert.IsTrue(diagnostic.Descriptor.IsEnabledByDefault);
            Assert.AreEqual("Design", diagnostic.Descriptor.Category);
        }
    }
}
