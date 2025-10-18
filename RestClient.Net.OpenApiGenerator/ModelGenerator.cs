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

        if (document.Components?.Schemas != null)
        {
            foreach (var schema in document.Components.Schemas)
            {
                if (schema.Value is OpenApiSchema schemaObj)
                {
                    var className = CodeGenerationHelpers.ToPascalCase(schema.Key);
                    var model = GenerateModel(className, schemaObj);
                    models.Add(model);
                }
            }
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
        var properties = (schema.Properties ?? new Dictionary<string, IOpenApiSchema>())
            .Select(p =>
            {
                var propName = CodeGenerationHelpers.ToPascalCase(p.Key);

                // Avoid property name conflict with class name
                if (propName.Equals(name, StringComparison.Ordinal))
                {
                    propName += "Value";
                }

                var propType = MapOpenApiType(p.Value);
                var propDesc = SanitizeDescription(
                    (p.Value as OpenApiSchema)?.Description ?? propName
                );
                return $"    /// <summary>{propDesc}</summary>\n    public {propType} {propName} {{ get; set; }}";
            })
            .ToList();

        var propertiesCode = string.Join("\n\n", properties);
        var classDesc = SanitizeDescription(schema.Description ?? name);

        return $$"""
            /// <summary>{{classDesc}}</summary>
            public class {{name}}
            {
            {{propertiesCode}}
            }
            """;
    }

    /// <summary>Sanitizes a description for use in XML comments.</summary>
    /// <param name="description">The description to sanitize.</param>
    /// <returns>A single-line description safe for XML comments.</returns>
    private static string SanitizeDescription(string description) =>
        description
            .Replace("\r\n", " ", StringComparison.Ordinal)
            .Replace("\n", " ", StringComparison.Ordinal)
            .Replace("\r", " ", StringComparison.Ordinal)
            .Trim();

    /// <summary>Maps an OpenAPI schema to a C# type.</summary>
    /// <param name="schema">The OpenAPI schema.</param>
    /// <returns>The C# type name.</returns>
    public static string MapOpenApiType(IOpenApiSchema? schema)
    {
        if (schema == null)
        {
            return "object";
        }

        // Check for schema reference first
        if (schema is OpenApiSchemaReference schemaRef)
        {
            return schemaRef.Reference.Id != null
                ? CodeGenerationHelpers.ToPascalCase(schemaRef.Reference.Id)
                : "object";
        }

        if (schema is not OpenApiSchema schemaObj)
        {
            return "object";
        }

        // Handle arrays
        if (schemaObj.Type == JsonSchemaType.Array)
        {
            return schemaObj.Items is OpenApiSchemaReference itemsRef
                    ? $"List<{(itemsRef.Reference.Id != null ? CodeGenerationHelpers.ToPascalCase(itemsRef.Reference.Id) : "object")}>"
                : schemaObj.Items is OpenApiSchema items
                    ? items.Type switch
                    {
                        JsonSchemaType.String => "List<string>",
                        JsonSchemaType.Integer => "List<int>",
                        JsonSchemaType.Number => "List<double>",
                        _ => "List<object>",
                    }
                : "List<object>";
        }

        // Handle primitive types
        return schemaObj.Type switch
        {
            JsonSchemaType.Integer => schemaObj.Format == "int64" ? "long" : "int",
            JsonSchemaType.Number => schemaObj.Format == "double" ? "double" : "float",
            JsonSchemaType.String => "string",
            JsonSchemaType.Boolean => "bool",
            _ => "object",
        };
    }
}
