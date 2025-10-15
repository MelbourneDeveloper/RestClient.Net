using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<Exhaustion.ExhaustionAnalyzer>;

namespace Exhaustion.Tests;

#pragma warning disable CA1515
#pragma warning disable SA1600

[TestClass]
public sealed class ExhaustionAnalyzerTests
{
    private const string IsExternalInitPolyfill =
        @"
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}
";

    private const string ResultTypeDefinition =
        @"
namespace TestTypes
{
    public abstract partial record Result<TSuccess, TFailure>
    {
        private Result() { }

        public sealed record Ok<TSuccess, TFailure>(TSuccess Value) : Result<TSuccess, TFailure>
        {
            public override string ToString() => $""Ok({Value})"";
        }

        public sealed record Error<TSuccess, TFailure>(TFailure Value) : Result<TSuccess, TFailure>
        {
            public override string ToString() => $""Error({Value})"";
        }
    }
}
";

    private const string HttpErrorTypeDefinition =
        @"
namespace TestTypes
{
    public abstract partial record HttpError<TError>
    {
        private HttpError() { }

        public sealed record ExceptionError(System.Exception Exception) : HttpError<TError>
        {
            public override string ToString() => $""ExceptionError({Exception.Message})"";
        }

        public sealed record ErrorResponseError(TError Body, int StatusCode) : HttpError<TError>
        {
            public override string ToString() => $""ErrorResponseError({StatusCode}: {Body})"";
        }
    }
}
";

    [TestMethod]
    public void DiagnosticRule_IsEnabledByDefault()
    {
        var rule = DiagnosticRules.ExhaustionRule;
        Assert.IsTrue(rule.IsEnabledByDefault, "Diagnostic should be enabled by default");
        Assert.AreEqual(DiagnosticSeverity.Warning, rule.DefaultSeverity);
        Assert.AreEqual("EXHAUSTION001", rule.Id);
    }

    [TestMethod]
    public async Task SwitchExpression_WithDefaultArm_ReportsDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public string Process(Result<int, string> result)
        {
            return {|#0:result switch
            {
                Result<int, string>.Ok<int, string>(var value) => ""success"",
                Result<int, string>.Error<int, string>(var error) => ""failure"",
                _ => throw new System.InvalidOperationException()
            }|};
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result has redundant default arm",
                "Matched: Error<Int32, String>, Ok<Int32, String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchExpression_WithOnlyOkArm_ReportsDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public string Process(Result<int, string> result)
        {
            return {|#0:result switch
            {
                Result<int, string>.Ok<int, string>(var value) => ""success"",
                _ => ""default""
            }|};
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result is not exhaustive",
                "Matched: Ok<Int32, String>; Missing: Error<Int32, String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchExpression_WithOnlyErrorArm_ReportsDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public string Process(Result<int, string> result)
        {
            return {|#0:result switch
            {
                Result<int, string>.Error<int, string>(var error) => ""failure"",
                _ => ""default""
            }|};
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result is not exhaustive",
                "Matched: Error<Int32, String>; Missing: Ok<Int32, String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchExpression_WithOkAndErrorArms_NoDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public string Process(Result<int, string> result)
        {
            return result switch
            {
                Result<int, string>.Ok<int, string>(var value) => ""success"",
                Result<int, string>.Error<int, string>(var error) => ""failure""
            };
        }
    }
}
";

        await VerifyCS.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchStatement_WithDefaultCase_ReportsDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public void Process(Result<int, string> result)
        {
            {|#0:switch (result)
            {
                case Result<int, string>.Ok<int, string>(var value):
                    System.Console.WriteLine(""success"");
                    break;
                case Result<int, string>.Error<int, string>(var error):
                    System.Console.WriteLine(""failure"");
                    break;
                default:
                    throw new System.InvalidOperationException();
            }|}
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result has redundant default arm",
                "Matched: Error<Int32, String>, Ok<Int32, String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchStatement_WithDiscardPattern_ReportsDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public void Process(Result<int, string> result)
        {
            {|#0:switch (result)
            {
                case Result<int, string>.Ok<int, string>(var value):
                    System.Console.WriteLine(""success"");
                    break;
                case Result<int, string>.Error<int, string>(var error):
                    System.Console.WriteLine(""failure"");
                    break;
                case var _:
                    throw new System.InvalidOperationException();
            }|}
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result has redundant default arm",
                "Matched: Error<Int32, String>, Ok<Int32, String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchStatement_WithOkAndErrorCases_NoDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public void Process(Result<int, string> result)
        {
            switch (result)
            {
                case Result<int, string>.Ok<int, string>(var value):
                    System.Console.WriteLine(""success"");
                    break;
                case Result<int, string>.Error<int, string>(var error):
                    System.Console.WriteLine(""failure"");
                    break;
            }
        }
    }
}
";

        await VerifyCS.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchStatement_MissingOkCase_ReportsDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public void Process(Result<int, string> result)
        {
            {|#0:switch (result)
            {
                case Result<int, string>.Error<int, string>(var error):
                    System.Console.WriteLine(""failure"");
                    break;
            }|}
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result is not exhaustive",
                "Matched: Error<Int32, String>; Missing: Ok<Int32, String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchStatement_MissingErrorCase_ReportsDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public void Process(Result<int, string> result)
        {
            {|#0:switch (result)
            {
                case Result<int, string>.Ok<int, string>(var value):
                    System.Console.WriteLine(""success"");
                    break;
            }|}
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result is not exhaustive",
                "Matched: Ok<Int32, String>; Missing: Error<Int32, String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task NonResultType_NoDiagnostic()
    {
        var test =
            @"
namespace TestCode
{
    public class TestClass
    {
        public string Process(int value)
        {
            return value switch
            {
                1 => ""one"",
                _ => ""other""
            };
        }
    }
}
";

        await VerifyCS.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task NestedResultType_WithDefaultArm_ReportsDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public string Process(Result<Result<int, string>, string> result)
        {
            return {|#0:result switch
            {
                Result<Result<int, string>, string>.Ok<Result<int, string>, string>(var innerResult) => ""outer success"",
                Result<Result<int, string>, string>.Error<Result<int, string>, string>(var error) => ""outer failure"",
                _ => throw new System.InvalidOperationException()
            }|};
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result is not exhaustive",
                "Matched: Error<Result<Int32, String>, String>, Ok<Result<Int32, String>, String>; Missing: Ok<Result<Int32, String>, String> with Error<Int32, String>, Ok<Result<Int32, String>, String> with Ok<Int32, String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task NestedClosedHierarchy_HttpErrorAsGenericArg_Missing_ReportsDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + HttpErrorTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public string Process(Result<string, HttpError<string>> result)
        {
            return {|#0:result switch
            {
                Result<string, HttpError<string>>.Ok<string, HttpError<string>>(var value) => ""success"",
                Result<string, HttpError<string>>.Error<string, HttpError<string>>(HttpError<string>.ExceptionError(var ex)) => ""exception"",
                _ => ""default""
            }|};
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result is not exhaustive",
                "Matched: Error<String, HttpError<String>> with ExceptionError<String>, Ok<String, HttpError<String>>; Missing: Error<String, HttpError<String>> with ErrorResponseError<String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task NestedClosedHierarchy_HttpErrorAsGenericArg_AllCases_NoDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + HttpErrorTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public string Process(Result<string, HttpError<string>> result)
        {
            return result switch
            {
                Result<string, HttpError<string>>.Ok<string, HttpError<string>>(var value) => ""success"",
                Result<string, HttpError<string>>.Error<string, HttpError<string>>(HttpError<string>.ExceptionError(var ex)) => ""exception"",
                Result<string, HttpError<string>>.Error<string, HttpError<string>>(HttpError<string>.ErrorResponseError(var body, var code)) => ""error response""
            };
        }
    }
}
";

        await VerifyCS.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task NestedClosedHierarchy_HttpErrorAsGenericArg_WithDiscard_ReportsDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + HttpErrorTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public string Process(Result<string, HttpError<string>> result)
        {
            return {|#0:result switch
            {
                Result<string, HttpError<string>>.Ok<string, HttpError<string>>(var value) => ""success"",
                Result<string, HttpError<string>>.Error<string, HttpError<string>>(HttpError<string>.ExceptionError(var ex)) => ""exception"",
                Result<string, HttpError<string>>.Error<string, HttpError<string>>(HttpError<string>.ErrorResponseError(var body, var code)) => ""error response"",
                _ => ""never""
            }|};
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result has redundant default arm",
                "Matched: Error<String, HttpError<String>> with ErrorResponseError<String>, Error<String, HttpError<String>> with ExceptionError<String>, Ok<String, HttpError<String>>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task NestedClosedHierarchy_SwitchOnHttpError_Missing_ReportsDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + HttpErrorTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public string Process(HttpError<string> error)
        {
            return {|#0:error switch
            {
                HttpError<string>.ExceptionError(var ex) => ""exception"",
                _ => ""default""
            }|};
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on HttpError is not exhaustive",
                "Matched: ExceptionError<String>; Missing: ErrorResponseError<String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task NestedClosedHierarchy_SwitchOnHttpError_AllCases_NoDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + HttpErrorTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public string Process(HttpError<string> error)
        {
            return error switch
            {
                HttpError<string>.ExceptionError(var ex) => ""exception"",
                HttpError<string>.ErrorResponseError(var body, var code) => ""error response""
            };
        }
    }
}
";

        await VerifyCS.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task NestedClosedHierarchy_SwitchOnHttpError_OneArmThrows_NoDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + HttpErrorTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public (string, int) Process(HttpError<string> error)
        {
            return error switch
            {
                HttpError<string>.ErrorResponseError(var body, var code) => (body, code),
                HttpError<string>.ExceptionError(var ex) => throw new System.InvalidOperationException(""Expected error response"", ex)
            };
        }
    }
}
";

        await VerifyCS.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task NestedClosedHierarchy_ResultWithHttpError_AllArms_NoDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + HttpErrorTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public (string, int) Process(Result<string, HttpError<string>> result)
        {
            return result switch
            {
                Result<string, HttpError<string>>.Ok<string, HttpError<string>>(var value) => (value, 0),
                Result<string, HttpError<string>>.Error<string, HttpError<string>>(HttpError<string>.ErrorResponseError(var body, var code)) => (body, code),
                Result<string, HttpError<string>>.Error<string, HttpError<string>>(HttpError<string>.ExceptionError(var ex)) => throw new System.InvalidOperationException(""Expected error response"", ex)
            };
        }
    }
}
";

        await VerifyCS.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task NestedClosedHierarchy_ResultUnitWithHttpError_AllArms_NoDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + HttpErrorTypeDefinition
            + @"
namespace TestTypes
{
    public sealed record Unit
    {
        public static Unit Value { get; } = new Unit();
        private Unit() { }
    }
}
"
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public (string, int) Process(Result<Unit, HttpError<string>> result)
        {
            return result switch
            {
                Result<Unit, HttpError<string>>.Ok<Unit, HttpError<string>>(var value) => (""ok"", 0),
                Result<Unit, HttpError<string>>.Error<Unit, HttpError<string>>(HttpError<string>.ErrorResponseError(var body, var code)) => (body, code),
                Result<Unit, HttpError<string>>.Error<Unit, HttpError<string>>(HttpError<string>.ExceptionError(var ex)) => throw new System.InvalidOperationException(""Expected error response"", ex)
            };
        }
    }
}
";

        await VerifyCS.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task TypeAliases_ResultUnitWithHttpError_MissingArm_ReportsDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + HttpErrorTypeDefinition
            + @"
namespace TestTypes
{
    public sealed record Unit
    {
        public static Unit Value { get; } = new Unit();
        private Unit() { }
    }
}
"
            + @"
namespace TestCode
{
    using TestTypes;
    using OkUnit = TestTypes.Result<TestTypes.Unit, TestTypes.HttpError<string>>.Ok<TestTypes.Unit, TestTypes.HttpError<string>>;
    using ErrorUnit = TestTypes.Result<TestTypes.Unit, TestTypes.HttpError<string>>.Error<TestTypes.Unit, TestTypes.HttpError<string>>;
    using ResponseErrorString = TestTypes.HttpError<string>.ErrorResponseError;
    using ExceptionErrorString = TestTypes.HttpError<string>.ExceptionError;

    public class TestClass
    {
        public (string, int) Process(Result<Unit, HttpError<string>> result)
        {
            return {|#0:result switch
            {
                ErrorUnit(ResponseErrorString(var b, var sc)) => (b, sc),
                ErrorUnit(ExceptionErrorString(var ex)) => throw new System.InvalidOperationException(""Expected error response"", ex)
            }|};
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result is not exhaustive",
                "Matched: Error<Unit, HttpError<String>> with ErrorResponseError<String>, Error<Unit, HttpError<String>> with ExceptionError<String>; Missing: Ok<Unit, HttpError<String>>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task TypeAliases_ResultUnitWithHttpError_AllArms_NoDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + HttpErrorTypeDefinition
            + @"
namespace TestTypes
{
    public sealed record Unit
    {
        public static Unit Value { get; } = new Unit();
        private Unit() { }
    }
}
"
            + @"
namespace TestCode
{
    using TestTypes;
    using OkUnit = TestTypes.Result<TestTypes.Unit, TestTypes.HttpError<string>>.Ok<TestTypes.Unit, TestTypes.HttpError<string>>;
    using ErrorUnit = TestTypes.Result<TestTypes.Unit, TestTypes.HttpError<string>>.Error<TestTypes.Unit, TestTypes.HttpError<string>>;
    using ResponseErrorString = TestTypes.HttpError<string>.ErrorResponseError;
    using ExceptionErrorString = TestTypes.HttpError<string>.ExceptionError;

    public class TestClass
    {
        public (string, int) Process(Result<Unit, HttpError<string>> result)
        {
            return result switch
            {
                OkUnit => throw new System.InvalidOperationException(""Expected error result""),
                ErrorUnit(ResponseErrorString(var b, var sc)) => (b, sc),
                ErrorUnit(ExceptionErrorString(var ex)) => throw new System.InvalidOperationException(""Expected error response"", ex)
            };
        }
    }
}
";

        await VerifyCS.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ObjectCatchAll_ReportsDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + HttpErrorTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;
    using ResponseError = TestTypes.HttpError<string>.ErrorResponseError;
    using ExceptionError = TestTypes.HttpError<string>.ExceptionError;

    public class TestClass
    {
        public string Process(HttpError<string> error)
        {
            return {|#0:error switch
            {
                ResponseError(var b, var sc) => b,
                object x => throw new System.InvalidOperationException(""Unexpected type"")
            }|};
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on HttpError is not exhaustive",
                "Matched: ErrorResponseError<String>; Missing: ExceptionError<String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task NestedPattern_DiscardOnClosedHierarchy_ReportsDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + HttpErrorTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;
    using OkStringAndHttpError = TestTypes.Result<string, TestTypes.HttpError<string>>.Ok<string, TestTypes.HttpError<string>>;
    using ErrorStringAndHttpError = TestTypes.Result<string, TestTypes.HttpError<string>>.Error<string, TestTypes.HttpError<string>>;

    public class TestClass
    {
        public string Process(Result<string, HttpError<string>> result)
        {
            return {|#0:result switch
            {
                OkStringAndHttpError(_) => ""ok"",
                ErrorStringAndHttpError(_) => ""error""
            }|};
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result is not exhaustive",
                "Matched: Error<String, HttpError<String>>, Ok<String, HttpError<String>>; Missing: Error<String, HttpError<String>> with ErrorResponseError<String>, Error<String, HttpError<String>> with ExceptionError<String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task NestedPattern_MissingNestedVariant_ReportsDiagnostic()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + HttpErrorTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;
    using OkStringAndHttpError = TestTypes.Result<string, TestTypes.HttpError<string>>.Ok<string, TestTypes.HttpError<string>>;
    using ErrorStringAndHttpError = TestTypes.Result<string, TestTypes.HttpError<string>>.Error<string, TestTypes.HttpError<string>>;
    using ResponseError = TestTypes.HttpError<string>.ErrorResponseError;
    using ExceptionError = TestTypes.HttpError<string>.ExceptionError;

    public class TestClass
    {
        public (string, int) Process(Result<string, HttpError<string>> result)
        {
            var (body, statusCode) = {|#0:result switch
            {
                OkStringAndHttpError(_) => throw new System.InvalidOperationException(""Expected error result""),
                ErrorStringAndHttpError(ResponseError(var b, var sc)) => (b, sc),
            }|};
            return (body.ToString(), statusCode);
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result is not exhaustive",
                "Matched: Error<String, HttpError<String>> with ErrorResponseError<String>, Ok<String, HttpError<String>>; Missing: Error<String, HttpError<String>> with ExceptionError<String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task TypeAliasInPositionalPattern_WithoutDestructuring_ShouldRecognizeNestedVariant()
    {
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + HttpErrorTypeDefinition
            + @"
namespace TestTypes
{
    public sealed record Unit
    {
        public static Unit Value { get; } = new Unit();
        private Unit() { }
    }
}
"
            + @"
namespace TestCode
{
    using TestTypes;
    using OkUnit = TestTypes.Result<TestTypes.Unit, TestTypes.HttpError<string>>.Ok<TestTypes.Unit, TestTypes.HttpError<string>>;
    using ErrorUnit = TestTypes.Result<TestTypes.Unit, TestTypes.HttpError<string>>.Error<TestTypes.Unit, TestTypes.HttpError<string>>;
    using ResponseErrorString = TestTypes.HttpError<string>.ErrorResponseError;
    using ExceptionErrorString = TestTypes.HttpError<string>.ExceptionError;

    public class TestClass
    {
        public Unit Process(Result<Unit, HttpError<string>> result)
        {
            return result switch
            {
                OkUnit(var value) => value,
                ErrorUnit(ResponseErrorString) => throw new System.InvalidOperationException(""Expected success result""),
                ErrorUnit(ExceptionErrorString) => throw new System.InvalidOperationException(""Expected success result"")
            };
        }
    }
}
";

        // This should NOT report a diagnostic because all cases are covered
        await VerifyCS.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchStatement_OnNonClosedHierarchyType_NoDiagnostic()
    {
        // Covers: requiredTypeNames.Count == 0 early return in AnalyzeSwitchStatement
        var test =
            @"
namespace TestCode
{
    public class TestClass
    {
        public string Process(int value)
        {
            switch (value)
            {
                case 1:
                    return ""one"";
                case 2:
                    return ""two"";
                default:
                    return ""other"";
            }
        }
    }
}
";

        await VerifyCS.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchStatement_WithStringType_NoDiagnostic()
    {
        // Covers: requiredTypeNames.Count == 0 for string type
        var test =
            @"
namespace TestCode
{
    public class TestClass
    {
        public int Process(string value)
        {
            switch (value)
            {
                case ""foo"":
                    return 1;
                case ""bar"":
                    return 2;
                default:
                    return 0;
            }
        }
    }
}
";

        await VerifyCS.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchStatement_WithDefaultCase_Covered()
    {
        // Explicitly covers: hasDefault branch with DefaultSwitchLabelSyntax
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public string Process(Result<int, string> result)
        {
            {|#0:switch (result)
            {
                case Result<int, string>.Ok<int, string>(var value):
                    return ""ok"";
                default:
                    return ""default"";
            }|}
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result is not exhaustive",
                "Matched: Ok<Int32, String>; Missing: Error<Int32, String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchStatement_WithVarDiscardCase_Covered()
    {
        // Explicitly covers: hasDefault branch with IsTopLevelDiscard check
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public string Process(Result<int, string> result)
        {
            {|#0:switch (result)
            {
                case Result<int, string>.Ok<int, string>(var value):
                    return ""ok"";
                case var _:
                    return ""catch-all"";
            }|}
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result is not exhaustive",
                "Matched: Ok<Int32, String>; Missing: Error<Int32, String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchStatement_WithOnlyDefaultLabel_Covered()
    {
        // Explicitly covers: hasDefault branch checking DefaultSwitchLabelSyntax
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public void Process(Result<int, string> result)
        {
            {|#0:switch (result)
            {
                case Result<int, string>.Error<int, string> e:
                    System.Console.WriteLine(""error"");
                    break;
                default:
                    System.Console.WriteLine(""default"");
                    break;
            }|}
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result is not exhaustive",
                "Matched: Error<Int32, String>; Missing: Ok<Int32, String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchStatement_AllTypesMatchedButHasDefault_StillReportsDiagnostic()
    {
        // Proves hasDefault logic: ALL types matched, but default present
        // This tests: hasDefault = true, missingTypes.Count = 0
        // Diagnostic triggered ONLY by hasDefault, not by missing types
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public void Process(Result<int, string> result)
        {
            {|#0:switch (result)
            {
                case Result<int, string>.Ok<int, string>(var value):
                    System.Console.WriteLine(""ok"");
                    break;
                case Result<int, string>.Error<int, string>(var error):
                    System.Console.WriteLine(""error"");
                    break;
                default:
                    System.Console.WriteLine(""unreachable"");
                    break;
            }|}
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result has redundant default arm",
                "Matched: Error<Int32, String>, Ok<Int32, String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchStatement_AllTypesMatchedButHasVarDiscard_StillReportsDiagnostic()
    {
        // Proves hasDefault logic: ALL types matched, but 'case var _:' present
        // This tests: hasDefault = true (via CasePatternSwitchLabelSyntax + IsTopLevelDiscard), missingTypes.Count = 0
        // Diagnostic triggered ONLY by hasDefault, not by missing types
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public void Process(Result<int, string> result)
        {
            {|#0:switch (result)
            {
                case Result<int, string>.Ok<int, string>(var value):
                    System.Console.WriteLine(""ok"");
                    break;
                case Result<int, string>.Error<int, string>(var error):
                    System.Console.WriteLine(""error"");
                    break;
                case var _:
                    System.Console.WriteLine(""unreachable"");
                    break;
            }|}
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result has redundant default arm",
                "Matched: Error<Int32, String>, Ok<Int32, String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchStatement_OnArrayType_NoDiagnostic()
    {
        // Forces: type is not INamedTypeSymbol (array type)
        // Arrays are not named types, so GetRequiredTypeNames returns empty
        var test =
            @"
namespace TestCode
{
    public class TestClass
    {
        public string Process(int[] values)
        {
            switch (values)
            {
                case [1, 2, 3]:
                    return ""match"";
                default:
                    return ""default"";
            }
        }
    }
}
";

        await VerifyCS.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchExpression_OnTypeParameter_NoDiagnostic()
    {
        // Forces: type is not INamedTypeSymbol (type parameter)
        // Type parameters are not named types, so GetRequiredTypeNames returns empty
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public string Process<T>(T value)
        {
            return value switch
            {
                null => ""null"",
                _ => ""not null""
            };
        }
    }
}
";

        await VerifyCS.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchExpression_OnAbstractRecordWithOnlyStaticConstructor_NoDiagnostic()
    {
        // Forces: constructors.Count == 0 (abstract record with only static constructor)
        // This type is not a closed hierarchy because it has no instance constructors
        var test =
            IsExternalInitPolyfill
            + @"
namespace TestCode
{
    public abstract record AbstractType
    {
        static AbstractType() { }
    }

    public class TestClass
    {
        public string Process(AbstractType value)
        {
            return value switch
            {
                null => ""null"",
                _ => ""not null""
            };
        }
    }
}
";

        await VerifyCS.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchExpression_OnSealedType_WithDefaultArm_ReportsDiagnostic()
    {
        // Tests: Sealed type (leaf node) with explicit match + default arm
        // The default arm is redundant because the sealed type is already matched
        var test =
            IsExternalInitPolyfill
            + HttpErrorTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;
    using ResponseErrorString = TestTypes.HttpError<string>.ErrorResponseError;

    public class TestClass
    {
        public (string, int) Process(HttpError<string>.ErrorResponseError error)
        {
            return {|#0:error switch
            {
                ResponseErrorString(var b, var sc) => (b, sc),
                _ => throw new System.InvalidOperationException(""Expected ErrorResponseError"")
            }|};
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on ErrorResponseError<String> has redundant default arm",
                "Matched: ErrorResponseError<String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchStatement_OnSealedType_WithDefaultCase_ReportsDiagnostic()
    {
        // Tests: Sealed type (leaf node) with explicit match + default case in switch statement
        // The default case is redundant because the sealed type is already matched
        var test =
            IsExternalInitPolyfill
            + HttpErrorTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;
    using ResponseErrorString = TestTypes.HttpError<string>.ErrorResponseError;

    public class TestClass
    {
        public void Process(HttpError<string>.ErrorResponseError error)
        {
            {|#0:switch (error)
            {
                case ResponseErrorString(var b, var sc):
                    System.Console.WriteLine($""{b}: {sc}"");
                    break;
                default:
                    throw new System.InvalidOperationException(""Expected ErrorResponseError"");
            }|}
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on ErrorResponseError<String> has redundant default arm",
                "Matched: ErrorResponseError<String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task ClosedHierarchy_TwoTypes_WithRedundantDefaultArm_ReportsDiagnostic()
    {
        // CRITICAL TEST: Proves that a redundant default arm is ILLEGAL when all types in a closed hierarchy are matched
        // This mimics the Match method pattern: when both Ok and Error are matched, the _ arm is redundant and should error
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public TResult Match<TResult>(
            Result<int, string> result,
            System.Func<int, TResult> onSuccess,
            System.Func<string, TResult> onError)
        {
            return {|#0:result switch
            {
                Result<int, string>.Ok<int, string>(var value) => onSuccess(value),
                Result<int, string>.Error<int, string>(var error) => onError(error),
                _ => throw new System.InvalidOperationException(""Result hierarchy violation"")
            }|};
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result has redundant default arm",
                "Matched: Error<Int32, String>, Ok<Int32, String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task SwitchStatement_NestedHierarchy_WithTypeAliases_MissingNestedCase_ReportsDiagnostic()
    {
        // Switch STATEMENT (not expression) on Result<T, HttpError<E>>
        // With type aliases (OkPosts, ErrorPosts, ExceptionErrorString, ResponseErrorString)
        // Only 2 cases: Ok + Error(ExceptionError)
        // Missing: Error(ErrorResponseError)
        // NO default case
        // This SHOULD trigger a diagnostic but
        var test =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + HttpErrorTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;
    using OkPosts = TestTypes.Result<System.Collections.Generic.List<string>, TestTypes.HttpError<string>>.Ok<System.Collections.Generic.List<string>, TestTypes.HttpError<string>>;
    using ErrorPosts = TestTypes.Result<System.Collections.Generic.List<string>, TestTypes.HttpError<string>>.Error<System.Collections.Generic.List<string>, TestTypes.HttpError<string>>;
    using ExceptionErrorString = TestTypes.HttpError<string>.ExceptionError;
    using ResponseErrorString = TestTypes.HttpError<string>.ErrorResponseError;

    public class TestClass
    {
        public void Process(Result<System.Collections.Generic.List<string>, HttpError<string>> result)
        {
            {|#0:switch (result)
            {
                case OkPosts(var posts):
                    System.Console.WriteLine($""Loaded {posts.Count} posts"");
                    break;
                case ErrorPosts(ExceptionErrorString(var ex)):
                    System.Console.WriteLine($""Error loading posts: {ex.Message}"");
                    break;
            }|}
        }
    }
}
";

        var expected = VerifyCS
            .Diagnostic(ExhaustionAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments(
                "Switch on Result is not exhaustive",
                "Matched: Error<List<String>, HttpError<String>> with ExceptionError<String>, Ok<List<String>, HttpError<String>>; Missing: Error<List<String>, HttpError<String>> with ErrorResponseError<String>"
            );

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task PatternAnalysis_DeclarationPattern_ArrayType_NoDiagnostic()
    {
        // Covers: GetPatternTypeName with DeclarationPatternSyntax where typeInfo.Type is NOT INamedTypeSymbol
        // Lines 126-127: WriteLine($"  -> Not a named type"); return null;
        // Array types are not named types, so this pattern returns null
        var test =
            @"
namespace TestCode
{
    public class TestClass
    {
        public string Process(object value)
        {
            return value switch
            {
                int[] arr => ""array"",
                _ => ""other""
            };
        }
    }
}
";

        // No diagnostic expected because array types are not closed hierarchies
        await VerifyCS.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task PatternAnalysis_TypePattern_ArrayType_NoDiagnostic()
    {
        // Covers: GetPatternTypeName with TypePatternSyntax where typeInfo.Type is NOT INamedTypeSymbol
        // Lines 179-180: WriteLine($"  -> Not a named type"); return null;
        // Array types in type patterns are not named types
        var test =
            @"
namespace TestCode
{
    public class TestClass
    {
        public string Process(object value)
        {
            return value switch
            {
                int[] => ""array"",
                _ => ""other""
            };
        }
    }
}
";

        // No diagnostic expected because array types are not closed hierarchies
        await VerifyCS.VerifyAnalyzerAsync(test).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task PatternAnalysis_RecursivePattern_WithInvalidType_NoDiagnostic()
    {
        // Covers: GetPatternTypeName with RecursivePatternSyntax where symbolInfo.Symbol is NOT INamedTypeSymbol
        // Lines 144-146: WriteLine($"  -> Not a named type symbol"); return null;
        // When recursive pattern's type doesn't resolve to a named type symbol
        var test =
            @"
namespace TestCode
{
    public class TestClass
    {
        public string Process<T>(T value)
        {
            return value switch
            {
                T(var _) => ""matched"",
                _ => ""other""
            };
        }
    }
}
";

        var expected = new[]
        {
            DiagnosticResult
                .CompilerError("CS1061")
                .WithSpan(10, 18, 10, 25)
                .WithArguments("T", "Deconstruct"),
            DiagnosticResult
                .CompilerError("CS8129")
                .WithSpan(10, 18, 10, 25)
                .WithArguments("T", "1"),
        };

        await VerifyCS.VerifyAnalyzerAsync(test, expected).ConfigureAwait(false);
    }
}
