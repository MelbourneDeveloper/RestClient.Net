namespace RestClient.Net.OpenApiGenerator;

internal readonly record struct ParameterInfo(
    string Name,
    string Type,
    bool IsPath,
    bool IsHeader,
    string OriginalName
);

/// <summary>Generates C# extension methods from OpenAPI operations.</summary>
internal static class ExtensionMethodGenerator
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
        var responseTypes = new HashSet<string>();

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
                var isDelete = operation.Key == HttpMethod.Delete;
                var resultResponseType = isDelete ? "Unit" : responseType;
                _ = responseTypes.Add(resultResponseType);

                var (publicMethod, privateDelegate) = GenerateMethod(
                    path.Key,
                    operation.Key,
                    operation.Value,
                    basePath,
                    document.Components?.Schemas
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
        var typeAliases = GenerateTypeAliasesFile(responseTypes, @namespace);

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
                    CancellationToken ct = default
                )
                {
                    var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                    System.Console.WriteLine($"[DEBUG] Response status: {response.StatusCode}, URL: {response.RequestMessage?.RequestUri}, body: {body}");
                    var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken: ct).ConfigureAwait(false);
                    return result ?? throw new InvalidOperationException($"Failed to deserialize response to type {typeof(T).Name}");
                }

                private static async Task<string> DeserializeString(
                    HttpResponseMessage response,
                    CancellationToken ct = default
                ) =>
                    await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

                private static async Task<string> DeserializeError(
                    HttpResponseMessage response,
                    CancellationToken ct = default
                )
                {
                    var content = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
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
        string path,
        HttpMethod operationType,
        OpenApiOperation operation,
        string basePath,
        IDictionary<string, IOpenApiSchema>? schemas = null
    )
    {
        var methodName = GetMethodName(operation, operationType, path);
        var parameters = GetParameters(operation, schemas);
        var requestBodyType = GetRequestBodyType(operation);
        var responseType = GetResponseType(operation);
        var fullPath = $"{basePath}{path}";
        var summary = operation.Summary ?? operation.Description ?? $"{methodName} operation";

        return GenerateHttpMethod(
            operationType,
            methodName,
            fullPath,
            parameters,
            requestBodyType,
            responseType,
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
        var resultType = $"Result<{resultResponseType}, HttpError<string>>";
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
                        ? $"BuildQueryString((\"{queryParams[0].OriginalName}\", param))"
                        : $"BuildQueryString({string.Join(", ", queryParams.Select(q => $"(\"{q.OriginalName}\", param.{q.Name})"))})"
                )
                : "string.Empty";

            var headersExpression = hasHeaderParams
                ? BuildHeadersDictionaryExpression(headerParams, "param", isSingleParam)
                : "null";

            var buildRequestBody =
                $"static param => new HttpRequestParts(new RelativeUrl($\"{path}{{{queryStringExpression}}}\"), null, {headersExpression})";

            return BuildMethod(
                methodName,
                createMethod,
                resultType,
                resultResponseType,
                nonPathParamsType,
                string.Join(", ", nonPathParams.Select(q => $"{q.Type} {q.Name}")),
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
            var allParamsList = parameters.Select(p => $"{p.Type} {p.Name}").ToList();
            var isSingleParam = parameters.Count == 1;
            var allParamsType = isSingleParam
                ? parameters[0].Type
                : $"({string.Join(", ", allParamsList)})";
            var allParamsNames = string.Join(", ", parameters.Select(p => p.Name));
            var paramInvocation = isSingleParam ? allParamsNames : $"({allParamsNames})";
            var deserializeMethod = isDelete ? "_deserializeUnit" : deserializer;

            var queryStringExpression = hasQueryParams
                ? (
                    isSingleParam && queryParams.Count == 1
                        ? $"BuildQueryString((\"{queryParams[0].OriginalName}\", param))"
                        : $"BuildQueryString({string.Join(", ", queryParams.Select(q => $"(\"{q.OriginalName}\", param.{q.Name})"))})"
                )
                : "string.Empty";

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
                $"static param => new HttpRequestParts(new RelativeUrl($\"{pathWithParam}{{{queryStringExpression}}}\"), null, {headersExpression})";

            return BuildMethod(
                methodName,
                createMethod,
                resultType,
                resultResponseType,
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
                pathParams.Select(p => $"{p.Type} {p.Name}")
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
                ? $"BuildQueryString({string.Join(", ", queryParams.Select(q => $"(\"{q.OriginalName}\", param.{q.Name})"))})"
                : "string.Empty";

            var headersExpression = hasHeaderParams
                ? BuildHeadersDictionaryExpression(headerParams, "param", false)
                : "null";

            var buildRequestBody =
                $"static param => new HttpRequestParts(new RelativeUrl($\"{pathWithParamInterpolation}{{{queryStringExpression}}}\"), CreateJsonContent(param.Body), {headersExpression})";

            // Public methods ALWAYS have individual parameters, never tuples
            var publicMethodParams = hasNonPathNonBodyParams
                ? string.Join(", ", parameters.Select(p => $"{p.Type} {p.Name}"))
                    + $", {bodyType} body"
                : string.Join(", ", pathParams.Select(p => $"{p.Type} {p.Name}"))
                    + $", {bodyType} body";

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
            return CodeGenerationHelpers.ToPascalCase(operation.OperationId);
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

        return $"{methodName}{CodeGenerationHelpers.ToPascalCase(pathPart)}";
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
            var type = ModelGenerator.MapOpenApiType(param.Schema, schemas);
            var sanitizedName = CodeGenerationHelpers.ToCamelCase(param.Name);
            parameters.Add(new ParameterInfo(sanitizedName, type, isPath, isHeader, param.Name));
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

    private static string GenerateTypeAliasesFile(HashSet<string> responseTypes, string @namespace)
    {
        if (responseTypes.Count == 0)
        {
            return string.Empty;
        }

        var aliases = GenerateTypeAliasesList(responseTypes, @namespace);

        return $$"""
            #pragma warning disable IDE0005 // Using directive is unnecessary.
            {{string.Join("\n", aliases)}}
            """;
    }

    private static List<string> GenerateTypeAliasesList(
        HashSet<string> responseTypes,
        string @namespace
    )
    {
        var aliases = new List<string>();

        if (responseTypes.Count == 0)
        {
            return aliases;
        }

        foreach (var responseType in responseTypes.OrderBy(t => t))
        {
            var typeName = responseType
                .Replace("List<", string.Empty, StringComparison.Ordinal)
                .Replace(">", string.Empty, StringComparison.Ordinal)
                .Replace(".", string.Empty, StringComparison.Ordinal);
            var pluralSuffix = responseType.StartsWith("List<", StringComparison.Ordinal)
                ? "s"
                : string.Empty;
            var aliasName = $"{typeName}{pluralSuffix}";

            // Qualify type names with namespace (except for System types and Outcome.Unit)
            var qualifiedType = responseType switch
            {
                "Unit" => "Outcome.Unit",
                "object" => "System.Object",
                "string" => "System.String",
                _ when responseType.StartsWith("List<", StringComparison.Ordinal) =>
                    responseType.Replace(
                        "List<",
                        $"System.Collections.Generic.List<{@namespace}.",
                        StringComparison.Ordinal
                    ),
                _ => $"{@namespace}.{responseType}",
            };

            // Generate Ok alias
            aliases.Add(
                $$"""
                global using Ok{{aliasName}} = Outcome.Result<{{qualifiedType}}, Outcome.HttpError<string>>.Ok<{{qualifiedType}}, Outcome.HttpError<string>>;
                """
            );

            // Generate Error alias
            aliases.Add(
                $$"""
                global using Error{{aliasName}} = Outcome.Result<{{qualifiedType}}, Outcome.HttpError<string>>.Error<{{qualifiedType}}, Outcome.HttpError<string>>;
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

        var privateDelegate = $$"""
            private static {{delegateType}}<{{resultResponseType}}, string, {{paramType}}> {{privateFunctionName}} { get; } =
                RestClient.Net.HttpClientFactoryExtensions.{{createMethod}}<{{resultResponseType}}, string, {{paramType}}>(
                    url: BaseUrl,
                    buildRequest: {{buildRequestBody}},
                    deserializeSuccess: {{deserializeMethod}},
                    deserializeError: DeserializeError
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

        // Only use param.ToString() directly if we have a single overall parameter that IS the header
        if (headerParams.Count == 1 && isSingleOverallParam)
        {
            var h = headerParams[0];
            return $"new Dictionary<string, string> {{ [\"{h.OriginalName}\"] = {paramPrefix}.ToString() ?? string.Empty }}";
        }

        // Otherwise, we have a tuple and need to access param.{headerName}
        var entries = string.Join(
            ", ",
            headerParams.Select(h =>
                $"[\"{h.OriginalName}\"] = {paramPrefix}.{h.Name}.ToString() ?? string.Empty"
            )
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
}
