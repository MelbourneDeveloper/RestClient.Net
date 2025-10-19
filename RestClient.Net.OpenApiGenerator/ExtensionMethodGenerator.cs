namespace RestClient.Net.OpenApiGenerator;

/// <summary>Parameter information for OpenAPI operations.</summary>
/// <param name="Name">The parameter name.</param>
/// <param name="Type">The parameter type.</param>
/// <param name="IsPath">Whether the parameter is a path parameter.</param>
/// <param name="IsHeader">Whether the parameter is a header parameter.</param>
/// <param name="OriginalName">The original parameter name from the OpenAPI spec.</param>
/// <param name="Required">Whether the parameter is required.</param>
/// <param name="DefaultValue">The default value for the parameter.</param>
public readonly record struct ParameterInfo(
    string Name,
    string Type,
    bool IsPath,
    bool IsHeader,
    string OriginalName,
    bool Required,
    string? DefaultValue
);

/// <summary>Generates C# extension methods from OpenAPI operations.</summary>
public static class ExtensionMethodGenerator
{
    /// <summary>Generates extension methods from an OpenAPI document.</summary>
    /// <param name="document">The OpenAPI document.</param>
    /// <param name="namespace">The namespace for generated code.</param>
    /// <param name="className">The class name for extension methods.</param>
    /// <param name="baseUrl">The base URL for API requests (not used, kept for API compatibility).</param>
    /// <param name="basePath">The base path for API requests.</param>
    /// <param name="jsonNamingPolicy">JSON naming policy (camelCase, PascalCase, snake_case).</param>
    /// <param name="caseInsensitive">Enable case-insensitive JSON deserialization.</param>
    /// <returns>Tuple containing the extension methods code and type aliases code.</returns>
#pragma warning disable IDE0060 // Remove unused parameter
    public static (string ExtensionMethods, string TypeAliases) GenerateExtensionMethods(
        OpenApiDocument document,
        string @namespace,
        string className,
        string baseUrl,
        string basePath,
        string jsonNamingPolicy = "camelCase",
        bool caseInsensitive = true
    )
#pragma warning restore IDE0060 // Remove unused parameter
    {
        var groupedMethods =
            new Dictionary<string, List<(string PublicMethod, string PrivateDelegate)>>();
        var resultTypes = new HashSet<(string SuccessType, string ErrorType)>();
        var methodNameCounts = new Dictionary<string, int>();

        foreach (var path in document.Paths)
        {
            if (path.Value?.Operations == null)
            {
                continue;
            }

            var resourceName = GetResourceNameFromPath(path.Key);

            foreach (var operation in path.Value.Operations)
            {
                var responseType = GetResponseType(operation.Value);
                var errorType = GetErrorType(operation.Value);
                var isDelete = operation.Key == HttpMethod.Delete;
                var resultResponseType = isDelete ? "Unit" : responseType;
                _ = resultTypes.Add((resultResponseType, errorType));

                var (publicMethod, privateDelegate) = GenerateMethod(
                    basePath,
                    path.Key,
                    operation.Key,
                    operation.Value,
                    document.Components?.Schemas,
                    methodNameCounts
                );
                if (!string.IsNullOrEmpty(publicMethod))
                {
                    if (!groupedMethods.TryGetValue(resourceName, out var methods))
                    {
                        methods = [];
                        groupedMethods[resourceName] = methods;
                    }
                    methods.Add((publicMethod, privateDelegate));
                }
            }
        }

        var (publicMethodsCode, privateDelegatesCode) = GenerateGroupedCode(groupedMethods);
        var typeAliases = GenerateTypeAliasesFile(resultTypes, @namespace);

        var namingPolicyCode = jsonNamingPolicy switch
        {
            var s when s.Equals("PascalCase", StringComparison.OrdinalIgnoreCase) => "null",
            var s
                when s.Equals("snake_case", StringComparison.OrdinalIgnoreCase)
                    || s.Equals("snakecase", StringComparison.OrdinalIgnoreCase) =>
                "JsonNamingPolicy.SnakeCaseLower",
            _ => "JsonNamingPolicy.CamelCase",
        };

        var caseInsensitiveCode = caseInsensitive ? "true" : "false";

        var extensionMethodsCode = $$"""
            #nullable enable
            using System;
            using System.Collections.Generic;
            using System.Net.Http;
            using System.Net.Http.Json;
            using System.Text.Json;
            using System.Threading;
            using System.Threading.Tasks;
            using RestClient.Net;
            using RestClient.Net.Utilities;
            using Outcome;
            using Urls;

            namespace {{@namespace}};

            /// <summary>Extension methods for API operations.</summary>
            public static class {{className}}
            {
                #region Configuration

                private static readonly AbsoluteUrl BaseUrl = "{{baseUrl}}".ToAbsoluteUrl();

                private static readonly JsonSerializerOptions JsonOptions = new()
                {
                    PropertyNameCaseInsensitive = {{caseInsensitiveCode}},
                    PropertyNamingPolicy = {{namingPolicyCode}}
                };

                private static readonly Deserialize<Unit> _deserializeUnit = static (_, _) =>
                    Task.FromResult(Unit.Value);

                #endregion

            {{publicMethodsCode}}

            {{privateDelegatesCode}}

                private static ProgressReportingHttpContent CreateJsonContent<T>(T data)
                {
                    var json = JsonSerializer.Serialize(data, JsonOptions);
                    System.Console.WriteLine($"[DEBUG] Serializing request: {json}");
                    return new ProgressReportingHttpContent(json, contentType: "application/json");
                }

                private static async Task<T> DeserializeJson<T>(
                    HttpResponseMessage response,
                    CancellationToken cancellationToken = default
                )
                {
                    var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    System.Console.WriteLine($"[DEBUG] Response status: {response.StatusCode}, URL: {response.RequestMessage?.RequestUri}, body: {body}");
                    var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
                    return result ?? throw new InvalidOperationException($"Failed to deserialize response to type {typeof(T).Name}");
                }

                private static async Task<string> DeserializeString(
                    HttpResponseMessage response,
                    CancellationToken cancellationToken = default
                ) =>
                    await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                private static async Task<string> DeserializeError(
                    HttpResponseMessage response,
                    CancellationToken cancellationToken = default
                )
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    return string.IsNullOrEmpty(content) ? "Unknown error" : content;
                }

                private static string BuildQueryString(params (string Key, object? Value)[] parameters)
                {
                    var parts = new List<string>();
                    foreach (var (key, value) in parameters)
                    {
                        if (value == null)
                        {
                            continue;
                        }

                        if (value is System.Collections.IEnumerable enumerable and not string)
                        {
                            foreach (var item in enumerable)
                            {
                                if (item != null)
                                {
                                    parts.Add($"{key}={Uri.EscapeDataString(item.ToString() ?? string.Empty)}");
                                }
                            }
                        }
                        else
                        {
                            parts.Add($"{key}={Uri.EscapeDataString(value.ToString() ?? string.Empty)}");
                        }
                    }
                    return parts.Count > 0 ? "?" + string.Join("&", parts) : string.Empty;
                }
            }
            """;

        return (extensionMethodsCode, typeAliases);
    }

    private static string GetResourceNameFromPath(string path)
    {
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
        {
            return "General";
        }

        var resourceSegment = segments[0];
        return CodeGenerationHelpers.ToPascalCase(resourceSegment);
    }

    private static (string PublicMethods, string PrivateDelegates) GenerateGroupedCode(
        Dictionary<string, List<(string PublicMethod, string PrivateDelegate)>> groupedMethods
    )
    {
        var publicSections = new List<string>();
        var allPrivateDelegates = new List<string>();

        foreach (var group in groupedMethods.OrderBy(g => g.Key))
        {
            var publicMethods = group.Value.Select(m => m.PublicMethod);
            var privateDelegates = group.Value.Select(m => m.PrivateDelegate);

            var publicMethodsCode = string.Join("\n\n", publicMethods);
            var indentedContent = CodeGenerationHelpers.Indent(publicMethodsCode, 1);
            var regionName = $"{group.Key} Operations";

            // #region markers at column 0, content indented by 4 spaces
            var section = $"    #region {regionName}\n\n{indentedContent}\n\n    #endregion";

            publicSections.Add(section);
            allPrivateDelegates.AddRange(privateDelegates);
        }

        var privateDelegatesCode = string.Join(
            "\n\n",
            allPrivateDelegates.Select(d => CodeGenerationHelpers.Indent(d, 1))
        );

        return (string.Join("\n\n", publicSections), privateDelegatesCode);
    }

    private static (string PublicMethod, string PrivateDelegate) GenerateMethod(
        string basePath,
        string path,
        HttpMethod operationType,
        OpenApiOperation operation,
        IDictionary<string, IOpenApiSchema>? schemas,
        Dictionary<string, int> methodNameCounts
    )
    {
        var methodName = GetMethodName(operation, operationType, path);

        // Ensure uniqueness by tracking method names and adding suffixes
        if (methodNameCounts.TryGetValue(methodName, out var count))
        {
            methodNameCounts[methodName] = count + 1;
            methodName = $"{methodName}{count + 1}";
        }
        else
        {
            methodNameCounts[methodName] = 1;
        }

        var parameters = GetParameters(operation, schemas);
        var requestBodyType = GetRequestBodyType(operation);
        var responseType = GetResponseType(operation);
        var errorType = GetErrorType(operation);
        var rawSummary = operation.Description ?? operation.Summary ?? $"{methodName} operation";
        var summary = FormatXmlDocSummary(rawSummary);

        return GenerateHttpMethod(
            operationType,
            methodName,
            basePath + path,
            parameters,
            requestBodyType,
            responseType,
            errorType,
            summary
        );
    }

#pragma warning disable CA1502 // Avoid excessive complexity - code generator method is inherently complex
    private static (string PublicMethod, string PrivateDelegate) GenerateHttpMethod(
        HttpMethod operationType,
        string methodName,
        string path,
        List<ParameterInfo> parameters,
        string? requestBodyType,
        string responseType,
        string errorType,
        string summary
    )
#pragma warning restore CA1502
    {
        var hasBody =
            operationType == HttpMethod.Post
            || operationType == HttpMethod.Put
            || operationType == HttpMethod.Patch;
        var isDelete = operationType == HttpMethod.Delete;
        var hasPathParams = parameters.Any(p => p.IsPath);
        var headerParams = parameters.Where(p => p.IsHeader).ToList();
        var queryParams = parameters.Where(p => !p.IsPath && !p.IsHeader).ToList();
        var nonPathParams = parameters.Where(p => !p.IsPath).ToList(); // Includes both query and header params
        var hasQueryParams = queryParams.Count > 0;
        var hasHeaderParams = headerParams.Count > 0;
        var hasNonPathParams = nonPathParams.Count > 0;

        var bodyType = requestBodyType ?? "object";
        var resultResponseType = isDelete ? "Unit" : responseType;
        var resultType = $"Result<{resultResponseType}, HttpError<{errorType}>>";
        var pathExpression = CodeGenerationHelpers.BuildPathExpression(path);
        var deserializer =
            responseType == "string" ? "DeserializeString" : $"DeserializeJson<{responseType}>";

        var verb =
            operationType == HttpMethod.Get ? "Get"
            : operationType == HttpMethod.Post ? "Post"
            : operationType == HttpMethod.Put ? "Put"
            : operationType == HttpMethod.Delete ? "Delete"
            : operationType == HttpMethod.Patch ? "Patch"
            : operationType == HttpMethod.Head ? "Head"
            : operationType == HttpMethod.Options ? "Options"
            : operationType.Method;
        var createMethod = $"Create{verb}";
        var delegateType = $"{verb}Async";

        // GET with no parameters OR body verbs with no path params
        if (
            (!hasPathParams && !hasNonPathParams && !hasBody)
            || (!hasPathParams && hasBody && !hasNonPathParams)
        )
        {
            if (!hasBody)
            {
                var deserializeMethod = isDelete ? "_deserializeUnit" : deserializer;
                var buildRequestBody =
                    $"static _ => new HttpRequestParts(new RelativeUrl(\"{path}\"), null, null)";

                return BuildMethod(
                    methodName,
                    createMethod,
                    resultType,
                    resultResponseType,
                    errorType,
                    "Unit",
                    string.Empty,
                    "Unit.Value",
                    buildRequestBody,
                    deserializeMethod,
                    summary
                );
            }
            else
            {
                var deserializeMethod = isDelete ? "_deserializeUnit" : deserializer;

                var buildRequestBody =
                    $"static body => new HttpRequestParts(new RelativeUrl(\"{path}\"), CreateJsonContent(body), null)";

                return BuildMethod(
                    methodName,
                    createMethod,
                    resultType,
                    resultResponseType,
                    errorType,
                    bodyType,
                    $"{bodyType} body",
                    "body",
                    buildRequestBody,
                    deserializeMethod,
                    summary
                );
            }
        }

        // GET with only query/header parameters (no path params, no body)
        if (!hasPathParams && hasNonPathParams && !hasBody)
        {
            var isSingleParam = nonPathParams.Count == 1;
            var nonPathParamsType = isSingleParam
                ? nonPathParams[0].Type
                : $"({string.Join(", ", nonPathParams.Select(q => $"{q.Type} {q.Name}"))})";
            var nonPathParamsNames = string.Join(", ", nonPathParams.Select(q => q.Name));
            var paramInvocation = isSingleParam ? nonPathParamsNames : $"({nonPathParamsNames})";
            var deserializeMethod = isDelete ? "_deserializeUnit" : deserializer;

            var queryStringExpression = hasQueryParams
                ? (
                    isSingleParam && queryParams.Count == 1
                        ? $"?{queryParams[0].OriginalName}={{param}}"
                        : $"?{string.Join("&", queryParams.Select(q => $"{q.OriginalName}={{param.{q.Name}}}"))}"
                )
                : string.Empty;

            var headersExpression = hasHeaderParams
                ? BuildHeadersDictionaryExpression(headerParams, "param", isSingleParam)
                : "null";

            var buildRequestBody =
                $"static param => new HttpRequestParts(new RelativeUrl($\"{path}{queryStringExpression}\"), null, {headersExpression})";

            return BuildMethod(
                methodName,
                createMethod,
                resultType,
                resultResponseType,
                errorType,
                nonPathParamsType,
                string.Join(", ", nonPathParams.Select(FormatParameterWithDefault)),
                paramInvocation,
                buildRequestBody,
                deserializeMethod,
                summary
            );
        }

        // Has path parameters (with or without body/query)
        var pathParams = parameters.Where(p => p.IsPath).ToList();
        var isSinglePathParam = pathParams.Count == 1;
        var pathParamsType = isSinglePathParam
            ? pathParams[0].Type
            : $"({string.Join(", ", pathParams.Select(p => $"{p.Type} {p.Name}"))})";
        var pathParamsNames = string.Join(", ", pathParams.Select(p => p.Name));
        var lambda = isSinglePathParam ? pathParams[0].Name : "param";

        // If we have query/header params along with path params and no body
        if (!hasBody && (hasQueryParams || hasHeaderParams))
        {
            var allParamsTypeList = parameters.Select(p => $"{p.Type} {p.Name}").ToList();
            var allParamsList = parameters.Select(FormatParameterWithDefault).ToList();
            var isSingleParam = parameters.Count == 1;
            var allParamsType = isSingleParam
                ? parameters[0].Type
                : $"({string.Join(", ", allParamsTypeList)})";
            var allParamsNames = string.Join(", ", parameters.Select(p => p.Name));
            var paramInvocation = isSingleParam ? allParamsNames : $"({allParamsNames})";
            var deserializeMethod = isDelete ? "_deserializeUnit" : deserializer;

            // Use BuildQueryString for nullable parameters to handle null values correctly
            var hasNullableQueryParams = queryParams.Any(q =>
                q.Type.Contains('?', StringComparison.Ordinal)
            );
            var queryStringExpression =
                !hasQueryParams ? string.Empty
                : hasNullableQueryParams
                    ? (
                        isSingleParam && queryParams.Count == 1
                            ? $"BuildQueryString((\"{queryParams[0].OriginalName}\", param))"
                            : $"BuildQueryString({string.Join(", ", queryParams.Select(q => $"(\"{q.OriginalName}\", param.{q.Name})"))})"
                    )
                : (
                    isSingleParam && queryParams.Count == 1
                        ? $"?{queryParams[0].OriginalName}={{param}}"
                        : $"?{string.Join("&", queryParams.Select(q => $"{q.OriginalName}={{param.{q.Name}}}"))}"
                );

            var headersExpression = hasHeaderParams
                ? BuildHeadersDictionaryExpression(headerParams, "param", isSingleParam)
                : "null";

            var sanitizedPath = CodeGenerationHelpers.SanitizePathParameters(
                pathExpression,
                parameters
            );
            var pathWithParam =
                isSingleParam && hasPathParams
                    ? sanitizedPath.Replace(
                        "{" + parameters.First(p => p.IsPath).Name + "}",
                        "{param}",
                        StringComparison.Ordinal
                    )
                    : sanitizedPath.Replace("{", "{param.", StringComparison.Ordinal);

            var buildRequestBody =
                hasQueryParams && hasNullableQueryParams
                    ? $"static param => new HttpRequestParts(new RelativeUrl($\"{pathWithParam}{{{queryStringExpression}}}\"), null, {headersExpression})"
                    : $"static param => new HttpRequestParts(new RelativeUrl($\"{pathWithParam}{queryStringExpression}\"), null, {headersExpression})";

            return BuildMethod(
                methodName,
                createMethod,
                resultType,
                resultResponseType,
                errorType,
                allParamsType,
                string.Join(", ", allParamsList),
                paramInvocation,
                buildRequestBody,
                deserializeMethod,
                summary
            );
        }

        if (!hasBody)
        {
            var deserializeMethod = isDelete ? "_deserializeUnit" : deserializer;
            var sanitizedPath = CodeGenerationHelpers.SanitizePathParameters(
                pathExpression,
                parameters
            );
            var pathWithParam = isSinglePathParam
                ? sanitizedPath
                : sanitizedPath.Replace("{", "{param.", StringComparison.Ordinal);
            var publicMethodParams = string.Join(
                ", ",
                pathParams.Select(FormatParameterWithDefault)
            );
            var publicMethodInvocation = isSinglePathParam
                ? pathParamsNames
                : $"({pathParamsNames})";

            var buildRequestBody =
                $"static {lambda} => new HttpRequestParts(new RelativeUrl($\"{pathWithParam}\"), null, null)";

            return BuildMethod(
                methodName,
                createMethod,
                resultType,
                resultResponseType,
                errorType,
                pathParamsType,
                publicMethodParams,
                publicMethodInvocation,
                buildRequestBody,
                deserializeMethod,
                summary
            );
        }
        else
        {
            // Has body with path params - may also have query/header params
            var hasNonPathNonBodyParams = hasQueryParams || hasHeaderParams;
            var compositeType = hasNonPathNonBodyParams
                ? $"({string.Join(", ", parameters.Select(p => $"{p.Type} {p.Name}"))}, {bodyType} Body)"
                : $"({pathParamsType} Params, {bodyType} Body)";

            var sanitizedPath = CodeGenerationHelpers.SanitizePathParameters(
                pathExpression,
                parameters
            );

            var pathWithParamInterpolation = hasNonPathNonBodyParams
                ? (
                    isSinglePathParam
                        ? sanitizedPath.Replace(
                            "{" + pathParams[0].Name + "}",
                            "{param." + pathParams[0].Name + "}",
                            StringComparison.Ordinal
                        )
                        : sanitizedPath.Replace("{", "{param.", StringComparison.Ordinal)
                )
                : (
                    isSinglePathParam
                        ? sanitizedPath.Replace(
                            "{" + pathParams[0].Name + "}",
                            "{param.Params}",
                            StringComparison.Ordinal
                        )
                        : sanitizedPath.Replace("{", "{param.Params.", StringComparison.Ordinal)
                );

            var queryStringExpression = hasQueryParams
                ? $"?{string.Join("&", queryParams.Select(q => $"{q.OriginalName}={{param.{q.Name}}}"))}"
                : string.Empty;

            var headersExpression = hasHeaderParams
                ? BuildHeadersDictionaryExpression(headerParams, "param", false)
                : "null";

            var buildRequestBody =
                $"static param => new HttpRequestParts(new RelativeUrl($\"{pathWithParamInterpolation}{queryStringExpression}\"), CreateJsonContent(param.Body), {headersExpression})";

            // Public methods ALWAYS have individual parameters, never tuples
            // Required parameters must come before optional ones
            var relevantParams = hasNonPathNonBodyParams ? parameters : pathParams;
            var requiredParams = relevantParams
                .Where(p => !HasDefault(p))
                .Select(FormatParameterWithDefault);
            var optionalParams = relevantParams
                .Where(HasDefault)
                .Select(FormatParameterWithDefault);
            var allParams = requiredParams.Concat([$"{bodyType} body"]).Concat(optionalParams);
            var publicMethodParams = string.Join(", ", allParams);

            var publicMethodInvocation =
                hasNonPathNonBodyParams
                    ? $"({string.Join(", ", parameters.Select(p => p.Name))}, body)"
                : isSinglePathParam ? $"({pathParamsNames}, body)"
                : $"(({pathParamsNames}), body)";

            return BuildMethod(
                methodName,
                createMethod,
                resultType,
                resultResponseType,
                errorType,
                compositeType,
                publicMethodParams,
                publicMethodInvocation,
                buildRequestBody,
                "DeserializeJson<" + resultResponseType + ">",
                summary
            );
        }
    }

    private static string GetMethodName(
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
            : operationType == HttpMethod.Head ? "Head"
            : operationType == HttpMethod.Options ? "Options"
            : operationType.Method;

        return $"{methodName}{CodeGenerationHelpers.ToPascalCase(pathPart)}Async";
    }

    private static string CleanOperationId(string operationId, HttpMethod operationType)
    {
        var cleaned = operationId;
        var removedPrefix = false;

        // Remove HTTP method prefix (e.g., "get_", "post_", "delete_")
#pragma warning disable CA1308 // Normalize strings to uppercase - we need lowercase for operation ID comparison
        var methodPrefix = operationType.Method.ToLowerInvariant() + "_";
#pragma warning restore CA1308
        if (cleaned.StartsWith(methodPrefix, StringComparison.OrdinalIgnoreCase))
        {
            cleaned = cleaned[methodPrefix.Length..];
            removedPrefix = true;
        }

        // Only remove HTTP method suffix if we didn't already remove the prefix
        // This prevents over-cleaning that causes name collisions
        if (!removedPrefix)
        {
#pragma warning disable CA1308 // Normalize strings to uppercase - we need lowercase for operation ID comparison
            var methodSuffix = "_" + operationType.Method.ToLowerInvariant();
#pragma warning restore CA1308
            if (cleaned.EndsWith(methodSuffix, StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned[..^methodSuffix.Length];
            }
        }

        // Remove double underscores
        while (cleaned.Contains("__", StringComparison.Ordinal))
        {
            cleaned = cleaned.Replace("__", "_", StringComparison.Ordinal);
        }

        // Remove leading/trailing underscores
        cleaned = cleaned.Trim('_');

        // Convert to PascalCase and add Async suffix
        return CodeGenerationHelpers.ToPascalCase(cleaned) + "Async";
    }

    private static string FormatParameterWithDefault(ParameterInfo param)
    {
        var defaultPart =
            param.DefaultValue != null
                ? param.Type switch
                {
                    var t when t.Contains('?', StringComparison.Ordinal) => " = null",
                    var t when t.StartsWith("string", StringComparison.Ordinal) =>
                        $" = \"{param.DefaultValue}\"",
                    var t when t.StartsWith("bool", StringComparison.Ordinal) =>
                        param.DefaultValue.Equals("true", StringComparison.OrdinalIgnoreCase)
                            ? " = true"
                            : " = false",
                    _ => $" = {param.DefaultValue}",
                }
            : param.Type.Contains('?', StringComparison.Ordinal) ? " = null"
            : string.Empty;

        return $"{param.Type} {param.Name}{defaultPart}";
    }

    private static bool HasDefault(ParameterInfo param) =>
        param.DefaultValue != null || param.Type.Contains('?', StringComparison.Ordinal);

    private static string FormatXmlDocSummary(string description)
    {
        // Take the first line/paragraph before any --- separator or double newline
        var lines = description.Split('\n');
        var summaryLines = new List<string>();

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            // Stop at separator or blank line that indicates the end of the summary
            if (trimmed == "---" || (summaryLines.Count > 0 && string.IsNullOrWhiteSpace(trimmed)))
            {
                break;
            }

            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                summaryLines.Add(trimmed);
            }
        }

        return summaryLines.Count > 0
            ? string.Join(" ", summaryLines)
            : description.Replace("\n", " ", StringComparison.Ordinal).Trim();
    }

    private static List<ParameterInfo> GetParameters(
        OpenApiOperation operation,
        IDictionary<string, IOpenApiSchema>? schemas = null
    )
    {
        var parameters = new List<ParameterInfo>();

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

            var isPath = param.In == ParameterLocation.Path;
            var isHeader = param.In == ParameterLocation.Header;
            var isQuery = param.In == ParameterLocation.Query;
            var required = param.Required;
            var baseType = ModelGenerator.MapOpenApiType(param.Schema, schemas);
            var sanitizedName = CodeGenerationHelpers.ToCamelCase(param.Name);

            // Extract default value if present, but only for simple types
            var rawDefaultValue = param.Schema?.Default?.ToString();
            var isSimpleType =
                baseType
                    is "string"
                        or "int"
                        or "long"
                        or "bool"
                        or "float"
                        or "double"
                        or "decimal";
            var defaultValue =
                isSimpleType && !string.IsNullOrEmpty(rawDefaultValue) ? rawDefaultValue : null;

            // For optional string query parameters without a schema default, use empty string
            // This allows direct URL interpolation without nullable types
            if (isQuery && !required && string.IsNullOrEmpty(defaultValue) && baseType == "string")
            {
                defaultValue = "";
            }

            // Make nullable if not required and no default value
            // Note: Even value types (int, bool, etc.) can and should be nullable when optional
            var hasNoDefault = defaultValue == null;
            var makeNullable = !required && hasNoDefault && !baseType.EndsWith('?');
            var type = makeNullable ? $"{baseType}?" : baseType;

            parameters.Add(
                new ParameterInfo(
                    sanitizedName,
                    type,
                    isPath,
                    isHeader,
                    param.Name,
                    required,
                    defaultValue
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

    private static string GenerateTypeAliasesFile(HashSet<(string SuccessType, string ErrorType)> resultTypes, string @namespace)
    {
        if (resultTypes.Count == 0)
        {
            return string.Empty;
        }

        var aliases = GenerateTypeAliasesList(resultTypes, @namespace);

        return $$"""
            #pragma warning disable IDE0005 // Using directive is unnecessary.
            {{string.Join("\n", aliases)}}
            """;
    }

    private static List<string> GenerateTypeAliasesList(
        HashSet<(string SuccessType, string ErrorType)> resultTypes,
        string @namespace
    )
    {
        var aliases = new List<string>();

        if (resultTypes.Count == 0)
        {
            return aliases;
        }

        foreach (var (successType, errorType) in resultTypes.OrderBy(t => t.SuccessType).ThenBy(t => t.ErrorType))
        {
            var typeName = successType
                .Replace("List<", string.Empty, StringComparison.Ordinal)
                .Replace(">", string.Empty, StringComparison.Ordinal)
                .Replace(".", string.Empty, StringComparison.Ordinal);
            var pluralSuffix = successType.StartsWith("List<", StringComparison.Ordinal)
                ? "s"
                : string.Empty;

            // Include error type in alias name if not string (to avoid conflicts)
            var errorTypeName = errorType == "string" ? "" : errorType
                .Replace("List<", string.Empty, StringComparison.Ordinal)
                .Replace(">", string.Empty, StringComparison.Ordinal)
                .Replace(".", string.Empty, StringComparison.Ordinal);
            var aliasName = $"{typeName}{pluralSuffix}{errorTypeName}";

            // Qualify type names with namespace (except for System types and Outcome.Unit)
            var qualifiedSuccessType = successType switch
            {
                "Unit" => "Outcome.Unit",
                "object" => "System.Object",
                "string" => "System.String",
                _ when successType.StartsWith("List<", StringComparison.Ordinal) =>
                    successType.Replace(
                        "List<",
                        $"System.Collections.Generic.List<{@namespace}.",
                        StringComparison.Ordinal
                    ),
                _ => $"{@namespace}.{successType}",
            };

            var qualifiedErrorType = errorType switch
            {
                "string" => "string",
                "object" => "System.Object",
                _ => $"{@namespace}.{errorType}",
            };

            // Generate Ok alias
            aliases.Add(
                $$"""
                global using Ok{{aliasName}} = Outcome.Result<{{qualifiedSuccessType}}, Outcome.HttpError<{{qualifiedErrorType}}>>.Ok<{{qualifiedSuccessType}}, Outcome.HttpError<{{qualifiedErrorType}}>>;
                """
            );

            // Generate Error alias
            aliases.Add(
                $$"""
                global using Error{{aliasName}} = Outcome.Result<{{qualifiedSuccessType}}, Outcome.HttpError<{{qualifiedErrorType}}>>.Error<{{qualifiedSuccessType}}, Outcome.HttpError<{{qualifiedErrorType}}>>;
                """
            );
        }

        return aliases;
    }

    private static (string PublicMethod, string PrivateDelegate) BuildMethod(
        string methodName,
        string createMethod,
        string resultType,
        string resultResponseType,
        string errorType,
        string paramType,
        string publicParams,
        string paramInvocation,
        string buildRequestBody,
        string deserializeMethod,
        string summary
    )
    {
        var privateFunctionName = $"_{char.ToLowerInvariant(methodName[0])}{methodName[1..]}";
        var paramsWithComma = string.IsNullOrEmpty(publicParams)
            ? string.Empty
            : $"{publicParams},";

        // Derive delegate type name: CreatePost → PostAsync, CreateGet → GetAsync, etc.
        var delegateType =
            createMethod.Replace("Create", string.Empty, StringComparison.Ordinal) + "Async";

        var deserializeErrorMethod =
            errorType == "string" ? "DeserializeError" : $"DeserializeJson<{errorType}>";

        var privateDelegate = $$"""
            private static {{delegateType}}<{{resultResponseType}}, {{errorType}}, {{paramType}}> {{privateFunctionName}} { get; } =
                RestClient.Net.HttpClientFactoryExtensions.{{createMethod}}<{{resultResponseType}}, {{errorType}}, {{paramType}}>(
                    url: BaseUrl,
                    buildRequest: {{buildRequestBody}},
                    deserializeSuccess: {{deserializeMethod}},
                    deserializeError: {{deserializeErrorMethod}}
                );
            """;

        var publicMethod = $$"""
            /// <summary>{{summary}}</summary>
            public static Task<{{resultType}}> {{methodName}}(
                this HttpClient httpClient,
                {{paramsWithComma}}
                CancellationToken cancellationToken = default
            ) => {{privateFunctionName}}(httpClient, {{paramInvocation}}, cancellationToken);
            """;

        return (publicMethod, privateDelegate);
    }

    private static string BuildHeadersDictionaryExpression(
        List<ParameterInfo> headerParams,
        string paramPrefix = "param",
        bool isSingleOverallParam = false
    )
    {
        if (headerParams.Count == 0)
        {
            return "null";
        }

        // Only use param?.ToString() directly if we have a single overall parameter that IS the header
        if (headerParams.Count == 1 && isSingleOverallParam)
        {
            var h = headerParams[0];
            return $"new Dictionary<string, string> {{ [\"{h.OriginalName}\"] = {paramPrefix}?.ToString() ?? string.Empty }}";
        }

        // Otherwise, we have a tuple and need to access param.{headerName}
        var entries = string.Join(
            ", ",
            headerParams.Select(h =>
            {
                var isNullable = h.Type.EndsWith('?');
                var accessor = isNullable
                    ? $"{paramPrefix}.{h.Name}?.ToString() ?? string.Empty"
                    : $"{paramPrefix}.{h.Name}.ToString()";
                return $"[\"{h.OriginalName}\"] = {accessor}";
            })
        );
        return $"new Dictionary<string, string> {{ {entries} }}";
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
        return content.Value?.Schema is OpenApiSchemaReference schemaRef
            ? schemaRef.Reference.Id != null
                ? CodeGenerationHelpers.ToPascalCase(schemaRef.Reference.Id)
                : "object"
            : content.Value?.Schema is OpenApiSchema schema
            && schema.Type == JsonSchemaType.Array
            && schema.Items is OpenApiSchemaReference itemsRef
                ? $"List<{(itemsRef.Reference.Id != null ? CodeGenerationHelpers.ToPascalCase(itemsRef.Reference.Id) : "object")}>"
                : ModelGenerator.MapOpenApiType(content.Value?.Schema);
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
}
