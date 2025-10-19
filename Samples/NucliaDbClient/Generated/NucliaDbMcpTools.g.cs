#nullable enable
using System.ComponentModel;
using System.Text.Json;
using Outcome;
using NucliaDB.Generated;
using ModelContextProtocol.Server;

namespace NucliaDB.Mcp;

/// <summary>MCP server tools for NucliaDb API.</summary>
[McpServerToolType]
public static class NucliaDbTools
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>Ask questions on a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xShowConsumption">xShowConsumption</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="xSynchronous">When set to true, outputs response as JSON in a non-streaming way. This is slower and requires waiting for entire answer to be ready.</param>
    /// <param name="body">Request body</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Ask questions on a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public static async Task<string> AskKnowledgeboxEndpointKbKbidAsk(HttpClient httpClient, string kbid, AskRequest body, string xNdbClient = "api", bool xShowConsumption = false, string? xNucliadbUser = null, string? xForwardedFor = null, bool xSynchronous = false)
    {
        var result = await httpClient.AskKnowledgeboxEndpointKbKbidAskAsync(kbid, body, xNdbClient, xShowConsumption, xNucliadbUser ?? "", xForwardedFor ?? "", xSynchronous, CancellationToken.None);

        return result switch
        {
            OkSyncAskResponseHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorSyncAskResponseHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>List resources of a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="query">The query to search for</param>
    /// <param name="filterExpression">Returns only documents that match this filter expression.Filtering examples can be found here: https://docs.nuclia.dev/docs/rag/advanced/search-filters This allows building complex filtering expressions and replaces the following parameters:`filters`, `range_*`, `with_status`.</param>
    /// <param name="filters">The list of filters to apply. Filtering examples can be found here: https://docs.nuclia.dev/docs/rag/advanced/search-filters</param>
    /// <param name="faceted">The list of facets to calculate. The facets follow the same syntax as filters: https://docs.nuclia.dev/docs/rag/advanced/search-filters</param>
    /// <param name="sortField">Field to sort results with (Score not supported in catalog)</param>
    /// <param name="sortLimit">sortLimit</param>
    /// <param name="sortOrder">Order to sort results with</param>
    /// <param name="pageNumber">The page number of the results to return</param>
    /// <param name="pageSize">The number of results to return per page. The maximum number of results per page allowed is 200.</param>
    /// <param name="withStatus">Filter results by resource processing status</param>
    /// <param name="rangeCreationStart">Resources created before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeCreationEnd">Resources created after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationStart">Resources modified before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationEnd">Resources modified after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="hidden">Set to filter only hidden or only non-hidden resources. Default is to return everything</param>
    /// <param name="show">Controls which types of metadata are serialized on resources of search results</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("List resources of a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public static async Task<string> CatalogGetKbKbidCatalog(HttpClient httpClient, string kbid, string? query = null, object? filterExpression = null, List<string>? filters = null, List<string>? faceted = null, string? sortField = null, object? sortLimit = null, string sortOrder = "desc", int pageNumber = 0, int pageSize = 20, object? withStatus = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, object? hidden = null, List<string>? show = null)
    {
        var result = await httpClient.CatalogGetKbKbidCatalogAsync(kbid, query ?? "", filterExpression, filters, faceted, sortField ?? "", sortLimit, sortOrder, pageNumber, pageSize, withStatus, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, hidden, show, CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxSearchResultsHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxSearchResultsHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>List resources of a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="body">Request body</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("List resources of a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public static async Task<string> CatalogPostKbKbidCatalog(HttpClient httpClient, string kbid, CatalogRequest body)
    {
        var result = await httpClient.CatalogPostKbKbidCatalogAsync(kbid, body, CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxSearchResultsHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxSearchResultsHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Send feedback for a search operation in a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Send feedback for a search operation in a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public static async Task<string> SendFeedbackEndpointKbKbidFeedback(HttpClient httpClient, string kbid, FeedbackRequest body, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var result = await httpClient.SendFeedbackEndpointKbKbidFeedbackAsync(kbid, body, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkobjectHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorobjectHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Find on a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="query">The query to search for</param>
    /// <param name="filterExpression">Returns only documents that match this filter expression.Filtering examples can be found here: https://docs.nuclia.dev/docs/rag/advanced/search-filters This allows building complex filtering expressions and replaces the following parameters:`fields`, `filters`, `range_*`, `resource_filters`, `keyword_filters`.</param>
    /// <param name="fields">The list of fields to search in. For instance: `a/title` to search only on title field. For more details on filtering by field, see: https://docs.nuclia.dev/docs/rag/advanced/search/#search-in-a-specific-field.</param>
    /// <param name="filters">The list of filters to apply. Filtering examples can be found here: https://docs.nuclia.dev/docs/rag/advanced/search-filters</param>
    /// <param name="topK">The number of results search should return. The maximum number of results allowed is 200.</param>
    /// <param name="minScore">Minimum similarity score to filter vector index results. If not specified, the default minimum score of the semantic model associated to the Knowledge Box will be used. Check out the documentation for more information on how to use this parameter: https://docs.nuclia.dev/docs/rag/advanced/search#minimum-score</param>
    /// <param name="minScoreSemantic">Minimum semantic similarity score to filter vector index results. If not specified, the default minimum score of the semantic model associated to the Knowledge Box will be used. Check out the documentation for more information on how to use this parameter: https://docs.nuclia.dev/docs/rag/advanced/search#minimum-score</param>
    /// <param name="minScoreBm25">Minimum bm25 score to filter paragraph and document index results</param>
    /// <param name="vectorset">Vectors index to perform the search in. If not provided, NucliaDB will use the default one</param>
    /// <param name="rangeCreationStart">Resources created before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeCreationEnd">Resources created after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationStart">Resources modified before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationEnd">Resources modified after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="features">List of search features to use. Each value corresponds to a lookup into on of the different indexes</param>
    /// <param name="debug">If set, the response will include some extra metadata for debugging purposes, like the list of queried nodes.</param>
    /// <param name="highlight">If set to true, the query terms will be highlighted in the results between <mark>...</mark> tags</param>
    /// <param name="show">Controls which types of metadata are serialized on resources of search results</param>
    /// <param name="fieldType">Define which field types are serialized on resources of search results</param>
    /// <param name="extracted">[Deprecated] Please use GET resource endpoint instead to get extracted metadata</param>
    /// <param name="withDuplicates">Whether to return duplicate paragraphs on the same document</param>
    /// <param name="withSynonyms">Whether to return matches for custom knowledge box synonyms of the query terms. Note: only supported for `keyword` and `fulltext` search options.</param>
    /// <param name="autofilter">If set to true, the search will automatically add filters to the query. For example, it will filter results containing the entities detected in the query</param>
    /// <param name="securityGroups">List of security groups to filter search results for. Only resources matching the query and containing the specified security groups will be returned. If empty, all resources will be considered for the search.</param>
    /// <param name="showHidden">If set to false (default), excludes hidden resources from search</param>
    /// <param name="rankFusion">Rank fusion algorithm to use to merge results from multiple retrievers (keyword, semantic)</param>
    /// <param name="reranker">Reranker let you specify which method you want to use to rerank your results at the end of retrieval</param>
    /// <param name="searchConfiguration">Load find parameters from this configuration. Parameters in the request override parameters from the configuration.</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Find on a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public static async Task<string> FindKnowledgeboxKbKbidFind(HttpClient httpClient, string kbid, string? query = null, object? filterExpression = null, List<string>? fields = null, List<string>? filters = null, object? topK = null, object? minScore = null, object? minScoreSemantic = null, float minScoreBm25 = 0, object? vectorset = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, List<string>? features = null, bool debug = false, bool highlight = false, List<string>? show = null, List<string>? fieldType = null, List<string>? extracted = null, bool withDuplicates = false, bool withSynonyms = false, bool autofilter = false, List<string>? securityGroups = null, bool showHidden = false, string rankFusion = "rrf", object? reranker = null, object? searchConfiguration = null, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var result = await httpClient.FindKnowledgeboxKbKbidFindAsync(kbid, query ?? "", filterExpression, fields, filters, topK, minScore, minScoreSemantic, minScoreBm25, vectorset, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, features, debug, highlight, show, fieldType, extracted, withDuplicates, withSynonyms, autofilter, securityGroups, showHidden, rankFusion, reranker, searchConfiguration, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxFindResultsHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxFindResultsHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Find on a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Find on a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public static async Task<string> FindPostKnowledgeboxKbKbidFind(HttpClient httpClient, string kbid, FindRequest body, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var result = await httpClient.FindPostKnowledgeboxKbKbidFindAsync(kbid, body, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxFindResultsHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxFindResultsHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Search on the Knowledge Box graph and retrieve triplets of vertex-edge-vertex --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Search on the Knowledge Box graph and retrieve triplets of vertex-edge-vertex --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public static async Task<string> GraphSearchKnowledgeboxKbKbidGraph(HttpClient httpClient, string kbid, GraphSearchRequest body, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var result = await httpClient.GraphSearchKnowledgeboxKbKbidGraphAsync(kbid, body, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkGraphSearchResponseHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorGraphSearchResponseHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Search on the Knowledge Box graph and retrieve nodes (vertices) --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Search on the Knowledge Box graph and retrieve nodes (vertices) --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public static async Task<string> GraphNodesSearchKnowledgeboxKbKbidGraphNodes(HttpClient httpClient, string kbid, GraphNodesSearchRequest body, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var result = await httpClient.GraphNodesSearchKnowledgeboxKbKbidGraphNodesAsync(kbid, body, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkGraphNodesSearchResponseHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorGraphNodesSearchResponseHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Search on the Knowledge Box graph and retrieve relations (edges) --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Search on the Knowledge Box graph and retrieve relations (edges) --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public static async Task<string> GraphRelationsSearchKnowledgeboxKbKbidGraphRelations(HttpClient httpClient, string kbid, GraphRelationsSearchRequest body, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var result = await httpClient.GraphRelationsSearchKnowledgeboxKbKbidGraphRelationsAsync(kbid, body, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkGraphRelationsSearchResponseHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorGraphRelationsSearchResponseHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Convenience endpoint that proxies requests to the Predict API. It adds the Knowledge Box configuration settings as headers to the predict API request. Refer to the Predict API documentation for more details about the request and response models: https://docs.nuclia.dev/docs/nua-api#tag/Predict --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="endpoint">endpoint</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Convenience endpoint that proxies requests to the Predict API. It adds the Knowledge Box configuration settings as headers to the predict API request. Refer to the Predict API documentation for more details about the request and response models: https://docs.nuclia.dev/docs/nua-api#tag/Predict --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public static async Task<string> PredictProxyEndpointKbKbidPredictEndpoint(HttpClient httpClient, string kbid, string endpoint, object body, string? xNucliadbUser = null, string xNdbClient = "api", string? xForwardedFor = null)
    {
        var result = await httpClient.PredictProxyEndpointKbKbidPredictEndpointAsync(kbid, endpoint, body, xNucliadbUser ?? "", xNdbClient, xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkobjectHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorobjectHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Convenience endpoint that proxies requests to the Predict API. It adds the Knowledge Box configuration settings as headers to the predict API request. Refer to the Predict API documentation for more details about the request and response models: https://docs.nuclia.dev/docs/nua-api#tag/Predict --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="endpoint">endpoint</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Convenience endpoint that proxies requests to the Predict API. It adds the Knowledge Box configuration settings as headers to the predict API request. Refer to the Predict API documentation for more details about the request and response models: https://docs.nuclia.dev/docs/nua-api#tag/Predict --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public static async Task<string> PredictProxyEndpointKbKbidPredictEndpoint2(HttpClient httpClient, string kbid, string endpoint, string? xNucliadbUser = null, string xNdbClient = "api", string? xForwardedFor = null)
    {
        var result = await httpClient.PredictProxyEndpointKbKbidPredictEndpointAsync2(kbid, endpoint, xNucliadbUser ?? "", xNdbClient, xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkobjectHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorobjectHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Ask questions to a resource --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="xShowConsumption">xShowConsumption</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="xSynchronous">When set to true, outputs response as JSON in a non-streaming way. This is slower and requires waiting for entire answer to be ready.</param>
    /// <param name="body">Request body</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Ask questions to a resource --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public static async Task<string> ResourceAskEndpointByUuidKbKbidResourceRidAsk(HttpClient httpClient, string kbid, string rid, AskRequest body, bool xShowConsumption = false, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null, bool xSynchronous = false)
    {
        var result = await httpClient.ResourceAskEndpointByUuidKbKbidResourceRidAskAsync(kbid, rid, body, xShowConsumption, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", xSynchronous, CancellationToken.None);

        return result switch
        {
            OkSyncAskResponseHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorSyncAskResponseHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Search on a single resource --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="query">query</param>
    /// <param name="filterExpression">Returns only documents that match this filter expression.Filtering examples can be found here: https://docs.nuclia.dev/docs/rag/advanced/search-filters This allows building complex filtering expressions and replaces the following parameters:`fields`, `filters`, `range_*`, `resource_filters`, `keyword_filters`.</param>
    /// <param name="fields">The list of fields to search in. For instance: `a/title` to search only on title field. For more details on filtering by field, see: https://docs.nuclia.dev/docs/rag/advanced/search/#search-in-a-specific-field.</param>
    /// <param name="filters">The list of filters to apply. Filtering examples can be found here: https://docs.nuclia.dev/docs/rag/advanced/search-filters</param>
    /// <param name="faceted">The list of facets to calculate. The facets follow the same syntax as filters: https://docs.nuclia.dev/docs/rag/advanced/search-filters</param>
    /// <param name="sortField">Field to sort results with (Score not supported in catalog)</param>
    /// <param name="sortOrder">Order to sort results with</param>
    /// <param name="topK">The number of results search should return. The maximum number of results allowed is 200.</param>
    /// <param name="rangeCreationStart">Resources created before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeCreationEnd">Resources created after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationStart">Resources modified before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationEnd">Resources modified after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="highlight">If set to true, the query terms will be highlighted in the results between <mark>...</mark> tags</param>
    /// <param name="debug">If set, the response will include some extra metadata for debugging purposes, like the list of queried nodes.</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Search on a single resource --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public static async Task<string> ResourceSearchKbKbidResourceRidSearch(HttpClient httpClient, string kbid, string rid, string query, object? filterExpression = null, List<string>? fields = null, List<string>? filters = null, List<string>? faceted = null, object? sortField = null, string sortOrder = "desc", object? topK = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, bool highlight = false, bool debug = false, string xNdbClient = "api")
    {
        var result = await httpClient.ResourceSearchKbKbidResourceRidSearchAsync(kbid, rid, query, filterExpression, fields, filters, faceted, sortField, sortOrder, topK, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, highlight, debug, xNdbClient, CancellationToken.None);

        return result switch
        {
            OkResourceSearchResultsHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceSearchResultsHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Search on a Knowledge Box and retrieve separate results for documents, paragraphs, and sentences. Usually, it is better to use `find` --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="query">The query to search for</param>
    /// <param name="filterExpression">Returns only documents that match this filter expression.Filtering examples can be found here: https://docs.nuclia.dev/docs/rag/advanced/search-filters This allows building complex filtering expressions and replaces the following parameters:`fields`, `filters`, `range_*`, `resource_filters`, `keyword_filters`.</param>
    /// <param name="fields">The list of fields to search in. For instance: `a/title` to search only on title field. For more details on filtering by field, see: https://docs.nuclia.dev/docs/rag/advanced/search/#search-in-a-specific-field.</param>
    /// <param name="filters">The list of filters to apply. Filtering examples can be found here: https://docs.nuclia.dev/docs/rag/advanced/search-filters</param>
    /// <param name="faceted">The list of facets to calculate. The facets follow the same syntax as filters: https://docs.nuclia.dev/docs/rag/advanced/search-filters</param>
    /// <param name="sortField">Field to sort results with (Score not supported in catalog)</param>
    /// <param name="sortLimit">sortLimit</param>
    /// <param name="sortOrder">Order to sort results with</param>
    /// <param name="topK">The number of results search should return. The maximum number of results allowed is 200.</param>
    /// <param name="minScore">Minimum similarity score to filter vector index results. If not specified, the default minimum score of the semantic model associated to the Knowledge Box will be used. Check out the documentation for more information on how to use this parameter: https://docs.nuclia.dev/docs/rag/advanced/search#minimum-score</param>
    /// <param name="minScoreSemantic">Minimum semantic similarity score to filter vector index results. If not specified, the default minimum score of the semantic model associated to the Knowledge Box will be used. Check out the documentation for more information on how to use this parameter: https://docs.nuclia.dev/docs/rag/advanced/search#minimum-score</param>
    /// <param name="minScoreBm25">Minimum bm25 score to filter paragraph and document index results</param>
    /// <param name="vectorset">Vectors index to perform the search in. If not provided, NucliaDB will use the default one</param>
    /// <param name="rangeCreationStart">Resources created before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeCreationEnd">Resources created after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationStart">Resources modified before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationEnd">Resources modified after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="features">List of search features to use. Each value corresponds to a lookup into on of the different indexes</param>
    /// <param name="debug">If set, the response will include some extra metadata for debugging purposes, like the list of queried nodes.</param>
    /// <param name="highlight">If set to true, the query terms will be highlighted in the results between <mark>...</mark> tags</param>
    /// <param name="show">Controls which types of metadata are serialized on resources of search results</param>
    /// <param name="fieldType">Define which field types are serialized on resources of search results</param>
    /// <param name="extracted">[Deprecated] Please use GET resource endpoint instead to get extracted metadata</param>
    /// <param name="withDuplicates">Whether to return duplicate paragraphs on the same document</param>
    /// <param name="withSynonyms">Whether to return matches for custom knowledge box synonyms of the query terms. Note: only supported for `keyword` and `fulltext` search options.</param>
    /// <param name="autofilter">If set to true, the search will automatically add filters to the query. For example, it will filter results containing the entities detected in the query</param>
    /// <param name="securityGroups">List of security groups to filter search results for. Only resources matching the query and containing the specified security groups will be returned. If empty, all resources will be considered for the search.</param>
    /// <param name="showHidden">If set to false (default), excludes hidden resources from search</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Search on a Knowledge Box and retrieve separate results for documents, paragraphs, and sentences. Usually, it is better to use `find` --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public static async Task<string> SearchKnowledgeboxKbKbidSearch(HttpClient httpClient, string kbid, string? query = null, object? filterExpression = null, List<string>? fields = null, List<string>? filters = null, List<string>? faceted = null, string? sortField = null, object? sortLimit = null, string sortOrder = "desc", int topK = 20, object? minScore = null, object? minScoreSemantic = null, float minScoreBm25 = 0, object? vectorset = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, List<string>? features = null, bool debug = false, bool highlight = false, List<string>? show = null, List<string>? fieldType = null, List<string>? extracted = null, bool withDuplicates = false, bool withSynonyms = false, bool autofilter = false, List<string>? securityGroups = null, bool showHidden = false, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var result = await httpClient.SearchKnowledgeboxKbKbidSearchAsync(kbid, query ?? "", filterExpression, fields, filters, faceted, sortField ?? "", sortLimit, sortOrder, topK, minScore, minScoreSemantic, minScoreBm25, vectorset, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, features, debug, highlight, show, fieldType, extracted, withDuplicates, withSynonyms, autofilter, securityGroups, showHidden, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxSearchResultsHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxSearchResultsHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Search on a Knowledge Box and retrieve separate results for documents, paragraphs, and sentences. Usually, it is better to use `find` --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Search on a Knowledge Box and retrieve separate results for documents, paragraphs, and sentences. Usually, it is better to use `find` --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public static async Task<string> SearchPostKnowledgeboxKbKbidSearch(HttpClient httpClient, string kbid, SearchRequest body, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var result = await httpClient.SearchPostKnowledgeboxKbKbidSearchAsync(kbid, body, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxSearchResultsHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxSearchResultsHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Ask questions to a resource --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="slug">slug</param>
    /// <param name="xShowConsumption">xShowConsumption</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="xSynchronous">When set to true, outputs response as JSON in a non-streaming way. This is slower and requires waiting for entire answer to be ready.</param>
    /// <param name="body">Request body</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Ask questions to a resource --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public static async Task<string> ResourceAskEndpointBySlugKbKbidSlugSlugAsk(HttpClient httpClient, string kbid, string slug, AskRequest body, bool xShowConsumption = false, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null, bool xSynchronous = false)
    {
        var result = await httpClient.ResourceAskEndpointBySlugKbKbidSlugSlugAskAsync(kbid, slug, body, xShowConsumption, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", xSynchronous, CancellationToken.None);

        return result switch
        {
            OkSyncAskResponseHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorSyncAskResponseHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Suggestions on a knowledge box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="query">The query to get suggestions for</param>
    /// <param name="fields">The list of fields to search in. For instance: `a/title` to search only on title field. For more details on filtering by field, see: https://docs.nuclia.dev/docs/rag/advanced/search/#search-in-a-specific-field.</param>
    /// <param name="filters">The list of filters to apply. Filtering examples can be found here: https://docs.nuclia.dev/docs/rag/advanced/search-filters</param>
    /// <param name="faceted">The list of facets to calculate. The facets follow the same syntax as filters: https://docs.nuclia.dev/docs/rag/advanced/search-filters</param>
    /// <param name="rangeCreationStart">Resources created before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeCreationEnd">Resources created after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationStart">Resources modified before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationEnd">Resources modified after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="features">Features enabled for the suggest endpoint.</param>
    /// <param name="show">Controls which types of metadata are serialized on resources of search results</param>
    /// <param name="fieldType">Define which field types are serialized on resources of search results</param>
    /// <param name="debug">If set, the response will include some extra metadata for debugging purposes, like the list of queried nodes.</param>
    /// <param name="highlight">If set to true, the query terms will be highlighted in the results between <mark>...</mark> tags</param>
    /// <param name="showHidden">If set to false (default), excludes hidden resources from search</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Suggestions on a knowledge box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public static async Task<string> SuggestKnowledgeboxKbKbidSuggest(HttpClient httpClient, string kbid, string query, List<string>? fields = null, List<string>? filters = null, List<string>? faceted = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, List<string>? features = null, List<string>? show = null, List<string>? fieldType = null, bool debug = false, bool highlight = false, bool showHidden = false, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var result = await httpClient.SuggestKnowledgeboxKbKbidSuggestAsync(kbid, query, fields, filters, faceted, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, features, show, fieldType, debug, highlight, showHidden, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxSuggestResultsHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxSuggestResultsHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Summarize Your Documents --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xShowConsumption">xShowConsumption</param>
    /// <param name="body">Request body</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Summarize Your Documents --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public static async Task<string> SummarizeEndpointKbKbidSummarize(HttpClient httpClient, string kbid, SummarizeRequest body, bool xShowConsumption = false)
    {
        var result = await httpClient.SummarizeEndpointKbKbidSummarizeAsync(kbid, body, xShowConsumption, CancellationToken.None);

        return result switch
        {
            OkSummarizedResponseHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorSummarizedResponseHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }
}