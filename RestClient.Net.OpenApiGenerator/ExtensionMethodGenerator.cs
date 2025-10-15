using Microsoft.OpenApi.Models;

namespace RestClient.Net.OpenApiGenerator;

/// <summary>Generates C# extension methods from OpenAPI operations.</summary>
internal static class ExtensionMethodGenerator
{
    /// <summary>Generates extension methods from an OpenAPI document.</summary>
    /// <param name="document">The OpenAPI document.</param>
    /// <param name="namespace">The namespace for generated code.</param>
    /// <param name="className">The class name for extension methods.</param>
    /// <param name="baseUrl">The base URL for API requests.</param>
    /// <param name="basePath">The base path for API requests.</param>
    /// <returns>Tuple containing the extension methods code and type aliases code.</returns>
    public static (string ExtensionMethods, string TypeAliases) GenerateExtensionMethods(
        OpenApiDocument document,
        string @namespace,
        string className,
        string baseUrl,
        string basePath
    )
    {
        var methods = new List<string>();
        var privateFunctions = new List<string>();
        var responseTypes = new HashSet<string>();

        foreach (var path in document.Paths)
        {
            foreach (var operation in path.Value.Operations)
            {
                var responseType = GetResponseType(operation.Value);
                var isDelete = operation.Key == OperationType.Delete;
                var resultResponseType = isDelete ? "Unit" : responseType;
                _ = responseTypes.Add(resultResponseType);

                var (method, functions) = GenerateMethod(
                    path.Key,
                    operation.Key,
                    operation.Value,
                    basePath
                );
                if (!string.IsNullOrEmpty(method))
                {
                    methods.Add(method);
                    privateFunctions.AddRange(functions);
                }
            }
        }

        var functionsCode = string.Join("\n\n", privateFunctions);
        var methodsCode = string.Join("\n\n", methods);
        var typeAliases = GenerateTypeAliasesFile(responseTypes, @namespace);

        var extensionMethodsCode = $$"""
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
                private static readonly AbsoluteUrl BaseUrl = "{{baseUrl}}".ToAbsoluteUrl();

                private static readonly JsonSerializerOptions JsonOptions = new()
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                private static readonly Deserialize<Unit> _deserializeUnit = static (_, _) => Task.FromResult(Unit.Value);

            {{CodeGenerationHelpers.Indent(functionsCode, 1)}}

            {{CodeGenerationHelpers.Indent(methodsCode, 1)}}

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
            }
            """;

        return (extensionMethodsCode, typeAliases);
    }

    private static (string Method, List<string> PrivateFunctions) GenerateMethod(
        string path,
        OperationType operationType,
        OpenApiOperation operation,
        string basePath
    )
    {
        var methodName = GetMethodName(operation, operationType, path);
        var parameters = GetParameters(operation);
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
    private static (string Method, List<string> PrivateFunctions) GenerateHttpMethod(
        OperationType operationType,
        string methodName,
        string path,
        List<(string Name, string Type, bool IsPath)> parameters,
        string? requestBodyType,
        string responseType,
        string summary
    )
#pragma warning restore CA1502
    {
        var hasBody =
            operationType is OperationType.Post or OperationType.Put or OperationType.Patch;
        var isDelete = operationType == OperationType.Delete;
        var hasPathParams = parameters.Any(p => p.IsPath);
        var queryParams = parameters.Where(p => !p.IsPath).ToList();
        var hasQueryParams = queryParams.Count > 0;

        var bodyType = requestBodyType ?? "object";
        var resultResponseType = isDelete ? "Unit" : responseType;
        var resultType = $"Result<{resultResponseType}, HttpError<string>>";
        var pathExpression = CodeGenerationHelpers.BuildPathExpression(path);
        var deserializer =
            responseType == "string" ? "DeserializeString" : $"DeserializeJson<{responseType}>";
        var queryString = hasQueryParams
            ? "?" + string.Join("&", queryParams.Select(q => $"{q.Name}={{{q.Name}}}"))
            : string.Empty;

        var verb = operationType.ToString();
        var createMethod = $"Create{verb}";
        var delegateType = $"{verb}Async";

        var privateFunctionName = $"_{char.ToLowerInvariant(methodName[0])}{methodName[1..]}";
        var privateFunctions = new List<string>();

        // GET with no parameters OR body verbs with no path params
        if ((!hasPathParams && !hasQueryParams && !hasBody) || (!hasPathParams && hasBody))
        {
            if (!hasBody)
            {
                var deserializeMethod = isDelete ? "_deserializeUnit" : deserializer;

                var method = $$"""
                        private static {{delegateType}}<{{resultResponseType}}, string, Unit> {{privateFunctionName}} { get; } =
                            RestClient.Net.HttpClientFactoryExtensions.{{createMethod}}<{{resultResponseType}}, string, Unit>(
                                url: BaseUrl,
                                buildRequest: static _ => new HttpRequestParts(new RelativeUrl("{{path}}"), null, null),
                                deserializeSuccess: {{deserializeMethod}},
                                deserializeError: DeserializeError
                            );

                        /// <summary>{{summary}}</summary>
                        public static Task<{{resultType}}> {{methodName}}(
                            this HttpClient httpClient,
                            CancellationToken ct = default
                        ) => {{privateFunctionName}}(httpClient, Unit.Value, ct);
                    """;
                return (method, privateFunctions);
            }
            else
            {
                var deserializeMethod = isDelete ? "_deserializeUnit" : deserializer;

                var method = $$"""
                        private static {{delegateType}}<{{resultResponseType}}, string, {{bodyType}}> {{privateFunctionName}} { get; } =
                            RestClient.Net.HttpClientFactoryExtensions.{{createMethod}}<{{resultResponseType}}, string, {{bodyType}}>(
                                url: BaseUrl,
                                buildRequest: static body => new HttpRequestParts(new RelativeUrl("{{path}}"), CreateJsonContent(body), null),
                                deserializeSuccess: {{deserializeMethod}},
                                deserializeError: DeserializeError
                            );

                        /// <summary>{{summary}}</summary>
                        public static Task<{{resultType}}> {{methodName}}(
                            this HttpClient httpClient,
                            {{bodyType}} body,
                            CancellationToken ct = default
                        ) => {{privateFunctionName}}(httpClient, body, ct);
                    """;
                return (method, privateFunctions);
            }
        }

        // GET with only query parameters
        if (!hasPathParams && hasQueryParams && !hasBody)
        {
            var isSingleParam = queryParams.Count == 1;
            var queryParamsType = isSingleParam
                ? queryParams[0].Type
                : $"({string.Join(", ", queryParams.Select(q => $"{q.Type} {q.Name}"))})";
            var queryParamsNames = string.Join(", ", queryParams.Select(q => q.Name));
            var paramInvocation = isSingleParam ? queryParamsNames : $"({queryParamsNames})";
            var deserializeMethod = isDelete ? "_deserializeUnit" : deserializer;
            var queryStringWithParam = isSingleParam
                ? "?" + string.Join("&", queryParams.Select(q => $"{q.Name}={{param}}"))
                : "?" + string.Join("&", queryParams.Select(q => $"{q.Name}={{param.{q.Name}}}"));

            var method = $$"""
                    private static {{delegateType}}<{{resultResponseType}}, string, {{queryParamsType}}> {{privateFunctionName}} { get; } =
                        RestClient.Net.HttpClientFactoryExtensions.{{createMethod}}<{{resultResponseType}}, string, {{queryParamsType}}>(
                            url: BaseUrl,
                            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"{{path}}{{queryStringWithParam}}"), null, null),
                            deserializeSuccess: {{deserializeMethod}},
                            deserializeError: DeserializeError
                        );

                    /// <summary>{{summary}}</summary>
                    public static Task<{{resultType}}> {{methodName}}(
                        this HttpClient httpClient,
                        {{string.Join(", ", queryParams.Select(q => $"{q.Type} {q.Name}"))}},
                        CancellationToken ct = default
                    ) => {{privateFunctionName}}(httpClient, {{paramInvocation}}, ct);
                """;
            return (method, privateFunctions);
        }

        // Has path parameters (with or without body/query)
        var pathParamsType = string.Join(", ", parameters.Where(p => p.IsPath).Select(p => p.Type));
        var pathParamsNames = string.Join(
            ", ",
            parameters.Where(p => p.IsPath).Select(p => p.Name)
        );
        var lambda =
            parameters.Count(p => p.IsPath) == 1
                ? parameters.First(p => p.IsPath).Name
                : $"({pathParamsNames})";

        // If we have query params along with path params and no body, we need to handle them specially
        if (!hasBody && hasQueryParams)
        {
            var allParamsList = parameters.Select(p => $"{p.Type} {p.Name}").ToList();
            var isSingleParam = parameters.Count == 1;
            var allParamsType = isSingleParam
                ? parameters[0].Type
                : $"({string.Join(", ", allParamsList)})";
            var allParamsNames = string.Join(", ", parameters.Select(p => p.Name));
            var paramInvocation = isSingleParam ? allParamsNames : $"({allParamsNames})";
            var deserializeMethod = isDelete ? "_deserializeUnit" : deserializer;
            var queryStringWithParam =
                isSingleParam && queryParams.Count == 1
                    ? "?" + string.Join("&", queryParams.Select(q => $"{q.Name}={{param}}"))
                    : "?"
                        + string.Join("&", queryParams.Select(q => $"{q.Name}={{param.{q.Name}}}"));
            var pathWithParam =
                isSingleParam && hasPathParams
                    ? pathExpression.Replace(
                        "{" + parameters.First(p => p.IsPath).Name + "}",
                        "{param}",
                        StringComparison.Ordinal
                    )
                    : pathExpression.Replace("{", "{param.", StringComparison.Ordinal);

            var method = $$"""
                    private static {{delegateType}}<{{resultResponseType}}, string, {{allParamsType}}> {{privateFunctionName}} { get; } =
                        RestClient.Net.HttpClientFactoryExtensions.{{createMethod}}<{{resultResponseType}}, string, {{allParamsType}}>(
                            url: BaseUrl,
                            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"{{pathWithParam}}{{queryStringWithParam}}"), null, null),
                            deserializeSuccess: {{deserializeMethod}},
                            deserializeError: DeserializeError
                        );

                    /// <summary>{{summary}}</summary>
                    public static Task<{{resultType}}> {{methodName}}(
                        this HttpClient httpClient,
                        {{string.Join(", ", allParamsList)}},
                        CancellationToken ct = default
                    ) => {{privateFunctionName}}(httpClient, {{paramInvocation}}, ct);
                """;
            return (method, privateFunctions);
        }

        if (!hasBody)
        {
            var deserializeMethod = isDelete ? "_deserializeUnit" : deserializer;

            var method = $$"""
                    private static {{delegateType}}<{{resultResponseType}}, string, {{pathParamsType}}> {{privateFunctionName}} { get; } =
                        RestClient.Net.HttpClientFactoryExtensions.{{createMethod}}<{{resultResponseType}}, string, {{pathParamsType}}>(
                            url: BaseUrl,
                            buildRequest: static {{lambda}} => new HttpRequestParts(new RelativeUrl($"{{pathExpression}}"), null, null),
                            deserializeSuccess: {{deserializeMethod}},
                            deserializeError: DeserializeError
                        );

                    /// <summary>{{summary}}</summary>
                    public static Task<{{resultType}}> {{methodName}}(
                        this HttpClient httpClient,
                        {{pathParamsType}} {{lambda}},
                        CancellationToken ct = default
                    ) => {{privateFunctionName}}(httpClient, {{lambda}}, ct);
                """;
            return (method, privateFunctions);
        }
        else
        {
            var compositeType = $"({pathParamsType} Params, {bodyType} Body)";
            var pathWithParamInterpolation = CodeGenerationHelpers
                .PathParameterRegex()
                .Replace(pathExpression, "{param.Params}");

            var method = $$"""
                    private static {{delegateType}}<{{resultResponseType}}, string, {{compositeType}}> {{privateFunctionName}} { get; } =
                        RestClient.Net.HttpClientFactoryExtensions.{{createMethod}}<{{resultResponseType}}, string, {{compositeType}}>(
                            url: BaseUrl,
                            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"{{pathWithParamInterpolation}}"), CreateJsonContent(param.Body), null),
                            deserializeSuccess: DeserializeJson<{{resultResponseType}}>,
                            deserializeError: DeserializeError
                        );

                    /// <summary>{{summary}}</summary>
                    public static Task<{{resultType}}> {{methodName}}(
                        this HttpClient httpClient,
                        {{compositeType}} param,
                        CancellationToken ct = default
                    ) => {{privateFunctionName}}(httpClient, param, ct);
                """;
            return (method, privateFunctions);
        }
    }

    private static string GetMethodName(
        OpenApiOperation operation,
        OperationType operationType,
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

        return operationType switch
        {
            OperationType.Get => $"Get{CodeGenerationHelpers.ToPascalCase(pathPart)}",
            OperationType.Post => $"Create{CodeGenerationHelpers.ToPascalCase(pathPart)}",
            OperationType.Put => $"Update{CodeGenerationHelpers.ToPascalCase(pathPart)}",
            OperationType.Delete => $"Delete{CodeGenerationHelpers.ToPascalCase(pathPart)}",
            OperationType.Patch => $"Patch{CodeGenerationHelpers.ToPascalCase(pathPart)}",
            _ => $"{operationType}{CodeGenerationHelpers.ToPascalCase(pathPart)}",
        };
    }

    private static List<(string Name, string Type, bool IsPath)> GetParameters(
        OpenApiOperation operation
    )
    {
        var parameters = new List<(string Name, string Type, bool IsPath)>();

        foreach (var param in operation.Parameters)
        {
            var isPath = param.In == ParameterLocation.Path;
            var type = ModelGenerator.MapOpenApiType(param.Schema);
            parameters.Add((param.Name, type, isPath));
        }

        return parameters;
    }

    private static string? GetRequestBodyType(OpenApiOperation operation) =>
        operation.RequestBody == null ? null
        : operation.RequestBody.Content.FirstOrDefault().Value?.Schema?.Reference != null
            ? operation.RequestBody.Content.FirstOrDefault().Value.Schema.Reference.Id
        : "object";

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
                using Ok{{aliasName}} = Outcome.Result<{{qualifiedType}}, Outcome.HttpError<string>>.Ok<{{qualifiedType}}, Outcome.HttpError<string>>;
                """
            );

            // Generate Error alias
            aliases.Add(
                $$"""
                using Error{{aliasName}} = Outcome.Result<{{qualifiedType}}, Outcome.HttpError<string>>.Error<{{qualifiedType}}, Outcome.HttpError<string>>;
                """
            );
        }

        return aliases;
    }

    private static string GetResponseType(OpenApiOperation operation)
    {
        var successResponse = operation.Responses.FirstOrDefault(r =>
            r.Key.StartsWith('2') || r.Key == "default"
        );

        if (successResponse.Value == null)
        {
            return "object";
        }

        var content = successResponse.Value.Content.FirstOrDefault();
        return content.Value?.Schema?.Reference != null ? content.Value.Schema.Reference.Id
            : content.Value?.Schema?.Type == "array"
            && content.Value.Schema.Items?.Reference != null
                ? $"List<{content.Value.Schema.Items.Reference.Id}>"
            : ModelGenerator.MapOpenApiType(content.Value?.Schema);
    }
}
