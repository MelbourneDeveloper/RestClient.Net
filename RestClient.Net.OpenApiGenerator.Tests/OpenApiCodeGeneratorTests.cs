using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RestClient.Net.OpenApiGenerator;
using GeneratorError = Outcome.Result<
    RestClient.Net.OpenApiGenerator.GeneratorResult,
    string
>.Error<RestClient.Net.OpenApiGenerator.GeneratorResult, string>;
using GeneratorOk = Outcome.Result<RestClient.Net.OpenApiGenerator.GeneratorResult, string>.Ok<
    RestClient.Net.OpenApiGenerator.GeneratorResult,
    string
>;

namespace RestClient.Net.OpenApiGenerator.Tests;

[TestClass]
public class OpenApiCodeGeneratorTests
{
    private static void AssertGenerationJourney(
        string spec,
        GeneratorResult expected,
        string? baseUrlOverride = null
    )
    {
        var output = Path.Combine(
            Path.GetTempPath(),
            "OpenApiAssertions",
            Guid.NewGuid().ToString("N")
        );
        try
        {
            var generated = GetSuccessResult(
                OpenApiCodeGenerator.Generate(
                    spec,
                    "TestApi",
                    "TestApiExtensions",
                    output,
                    baseUrlOverride
                )
            );
            Assert.AreEqual(
                expected.ExtensionMethodsCode,
                generated.ExtensionMethodsCode,
                "Repeating generation must preserve extension method behavior and ordering."
            );
            Assert.AreEqual(
                expected.ModelsCode,
                generated.ModelsCode,
                "Repeating generation must preserve model types and property ordering."
            );
            CollectionAssert.AreEquivalent(
                (string[])["TestApiExtensions.g.cs", "TestApiModels.g.cs", "GlobalUsings.g.cs"],
                Directory.GetFiles(output).Select(Path.GetFileName).ToArray(),
                "A generation must write exactly the client, models, and aliases."
            );
            Assert.AreEqual(
                generated.ExtensionMethodsCode,
                File.ReadAllText(Path.Combine(output, "TestApiExtensions.g.cs")),
                "The saved client must match the returned client code."
            );
            Assert.AreEqual(
                generated.ModelsCode,
                File.ReadAllText(Path.Combine(output, "TestApiModels.g.cs")),
                "The saved models must match the returned model code."
            );
            var aliases = File.ReadAllText(Path.Combine(output, "GlobalUsings.g.cs"));
            Assert.IsFalse(
                string.IsNullOrWhiteSpace(aliases),
                "Result aliases must be written for generated callers."
            );

            AssertGeneratedContract(spec, generated, aliases);

            File.WriteAllText(Path.Combine(output, "TestApiExtensions.g.cs"), "stale client");
            File.WriteAllText(Path.Combine(output, "TestApiModels.g.cs"), "stale models");
            var regenerated = GetSuccessResult(
                OpenApiCodeGenerator.Generate(
                    spec,
                    "TestApi",
                    "TestApiExtensions",
                    output,
                    baseUrlOverride
                )
            );
            Assert.AreEqual(
                generated,
                regenerated,
                "Regeneration must recover identical code from an existing output directory."
            );
            Assert.AreEqual(
                generated.ExtensionMethodsCode,
                File.ReadAllText(Path.Combine(output, "TestApiExtensions.g.cs")),
                "Regeneration must replace stale client contents."
            );
            Assert.AreEqual(
                generated.ModelsCode,
                File.ReadAllText(Path.Combine(output, "TestApiModels.g.cs")),
                "Regeneration must replace stale model contents."
            );
            Assert.AreEqual(
                aliases,
                File.ReadAllText(Path.Combine(output, "GlobalUsings.g.cs")),
                "Regeneration must preserve the matching alias contract."
            );
        }
        finally
        {
            if (Directory.Exists(output))
            {
                Directory.Delete(output, true);
            }
        }
    }

    private static void AssertGeneratedContract(
        string spec,
        GeneratorResult generated,
        string aliases
    )
    {
        var clientTree = CSharpSyntaxTree.ParseText(generated.ExtensionMethodsCode);
        var modelsTree = CSharpSyntaxTree.ParseText(generated.ModelsCode);
        var aliasesTree = CSharpSyntaxTree.ParseText(aliases);
        foreach (var tree in new[] { clientTree, modelsTree, aliasesTree })
        {
            var errors = tree.GetDiagnostics()
                .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                .ToArray();
            Assert.AreEqual(
                0,
                errors.Length,
                "Generated files must be valid C# syntax: "
                    + string.Join(
                        Environment.NewLine,
                        errors.Select(diagnostic => diagnostic.ToString())
                    )
            );
        }
        using var document = JsonDocument.Parse(spec);
        AssertClientContract(clientTree, document.RootElement);
        AssertModelContract(modelsTree, document.RootElement);
        AssertGeneratedCompilation(clientTree, modelsTree, aliasesTree);
    }

    private static void AssertClientContract(SyntaxTree clientTree, JsonElement spec)
    {
        var client = clientTree
            .GetRoot()
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Single();
        Assert.AreEqual("TestApiExtensions", client.Identifier.ValueText);
        Assert.IsTrue(client.Modifiers.Any(SyntaxKind.PublicKeyword));
        Assert.IsTrue(client.Modifiers.Any(SyntaxKind.StaticKeyword));
        Assert.AreEqual(
            "TestApi",
            clientTree
                .GetRoot()
                .DescendantNodes()
                .OfType<FileScopedNamespaceDeclarationSyntax>()
                .Single()
                .Name.ToString()
        );
        string[] supportedVerbs =
        [
            "get",
            "post",
            "put",
            "patch",
            "delete",
            "head",
            "options",
            "trace",
        ];
        var operations = spec.GetProperty("paths")
            .EnumerateObject()
            .SelectMany(path => path.Value.EnumerateObject())
            .Where(operation => supportedVerbs.Contains(operation.Name, StringComparer.Ordinal))
            .ToArray();
        var methods = client
            .Members.OfType<MethodDeclarationSyntax>()
            .Where(method => method.Modifiers.Any(SyntaxKind.PublicKeyword))
            .ToArray();
        Assert.AreEqual(
            operations.Length,
            methods.Length,
            "Every declared HTTP operation must expose exactly one public client method."
        );
        Assert.AreEqual(
            methods.Length,
            methods
                .Select(method => method.Identifier.ValueText)
                .Distinct(StringComparer.Ordinal)
                .Count(),
            "Generated operation names must remain unique."
        );
        foreach (var method in methods)
        {
            AssertOperationContract(method);
        }
    }

    private static void AssertOperationContract(MethodDeclarationSyntax method)
    {
        Assert.IsTrue(
            method.Modifiers.Any(SyntaxKind.StaticKeyword),
            "API methods must be callable as static extensions."
        );
        Assert.IsTrue(
            method.Identifier.ValueText.EndsWith("Async", StringComparison.Ordinal),
            "HTTP operations must expose the asynchronous contract."
        );
        var receiverType = method.ParameterList.Parameters[0].Type;
        Assert.IsNotNull(receiverType);
        Assert.AreEqual("HttpClient", receiverType.ToString());
        Assert.IsTrue(
            method.ParameterList.Parameters[0].Modifiers.Any(SyntaxKind.ThisKeyword),
            "The first argument must be the receiving HttpClient."
        );
        var cancellationType = method.ParameterList.Parameters[^1].Type;
        Assert.IsNotNull(cancellationType);
        Assert.AreEqual("CancellationToken", cancellationType.ToString());
        Assert.AreEqual(
            "cancellationToken",
            method.ParameterList.Parameters[^1].Identifier.ValueText
        );
        Assert.IsNotNull(
            method.ParameterList.Parameters[^1].Default,
            "Callers must be able to omit the cancellation token."
        );
        Assert.IsTrue(
            method.ReturnType.ToString().StartsWith("Task<Result<", StringComparison.Ordinal),
            "Operations must expose typed asynchronous success/error results."
        );
        Assert.IsNotNull(
            method.ExpressionBody,
            "Each operation must invoke its configured request delegate."
        );
        Assert.IsTrue(
            method
                .ExpressionBody.Expression.ToString()
                .Contains("cancellationToken", StringComparison.Ordinal),
            "The operation must forward the caller's cancellation token."
        );
    }

    private static void AssertModelContract(SyntaxTree modelsTree, JsonElement spec)
    {
        Assert.AreEqual(
            "TestApi",
            modelsTree
                .GetRoot()
                .DescendantNodes()
                .OfType<FileScopedNamespaceDeclarationSyntax>()
                .Single()
                .Name.ToString()
        );
        var schemas =
            spec.TryGetProperty("components", out var components)
            && components.TryGetProperty("schemas", out var schemaDefinitions)
                ? schemaDefinitions
                    .EnumerateObject()
                    .Where(schema =>
                        !(
                            schema.Value.TryGetProperty("type", out var type)
                            && type.GetString() == "string"
                            && schema.Value.TryGetProperty("enum", out var values)
                            && values.GetArrayLength() > 0
                        )
                    )
                    .ToArray()
                : [];
        var records = modelsTree
            .GetRoot()
            .DescendantNodes()
            .OfType<RecordDeclarationSyntax>()
            .ToArray();
        Assert.AreEqual(
            schemas.Length,
            records.Length,
            "Every model schema must produce one record; string enums stay strings."
        );
        Assert.AreEqual(
            records.Length,
            records
                .Select(record => record.Identifier.ValueText)
                .Distinct(StringComparer.Ordinal)
                .Count(),
            "Generated model names must remain unique."
        );
        foreach (var record in records)
        {
            Assert.IsTrue(record.Modifiers.Any(SyntaxKind.PublicKeyword));
            Assert.IsNotNull(
                record.ParameterList,
                "Generated records must retain their property constructor."
            );
            Assert.AreEqual(
                record.ParameterList.Parameters.Count,
                record
                    .ParameterList.Parameters.Select(parameter => parameter.Identifier.ValueText)
                    .Distinct(StringComparer.Ordinal)
                    .Count(),
                "A record must not duplicate property names."
            );
        }
    }

    private static void AssertGeneratedCompilation(
        SyntaxTree clientTree,
        SyntaxTree modelsTree,
        SyntaxTree aliasesTree
    )
    {
        var platformAssemblies =
            (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
            ?? throw new InvalidOperationException("Runtime references are unavailable.");
        var references = platformAssemblies
            .Split(Path.PathSeparator)
            .Distinct(StringComparer.Ordinal)
            .Select(path => MetadataReference.CreateFromFile(path));
        var compilation = CSharpCompilation.Create(
            "GeneratedClientAssertions",
            [
                clientTree,
                modelsTree,
                aliasesTree,
                CSharpSyntaxTree.ParseText(
                    "global using System; global using System.Collections.Generic;"
                ),
            ],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        var compilationErrors = compilation
            .GetDiagnostics()
            .Where(diagnostic =>
                diagnostic.Severity == DiagnosticSeverity.Error || diagnostic.Id == "CS8632"
            )
            .ToArray();
        Assert.AreEqual(
            0,
            compilationErrors.Length,
            "Generated client, models, and aliases must compile together: "
                + string.Join(
                    Environment.NewLine,
                    compilationErrors.Select(diagnostic => diagnostic.ToString())
                )
        );
    }

    private static void AssertErrorRecoveryJourney(
        string spec,
        string expectedError,
        string? baseUrlOverride = null
    )
    {
        var output = Path.Combine(
            Path.GetTempPath(),
            "OpenApiAssertions",
            Guid.NewGuid().ToString("N")
        );
        try
        {
            var failed = OpenApiCodeGenerator.Generate(
                spec,
                "TestApi",
                "TestApiExtensions",
                output,
                baseUrlOverride
            );
            Assert.IsInstanceOfType<GeneratorError>(failed);
            Assert.AreEqual(
                expectedError,
                ((GeneratorError)failed).Value,
                "Repeating invalid input must preserve the actionable error."
            );
            Assert.IsFalse(
                Directory.Exists(output),
                "Invalid input must not leave partial generated artifacts."
            );
            var recovered = GetSuccessResult(
                OpenApiCodeGenerator.Generate(
                    SimpleOpenApiSpec,
                    "TestApi",
                    "TestApiExtensions",
                    output
                )
            );
            Assert.IsTrue(
                File.Exists(Path.Combine(output, "TestApiExtensions.g.cs")),
                "A corrected specification must recover using the same destination."
            );
            Assert.IsTrue(File.Exists(Path.Combine(output, "TestApiModels.g.cs")));
            Assert.IsTrue(File.Exists(Path.Combine(output, "GlobalUsings.g.cs")));
            AssertGenerationJourney(SimpleOpenApiSpec, recovered);
        }
        finally
        {
            if (Directory.Exists(output))
            {
                Directory.Delete(output, true);
            }
        }
    }

    private static GeneratorResult GetSuccessResult(
        Outcome.Result<GeneratorResult, string> result
    ) =>
#pragma warning disable CS8509
        result switch
        {
            GeneratorOk(var r) => r,
            GeneratorError(var error) => throw new AssertFailedException(
                $"Generation failed: {error}"
            ),
        };
#pragma warning restore CS8509

    private const string SimpleOpenApiSpec = """
        {
          "openapi": "3.0.0",
          "info": {
            "title": "Test API",
            "version": "1.0.0"
          },
          "servers": [
            {
              "url": "https://api.test.com/v1"
            }
          ],
          "paths": {
            "/pets": {
              "get": {
                "operationId": "listPets",
                "parameters": [
                  {
                    "name": "limit",
                    "in": "query",
                    "schema": {
                      "type": "integer"
                    }
                  }
                ],
                "responses": {
                  "200": {
                    "description": "Success",
                    "content": {
                      "application/json": {
                        "schema": {
                          "type": "array",
                          "items": {
                            "$ref": "#/components/schemas/Pet"
                          }
                        }
                      }
                    }
                  }
                }
              },
              "post": {
                "operationId": "createPet",
                "requestBody": {
                  "content": {
                    "application/json": {
                      "schema": {
                        "$ref": "#/components/schemas/Pet"
                      }
                    }
                  }
                },
                "responses": {
                  "200": {
                    "description": "Success",
                    "content": {
                      "application/json": {
                        "schema": {
                          "$ref": "#/components/schemas/Pet"
                        }
                      }
                    }
                  }
                }
              }
            },
            "/pets/{petId}": {
              "get": {
                "operationId": "getPet",
                "parameters": [
                  {
                    "name": "petId",
                    "in": "path",
                    "required": true,
                    "schema": {
                      "type": "integer",
                      "format": "int64"
                    }
                  }
                ],
                "responses": {
                  "200": {
                    "description": "Success",
                    "content": {
                      "application/json": {
                        "schema": {
                          "$ref": "#/components/schemas/Pet"
                        }
                      }
                    }
                  }
                }
              },
              "delete": {
                "operationId": "deletePet",
                "parameters": [
                  {
                    "name": "petId",
                    "in": "path",
                    "required": true,
                    "schema": {
                      "type": "integer",
                      "format": "int64"
                    }
                  },
                  {
                    "name": "api_key",
                    "in": "query",
                    "schema": {
                      "type": "string"
                    }
                  }
                ],
                "responses": {
                  "204": {
                    "description": "Success"
                  }
                }
              }
            }
          },
          "components": {
            "schemas": {
              "Pet": {
                "type": "object",
                "required": ["name"],
                "properties": {
                  "id": {
                    "type": "integer",
                    "format": "int64"
                  },
                  "name": {
                    "type": "string"
                  },
                  "tag": {
                    "type": "string"
                  }
                }
              }
            }
          }
        }
        """;

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Generate_ModelNamedUnit_AliasesPreserveModelIdentity(bool useArraySuccess)
    {
        var output = Path.Combine(
            Path.GetTempPath(),
            "OpenApiUnitAssertions",
            Guid.NewGuid().ToString("N")
        );
        var successSchema = useArraySuccess
            ? """{"type":"array","items":{"$ref":"#/components/schemas/Unit"}}"""
            : """{"type":"object"}""";
        var errorSchema = useArraySuccess
            ? """{"type":"string"}"""
            : """{"$ref":"#/components/schemas/Unit"}""";
        var spec = """
            {
              "openapi":"3.0.0",
              "info":{"title":"Model identity","version":"1.0.0"},
              "servers":[{"url":"https://api.test.com"}],
              "paths":{"/values":{"get":{"operationId":"getValues","responses":{
                "200":{"description":"Success","content":{"application/json":{"schema":SUCCESS_SCHEMA}}},
                "400":{"description":"Failure","content":{"application/json":{"schema":ERROR_SCHEMA}}}
              }}}},
              "components":{"schemas":{"Unit":{"type":"object","properties":{"value":{"type":"string"}}}}}
            }
            """.Replace("SUCCESS_SCHEMA", successSchema, StringComparison.Ordinal).Replace(
            "ERROR_SCHEMA",
            errorSchema,
            StringComparison.Ordinal
        );
        try
        {
            var result = GetSuccessResult(
                OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", output)
            );
            var aliases = File.ReadAllText(Path.Combine(output, "GlobalUsings.g.cs"));
            var successType = useArraySuccess
                ? "System.Collections.Generic.List<TestApi.Unit>"
                : "System.Object";
            var errorType = useArraySuccess ? "string" : "TestApi.Unit";
            var aliasName = useArraySuccess ? "Units" : "objectUnit";
            Assert.IsTrue(
                aliases.Contains(
                    $"Outcome.Result<{successType}, Outcome.HttpError<{errorType}>>.Ok<{successType}, Outcome.HttpError<{errorType}>>",
                    StringComparison.Ordinal
                ),
                "The Ok alias must bind to the generated Unit model, not the Outcome success sentinel."
            );
            Assert.IsTrue(
                aliases.Contains(
                    $"Outcome.Result<{successType}, Outcome.HttpError<{errorType}>>.Error<{successType}, Outcome.HttpError<{errorType}>>",
                    StringComparison.Ordinal
                ),
                "The Error alias must preserve the same generated model identity."
            );
            Assert.IsFalse(
                aliases.Contains("Outcome.Unit", StringComparison.Ordinal),
                "Model Unit references must not be replaced with the unrelated sentinel type."
            );
            Assert.IsTrue(
                result.ModelsCode.Contains(
                    "public record Unit(string Value)",
                    StringComparison.Ordinal
                )
            );
            AssertGenerationJourney(spec, result);
            var consumer = $$"""
                public static class ModelUnitConsumer
                {
                    public static object Read(Outcome.Result<{{successType}}, Outcome.HttpError<{{errorType}}>> result)
                    {
                        if (result is Ok{{aliasName}} ok) return ok.Value;
                        if (result is Error{{aliasName}} error) return error.Value;
                        throw new System.InvalidOperationException();
                    }
                }
                """;
            // Consumer patterns verify alias/result identity beyond checking syntactically valid targets.
            AssertGeneratedCompilation(
                CSharpSyntaxTree.ParseText(consumer),
                CSharpSyntaxTree.ParseText(result.ModelsCode),
                CSharpSyntaxTree.ParseText(aliases)
            );
            var repeated = GetSuccessResult(
                OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", output)
            );
            Assert.AreEqual(result, repeated, "Regeneration must preserve model-backed aliases.");
            Assert.AreEqual(aliases, File.ReadAllText(Path.Combine(output, "GlobalUsings.g.cs")));
        }
        finally
        {
            if (Directory.Exists(output))
            {
                Directory.Delete(output, true);
            }
        }
    }

    [TestMethod]
    [DataRow("integer", "int", "System.Int32")]
    [DataRow("string", "string", "System.String")]
    public void Generate_NullablePrimitiveResponses_HaveCompilableDistinctAliases(
        string schemaType,
        string csharpType,
        string qualifiedType
    )
    {
        var output = Path.Combine(
            Path.GetTempPath(),
            "OpenApiNullableAssertions",
            Guid.NewGuid().ToString("N")
        );
        var spec = JsonSerializer.Serialize(
            new
            {
                openapi = "3.0.0",
                info = new { title = "Nullable aliases", version = "1.0.0" },
                servers = new[] { new { url = "https://api.test.com" } },
                paths = new Dictionary<string, object>
                {
                    ["/optional"] = new
                    {
                        get = new
                        {
                            operationId = "getOptional",
                            responses = new Dictionary<string, object>
                            {
                                ["200"] = new
                                {
                                    description = "Optional value",
                                    content = new Dictionary<string, object>
                                    {
                                        ["application/json"] = new
                                        {
                                            schema = new
                                            {
                                                anyOf = new[]
                                                {
                                                    new { type = schemaType },
                                                    new { type = "null" },
                                                },
                                            },
                                        },
                                    },
                                },
                            },
                        },
                    },
                    ["/required"] = new
                    {
                        get = new
                        {
                            operationId = "getRequired",
                            responses = new Dictionary<string, object>
                            {
                                ["200"] = new
                                {
                                    description = "Required value",
                                    content = new Dictionary<string, object>
                                    {
                                        ["application/json"] = new
                                        {
                                            schema = new { type = schemaType },
                                        },
                                    },
                                },
                            },
                        },
                    },
                },
            }
        );
        try
        {
            var result = GetSuccessResult(
                OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", output)
            );
            Assert.IsTrue(
                result.ExtensionMethodsCode.Contains(
                    $"Task<Result<{csharpType}?, HttpError<string>>>",
                    StringComparison.Ordinal
                ),
                "Nullable responses must retain their public result annotation."
            );
            Assert.IsTrue(
                result.ExtensionMethodsCode.Contains(
                    $"Task<Result<{csharpType}, HttpError<string>>>",
                    StringComparison.Ordinal
                ),
                "The same client must retain its nonnullable operation."
            );
            AssertGenerationJourney(spec, result);
            var aliases = File.ReadAllText(Path.Combine(output, "GlobalUsings.g.cs"));
            Assert.IsTrue(
                aliases.StartsWith("#nullable enable\n", StringComparison.Ordinal),
                "Nullable aliases must declare their own annotation context for consuming projects."
            );
            var directives = CSharpSyntaxTree.ParseText(aliases).GetCompilationUnitRoot().Usings;
            Assert.AreEqual(
                4,
                directives.Count,
                "Both response forms need separate Ok and Error aliases."
            );
            Assert.AreEqual(
                4,
                directives
                    .Select(directive => directive.Alias?.Name.Identifier.ValueText)
                    .Distinct(StringComparer.Ordinal)
                    .Count(),
                "Nullable aliases must not collide with nonnullable aliases."
            );
            foreach (var directive in directives)
            {
                Assert.IsNotNull(directive.Alias);
                Assert.IsTrue(
                    SyntaxFacts.IsValidIdentifier(directive.Alias.Name.Identifier.ValueText),
                    "A nullable annotation must never become part of a C# alias identifier."
                );
            }

            Assert.IsTrue(
                aliases.Contains($"Outcome.Result<{qualifiedType}?,", StringComparison.Ordinal),
                "Nullable aliases must qualify their primitive target correctly."
            );
            Assert.IsTrue(
                aliases.Contains($"Outcome.Result<{qualifiedType},", StringComparison.Ordinal),
                "Nonnullable aliases must remain separately usable."
            );
        }
        finally
        {
            if (Directory.Exists(output))
            {
                Directory.Delete(output, true);
            }
        }
    }

    [TestMethod]
    [DataRow("integer", null, "int", false, false)]
    [DataRow("integer", "int64", "long", false, false)]
    [DataRow("number", null, "float", false, false)]
    [DataRow("number", "double", "double", false, false)]
    [DataRow("boolean", null, "bool", false, false)]
    [DataRow("string", null, "string", false, false)]
    [DataRow("object", null, "object", false, false)]
    [DataRow("integer", null, "int", true, false)]
    [DataRow("number", null, "double", true, false)]
    [DataRow("string", null, "string", true, false)]
    [DataRow("object", null, "object", true, false)]
    [DataRow("integer", null, "int", false, true)]
    [DataRow("integer", "int64", "long", false, true)]
    [DataRow("number", null, "float", false, true)]
    [DataRow("number", "double", "double", false, true)]
    [DataRow("boolean", null, "bool", false, true)]
    [DataRow("string", null, "string", false, true)]
    [DataRow("object", null, "object", false, true)]
    [DataRow("integer", null, "int", true, true)]
    [DataRow("number", null, "double", true, true)]
    [DataRow("string", null, "string", true, true)]
    [DataRow("object", null, "object", true, true)]
    public void Generate_PrimitiveResponses_HaveCompilableAliases(
        string schemaType,
        string? format,
        string elementType,
        bool isArray,
        bool useModelError
    )
    {
        var primitive = new Dictionary<string, object> { ["type"] = schemaType };
        if (format != null)
        {
            primitive.Add("format", format);
        }

        var schema = isArray
            ? new Dictionary<string, object> { ["type"] = "array", ["items"] = primitive }
            : primitive;
        var errorSchema = useModelError
            ? new Dictionary<string, object> { ["$ref"] = "#/components/schemas/Failure" }
            : new Dictionary<string, object> { ["type"] = "string" };
        var spec = JsonSerializer.Serialize(
            new
            {
                openapi = "3.0.0",
                info = new { title = "Primitive aliases", version = "1.0.0" },
                servers = new[] { new { url = "https://api.test.com" } },
                paths = new Dictionary<string, object>
                {
                    ["/values"] = new
                    {
                        get = new
                        {
                            operationId = "getValues",
                            responses = new Dictionary<string, object>
                            {
                                ["200"] = new
                                {
                                    description = "Success",
                                    content = new Dictionary<string, object>
                                    {
                                        ["application/json"] = new { schema },
                                    },
                                },
                                ["400"] = new
                                {
                                    description = "Failure",
                                    content = new Dictionary<string, object>
                                    {
                                        ["application/json"] = new { schema = errorSchema },
                                    },
                                },
                            },
                        },
                    },
                },
                components = new
                {
                    schemas = new Dictionary<string, object>
                    {
                        ["Failure"] = new
                        {
                            type = "object",
                            properties = new { message = new { type = "string" } },
                        },
                    },
                },
            }
        );
        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );
        var successType = isArray ? $"List<{elementType}>" : elementType;
        var errorType = useModelError ? "Failure" : "string";
        Assert.IsTrue(
            result.ExtensionMethodsCode.Contains(
                $"Task<Result<{successType}, HttpError<{errorType}>>>",
                StringComparison.Ordinal
            ),
            "The declared primitive response and existing error schema must retain their typed public contract."
        );
        Assert.IsTrue(
            result.ModelsCode.Contains(
                "public record Failure(string Message)",
                StringComparison.Ordinal
            ),
            "The error model must remain generated alongside primitive success types."
        );
        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_WithValidSpec_ProducesNonEmptyCode()
    {
        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(
                SimpleOpenApiSpec,
                "TestApi",
                "TestApiExtensions",
                Path.GetTempPath()
            )
        );

        // Debug: Output actual result if empty
        if (string.IsNullOrWhiteSpace(result.ExtensionMethodsCode))
        {
            Assert.Fail($"ExtensionMethodsCode is empty. Content: '{result.ExtensionMethodsCode}'");
        }

        Assert.IsFalse(
            string.IsNullOrWhiteSpace(result.ModelsCode),
            $"ModelsCode is empty. Content: '{result.ModelsCode}'"
        );
        Assert.IsTrue(
            result.ExtensionMethodsCode.Contains("public static class TestApiExtensions"),
            $"Missing class declaration. Code: {result.ExtensionMethodsCode.Substring(0, Math.Min(500, result.ExtensionMethodsCode.Length))}"
        );
        Assert.IsTrue(
            result.ModelsCode.Contains("public record Pet"),
            $"Missing Pet record. Code: {result.ModelsCode}"
        );

        AssertGenerationJourney(SimpleOpenApiSpec, result);
    }

    [TestMethod]
    public void Generate_CreatesCorrectBaseUrl()
    {
        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(
                SimpleOpenApiSpec,
                "TestApi",
                "TestApiExtensions",
                Path.GetTempPath()
            )
        );

        Assert.IsTrue(
            result.ExtensionMethodsCode.Contains("\"https://api.test.com\""),
            $"Missing base URL. Generated code:\n{result.ExtensionMethodsCode.Substring(0, Math.Min(1000, result.ExtensionMethodsCode.Length))}"
        );
        Assert.IsFalse(
            result.ExtensionMethodsCode.Contains("\"/v1\""),
            $"Found /v1 in generated code"
        );

        AssertGenerationJourney(SimpleOpenApiSpec, result);
    }

    [TestMethod]
    public void Generate_CreatesCorrectRelativeUrls()
    {
        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(
                SimpleOpenApiSpec,
                "TestApi",
                "TestApiExtensions",
                Path.GetTempPath()
            )
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("\"/v1/pets\""));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("\"/v1/pets/{petId}\""));

        AssertGenerationJourney(SimpleOpenApiSpec, result);
    }

    [TestMethod]
    public void Generate_IncludesQueryParameters()
    {
        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(
                SimpleOpenApiSpec,
                "TestApi",
                "TestApiExtensions",
                Path.GetTempPath()
            )
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("int? limit"));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("?limit={param}"));

        AssertGenerationJourney(SimpleOpenApiSpec, result);
    }

    [TestMethod]
    public void Generate_HandlesPathAndQueryParametersTogether()
    {
        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(
                SimpleOpenApiSpec,
                "TestApi",
                "TestApiExtensions",
                Path.GetTempPath()
            )
        );

        Assert.IsTrue(
            result.ExtensionMethodsCode.Contains("string apiKey")
                && result.ExtensionMethodsCode.Contains("long petId"),
            $"Missing parameters. Code:\n{result.ExtensionMethodsCode}"
        );
        Assert.IsTrue(
            result.ExtensionMethodsCode.Contains("?api_key={param.apiKey}"),
            $"Missing direct interpolation. Code:\n{result.ExtensionMethodsCode}"
        );

        AssertGenerationJourney(SimpleOpenApiSpec, result);
    }

    [TestMethod]
    public void Generate_ResolvesSchemaReferences()
    {
        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(
                SimpleOpenApiSpec,
                "TestApi",
                "TestApiExtensions",
                Path.GetTempPath()
            )
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("Result<Pet,"));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("Result<List<Pet>,"));
        Assert.IsFalse(result.ExtensionMethodsCode.Contains("Result<object,"));

        AssertGenerationJourney(SimpleOpenApiSpec, result);
    }

    [TestMethod]
    public void Generate_CreatesGetMethod()
    {
        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(
                SimpleOpenApiSpec,
                "TestApi",
                "TestApiExtensions",
                Path.GetTempPath()
            )
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("ListPets"));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("CreateGet"));

        AssertGenerationJourney(SimpleOpenApiSpec, result);
    }

    [TestMethod]
    public void Generate_CreatesPostMethod()
    {
        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(
                SimpleOpenApiSpec,
                "TestApi",
                "TestApiExtensions",
                Path.GetTempPath()
            )
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("CreatePet"));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("CreatePost"));

        AssertGenerationJourney(SimpleOpenApiSpec, result);
    }

    [TestMethod]
    public void Generate_CreatesDeleteMethod()
    {
        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(
                SimpleOpenApiSpec,
                "TestApi",
                "TestApiExtensions",
                Path.GetTempPath()
            )
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("DeletePet"));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("Result<Unit,"));

        AssertGenerationJourney(SimpleOpenApiSpec, result);
    }

    [TestMethod]
    public void Generate_WithNoServerUrl_ThrowsException()
    {
        var specWithoutServer = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "paths": {}
            }
            """;

        var result = OpenApiCodeGenerator.Generate(
            specWithoutServer,
            "TestApi",
            "TestApiExtensions",
            Path.GetTempPath()
        );

#pragma warning disable CS8509
        var error = result switch
        {
            GeneratorError(var e) => e,
            GeneratorOk => throw new AssertFailedException("Expected error but got success"),
        };
#pragma warning restore CS8509

        Assert.IsTrue(error.Contains("must specify at least one server"));

        AssertErrorRecoveryJourney(specWithoutServer, error);
    }

    [TestMethod]
    public void Generate_WithRelativeServerUrl_RequiresOverride()
    {
        var specWithRelativeUrl = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "/api/v1" }],
              "paths": {}
            }
            """;

        var result = OpenApiCodeGenerator.Generate(
            specWithRelativeUrl,
            "TestApi",
            "TestApiExtensions",
            Path.GetTempPath()
        );

#pragma warning disable CS8509
        var error = result switch
        {
            GeneratorError(var e) => e,
            GeneratorOk => throw new AssertFailedException("Expected error but got success"),
        };
#pragma warning restore CS8509

        Assert.IsTrue(error.Contains("relative"));
        Assert.IsTrue(error.Contains("baseUrlOverride"));

        AssertErrorRecoveryJourney(specWithRelativeUrl, error);
    }

    [TestMethod]
    public void Generate_WithRelativeServerUrlAndOverride_Succeeds()
    {
        var specWithRelativeUrl = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "/api/v1" }],
              "paths": {
                "/test": {
                  "get": {
                    "responses": {
                      "200": {
                        "description": "Success"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(
                specWithRelativeUrl,
                "TestApi",
                "TestApiExtensions",
                Path.GetTempPath(),
                "https://example.com"
            )
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("https://example.com"));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("/api/v1/test"));

        AssertGenerationJourney(specWithRelativeUrl, result, "https://example.com");
    }

    [TestMethod]
    public void Generate_CreatesModelWithCorrectProperties()
    {
        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(
                SimpleOpenApiSpec,
                "TestApi",
                "TestApiExtensions",
                Path.GetTempPath()
            )
        );

        Assert.IsTrue(result.ModelsCode.Contains("public record Pet("));
        Assert.IsTrue(result.ModelsCode.Contains("long Id"));
        Assert.IsTrue(result.ModelsCode.Contains("string Name"));
        Assert.IsTrue(result.ModelsCode.Contains("string Tag"));

        AssertGenerationJourney(SimpleOpenApiSpec, result);
    }

    [TestMethod]
    public void Generate_WritesFilesToOutputPath()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        try
        {
            var result = GetSuccessResult(
                OpenApiCodeGenerator.Generate(
                    SimpleOpenApiSpec,
                    "TestApi",
                    "TestApiExtensions",
                    tempDir
                )
            );

            var extensionsFile = Path.Combine(tempDir, "TestApiExtensions.g.cs");
            var modelsFile = Path.Combine(tempDir, "TestApiModels.g.cs");

            Assert.IsTrue(File.Exists(extensionsFile));
            Assert.IsTrue(File.Exists(modelsFile));
            Assert.IsTrue(new FileInfo(extensionsFile).Length > 0);
            Assert.IsTrue(new FileInfo(modelsFile).Length > 0);
            AssertGenerationJourney(SimpleOpenApiSpec, result);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [TestMethod]
    public void Generate_CreatesPrivateStaticFuncFields()
    {
        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(
                SimpleOpenApiSpec,
                "TestApi",
                "TestApiExtensions",
                Path.GetTempPath()
            )
        );

        // Verify that private static properties with delegates are generated
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("{ get; } ="));

        // Verify that the Unit deserializer is generated
        Assert.IsTrue(
            result.ExtensionMethodsCode.Contains(
                "private static readonly Deserialize<Outcome.Unit> _deserializeUnit"
            )
        );

        // Verify that public methods call the private delegates with HttpClient as first parameter
        Assert.IsTrue(
            result.ExtensionMethodsCode.Contains("(httpClient,")
                && result.ExtensionMethodsCode.Contains(", cancellationToken)")
        );

        AssertGenerationJourney(SimpleOpenApiSpec, result);
    }

    [TestMethod]
    public void Generate_HandlesAnyOfSchemasInProperties()
    {
        var specWithAnyOf = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/kb": {
                  "get": {
                    "operationId": "getKb",
                    "responses": {
                      "200": {
                        "description": "Success",
                        "content": {
                          "application/json": {
                            "schema": {
                              "$ref": "#/components/schemas/KnowledgeBoxObj"
                            }
                          }
                        }
                      }
                    }
                  }
                }
              },
              "components": {
                "schemas": {
                  "KnowledgeBoxObj": {
                    "type": "object",
                    "required": ["uuid"],
                    "properties": {
                      "slug": {
                        "anyOf": [
                          { "type": "string", "maxLength": 250 },
                          { "type": "null" }
                        ]
                      },
                      "uuid": {
                        "type": "string"
                      },
                      "config": {
                        "anyOf": [
                          { "$ref": "#/components/schemas/KnowledgeBoxConfig" },
                          { "type": "null" }
                        ]
                      },
                      "model": {
                        "anyOf": [
                          { "$ref": "#/components/schemas/SemanticModelMetadata" },
                          { "type": "null" }
                        ]
                      }
                    }
                  },
                  "KnowledgeBoxConfig": {
                    "type": "object",
                    "properties": {
                      "title": { "type": "string" }
                    }
                  },
                  "SemanticModelMetadata": {
                    "type": "object",
                    "properties": {
                      "similarity_function": { "type": "string" }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(
                specWithAnyOf,
                "TestApi",
                "TestApiExtensions",
                Path.GetTempPath()
            )
        );

        // Verify anyOf properties are correctly typed - THIS PROVES BUG #142 IS FIXED
        Assert.IsTrue(result.ModelsCode.Contains("public record KnowledgeBoxObj("));
        Assert.IsTrue(
            result.ModelsCode.Contains("string? Slug"),
            $"Expected 'string? Slug' but got: {result.ModelsCode}"
        );
        Assert.IsTrue(
            result.ModelsCode.Contains("string Uuid"),
            $"Expected 'string Uuid' but got: {result.ModelsCode}"
        );
        Assert.IsTrue(
            result.ModelsCode.Contains("KnowledgeBoxConfig? Config"),
            $"Expected 'KnowledgeBoxConfig? Config' but got: {result.ModelsCode}"
        );
        Assert.IsTrue(
            result.ModelsCode.Contains("SemanticModelMetadata? Model"),
            $"Expected 'SemanticModelMetadata? Model' but got: {result.ModelsCode}"
        );
        Assert.IsFalse(result.ModelsCode.Contains("object Slug"), "Should not have 'object Slug'");
        Assert.IsFalse(
            result.ModelsCode.Contains("object Config"),
            "Should not have 'object Config'"
        );
        Assert.IsFalse(
            result.ModelsCode.Contains("object Model"),
            "Should not have 'object Model'"
        );

        AssertGenerationJourney(specWithAnyOf, result);
    }

    [TestMethod]
    public void Generate_HandlesAnyOfWithInteger()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/resource": {
                  "get": {
                    "operationId": "getResource",
                    "responses": {
                      "200": {
                        "description": "Success",
                        "content": {
                          "application/json": {
                            "schema": {
                              "$ref": "#/components/schemas/Resource"
                            }
                          }
                        }
                      }
                    }
                  }
                }
              },
              "components": {
                "schemas": {
                  "Resource": {
                    "type": "object",
                    "properties": {
                      "count": {
                        "anyOf": [
                          { "type": "integer" },
                          { "type": "null" }
                        ]
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ModelsCode.Contains("int? Count"));
        Assert.IsFalse(result.ModelsCode.Contains("object Count"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesArrayOfIntegers()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/numbers": {
                  "get": {
                    "operationId": "getNumbers",
                    "responses": {
                      "200": {
                        "description": "Success",
                        "content": {
                          "application/json": {
                            "schema": {
                              "type": "array",
                              "items": {
                                "type": "integer"
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("Result<List<int>,"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesInt64Format()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/resource/{id}": {
                  "get": {
                    "operationId": "getResource",
                    "parameters": [
                      {
                        "name": "id",
                        "in": "path",
                        "required": true,
                        "schema": {
                          "type": "integer",
                          "format": "int64"
                        }
                      }
                    ],
                    "responses": {
                      "200": {
                        "description": "Success",
                        "content": {
                          "application/json": {
                            "schema": {
                              "$ref": "#/components/schemas/Resource"
                            }
                          }
                        }
                      }
                    }
                  }
                }
              },
              "components": {
                "schemas": {
                  "Resource": {
                    "type": "object",
                    "properties": {
                      "id": {
                        "type": "integer",
                        "format": "int64"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ModelsCode.Contains("long Id"));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("long id"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesDoubleFormat()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/resource": {
                  "get": {
                    "operationId": "getResource",
                    "responses": {
                      "200": {
                        "description": "Success",
                        "content": {
                          "application/json": {
                            "schema": {
                              "$ref": "#/components/schemas/Resource"
                            }
                          }
                        }
                      }
                    }
                  }
                }
              },
              "components": {
                "schemas": {
                  "Resource": {
                    "type": "object",
                    "properties": {
                      "price": {
                        "type": "number",
                        "format": "double"
                      },
                      "rating": {
                        "type": "number"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ModelsCode.Contains("double Price"));
        Assert.IsTrue(result.ModelsCode.Contains("float Rating"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesBooleanType()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/resource": {
                  "get": {
                    "operationId": "getResource",
                    "responses": {
                      "200": {
                        "description": "Success",
                        "content": {
                          "application/json": {
                            "schema": {
                              "$ref": "#/components/schemas/Resource"
                            }
                          }
                        }
                      }
                    }
                  }
                }
              },
              "components": {
                "schemas": {
                  "Resource": {
                    "type": "object",
                    "properties": {
                      "isActive": {
                        "type": "boolean"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ModelsCode.Contains("bool IsActive"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_SkipsStringEnums()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/resource": {
                  "get": {
                    "operationId": "getResource",
                    "responses": {
                      "200": {
                        "description": "Success",
                        "content": {
                          "application/json": {
                            "schema": {
                              "$ref": "#/components/schemas/Resource"
                            }
                          }
                        }
                      }
                    }
                  }
                }
              },
              "components": {
                "schemas": {
                  "Status": {
                    "type": "string",
                    "enum": ["active", "inactive", "pending"]
                  },
                  "Resource": {
                    "type": "object",
                    "properties": {
                      "status": {
                        "$ref": "#/components/schemas/Status"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        // String enums should not generate records, and references should map to string
        Assert.IsFalse(result.ModelsCode.Contains("public record Status"));
        Assert.IsTrue(result.ModelsCode.Contains("string Status"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesArrayOfReferences()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/items": {
                  "get": {
                    "operationId": "getItems",
                    "responses": {
                      "200": {
                        "description": "Success",
                        "content": {
                          "application/json": {
                            "schema": {
                              "type": "array",
                              "items": {
                                "$ref": "#/components/schemas/Item"
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }
              },
              "components": {
                "schemas": {
                  "Item": {
                    "type": "object",
                    "properties": {
                      "name": {
                        "type": "string"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("Result<List<Item>,"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesComplexNestedStructures()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/order": {
                  "get": {
                    "operationId": "getOrder",
                    "responses": {
                      "200": {
                        "description": "Success",
                        "content": {
                          "application/json": {
                            "schema": {
                              "$ref": "#/components/schemas/Order"
                            }
                          }
                        }
                      }
                    }
                  }
                }
              },
              "components": {
                "schemas": {
                  "Order": {
                    "type": "object",
                    "properties": {
                      "id": {
                        "type": "integer",
                        "format": "int64"
                      },
                      "customer": {
                        "$ref": "#/components/schemas/Customer"
                      },
                      "items": {
                        "type": "array",
                        "items": {
                          "$ref": "#/components/schemas/OrderItem"
                        }
                      },
                      "total": {
                        "type": "number",
                        "format": "double"
                      }
                    }
                  },
                  "Customer": {
                    "type": "object",
                    "properties": {
                      "name": {
                        "type": "string"
                      },
                      "email": {
                        "type": "string"
                      }
                    }
                  },
                  "OrderItem": {
                    "type": "object",
                    "properties": {
                      "productId": {
                        "type": "integer"
                      },
                      "quantity": {
                        "type": "integer"
                      },
                      "price": {
                        "type": "number"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        // Verify Order model
        Assert.IsTrue(result.ModelsCode.Contains("public record Order("));
        Assert.IsTrue(result.ModelsCode.Contains("long Id"));
        Assert.IsTrue(result.ModelsCode.Contains("Customer Customer"));
        Assert.IsTrue(result.ModelsCode.Contains("List<OrderItem> Items"));
        Assert.IsTrue(result.ModelsCode.Contains("double Total"));

        // Verify Customer model
        Assert.IsTrue(result.ModelsCode.Contains("public record Customer("));
        Assert.IsTrue(result.ModelsCode.Contains("string Name"));
        Assert.IsTrue(result.ModelsCode.Contains("string Email"));

        // Verify OrderItem model
        Assert.IsTrue(result.ModelsCode.Contains("public record OrderItem("));
        Assert.IsTrue(result.ModelsCode.Contains("int ProductId"));
        Assert.IsTrue(result.ModelsCode.Contains("int Quantity"));
        Assert.IsTrue(result.ModelsCode.Contains("float Price"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesPutMethod()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/pets/{petId}": {
                  "put": {
                    "operationId": "updatePet",
                    "parameters": [
                      {
                        "name": "petId",
                        "in": "path",
                        "required": true,
                        "schema": {
                          "type": "integer"
                        }
                      }
                    ],
                    "requestBody": {
                      "content": {
                        "application/json": {
                          "schema": {
                            "$ref": "#/components/schemas/Pet"
                          }
                        }
                      }
                    },
                    "responses": {
                      "200": {
                        "description": "Success",
                        "content": {
                          "application/json": {
                            "schema": {
                              "$ref": "#/components/schemas/Pet"
                            }
                          }
                        }
                      }
                    }
                  }
                }
              },
              "components": {
                "schemas": {
                  "Pet": {
                    "type": "object",
                    "properties": {
                      "name": {
                        "type": "string"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("UpdatePet"));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("CreatePut"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesPatchMethod()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/pets/{petId}": {
                  "patch": {
                    "operationId": "patchPet",
                    "parameters": [
                      {
                        "name": "petId",
                        "in": "path",
                        "required": true,
                        "schema": {
                          "type": "integer"
                        }
                      }
                    ],
                    "requestBody": {
                      "content": {
                        "application/json": {
                          "schema": {
                            "$ref": "#/components/schemas/PetUpdate"
                          }
                        }
                      }
                    },
                    "responses": {
                      "200": {
                        "description": "Success",
                        "content": {
                          "application/json": {
                            "schema": {
                              "$ref": "#/components/schemas/Pet"
                            }
                          }
                        }
                      }
                    }
                  }
                }
              },
              "components": {
                "schemas": {
                  "Pet": {
                    "type": "object",
                    "properties": {
                      "name": {
                        "type": "string"
                      }
                    }
                  },
                  "PetUpdate": {
                    "type": "object",
                    "properties": {
                      "name": {
                        "type": "string"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("PatchPet"));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("CreatePatch"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesOptionalQueryParameters()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/pets": {
                  "get": {
                    "operationId": "listPets",
                    "parameters": [
                      {
                        "name": "limit",
                        "in": "query",
                        "required": false,
                        "schema": {
                          "type": "integer"
                        }
                      },
                      {
                        "name": "offset",
                        "in": "query",
                        "required": false,
                        "schema": {
                          "type": "integer"
                        }
                      }
                    ],
                    "responses": {
                      "200": {
                        "description": "Success"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("int? limit"));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("int? offset"));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("BuildQueryString"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesHeaderParameters()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/pets": {
                  "get": {
                    "operationId": "listPets",
                    "parameters": [
                      {
                        "name": "X-API-Key",
                        "in": "header",
                        "required": true,
                        "schema": {
                          "type": "string"
                        }
                      }
                    ],
                    "responses": {
                      "200": {
                        "description": "Success"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        // Just verify it compiled and generated - the actual parameter handling is implementation detail
        Assert.IsFalse(string.IsNullOrEmpty(result.ExtensionMethodsCode));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("ListPets"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesPathAndQueryParametersCombined()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/pets/{petId}/friends": {
                  "get": {
                    "operationId": "getPetFriends",
                    "parameters": [
                      {
                        "name": "petId",
                        "in": "path",
                        "required": true,
                        "schema": {
                          "type": "integer"
                        }
                      },
                      {
                        "name": "limit",
                        "in": "query",
                        "required": false,
                        "schema": {
                          "type": "integer"
                        }
                      }
                    ],
                    "responses": {
                      "200": {
                        "description": "Success"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("petId"));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("limit"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesMultilineDescriptions()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/pets": {
                  "get": {
                    "operationId": "listPets",
                    "description": "List all pets\n\n---\n\nReturns a paginated list of pets",
                    "responses": {
                      "200": {
                        "description": "Success"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        // Should only include the summary before the --- separator
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("/// <summary>List all pets</summary>"));
        Assert.IsFalse(result.ExtensionMethodsCode.Contains("Returns a paginated list"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesNoPathParameters()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/health": {
                  "get": {
                    "operationId": "getHealth",
                    "responses": {
                      "200": {
                        "description": "Success"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("GetHealth"));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("Unit.Value"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesDefaultParameterValues()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/pets": {
                  "get": {
                    "operationId": "listPets",
                    "parameters": [
                      {
                        "name": "limit",
                        "in": "query",
                        "required": false,
                        "schema": {
                          "type": "integer",
                          "default": 10
                        }
                      }
                    ],
                    "responses": {
                      "200": {
                        "description": "Success"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("limit = 10"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_SanitizesParameterNames()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/pets/{pet_id}": {
                  "get": {
                    "operationId": "getPet",
                    "parameters": [
                      {
                        "name": "pet_id",
                        "in": "path",
                        "required": true,
                        "schema": {
                          "type": "integer"
                        }
                      }
                    ],
                    "responses": {
                      "200": {
                        "description": "Success"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        // Parameter name should be converted to camelCase
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("int petId"));
        // But path template should preserve original name
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("{petId}"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesOperationWithoutOperationId()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/pets": {
                  "get": {
                    "responses": {
                      "200": {
                        "description": "Success"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        // Should generate method name from path and HTTP method
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("GetPets"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesMultipleResponseTypes()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/pets": {
                  "get": {
                    "operationId": "listPets",
                    "responses": {
                      "200": {
                        "description": "Success",
                        "content": {
                          "application/json": {
                            "schema": {
                              "$ref": "#/components/schemas/Pet"
                            }
                          }
                        }
                      },
                      "404": {
                        "description": "Not Found",
                        "content": {
                          "application/json": {
                            "schema": {
                              "$ref": "#/components/schemas/Error"
                            }
                          }
                        }
                      },
                      "500": {
                        "description": "Server Error",
                        "content": {
                          "application/json": {
                            "schema": {
                              "$ref": "#/components/schemas/Error"
                            }
                          }
                        }
                      }
                    }
                  }
                }
              },
              "components": {
                "schemas": {
                  "Pet": {
                    "type": "object",
                    "properties": {
                      "name": { "type": "string" }
                    }
                  },
                  "Error": {
                    "type": "object",
                    "properties": {
                      "message": { "type": "string" }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("Result<Pet,"));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("Error>"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesEmptyResponseContent()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/pets/{petId}": {
                  "delete": {
                    "operationId": "deletePet",
                    "parameters": [
                      {
                        "name": "petId",
                        "in": "path",
                        "required": true,
                        "schema": {
                          "type": "integer"
                        }
                      }
                    ],
                    "responses": {
                      "204": {
                        "description": "Deleted"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("DeletePet"));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("CreateDelete"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesRequiredHeaderParameters()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/pets": {
                  "get": {
                    "operationId": "listPets",
                    "parameters": [
                      {
                        "name": "Authorization",
                        "in": "header",
                        "required": true,
                        "schema": {
                          "type": "string"
                        }
                      },
                      {
                        "name": "X-Request-ID",
                        "in": "header",
                        "required": false,
                        "schema": {
                          "type": "string"
                        }
                      }
                    ],
                    "responses": {
                      "200": {
                        "description": "Success"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(
            result.ExtensionMethodsCode.Contains("authorization"),
            "Should contain 'authorization'"
        );
        Assert.IsTrue(
            result.ExtensionMethodsCode.Contains("xRequestID"),
            "Should contain 'xRequestID'"
        );

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesStringDefaultValues()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/search": {
                  "get": {
                    "operationId": "search",
                    "parameters": [
                      {
                        "name": "query",
                        "in": "query",
                        "required": false,
                        "schema": {
                          "type": "string",
                          "default": "test"
                        }
                      }
                    ],
                    "responses": {
                      "200": {
                        "description": "Success"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("query = \"test\""));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesOperationWithOnlyErrorResponse()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/test": {
                  "get": {
                    "operationId": "testOp",
                    "responses": {
                      "400": {
                        "description": "Bad Request",
                        "content": {
                          "application/json": {
                            "schema": {
                              "$ref": "#/components/schemas/Error"
                            }
                          }
                        }
                      }
                    }
                  }
                }
              },
              "components": {
                "schemas": {
                  "Error": {
                    "type": "object",
                    "properties": {
                      "message": { "type": "string" }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("TestOp"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesRequiredQueryParameters()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/search": {
                  "get": {
                    "operationId": "search",
                    "parameters": [
                      {
                        "name": "query",
                        "in": "query",
                        "required": true,
                        "schema": {
                          "type": "string"
                        }
                      }
                    ],
                    "responses": {
                      "200": {
                        "description": "Success"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        // Required query params should not be nullable
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("string query"));
        Assert.IsFalse(result.ExtensionMethodsCode.Contains("string? query"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesPostWithoutRequestBody()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/action": {
                  "post": {
                    "operationId": "performAction",
                    "responses": {
                      "200": {
                        "description": "Success"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("PerformAction"));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("CreatePost"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesBooleanDefaultValue()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/items": {
                  "get": {
                    "operationId": "listItems",
                    "parameters": [
                      {
                        "name": "includeDeleted",
                        "in": "query",
                        "required": false,
                        "schema": {
                          "type": "boolean",
                          "default": false
                        }
                      }
                    ],
                    "responses": {
                      "200": {
                        "description": "Success"
                      }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("includeDeleted = false"));

        AssertGenerationJourney(spec, result);
    }

    [TestMethod]
    public void Generate_HandlesPathWithMultipleOperations()
    {
        var spec = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test API", "version": "1.0.0" },
              "servers": [{ "url": "https://api.test.com" }],
              "paths": {
                "/pets/{petId}": {
                  "get": {
                    "operationId": "getPet",
                    "parameters": [
                      {
                        "name": "petId",
                        "in": "path",
                        "required": true,
                        "schema": { "type": "integer" }
                      }
                    ],
                    "responses": {
                      "200": { "description": "Success" }
                    }
                  },
                  "put": {
                    "operationId": "updatePet",
                    "parameters": [
                      {
                        "name": "petId",
                        "in": "path",
                        "required": true,
                        "schema": { "type": "integer" }
                      }
                    ],
                    "responses": {
                      "200": { "description": "Success" }
                    }
                  },
                  "delete": {
                    "operationId": "deletePet",
                    "parameters": [
                      {
                        "name": "petId",
                        "in": "path",
                        "required": true,
                        "schema": { "type": "integer" }
                      }
                    ],
                    "responses": {
                      "204": { "description": "Deleted" }
                    }
                  }
                }
              }
            }
            """;

        var result = GetSuccessResult(
            OpenApiCodeGenerator.Generate(spec, "TestApi", "TestApiExtensions", Path.GetTempPath())
        );

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("GetPet"));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("UpdatePet"));
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("DeletePet"));

        AssertGenerationJourney(spec, result);
    }
}
