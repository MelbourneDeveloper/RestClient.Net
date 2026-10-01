using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Exhaustion.Tests;

#pragma warning disable CA1515
#pragma warning disable SA1600
#pragma warning disable CA1506

[TestClass]
public sealed class LargeHierarchyRegressionTests
{
    private const string IsolatedProcessVariable = "EXHAUSTION_146_ISOLATED_TEST";

    [TestMethod]
    public async Task SqlParserAstSwitches_CompleteWithoutAnalyzerExceptionOrUnboundedDiagnostic()
    {
        // Keep the unfixed analyzer contained even when the whole suite is run without a heap limit.
        if (Environment.GetEnvironmentVariable(IsolatedProcessVariable) != "1")
        {
            await RunInHeapLimitedProcessAsync().ConfigureAwait(false);
            return;
        }

        const string source = """
            using System.Linq;
            using SqlParser.Ast;

            public static class PredicateReader
            {
                private static Expression? GuardedPredicate(Expression? selection) =>
                    selection switch
                    {
                        Expression.UnaryOp { Op: UnaryOperator.Not } not => not.Expression,
                        _ => null,
                    };

                private static bool IsRowAlias(Ident ident) => ident.Value == "row";

                private static Expression StripRowAlias(Expression expression) =>
                    expression switch
                    {
                        Expression.CompoundIdentifier c when c.Idents.Count > 1 && IsRowAlias(c.Idents[0]) =>
                            c with { Idents = [.. c.Idents.Skip(1)] },
                        Expression.BinaryOp binary => binary with
                        {
                            Left = StripRowAlias(binary.Left),
                            Right = StripRowAlias(binary.Right),
                        },
                        Expression.UnaryOp unary => unary with { Expression = StripRowAlias(unary.Expression) },
                        Expression.Nested nested => StripRowAlias(nested.Expression),
                        Expression.IsNull check => check with { Expression = StripRowAlias(check.Expression) },
                        Expression.IsNotNull check => check with { Expression = StripRowAlias(check.Expression) },
                        _ => expression,
                    };
            }
            """;

        await AssertBoundedAnalysisAsync(source, 2).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SqlParserAstSwitchStatement_CompleteWithoutAnalyzerExceptionOrUnboundedDiagnostic()
    {
        if (Environment.GetEnvironmentVariable(IsolatedProcessVariable) != "1")
        {
            await RunInHeapLimitedProcessAsync().ConfigureAwait(false);
            return;
        }

        const string source = """
            using SqlParser.Ast;

            public static class PredicateReader
            {
                public static Expression? Unwrap(Expression? expression)
                {
                    switch (expression)
                    {
                        case Expression.UnaryOp { Op: UnaryOperator.Not } not:
                            return not.Expression;
                        case Expression.Nested nested:
                            return nested.Expression;
                        default:
                            return expression;
                    }
                }
            }
            """;

        await AssertBoundedAnalysisAsync(source, 1).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ConstructorProductBeyondInt64_ReportsControlledDiagnostic()
    {
        if (Environment.GetEnvironmentVariable(IsolatedProcessVariable) != "1")
        {
            await RunInHeapLimitedProcessAsync().ConfigureAwait(false);
            return;
        }

        // 2^64 combinations also exercises overflow-safe product checks. Keep this isolated.
        var parameters = string.Join(", ", Enumerable.Range(0, 64).Select(index => $"Choice P{index}"));
        var source = $$"""
            public abstract record Choice
            {
                private Choice() { }
                public sealed record A : Choice;
                public sealed record B : Choice;
            }
            public abstract record Root
            {
                private Root() { }
                public sealed record Branch({{parameters}}) : Root;
            }
            public static class Consumer
            {
                public static int Evaluate(Root root) => root switch
                {
                    Root.Branch => 1,
                    _ => 0,
                };
            }
            """;

        await AssertBoundedAnalysisAsync(source, 1).ConfigureAwait(false);
    }

    private static async Task AssertBoundedAnalysisAsync(string source, int expectedSwitchCount)
    {
        var platformAssemblies = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
            ?? throw new InvalidOperationException("Runtime assembly references are unavailable.");
        var references = platformAssemblies.Split(Path.PathSeparator)
            .Append(typeof(SqlParser.Ast.Expression).Assembly.Location)
            .Distinct(StringComparer.Ordinal)
            .Select(path => MetadataReference.CreateFromFile(path));
        var compilation = CSharpCompilation.Create(
            "SqlParserIssue146",
            [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp12))],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable)
        );
        var compilerErrors = compilation.GetDiagnostics()
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .ToArray();
        Assert.AreEqual(0, compilerErrors.Length, string.Join(Environment.NewLine, compilerErrors.Select(diagnostic => diagnostic.ToString())));

        var allocatedBeforeAnalysis = GC.GetTotalAllocatedBytes();
        var stopwatch = Stopwatch.StartNew();
        var diagnostics = await compilation.WithAnalyzers(
            [new ExhaustionAnalyzer()]
        ).GetAnalyzerDiagnosticsAsync().ConfigureAwait(false);

        Assert.IsFalse(
            diagnostics.Any(diagnostic => diagnostic.Id == "AD0001"),
            "Issue #146: Exhaustion threw while expanding the SqlParserCS 0.6.5 AST. "
                + string.Join(Environment.NewLine, diagnostics.Select(diagnostic => diagnostic.ToString()))
        );
        stopwatch.Stop();
        var allocatedDuringAnalysis = GC.GetTotalAllocatedBytes() - allocatedBeforeAnalysis;
        Assert.AreEqual(expectedSwitchCount, diagnostics.Length, "Every switch must still be analyzed and produce a controlled diagnostic.");
        Assert.AreEqual(expectedSwitchCount, diagnostics.Select(diagnostic => diagnostic.Location.SourceSpan).Distinct().Count(), "Each switch must have its own diagnostic.");
        Assert.IsTrue(diagnostics.All(diagnostic => diagnostic.Id == "EXHAUSTION002"));
        Assert.IsTrue(diagnostics.All(diagnostic => diagnostic.Severity == DiagnosticSeverity.Warning));
        var syntaxRoot = await compilation.SyntaxTrees.Single().GetRootAsync().ConfigureAwait(false);
        var switchSpans = syntaxRoot.DescendantNodes()
            .Where(node => node is SwitchExpressionSyntax or SwitchStatementSyntax)
            .Select(node => node.Span)
            .ToHashSet();
        Assert.IsTrue(diagnostics.All(diagnostic => diagnostic.Location.IsInSource && switchSpans.Contains(diagnostic.Location.SourceSpan)), "Every diagnostic must identify an actual switch.");
        Assert.IsTrue(diagnostics.All(diagnostic => diagnostic.GetMessage(CultureInfo.InvariantCulture).Length < 4096), "Diagnostics must not materialize the AST constructor Cartesian product.");
        Assert.IsTrue(allocatedDuringAnalysis < 128 * 1024 * 1024, $"Analysis allocated {allocatedDuringAnalysis:N0} bytes, exceeding 128 MiB.");
        Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(20), $"Analysis took {stopwatch.Elapsed}.");
        Console.WriteLine($"Issue #146: analyzed {expectedSwitchCount} switches; allocated {allocatedDuringAnalysis:N0} bytes in {stopwatch.Elapsed}; managed heap {GC.GetTotalMemory(false):N0} bytes.");
    }

    private static async Task RunInHeapLimitedProcessAsync([CallerMemberName] string testMethod = "")
    {
        var startInfo = new ProcessStartInfo("dotnet")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        startInfo.ArgumentList.Add("vstest");
        startInfo.ArgumentList.Add(typeof(LargeHierarchyRegressionTests).Assembly.Location);
        startInfo.ArgumentList.Add($"--TestCaseFilter:FullyQualifiedName=Exhaustion.Tests.LargeHierarchyRegressionTests.{testMethod}");
        startInfo.ArgumentList.Add("--logger:console;verbosity=detailed");
        startInfo.Environment[IsolatedProcessVariable] = "1";
        startInfo.Environment["DOTNET_GCHeapHardLimit"] = "0x20000000";
        startInfo.Environment["DOTNET_PROCESSOR_COUNT"] = "2";

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Could not start the isolated regression process.");
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        try
        {
            await process.WaitForExitAsync(timeout.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            process.Kill(entireProcessTree: true);
            await process.WaitForExitAsync().ConfigureAwait(false);
            Assert.Fail("Issue #146: AST analysis did not complete within 60 seconds; the isolated process tree was terminated.");
        }

        var output = await stdout.ConfigureAwait(false) + await stderr.ConfigureAwait(false);
        Console.WriteLine(output);
        Assert.AreEqual(0, process.ExitCode, "The 512 MiB heap-limited AST regression failed:\n" + output);
        StringAssert.Contains(output, "Issue #146: analyzed", "The child must execute the regression and all of its assertions.", StringComparison.Ordinal);
    }
}
