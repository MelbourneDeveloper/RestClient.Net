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
    }

    [TestMethod]
    public void Generate_WritesFilesToOutputPath()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        try
        {
            _ = GetSuccessResult(
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
                "private static readonly Deserialize<Unit> _deserializeUnit"
            )
        );

        // Verify that public methods call the private delegates with HttpClient as first parameter
        Assert.IsTrue(
            result.ExtensionMethodsCode.Contains("(httpClient,")
                && result.ExtensionMethodsCode.Contains(", cancellationToken)")
        );
    }
}
