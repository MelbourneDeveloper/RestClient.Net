using Microsoft.OpenApi.Models;

namespace RestClient.Net.OpenApiGenerator;

/// <summary>Generates C# model classes from OpenAPI schemas.</summary>
internal static class ModelGenerator
{
    /// <summary>Generates C# models from an OpenAPI document.</summary>
    /// <param name="document">The OpenAPI document.</param>
    /// <param name="namespace">The namespace for the generated models.</param>
    /// <returns>The generated models code.</returns>
    public static string GenerateModels(OpenApiDocument document, string @namespace)
    {
        var models = new List<string>();

        foreach (
            var schema in document.Components?.Schemas ?? new Dictionary<string, OpenApiSchema>()
        )
        {
            var model = GenerateModel(schema.Key, schema.Value);
            models.Add(model);
        }

        var modelsCode = string.Join("\n\n", models);

        return $$"""
            namespace {{@namespace}};

            {{modelsCode}}
            """;
    }

    /// <summary>Generates a single C# model class from an OpenAPI schema.</summary>
    /// <param name="name">The name of the model.</param>
    /// <param name="schema">The OpenAPI schema.</param>
    /// <returns>The generated model code.</returns>
    private static string GenerateModel(string name, OpenApiSchema schema)
    {
        var properties = schema
            .Properties.Select(p =>
            {
                var propName = CodeGenerationHelpers.ToPascalCase(p.Key);
                var propType = MapOpenApiType(p.Value);
                var propDesc = p.Value.Description ?? propName;
                return $"    /// <summary>{propDesc}</summary>\n    public {propType} {propName} {{ get; set; }}";
            })
            .ToList();

        var propertiesCode = string.Join("\n\n", properties);

        return $$"""
            /// <summary>{{schema.Description ?? name}}</summary>
            public class {{name}}
            {
            {{propertiesCode}}
            }
            """;
    }

    /// <summary>Maps an OpenAPI schema to a C# type.</summary>
    /// <param name="schema">The OpenAPI schema.</param>
    /// <returns>The C# type name.</returns>
    public static string MapOpenApiType(OpenApiSchema? schema)
    {
        if (schema == null)
        {
            return "object";
        }

        // Check for schema reference first
        if (schema.Reference != null)
        {
            return schema.Reference.Id;
        }

        // Handle arrays
        if (schema.Type == "array")
        {
            return schema.Items?.Reference != null ? $"List<{schema.Items.Reference.Id}>"
                : schema.Items?.Type == "string" ? "List<string>"
                : schema.Items?.Type == "integer" ? "List<int>"
                : schema.Items?.Type == "number" ? "List<double>"
                : "List<object>";
        }

        // Handle primitive types
        return schema.Type switch
        {
            "integer" => schema.Format == "int64" ? "long" : "int",
            "number" => schema.Format == "double" ? "double" : "float",
            "string" => "string",
            "boolean" => "bool",
            _ => "object",
        };
    }
}
