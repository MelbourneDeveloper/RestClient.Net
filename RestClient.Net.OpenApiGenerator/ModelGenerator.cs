namespace RestClient.Net.OpenApiGenerator;

/// <summary>Generates C# model classes from OpenAPI schemas.</summary>
public static class ModelGenerator
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
                    // Skip string enums - they'll be mapped to string type
                    if (IsStringEnum(schemaObj))
                    {
                        continue;
                    }

                    var className = CodeGenerationHelpers.ToPascalCase(schema.Key);
                    var model = GenerateModel(className, schemaObj, document.Components.Schemas);
                    models.Add(model);
                }
            }
        }

        var modelsCode = string.Join("\n\n", models);

        return $$"""
            #nullable enable
            namespace {{@namespace}};

            {{modelsCode}}
            """;
    }

    /// <summary>Checks if a schema is a string enum.</summary>
    /// <param name="schema">The schema to check.</param>
    /// <returns>True if the schema is a string enum.</returns>
    private static bool IsStringEnum(OpenApiSchema schema) =>
        schema.Type == JsonSchemaType.String && schema.Enum != null && schema.Enum.Count > 0;

    /// <summary>Generates a single C# model record from an OpenAPI schema.</summary>
    /// <param name="name">The name of the model.</param>
    /// <param name="schema">The OpenAPI schema.</param>
    /// <param name="schemas">Optional schemas dictionary to check for string enums.</param>
    /// <returns>The generated model code.</returns>
    private static string GenerateModel(
        string name,
        OpenApiSchema schema,
        IDictionary<string, IOpenApiSchema>? schemas = null
    )
    {
        var parameters = (schema.Properties ?? new Dictionary<string, IOpenApiSchema>())
            .Select(p =>
            {
                var propName = CodeGenerationHelpers.ToPascalCase(p.Key);

                // Avoid property name conflict with record name
                if (propName.Equals(name, StringComparison.Ordinal))
                {
                    propName += "Value";
                }

                var propType = MapOpenApiType(p.Value, schemas);
                var propDesc = SanitizeDescription(
                    (p.Value as OpenApiSchema)?.Description ?? propName
                );
                return (
                    ParamDoc: $"/// <param name=\"{propName}\">{propDesc}</param>",
                    ParamDecl: $"{propType} {propName}"
                );
            })
            .ToList();

        var paramDocs = string.Join("\n", parameters.Select(p => p.ParamDoc));
        var paramDecls = string.Join(", ", parameters.Select(p => p.ParamDecl));
        var recordDesc = SanitizeDescription(schema.Description ?? name);

        return $$"""
            /// <summary>{{recordDesc}}</summary>
            {{paramDocs}}
            public record {{name}}({{paramDecls}});
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
    /// <param name="schemas">Optional schemas dictionary to check for string enums.</param>
    /// <returns>The C# type name.</returns>
    public static string MapOpenApiType(
        IOpenApiSchema? schema,
        IDictionary<string, IOpenApiSchema>? schemas = null
    )
    {
        if (schema == null)
        {
            return "object";
        }

        // Check for schema reference first
        if (schema is OpenApiSchemaReference schemaRef)
        {
            // Return "string" if this is a reference to a string enum, otherwise return the class name
            return schemaRef.Reference.Id == null ? "object"
                : schemas != null
                && schemas.TryGetValue(schemaRef.Reference.Id, out var referencedSchema)
                && referencedSchema is OpenApiSchema refSchemaObj
                && IsStringEnum(refSchemaObj)
                    ? "string"
                : CodeGenerationHelpers.ToPascalCase(schemaRef.Reference.Id);
        }

        if (schema is not OpenApiSchema schemaObj)
        {
            return "object";
        }

        // Handle anyOf schemas
        if (schemaObj.AnyOf != null && schemaObj.AnyOf.Count > 0)
        {
            // Filter out null types and get the first non-null type
            // Handle both OpenApiSchema with Type != Null and OpenApiSchemaReference
            var nonNullSchema = schemaObj.AnyOf.FirstOrDefault(s =>
                s is OpenApiSchemaReference
                || (s is OpenApiSchema os && os.Type != JsonSchemaType.Null)
            );

            if (nonNullSchema != null)
            {
                // Recursively resolve the type
                var baseType = MapOpenApiType(nonNullSchema, schemas);
                // Make it nullable since it's in an anyOf with null
                return baseType.EndsWith('?') ? baseType : $"{baseType}?";
            }
        }

        // Handle arrays
        if (schemaObj.Type == JsonSchemaType.Array)
        {
            return schemaObj.Items is OpenApiSchemaReference itemsRef
                ? itemsRef.Reference.Id == null
                    ? "List<object>"
                    : schemas != null
                    && schemas.TryGetValue(itemsRef.Reference.Id, out var itemsSchema)
                    && itemsSchema is OpenApiSchema itemsSchemaObj
                    && IsStringEnum(itemsSchemaObj)
                        ? "List<string>"
                        : $"List<{CodeGenerationHelpers.ToPascalCase(itemsRef.Reference.Id)}>"
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
