using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;
using RestClient.Net.OpenApiGenerator;

// Note: These tests are supplemental to the comprehensive integration tests in OpenApiCodeGeneratorTests.cs
namespace RestClient.Net.OpenApiGenerator.Tests;

[TestClass]
public sealed class ModelGeneratorTests
{
    private static OpenApiSchemaReference CreateSchemaRefWithNullId()
    {
        var schemaRef = new OpenApiSchemaReference("PlaceholderForReflection", null!, null!);
        var referenceProperty = typeof(OpenApiSchemaReference).GetProperty("Reference");
        var reference = referenceProperty?.GetValue(schemaRef);
        if (reference != null)
        {
            var idField = reference.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
            if (idField != null && idField.CanWrite)
            {
                idField.SetValue(reference, null);
            }
        }

        return schemaRef;
    }

    [TestMethod]
    public void MapOpenApiType_WithNullSchema_ReturnsObject()
    {
        var result = ModelGenerator.MapOpenApiType(null, null);

        Assert.AreEqual("object", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithStringType_ReturnsString()
    {
        var schema = new OpenApiSchema { Type = JsonSchemaType.String };

        var result = ModelGenerator.MapOpenApiType(schema, null);

        Assert.AreEqual("string", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithIntegerType_ReturnsInt()
    {
        var schema = new OpenApiSchema { Type = JsonSchemaType.Integer };

        var result = ModelGenerator.MapOpenApiType(schema, null);

        Assert.AreEqual("int", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithIntegerTypeInt64Format_ReturnsLong()
    {
        var schema = new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int64" };

        var result = ModelGenerator.MapOpenApiType(schema, null);

        Assert.AreEqual("long", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithNumberType_ReturnsFloat()
    {
        var schema = new OpenApiSchema { Type = JsonSchemaType.Number };

        var result = ModelGenerator.MapOpenApiType(schema, null);

        Assert.AreEqual("float", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithNumberTypeDoubleFormat_ReturnsDouble()
    {
        var schema = new OpenApiSchema { Type = JsonSchemaType.Number, Format = "double" };

        var result = ModelGenerator.MapOpenApiType(schema, null);

        Assert.AreEqual("double", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithBooleanType_ReturnsBool()
    {
        var schema = new OpenApiSchema { Type = JsonSchemaType.Boolean };

        var result = ModelGenerator.MapOpenApiType(schema, null);

        Assert.AreEqual("bool", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithArrayOfStrings_ReturnsListOfString()
    {
        var schema = new OpenApiSchema
        {
            Type = JsonSchemaType.Array,
            Items = new OpenApiSchema { Type = JsonSchemaType.String },
        };

        var result = ModelGenerator.MapOpenApiType(schema, null);

        Assert.AreEqual("List<string>", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithArrayOfIntegers_ReturnsListOfInt()
    {
        var schema = new OpenApiSchema
        {
            Type = JsonSchemaType.Array,
            Items = new OpenApiSchema { Type = JsonSchemaType.Integer },
        };

        var result = ModelGenerator.MapOpenApiType(schema, null);

        Assert.AreEqual("List<int>", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithArrayOfNumbers_ReturnsListOfDouble()
    {
        var schema = new OpenApiSchema
        {
            Type = JsonSchemaType.Array,
            Items = new OpenApiSchema { Type = JsonSchemaType.Number },
        };

        var result = ModelGenerator.MapOpenApiType(schema, null);

        Assert.AreEqual("List<double>", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithArrayOfReferences_ReturnsListOfClassName()
    {
        var schema = new OpenApiSchema
        {
            Type = JsonSchemaType.Array,
            Items = new OpenApiSchemaReference("Pet", null!, null!),
        };

        var result = ModelGenerator.MapOpenApiType(schema, null);

        Assert.AreEqual("List<Pet>", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithArrayOfReferencesWithEmptyId_ReturnsListOfObject()
    {
        var schema = new OpenApiSchema
        {
            Type = JsonSchemaType.Array,
            Items = CreateSchemaRefWithNullId(),
        };

        var result = ModelGenerator.MapOpenApiType(schema, null);

        Assert.AreEqual("List<object>", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithArrayOfUnknownType_ReturnsListOfObject()
    {
        var schema = new OpenApiSchema
        {
            Type = JsonSchemaType.Array,
            Items = new OpenApiSchema { Type = JsonSchemaType.Object },
        };

        var result = ModelGenerator.MapOpenApiType(schema, null);

        Assert.AreEqual("List<object>", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithSchemaReference_ReturnsClassName()
    {
        var schemaRef = new OpenApiSchemaReference("Pet", null!, null!);

        var result = ModelGenerator.MapOpenApiType(schemaRef, null);

        Assert.AreEqual("Pet", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithSchemaReferenceWithEmptyId_ReturnsObject()
    {
        var schemaRef = CreateSchemaRefWithNullId();

        var result = ModelGenerator.MapOpenApiType(schemaRef, null);

        Assert.AreEqual("object", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithSchemaReferenceToStringEnum_ReturnsString()
    {
        var schemas = new Dictionary<string, IOpenApiSchema>
        {
            ["Status"] = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Enum = [JsonValue.Create("active")!, JsonValue.Create("inactive")!],
            },
        };
        var schemaRef = new OpenApiSchemaReference("Status", null!, null!);

        var result = ModelGenerator.MapOpenApiType(schemaRef, schemas);

        Assert.AreEqual("string", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithSchemaReferenceToNonEnum_ReturnsClassName()
    {
        var schemas = new Dictionary<string, IOpenApiSchema>
        {
            ["Pet"] = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>(),
            },
        };
        var schemaRef = new OpenApiSchemaReference("Pet", null!, null!);

        var result = ModelGenerator.MapOpenApiType(schemaRef, schemas);

        Assert.AreEqual("Pet", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithArrayOfStringEnumReferences_ReturnsListOfString()
    {
        var schemas = new Dictionary<string, IOpenApiSchema>
        {
            ["Status"] = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Enum = [JsonValue.Create("active")!, JsonValue.Create("inactive")!],
            },
        };
        var schema = new OpenApiSchema
        {
            Type = JsonSchemaType.Array,
            Items = new OpenApiSchemaReference("Status", null!, null!),
        };

        var result = ModelGenerator.MapOpenApiType(schema, schemas);

        Assert.AreEqual("List<string>", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithAnyOfSchemaReferenceAndNull_ReturnsNullableClassName()
    {
        var schemas = new Dictionary<string, IOpenApiSchema>
        {
            ["Config"] = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>(),
            },
        };
        var schema = new OpenApiSchema
        {
            AnyOf =
            [
                new OpenApiSchemaReference("Config", null!, null!),
                new OpenApiSchema { Type = JsonSchemaType.Null },
            ],
        };

        var result = ModelGenerator.MapOpenApiType(schema, schemas);

        Assert.AreEqual("Config?", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithAnyOfStringAndNull_ReturnsNullableString()
    {
        var schema = new OpenApiSchema
        {
            AnyOf =
            [
                new OpenApiSchema { Type = JsonSchemaType.String },
                new OpenApiSchema { Type = JsonSchemaType.Null },
            ],
        };

        var result = ModelGenerator.MapOpenApiType(schema, null);

        Assert.AreEqual("string?", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithAnyOfIntegerAndNull_ReturnsNullableInt()
    {
        var schema = new OpenApiSchema
        {
            AnyOf =
            [
                new OpenApiSchema { Type = JsonSchemaType.Integer },
                new OpenApiSchema { Type = JsonSchemaType.Null },
            ],
        };

        var result = ModelGenerator.MapOpenApiType(schema, null);

        Assert.AreEqual("int?", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithAnyOfOnlyNull_ReturnsObject()
    {
        var schema = new OpenApiSchema
        {
            AnyOf = [new OpenApiSchema { Type = JsonSchemaType.Null }],
        };

        var result = ModelGenerator.MapOpenApiType(schema, null);

        Assert.AreEqual("object", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithEmptyAnyOf_ReturnsObject()
    {
        var schema = new OpenApiSchema { AnyOf = [] };

        var result = ModelGenerator.MapOpenApiType(schema, null);

        Assert.AreEqual("object", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithUnknownType_ReturnsObject()
    {
        var schema = new OpenApiSchema { Type = JsonSchemaType.Object };

        var result = ModelGenerator.MapOpenApiType(schema, null);

        Assert.AreEqual("object", result);
    }

    [TestMethod]
    public void GenerateModels_WithSimpleSchema_GeneratesRecord()
    {
        var document = new OpenApiDocument
        {
            Components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, IOpenApiSchema>
                {
                    ["Pet"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,
                        Description = "A pet",
                        Properties = new Dictionary<string, IOpenApiSchema>
                        {
                            ["name"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String,
                                Description = "Pet name",
                            },
                            ["age"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.Integer,
                                Description = "Pet age",
                            },
                        },
                    },
                },
            },
        };

        var result = ModelGenerator.GenerateModels(document, "TestNamespace");

        Assert.IsTrue(result.Contains("namespace TestNamespace;"));
        Assert.IsTrue(result.Contains("public record Pet("));
        Assert.IsTrue(result.Contains("string Name"));
        Assert.IsTrue(result.Contains("int Age"));
    }

    [TestMethod]
    public void GenerateModels_WithStringEnum_SkipsGeneration()
    {
        var document = new OpenApiDocument
        {
            Components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, IOpenApiSchema>
                {
                    ["Status"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String,
                        Enum = [JsonValue.Create("active")!, JsonValue.Create("inactive")!],
                    },
                },
            },
        };

        var result = ModelGenerator.GenerateModels(document, "TestNamespace");

        Assert.IsFalse(result.Contains("internal record Status"));
    }

    [TestMethod]
    public void GenerateModels_WithPropertyNameMatchingRecordName_AppendsValue()
    {
        var document = new OpenApiDocument
        {
            Components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, IOpenApiSchema>
                {
                    ["Pet"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,
                        Properties = new Dictionary<string, IOpenApiSchema>
                        {
                            ["pet"] = new OpenApiSchema { Type = JsonSchemaType.String },
                        },
                    },
                },
            },
        };

        var result = ModelGenerator.GenerateModels(document, "TestNamespace");

        Assert.IsTrue(result.Contains("string PetValue"));
        Assert.IsFalse(result.Contains("string Pet,"));
        Assert.IsFalse(result.Contains("string Pet)"));
    }

    [TestMethod]
    public void GenerateModels_WithMultilineDescription_SanitizesToSingleLine()
    {
        var document = new OpenApiDocument
        {
            Components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, IOpenApiSchema>
                {
                    ["Pet"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,
                        Description = "A pet\nWith multiple\r\nlines",
                        Properties = new Dictionary<string, IOpenApiSchema>
                        {
                            ["name"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String,
                                Description = "Pet\nname\r\ndescription",
                            },
                        },
                    },
                },
            },
        };

        var result = ModelGenerator.GenerateModels(document, "TestNamespace");

        Assert.IsTrue(result.Contains("/// <summary>A pet With multiple lines</summary>"));
        Assert.IsTrue(result.Contains("/// <param name=\"Name\">Pet name description</param>"));
        Assert.IsFalse(result.Contains("\r\n"));
    }

    [TestMethod]
    public void GenerateModels_WithNoComponents_ReturnsEmptyNamespace()
    {
        var document = new OpenApiDocument { Components = null };

        var result = ModelGenerator.GenerateModels(document, "TestNamespace");

        Assert.IsTrue(result.Contains("namespace TestNamespace;"));
        Assert.IsFalse(result.Contains("internal record"));
    }

    [TestMethod]
    public void GenerateModels_WithNoSchemas_ReturnsEmptyNamespace()
    {
        var document = new OpenApiDocument
        {
            Components = new OpenApiComponents { Schemas = null },
        };

        var result = ModelGenerator.GenerateModels(document, "TestNamespace");

        Assert.IsTrue(result.Contains("namespace TestNamespace;"));
        Assert.IsFalse(result.Contains("internal record"));
    }

    [TestMethod]
    public void GenerateModels_WithNullableEnabled_GeneratesNullableDirective()
    {
        var document = new OpenApiDocument
        {
            Components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, IOpenApiSchema>(),
            },
        };

        var result = ModelGenerator.GenerateModels(document, "TestNamespace");

        Assert.IsTrue(result.StartsWith("#nullable enable", StringComparison.Ordinal));
    }

    [TestMethod]
    public void MapOpenApiType_WithAnyOfStringEnumReferenceAndNull_ReturnsNullableString()
    {
        var schemas = new Dictionary<string, IOpenApiSchema>
        {
            ["Status"] = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Enum = [JsonValue.Create("active")!, JsonValue.Create("inactive")!],
            },
        };
        var schema = new OpenApiSchema
        {
            AnyOf =
            [
                new OpenApiSchemaReference("Status", null!, null!),
                new OpenApiSchema { Type = JsonSchemaType.Null },
            ],
        };

        var result = ModelGenerator.MapOpenApiType(schema, schemas);

        // String enums are mapped to string, so anyOf with null should give string?
        Assert.AreEqual("string?", result);
    }

    [TestMethod]
    public void MapOpenApiType_WithAnyOfAlreadyNullableType_DoesNotAddDoubleQuestionMark()
    {
        var schema = new OpenApiSchema
        {
            AnyOf =
            [
                new OpenApiSchemaReference("Config", null!, null!),
                new OpenApiSchema { Type = JsonSchemaType.Null },
            ],
        };

        var result = ModelGenerator.MapOpenApiType(schema, null);

        Assert.AreEqual("Config?", result);
        Assert.IsFalse(result.Contains("??"));
    }
}
