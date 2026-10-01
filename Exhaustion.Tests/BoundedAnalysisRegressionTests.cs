using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Exhaustion.Tests;

#pragma warning disable CA1515
#pragma warning disable SA1600

[TestClass]
public sealed class BoundedAnalysisRegressionTests
{
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
            var hierarchyCompilation = CreateCompilation("Issue146HierarchyMetadata", hierarchy, references);
            AssertCompiles(hierarchyCompilation);

            using var stream = new MemoryStream();
            var emitResult = hierarchyCompilation.Emit(stream);
            Assert.IsTrue(
                emitResult.Success,
                string.Join(Environment.NewLine, emitResult.Diagnostics.Select(diagnostic => diagnostic.ToString()))
            );
            references.Add(MetadataReference.CreateFromImage(stream.ToArray()));
            source = consumer;
        }

        var compilation = CreateCompilation("Issue146BoundedRegression", source, references);
        AssertCompiles(compilation);

        var diagnostics = await compilation.WithAnalyzers(
            [new ExhaustionAnalyzer()]
        ).GetAnalyzerDiagnosticsAsync().ConfigureAwait(false);

        Assert.IsFalse(
            diagnostics.Any(diagnostic => diagnostic.Id == "AD0001"),
            "Issue #146: a valid recursive AST switch must not throw an analyzer exception. "
                + string.Join(Environment.NewLine, diagnostics.Where(diagnostic => diagnostic.Id == "AD0001"))
        );
        Assert.AreEqual(1, diagnostics.Length, "The AST switch must produce one controlled diagnostic.");
        var diagnostic = diagnostics[0];
        Assert.IsTrue(diagnostic.Id.StartsWith("EXHAUSTION", StringComparison.Ordinal));
        Assert.IsTrue(diagnostic.Location.IsInSource, "The diagnostic must identify the affected switch.");
        var message = diagnostic.GetMessage(CultureInfo.InvariantCulture);
        Assert.IsTrue(
            message.Length < 4096,
            $"Issue #146: the analyzer materialized a {message.Length}-character diagnostic from "
                + $"25^3 constructor combinations (metadata={useMetadata}, statement={useSwitchStatement}); "
                + "large-hierarchy diagnostics must remain bounded."
        );
    }

    private static string BuildHierarchy()
    {
        var source = new StringBuilder("""
            namespace Issue146.Types
            {
                public abstract record Expression
                {
                    private Expression() { }
            """);
        for (var index = 0; index < 24; index++)
        {
            _ = source.Append("public sealed record Leaf").Append(index).AppendLine(" : Expression;");
        }

        _ = source.AppendLine("public sealed record Branch(Expression Left, Expression Middle, Expression Right) : Expression;");
        _ = source.AppendLine("} }");
        return source.ToString();
    }

    private static string BuildConsumer(bool useSwitchStatement) => useSwitchStatement
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
        var platformAssemblies = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
            ?? throw new InvalidOperationException("Runtime assembly references are unavailable.");
        return [.. platformAssemblies.Split(Path.PathSeparator)
            .Distinct(StringComparer.Ordinal)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))];
    }

    private static CSharpCompilation CreateCompilation(
        string assemblyName,
        string source,
        List<MetadataReference> references
    ) => CSharpCompilation.Create(
        assemblyName,
        [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp12))],
        references,
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
    );

    private static void AssertCompiles(CSharpCompilation compilation)
    {
        var errors = compilation.GetDiagnostics()
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .ToArray();
        Assert.AreEqual(
            0,
            errors.Length,
            "The fixture must compile before analyzer failures can establish issue #146: "
                + string.Join(Environment.NewLine, errors.Select(diagnostic => diagnostic.ToString()))
        );
    }
}
