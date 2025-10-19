namespace RestClient.Net.McpGenerator;

internal readonly record struct McpParameterInfo(
    string Name,
    string Type,
    string Description,
    bool Required,
    string? DefaultValue,
    bool IsPath,
    bool IsHeader
);

/// <summary>Generates MCP tool classes that use RestClient.Net extensions.</summary>
internal static class McpToolGenerator
{
    /// <summary>Generates MCP tools that wrap generated extension methods.</summary>
    /// <param name="document">The OpenAPI document.</param>
    /// <param name="namespace">The namespace for the MCP server.</param>
    /// <param name="serverName">The MCP server name.</param>
    /// <param name="extensionsNamespace">The namespace of the extensions.</param>
    /// <returns>The generated MCP tools code.</returns>
    public static string GenerateTools(
        OpenApiDocument document,
        string @namespace,
        string serverName,
        string extensionsNamespace
    )
    {
        var tools = new List<string>();
        var methodNameCounts = new Dictionary<string, int>();

        foreach (var path in document.Paths)
        {
            if (path.Value?.Operations == null)
            {
                continue;
            }

            foreach (var operation in path.Value.Operations)
            {
                var toolMethod = GenerateTool(
                    path.Key,
                    operation.Key,
                    operation.Value,
                    document.Components?.Schemas,
                    methodNameCounts
                );

                if (!string.IsNullOrEmpty(toolMethod))
                {
                    tools.Add(toolMethod);
                }
            }
        }

        var toolsCode = string.Join("\n\n    ", tools);

        return $$"""
            #nullable enable
            using System.ComponentModel;
            using System.Text.Json;
            using Outcome;
            using {{extensionsNamespace}};

            namespace {{@namespace}};

            /// <summary>MCP server tools for {{serverName}} API.</summary>
            public class {{serverName}}Tools(IHttpClientFactory httpClientFactory)
            {
                private static readonly JsonSerializerOptions JsonOptions = new()
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                {{toolsCode}}
            }
            """;
    }

    private static string GenerateTool(
        string path,
        HttpMethod operationType,
        OpenApiOperation operation,
        IDictionary<string, IOpenApiSchema>? schemas,
        Dictionary<string, int> methodNameCounts
    )
    {
        var extensionMethodName = GetExtensionMethodName(operation, operationType, path);

        if (methodNameCounts.TryGetValue(extensionMethodName, out var count))
        {
            methodNameCounts[extensionMethodName] = count + 1;
            extensionMethodName = $"{extensionMethodName}{count + 1}";
        }
        else
        {
            methodNameCounts[extensionMethodName] = 1;
        }

        var mcpToolName = extensionMethodName.Replace(
            "Async",
            string.Empty,
            StringComparison.Ordinal
        );
        var parameters = GetParameters(operation, schemas);
        // Match ExtensionMethodGenerator behavior: POST/PUT/PATCH always have body
        var hasBody =
            operationType == HttpMethod.Post
            || operationType == HttpMethod.Put
            || operationType == HttpMethod.Patch;
        var bodyType = GetRequestBodyType(operation) ?? "object";
        var responseType = GetResponseType(operation);
        var errorType = GetErrorType(operation);
        var isDelete = operationType == HttpMethod.Delete;
        var resultResponseType = isDelete ? "Unit" : responseType;

        // Build the full response type name for alias lookup
        // When error type is not "string", append it to response type (e.g., "KnowledgeBoxObjHTTPValidationError")
        var fullResponseType =
            errorType != "string" ? $"{resultResponseType}{errorType}" : resultResponseType;

        var summary = operation.Description ?? operation.Summary ?? $"{mcpToolName} operation";

        return GenerateToolMethod(
            mcpToolName,
            extensionMethodName,
            summary,
            parameters,
            hasBody,
            bodyType,
            fullResponseType,
            errorType
        );
    }

    private static string GenerateToolMethod(
        string toolName,
        string extensionMethodName,
        string summary,
        List<McpParameterInfo> parameters,
        bool hasBody,
        string bodyType,
        string responseType,
        string errorType
    )
    {
        var methodParams = new List<string>();
        var extensionCallArgs = new List<string>();

        // Separate required and optional parameters
        // A parameter is optional if it has a default value OR is nullable, regardless of Required flag
        var optionalParams = parameters
            .Where(p => p.DefaultValue != null || p.Type.Contains('?', StringComparison.Ordinal))
            .ToList();
        var requiredParams = parameters.Except(optionalParams).ToList();

        // Add required parameters first
        foreach (var param in requiredParams)
        {
            methodParams.Add($"{param.Type} {param.Name}");
            extensionCallArgs.Add(param.Name);
        }

        // Add body if required (body is always required when present)
        if (hasBody)
        {
            methodParams.Add($"{bodyType} body");
            extensionCallArgs.Add("body");
        }

        // Add optional parameters last
        foreach (var param in optionalParams)
        {
            methodParams.Add(FormatParameter(param));

            // For optional strings with default "", use null coalescing
            if (param.Type == "string?" && string.IsNullOrEmpty(param.DefaultValue))
            {
                extensionCallArgs.Add($"{param.Name} ?? \"\"");
            }
            else
            {
                extensionCallArgs.Add(param.Name);
            }
        }

        var paramDescriptions = string.Join(
            "\n    ",
            parameters.Select(p =>
                $"/// <param name=\"{p.Name}\">{SanitizeDescription(p.Description)}</param>"
            )
        );

        if (hasBody)
        {
            paramDescriptions += "\n    /// <param name=\"body\">Request body</param>";
        }

        var methodParamsStr =
            methodParams.Count > 0 ? string.Join(", ", methodParams) : string.Empty;

        // Always add CancellationToken.None as last parameter
        extensionCallArgs.Add("CancellationToken.None");

        var extensionCallArgsStr = string.Join(", ", extensionCallArgs);

        var okAlias = $"Ok{responseType}";
        var errorAlias = $"Error{responseType}";

        return $$"""
            /// <summary>{{SanitizeDescription(summary)}}</summary>
                {{paramDescriptions}}
                [Description("{{SanitizeDescription(summary)}}")]
                public async Task<string> {{toolName}}({{methodParamsStr}})
                {
                    var httpClient = httpClientFactory.CreateClient();
                    var result = await httpClient.{{extensionMethodName}}({{extensionCallArgsStr}});

                    return result switch
                    {
                        {{okAlias}}(var success) =>
                            JsonSerializer.Serialize(success, JsonOptions),
                        {{errorAlias}}(var httpError) => httpError switch
                        {
                            HttpError<{{errorType}}>.ErrorResponseError err =>
                                $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                            HttpError<{{errorType}}>.ExceptionError err =>
                                $"Exception: {err.Exception.Message}",
                            _ => "Unknown error"
                        },
                        _ => "Unknown result"
                    };
                }
            """;
    }

    private static string FormatParameter(McpParameterInfo param)
    {
        var isNullable = param.Type.Contains('?', StringComparison.Ordinal);

        var defaultPart =
            isNullable ? " = null"
            : param.DefaultValue != null
                ? param.Type switch
                {
                    var t when t.StartsWith("string", StringComparison.Ordinal) =>
                        $" = \"{param.DefaultValue}\"",
                    var t when t.StartsWith("bool", StringComparison.Ordinal) =>
                        param.DefaultValue.Equals("true", StringComparison.OrdinalIgnoreCase)
                            ? " = true"
                            : " = false",
                    _ => $" = {param.DefaultValue}",
                }
            : string.Empty;

        return $"{param.Type} {param.Name}{defaultPart}";
    }

    private static List<McpParameterInfo> GetParameters(
        OpenApiOperation operation,
        IDictionary<string, IOpenApiSchema>? schemas
    )
    {
        var parameters = new List<McpParameterInfo>();

        if (operation.Parameters == null)
        {
            return parameters;
        }

        foreach (var param in operation.Parameters)
        {
            if (param.Name == null)
            {
                continue;
            }

            var sanitizedName = CodeGenerationHelpers.ToCamelCase(param.Name);
            var baseType = ModelGenerator.MapOpenApiType(param.Schema, schemas);
            var required = param.Required;
            var description = param.Description ?? sanitizedName;
            var isPath = param.In == ParameterLocation.Path;
            var isHeader = param.In == ParameterLocation.Header;
            var isQuery = param.In == ParameterLocation.Query;

            // Extract default value - match the extension generator logic
            var rawDefaultValue = param.Schema?.Default?.ToString();
            var isSimpleType =
                baseType
                    is "string"
                        or "int"
                        or "long"
                        or "float"
                        or "double"
                        or "decimal"
                        or "bool";
            var defaultValue =
                isSimpleType && !string.IsNullOrEmpty(rawDefaultValue) ? rawDefaultValue : null;

            // For optional string query parameters without a schema default, use empty string
            var hasNoDefault = defaultValue == null;
            if (!required && baseType == "string" && isQuery && hasNoDefault)
            {
                defaultValue = "";
            }

            // Make nullable if not required and no default value
            // For strings with default "", DON'T make nullable - pass the parameter and use ?? ""
            var makeNullable = !required && hasNoDefault && !baseType.EndsWith('?');
            var type = makeNullable ? $"{baseType}?" : baseType;

            parameters.Add(
                new McpParameterInfo(
                    sanitizedName,
                    type,
                    description,
                    required,
                    defaultValue,
                    isPath,
                    isHeader
                )
            );
        }

        return parameters;
    }

    private static string? GetRequestBodyType(OpenApiOperation operation)
    {
        if (operation.RequestBody?.Content == null)
        {
            return null;
        }

        var firstContent = operation.RequestBody.Content.FirstOrDefault();
        return firstContent.Value?.Schema is OpenApiSchemaReference schemaRef
            ? schemaRef.Reference.Id != null
                ? CodeGenerationHelpers.ToPascalCase(schemaRef.Reference.Id)
                : "object"
            : "object";
    }

    private static string GetResponseType(OpenApiOperation operation)
    {
        var successResponse = operation.Responses?.FirstOrDefault(r =>
            r.Key.StartsWith('2') || r.Key == "default"
        );

        if (successResponse?.Value?.Content == null)
        {
            return "object";
        }

        var content = successResponse.Value.Value.Content.FirstOrDefault();

        // Check if it's a schema reference (named type)
        if (content.Value?.Schema is OpenApiSchemaReference schemaRef)
        {
            return schemaRef.Reference.Id != null
                ? CodeGenerationHelpers.ToPascalCase(schemaRef.Reference.Id)
                : "object";
        }

        // Check for primitive types
        if (content.Value?.Schema != null)
        {
            var schema = content.Value.Schema;
            return schema.Type switch
            {
                JsonSchemaType.String => "string",
                JsonSchemaType.Integer => schema.Format == "int64" ? "long" : "int",
                JsonSchemaType.Number => schema.Format == "float" ? "float" : "double",
                JsonSchemaType.Boolean => "bool",
                JsonSchemaType.Array => "object", // Arrays are complex
                _ => "object",
            };
        }

        return "object";
    }

    private static string GetErrorType(OpenApiOperation operation)
    {
        var errorResponse = operation.Responses?.FirstOrDefault(r =>
            r.Key.StartsWith('4') || r.Key.StartsWith('5')
        );

        if (errorResponse?.Value?.Content == null)
        {
            return "string";
        }

        var content = errorResponse.Value.Value.Content.FirstOrDefault();
        return content.Value?.Schema is OpenApiSchemaReference schemaRef
            ? schemaRef.Reference.Id != null
                ? CodeGenerationHelpers.ToPascalCase(schemaRef.Reference.Id)
                : "string"
            : "string";
    }

    private static string GetExtensionMethodName(
        OpenApiOperation operation,
        HttpMethod operationType,
        string path
    )
    {
        if (!string.IsNullOrEmpty(operation.OperationId))
        {
            return CleanOperationId(operation.OperationId, operationType);
        }

        var pathPart =
            path.Split('/').LastOrDefault(p => !string.IsNullOrEmpty(p) && !p.StartsWith('{'))
            ?? "Resource";

        var methodName =
            operationType == HttpMethod.Get ? "Get"
            : operationType == HttpMethod.Post ? "Create"
            : operationType == HttpMethod.Put ? "Update"
            : operationType == HttpMethod.Delete ? "Delete"
            : operationType == HttpMethod.Patch ? "Patch"
            : operationType.Method;

        return $"{methodName}{CodeGenerationHelpers.ToPascalCase(pathPart)}Async";
    }

    private static string CleanOperationId(string operationId, HttpMethod operationType)
    {
        var cleaned = operationId;
        var removedPrefix = false;

#pragma warning disable CA1308
        var methodPrefix = operationType.Method.ToLowerInvariant() + "_";
#pragma warning restore CA1308
        if (cleaned.StartsWith(methodPrefix, StringComparison.OrdinalIgnoreCase))
        {
            cleaned = cleaned[methodPrefix.Length..];
            removedPrefix = true;
        }

        if (!removedPrefix)
        {
#pragma warning disable CA1308
            var methodSuffix = "_" + operationType.Method.ToLowerInvariant();
#pragma warning restore CA1308
            if (cleaned.EndsWith(methodSuffix, StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned[..^methodSuffix.Length];
            }
        }

        while (cleaned.Contains("__", StringComparison.Ordinal))
        {
            cleaned = cleaned.Replace("__", "_", StringComparison.Ordinal);
        }

        cleaned = cleaned.Trim('_');

        return CodeGenerationHelpers.ToPascalCase(cleaned) + "Async";
    }

    private static string SanitizeDescription(string description) =>
        description
            .Replace("\r\n", " ", StringComparison.Ordinal)
            .Replace("\n", " ", StringComparison.Ordinal)
            .Replace("\r", " ", StringComparison.Ordinal)
            .Replace("\"", "'", StringComparison.Ordinal)
            .Trim();
}
