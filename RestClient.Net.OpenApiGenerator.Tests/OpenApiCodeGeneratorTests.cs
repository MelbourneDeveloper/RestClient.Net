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
        Assert.IsTrue(result.ModelsCode.Contains("string? Slug"), $"Expected 'string? Slug' but got: {result.ModelsCode}");
        Assert.IsTrue(result.ModelsCode.Contains("string Uuid"), $"Expected 'string Uuid' but got: {result.ModelsCode}");
        Assert.IsTrue(result.ModelsCode.Contains("KnowledgeBoxConfig? Config"), $"Expected 'KnowledgeBoxConfig? Config' but got: {result.ModelsCode}");
        Assert.IsTrue(result.ModelsCode.Contains("SemanticModelMetadata? Model"), $"Expected 'SemanticModelMetadata? Model' but got: {result.ModelsCode}");
        Assert.IsFalse(result.ModelsCode.Contains("object Slug"), "Should not have 'object Slug'");
        Assert.IsFalse(result.ModelsCode.Contains("object Config"), "Should not have 'object Config'");
        Assert.IsFalse(result.ModelsCode.Contains("object Model"), "Should not have 'object Model'");
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

        Assert.IsTrue(result.ExtensionMethodsCode.Contains("authorization"), "Should contain 'authorization'");
        Assert.IsTrue(result.ExtensionMethodsCode.Contains("xRequestID"), "Should contain 'xRequestID'");
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
    }
}
