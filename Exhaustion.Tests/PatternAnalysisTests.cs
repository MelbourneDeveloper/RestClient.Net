using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Exhaustion.Tests;

#pragma warning disable CA1515
#pragma warning disable SA1600

/// <summary>
/// Tests for the PatternAnalysis module, specifically GetNestedVariants.
/// </summary>
[TestClass]
public sealed class PatternAnalysisTests
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

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Maintainability",
        "CA1506:Avoid excessive class coupling",
        Justification = "Test helper method requires Roslyn compilation setup"
    )]
    private static (
        RecursivePatternSyntax,
        SemanticModel,
        INamedTypeSymbol
    ) ParsePatternWithSemantics(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        };
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var switchExpr = root.DescendantNodes().OfType<SwitchExpressionSyntax>().First();
        var pattern =
            switchExpr.Arms.First().Pattern as RecursivePatternSyntax
            ?? throw new InvalidOperationException("Pattern is not a RecursivePatternSyntax");

        var typeInfo = semanticModel.GetTypeInfo(switchExpr.GoverningExpression);
        var outerType =
            typeInfo.Type as INamedTypeSymbol
            ?? throw new InvalidOperationException("Could not determine outer type");

        return (pattern, semanticModel, outerType);
    }

    [TestMethod]
    public void GetNestedVariants_WithNoPositionalPatternClause_ReturnsEmptyList()
    {
        // Arrange - Pattern with property pattern but no positional pattern
        var code =
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
                Result<string, HttpError<string>>.Error<string, HttpError<string>> { Value: HttpError<string>.ExceptionError } => ""exception"",
                _ => ""default""
            };
        }
    }
}";

        var (pattern, model, outerType) = ParsePatternWithSemantics(code);

        // Act
        var variants = PatternAnalysis.GetNestedVariants(pattern, model, outerType);

        // Assert
        Assert.AreEqual(
            0,
            variants.Count,
            "GetNestedVariants should return empty list when there is no positional pattern clause"
        );
    }

    [TestMethod]
    public void GetNestedVariants_WithNoConstructorParameters_ReturnsEmptyList()
    {
        // Arrange - Simple type with no constructor parameters that are closed hierarchies
        var code =
            IsExternalInitPolyfill
            + @"
namespace TestTypes
{
    public record SimpleType(int Value);
}

namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public int Process(SimpleType simple)
        {
            return simple switch
            {
                SimpleType(var value) => value,
                _ => 0
            };
        }
    }
}";

        var (pattern, model, outerType) = ParsePatternWithSemantics(code);

        // Act
        var variants = PatternAnalysis.GetNestedVariants(pattern, model, outerType);

        // Assert
        Assert.AreEqual(
            0,
            variants.Count,
            "GetNestedVariants should return empty list when constructor parameters have no closed hierarchies"
        );
    }

    [TestMethod]
    public void GetNestedVariants_WithSingleRecursivePattern_ReturnsOneVariant()
    {
        // Arrange - Pattern matching nested type
        var code =
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
                Result<string, HttpError<string>>.Error<string, HttpError<string>>(HttpError<string>.ExceptionError(var ex)) => ""exception"",
                _ => ""default""
            };
        }
    }
}";

        var (pattern, model, outerType) = ParsePatternWithSemantics(code);

        // Act
        var variants = PatternAnalysis.GetNestedVariants(pattern, model, outerType);

        // Assert
        Assert.AreEqual(
            1,
            variants.Count,
            "GetNestedVariants should return one variant for single nested recursive pattern"
        );
        Assert.AreEqual("ExceptionError", variants[0].Name, "The variant should be ExceptionError");
    }

    [TestMethod]
    public void GetNestedVariants_WithMultipleNestedPatterns_ReturnsAllVariants()
    {
        // Arrange - Pattern with multiple nested types (though this is a complex scenario)
        var code =
            IsExternalInitPolyfill
            + @"
namespace TestTypes
{
    public abstract record Status
    {
        private Status() { }
        public sealed record Active : Status;
        public sealed record Inactive : Status;
    }

    public abstract record Priority
    {
        private Priority() { }
        public sealed record High : Priority;
        public sealed record Low : Priority;
    }

    public record Task(Status Status, Priority Priority);
}

namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public string Process(Task task)
        {
            return task switch
            {
                Task(Status.Active, Priority.High) => ""urgent"",
                _ => ""default""
            };
        }
    }
}";

        var (pattern, model, outerType) = ParsePatternWithSemantics(code);

        // Act
        var variants = PatternAnalysis.GetNestedVariants(pattern, model, outerType);

        // Assert
        Assert.AreEqual(
            2,
            variants.Count,
            "GetNestedVariants should return two variants for two nested patterns with closed hierarchies"
        );
        Assert.IsTrue(
            variants[0].Name == "Active" || variants[1].Name == "Active",
            "Variants should include Active"
        );
        Assert.IsTrue(
            variants[0].Name == "High" || variants[1].Name == "High",
            "Variants should include High"
        );
    }

    [TestMethod]
    public void GetNestedVariants_WithDeclarationPattern_ReturnsVariant()
    {
        // Arrange - Pattern with declaration pattern
        var code =
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
                Result<string, HttpError<string>>.Error<string, HttpError<string>>(HttpError<string>.ExceptionError ex) => ""exception"",
                _ => ""default""
            };
        }
    }
}";

        var (pattern, model, outerType) = ParsePatternWithSemantics(code);

        // Act
        var variants = PatternAnalysis.GetNestedVariants(pattern, model, outerType);

        // Assert
        Assert.AreEqual(
            1,
            variants.Count,
            "GetNestedVariants should return one variant for declaration pattern"
        );
        Assert.AreEqual("ExceptionError", variants[0].Name, "The variant should be ExceptionError");
    }

    [TestMethod]
    public void GetNestedVariants_WithTypePattern_ReturnsVariant()
    {
        // Arrange - Pattern with type pattern (type without variable)
        var code =
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
                Result<string, HttpError<string>>.Error<string, HttpError<string>>(HttpError<string>.ExceptionError) => ""exception"",
                _ => ""default""
            };
        }
    }
}";

        var (pattern, model, outerType) = ParsePatternWithSemantics(code);

        // Act
        var variants = PatternAnalysis.GetNestedVariants(pattern, model, outerType);

        // Assert
        Assert.AreEqual(
            1,
            variants.Count,
            "GetNestedVariants should return one variant for type pattern"
        );
        Assert.AreEqual("ExceptionError", variants[0].Name, "The variant should be ExceptionError");
    }

    [TestMethod]
    public void GetNestedVariants_WithConstantPattern_TypeAlias_ReturnsVariant()
    {
        // Arrange - Pattern with constant pattern using type alias
        var code =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + HttpErrorTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;
    using ExceptionErrorString = TestTypes.HttpError<string>.ExceptionError;

    public class TestClass
    {
        public string Process(Result<string, HttpError<string>> result)
        {
            return result switch
            {
                Result<string, HttpError<string>>.Error<string, HttpError<string>>(ExceptionErrorString) => ""exception"",
                _ => ""default""
            };
        }
    }
}";

        var (pattern, model, outerType) = ParsePatternWithSemantics(code);

        // Act
        var variants = PatternAnalysis.GetNestedVariants(pattern, model, outerType);

        // Assert
        Assert.AreEqual(
            1,
            variants.Count,
            "GetNestedVariants should return one variant for constant pattern with type alias"
        );
        Assert.AreEqual("ExceptionError", variants[0].Name, "The variant should be ExceptionError");
    }

    [TestMethod]
    public void GetNestedVariants_WithDiscardPattern_ReturnsEmptyList()
    {
        // Arrange - Pattern with discard (should not be considered a specific variant)
        var code =
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
                Result<string, HttpError<string>>.Error<string, HttpError<string>>(_) => ""error"",
                _ => ""default""
            };
        }
    }
}";

        var (pattern, model, outerType) = ParsePatternWithSemantics(code);

        // Act
        var variants = PatternAnalysis.GetNestedVariants(pattern, model, outerType);

        // Assert
        Assert.AreEqual(
            0,
            variants.Count,
            "GetNestedVariants should return empty list when nested pattern is a discard"
        );
    }

    [TestMethod]
    public void GetNestedVariants_WithVarPattern_ReturnsEmptyList()
    {
        // Arrange - Pattern with var pattern (binds to variable but doesn't specify variant)
        var code =
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
                Result<string, HttpError<string>>.Error<string, HttpError<string>>(var error) => ""error"",
                _ => ""default""
            };
        }
    }
}";

        var (pattern, model, outerType) = ParsePatternWithSemantics(code);

        // Act
        var variants = PatternAnalysis.GetNestedVariants(pattern, model, outerType);

        // Assert
        Assert.AreEqual(
            0,
            variants.Count,
            "GetNestedVariants should return empty list when nested pattern is a var pattern"
        );
    }

    [TestMethod]
    public void GetNestedVariants_WithMixedPatterns_ReturnsOnlySpecificVariants()
    {
        // Arrange - Pattern with mix of specific variant and discard
        var code =
            IsExternalInitPolyfill
            + @"
namespace TestTypes
{
    public abstract record Status
    {
        private Status() { }
        public sealed record Active : Status;
        public sealed record Inactive : Status;
    }

    public abstract record Priority
    {
        private Priority() { }
        public sealed record High : Priority;
        public sealed record Low : Priority;
    }

    public record Task(Status Status, Priority Priority);
}

namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public string Process(Task task)
        {
            return task switch
            {
                Task(Status.Active, _) => ""active"",
                _ => ""default""
            };
        }
    }
}";

        var (pattern, model, outerType) = ParsePatternWithSemantics(code);

        // Act
        var variants = PatternAnalysis.GetNestedVariants(pattern, model, outerType);

        // Assert
        Assert.AreEqual(
            1,
            variants.Count,
            "GetNestedVariants should return only one variant (Active) when one is specific and one is discard"
        );
        Assert.AreEqual("Active", variants[0].Name, "The variant should be Active");
    }

    [TestMethod]
    public void GetNestedVariants_RecursivePattern_WithExplicitType_ReturnsVariant()
    {
        // Arrange - Recursive pattern with explicit type syntax
        var code =
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
                Result<string, HttpError<string>>.Error<string, HttpError<string>>(HttpError<string>.ErrorResponseError(var body, var code)) => body,
                _ => ""default""
            };
        }
    }
}";

        var (pattern, model, outerType) = ParsePatternWithSemantics(code);

        // Act
        var variants = PatternAnalysis.GetNestedVariants(pattern, model, outerType);

        // Assert
        Assert.AreEqual(
            1,
            variants.Count,
            "GetNestedVariants should return one variant for recursive pattern with explicit type"
        );
        Assert.AreEqual(
            "ErrorResponseError",
            variants[0].Name,
            "The variant should be ErrorResponseError"
        );
    }

    [TestMethod]
    public void GetNestedVariants_RecursivePattern_FallbackToConvertedType_ReturnsVariant()
    {
        // Arrange - This tests the fallback path where Type is null but ConvertedType has the info
        var code =
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
                Result<string, HttpError<string>>.Error<string, HttpError<string>>(HttpError<string>.ExceptionError(var ex)) => ""exception"",
                _ => ""default""
            };
        }
    }
}";

        var (pattern, model, outerType) = ParsePatternWithSemantics(code);

        // Act
        var variants = PatternAnalysis.GetNestedVariants(pattern, model, outerType);

        // Assert
        Assert.AreEqual(
            1,
            variants.Count,
            "GetNestedVariants should successfully use ConvertedType as fallback"
        );
        Assert.AreEqual("ExceptionError", variants[0].Name, "The variant should be ExceptionError");
    }

    [TestMethod]
    public void GetNestedVariants_ConstantPattern_FallbackToConvertedType_ReturnsVariant()
    {
        // Arrange - Tests fallback paths in constant pattern handling
        var code =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + HttpErrorTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;
    using ResponseErrorString = TestTypes.HttpError<string>.ErrorResponseError;

    public class TestClass
    {
        public string Process(Result<string, HttpError<string>> result)
        {
            return result switch
            {
                Result<string, HttpError<string>>.Error<string, HttpError<string>>(ResponseErrorString) => ""response error"",
                _ => ""default""
            };
        }
    }
}";

        var (pattern, model, outerType) = ParsePatternWithSemantics(code);

        // Act
        var variants = PatternAnalysis.GetNestedVariants(pattern, model, outerType);

        // Assert
        Assert.AreEqual(
            1,
            variants.Count,
            "GetNestedVariants should successfully use type info from constant pattern"
        );
        Assert.AreEqual(
            "ErrorResponseError",
            variants[0].Name,
            "The variant should be ErrorResponseError"
        );
    }

    [TestMethod]
    public void GetMatchedTypes_WithMultiplePatterns_ReturnsAllMatchedTypes()
    {
        // Arrange
        var code =
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
                Result<string, HttpError<string>>.Ok<string, HttpError<string>>(var value) => value,
                Result<string, HttpError<string>>.Error<string, HttpError<string>>(var error) => ""error"",
                _ => ""default""
            };
        }
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        };
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var switchExpr = root.DescendantNodes().OfType<SwitchExpressionSyntax>().First();

        // Act
        var matchedTypes = PatternAnalysis.GetMatchedTypes(switchExpr, semanticModel);

        // Assert
        Assert.AreEqual(2, matchedTypes.Count, "Should match two types: Ok and Error");
        Assert.IsTrue(matchedTypes.Contains("Ok<String, HttpError<String>>"), "Should contain Ok");
        Assert.IsTrue(
            matchedTypes.Contains("Error<String, HttpError<String>>"),
            "Should contain Error"
        );
    }

    [TestMethod]
    public void GetMatchedTypes_WithTopLevelDiscard_SkipsDiscard()
    {
        // Arrange
        var code =
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
                Result<int, string>.Ok<int, string>(var value) => value.ToString(),
                _ => ""default""
            };
        }
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        };
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var switchExpr = root.DescendantNodes().OfType<SwitchExpressionSyntax>().First();

        // Act
        var matchedTypes = PatternAnalysis.GetMatchedTypes(switchExpr, semanticModel);

        // Assert
        Assert.AreEqual(1, matchedTypes.Count, "Should only match Ok, not the discard");
        Assert.IsTrue(matchedTypes.Contains("Ok<Int32, String>"), "Should contain Ok");
    }

    [TestMethod]
    public void GetMatchedTypesFromStatement_WithMultiplePatterns_ReturnsAllMatchedTypes()
    {
        // Arrange
        var code =
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
            switch (result)
            {
                case Result<string, HttpError<string>>.Ok<string, HttpError<string>>(var value):
                    return value;
                case Result<string, HttpError<string>>.Error<string, HttpError<string>>(var error):
                    return ""error"";
                default:
                    return ""default"";
            }
        }
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        };
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var switchStmt = root.DescendantNodes().OfType<SwitchStatementSyntax>().First();

        // Act
        var matchedTypes = PatternAnalysis.GetMatchedTypesFromStatement(switchStmt, semanticModel);

        // Assert
        Assert.AreEqual(2, matchedTypes.Count, "Should match two types: Ok and Error");
        Assert.IsTrue(matchedTypes.Contains("Ok<String, HttpError<String>>"), "Should contain Ok");
        Assert.IsTrue(
            matchedTypes.Contains("Error<String, HttpError<String>>"),
            "Should contain Error"
        );
    }

    [TestMethod]
    public void GetMatchedTypesFromStatement_WithTopLevelDiscard_SkipsDiscard()
    {
        // Arrange
        var code =
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
            switch (result)
            {
                case Result<int, string>.Ok<int, string>(var value):
                    return value.ToString();
                case _:
                    return ""default"";
            }
        }
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        };
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var switchStmt = root.DescendantNodes().OfType<SwitchStatementSyntax>().First();

        // Act
        var matchedTypes = PatternAnalysis.GetMatchedTypesFromStatement(switchStmt, semanticModel);

        // Assert
        Assert.AreEqual(1, matchedTypes.Count, "Should only match Ok, not the discard");
        Assert.IsTrue(matchedTypes.Contains("Ok<Int32, String>"), "Should contain Ok");
    }

    [TestMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Maintainability",
        "CA1506:Avoid excessive class coupling",
        Justification = "Test method requires Roslyn compilation setup"
    )]
    public void GetMatchedTypesFromStatement_WithNonPatternLabels_OnlyProcessesPatterns()
    {
        // Arrange - switch statement with constant case labels (not pattern labels)
        var code =
            IsExternalInitPolyfill
            + @"
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
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        };
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var switchStmt = root.DescendantNodes().OfType<SwitchStatementSyntax>().First();

        // Verify that the labels are NOT CasePatternSwitchLabelSyntax
        var labels = switchStmt.Sections.SelectMany(s => s.Labels).ToList();
        var patternLabels = labels.OfType<CasePatternSwitchLabelSyntax>().ToList();
        Assert.AreEqual(
            0,
            patternLabels.Count,
            "Should have no pattern labels, only constant labels"
        );

        // Act
        var matchedTypes = PatternAnalysis.GetMatchedTypesFromStatement(switchStmt, semanticModel);

        // Assert
        Assert.AreEqual(
            0,
            matchedTypes.Count,
            "Should not match any types for constant case labels"
        );
    }

    [TestMethod]
    public void GetPatternTypeName_ConstantPattern_WithSymbol_ReturnsTypeName()
    {
        // Arrange
        var code =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;
    using OkInt = TestTypes.Result<int, string>.Ok<int, string>;

    public class TestClass
    {
        public string Process(Result<int, string> result)
        {
            return result switch
            {
                OkInt => ""ok"",
                _ => ""default""
            };
        }
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        };
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var switchExpr = root.DescendantNodes().OfType<SwitchExpressionSyntax>().First();
        var pattern = switchExpr.Arms.First().Pattern;

        // Act
        var typeName = PatternAnalysis.GetPatternTypeName(pattern, semanticModel);

        // Assert
        Assert.AreEqual(
            "Ok<Int32, String>",
            typeName,
            "Should extract type name from constant pattern"
        );
    }

    [TestMethod]
    public void GetPatternTypeName_DeclarationPattern_ReturnsTypeName()
    {
        // Arrange
        var code =
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
                Result<int, string>.Ok<int, string> ok => ok.Value.ToString(),
                _ => ""default""
            };
        }
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        };
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var switchExpr = root.DescendantNodes().OfType<SwitchExpressionSyntax>().First();
        var pattern = switchExpr.Arms.First().Pattern;

        // Act
        var typeName = PatternAnalysis.GetPatternTypeName(pattern, semanticModel);

        // Assert
        Assert.AreEqual(
            "Ok<Int32, String>",
            typeName,
            "Should extract type name from declaration pattern"
        );
    }

    [TestMethod]
    public void GetPatternTypeName_RecursivePattern_WithoutNestedVariants_ReturnsTypeName()
    {
        // Arrange
        var code =
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
                Result<int, string>.Ok<int, string>(var value) => value.ToString(),
                _ => ""default""
            };
        }
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        };
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var switchExpr = root.DescendantNodes().OfType<SwitchExpressionSyntax>().First();
        var pattern = switchExpr.Arms.First().Pattern;

        // Act
        var typeName = PatternAnalysis.GetPatternTypeName(pattern, semanticModel);

        // Assert
        Assert.AreEqual(
            "Ok<Int32, String>",
            typeName,
            "Should extract type name from recursive pattern without nested variants"
        );
    }

    [TestMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Maintainability",
        "CA1506:Avoid excessive class coupling",
        Justification = "Test method requires Roslyn compilation setup"
    )]
    public void GetPatternTypeName_RecursivePattern_WithNestedVariants_ReturnsTypeNameWithVariants()
    {
        // Arrange
        var code =
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
                Result<string, HttpError<string>>.Error<string, HttpError<string>>(HttpError<string>.ExceptionError(var ex)) => ""exception"",
                _ => ""default""
            };
        }
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        };
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var switchExpr = root.DescendantNodes().OfType<SwitchExpressionSyntax>().First();
        var pattern = switchExpr.Arms.First().Pattern;

        // Act
        var typeName = PatternAnalysis.GetPatternTypeName(pattern, semanticModel);

        // Assert
        Assert.IsNotNull(typeName, "Should return a type name");
        Assert.IsTrue(
            typeName.Contains("with", StringComparison.Ordinal),
            "Should include 'with' for nested variants"
        );
        Assert.IsTrue(
            typeName.Contains("ExceptionError", StringComparison.Ordinal),
            "Should include nested variant name"
        );
    }

    [TestMethod]
    public void GetPatternTypeName_TypePattern_ReturnsTypeName()
    {
        // Arrange
        var code =
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
                Result<int, string>.Ok<int, string> => ""ok"",
                _ => ""default""
            };
        }
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        };
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var switchExpr = root.DescendantNodes().OfType<SwitchExpressionSyntax>().First();
        var pattern = switchExpr.Arms.First().Pattern;

        // Act
        var typeName = PatternAnalysis.GetPatternTypeName(pattern, semanticModel);

        // Assert
        Assert.AreEqual(
            "Ok<Int32, String>",
            typeName,
            "Should extract type name from type pattern"
        );
    }

    [TestMethod]
    public void GetPatternTypeName_DiscardPattern_ReturnsNull()
    {
        // Arrange
        var code =
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
                Result<int, string>.Ok<int, string>(var value) => value.ToString(),
                _ => ""default""
            };
        }
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        };
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var switchExpr = root.DescendantNodes().OfType<SwitchExpressionSyntax>().First();
        var pattern = switchExpr.Arms.Skip(1).First().Pattern; // Get the discard pattern

        // Act
        var typeName = PatternAnalysis.GetPatternTypeName(pattern, semanticModel);

        // Assert
        Assert.IsNull(typeName, "Should return null for discard pattern");
    }

    [TestMethod]
    public void GetPatternTypeName_RecursivePattern_WithExplicitTypeAlias_ReturnsTypeName()
    {
        // Arrange
        var code =
            IsExternalInitPolyfill
            + ResultTypeDefinition
            + HttpErrorTypeDefinition
            + @"
namespace TestCode
{
    using TestTypes;
    using ErrorString = TestTypes.Result<string, HttpError<string>>.Error<string, HttpError<string>>;

    public class TestClass
    {
        public string Process(Result<string, HttpError<string>> result)
        {
            return result switch
            {
                ErrorString(var error) => ""error"",
                _ => ""default""
            };
        }
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        };
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var switchExpr = root.DescendantNodes().OfType<SwitchExpressionSyntax>().First();
        var pattern = switchExpr.Arms.First().Pattern;

        // Act
        var typeName = PatternAnalysis.GetPatternTypeName(pattern, semanticModel);

        // Assert
        Assert.AreEqual(
            "Error<String, HttpError<String>>",
            typeName,
            "Should extract type name from recursive pattern with explicit type alias"
        );
    }

    [TestMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Maintainability",
        "CA1506:Avoid excessive class coupling",
        Justification = "Test method requires Roslyn compilation setup"
    )]
    public void GetPatternTypeName_RecursivePattern_FallbackToConvertedType_ReturnsTypeName()
    {
        // Arrange - Test the fallback path where Type is null but ConvertedType exists
        var code =
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
                Result<int, string>.Error<int, string>(var error) => error,
                _ => ""default""
            };
        }
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        };
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var switchExpr = root.DescendantNodes().OfType<SwitchExpressionSyntax>().First();
        var pattern = switchExpr.Arms.First().Pattern as RecursivePatternSyntax;

        Assert.IsNotNull(pattern, "Pattern should be RecursivePatternSyntax");
        Assert.IsNotNull(pattern.Type, "Pattern should have explicit Type property set");

        // Act
        var typeName = PatternAnalysis.GetPatternTypeName(pattern, semanticModel);

        // Assert
        Assert.AreEqual(
            "Error<Int32, String>",
            typeName,
            "Should use ConvertedType fallback for recursive patterns"
        );
    }

    [TestMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Maintainability",
        "CA1506:Avoid excessive class coupling",
        Justification = "Test method requires Roslyn compilation setup"
    )]
    public void GetPatternTypeName_RecursivePattern_WithNullType_ReturnsNull()
    {
        // Arrange - RecursivePatternSyntax where Type property is null (e.g., positional pattern without explicit type)
        var code =
            IsExternalInitPolyfill
            + @"
namespace TestTypes
{
    public record Point(int X, int Y);
}

namespace TestCode
{
    using TestTypes;

    public class TestClass
    {
        public int Process(Point point)
        {
            return point switch
            {
                (var x, var y) => x + y,
                _ => 0
            };
        }
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        };
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var switchExpr = root.DescendantNodes().OfType<SwitchExpressionSyntax>().First();
        var pattern = switchExpr.Arms.First().Pattern;

        // Verify this is a RecursivePatternSyntax with null Type
        Assert.IsTrue(
            pattern is RecursivePatternSyntax,
            "Pattern should be RecursivePatternSyntax"
        );
        var recursivePattern = pattern as RecursivePatternSyntax;
        Assert.IsNull(
            recursivePattern?.Type,
            "RecursivePattern.Type should be null for positional patterns without explicit type"
        );

        // Act
        var typeName = PatternAnalysis.GetPatternTypeName(pattern, semanticModel);

        // Assert
        Assert.IsNull(typeName, "Should return null when RecursivePatternSyntax.Type is null");
    }
}
