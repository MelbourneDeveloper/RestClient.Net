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

namespace NucliaDB.Generated;

/// <summary>Extension methods for API operations.</summary>
public static class NucliaDBApiExtensions
{
    #region Configuration

    private static readonly AbsoluteUrl BaseUrl = "http://localhost:8080".ToAbsoluteUrl();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly Deserialize<Unit> _deserializeUnit = static (_, _) =>
        Task.FromResult(Unit.Value);

    #endregion

    #region Kb Operations

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`</summary>
    public static Task<Result<KnowledgeBoxObj, HttpError<HTTPValidationError>>> KbBySlugKbSSlugGetAsync(
        this HttpClient httpClient,
        string slug,
        CancellationToken cancellationToken = default
    ) => _kbBySlugKbSSlugGetAsync(httpClient, slug, cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`</summary>
    public static Task<Result<KnowledgeBoxObj, HttpError<HTTPValidationError>>> KbKbKbidGetAsync(
        this HttpClient httpClient,
        string kbid, string xNUCLIADBROLES = "READER",
        CancellationToken cancellationToken = default
    ) => _kbKbKbidGetAsync(httpClient, (kbid, xNUCLIADBROLES), cancellationToken);
    
    /// <summary>Ask questions on a Knowledge Box</summary>
    public static Task<Result<SyncAskResponse, HttpError<HTTPValidationError>>> AskKnowledgeboxEndpointKbKbidAskAsync(
        this HttpClient httpClient,
        string kbid, AskRequest body, string xNdbClient = "api", bool xShowConsumption = false, string? xNucliadbUser = null, string? xForwardedFor = null, bool xSynchronous = false,
        CancellationToken cancellationToken = default
    ) => _askKnowledgeboxEndpointKbKbidAskAsync(httpClient, (kbid, xNdbClient, xShowConsumption, xNucliadbUser, xForwardedFor, xSynchronous, body), cancellationToken);
    
    /// <summary>List resources of a Knowledge Box</summary>
    public static Task<Result<KnowledgeboxSearchResults, HttpError<HTTPValidationError>>> CatalogGetKbKbidCatalogAsync(
        this HttpClient httpClient,
        string kbid, string query = "", object? filterExpression = null, List<string>? filters = null, List<string>? faceted = null, string sortField = "", object? sortLimit = null, string sortOrder = "desc", int pageNumber = 0, int pageSize = 20, object? withStatus = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, object? hidden = null, List<string>? show = null,
        CancellationToken cancellationToken = default
    ) => _catalogGetKbKbidCatalogAsync(httpClient, (kbid, query, filterExpression, filters, faceted, sortField, sortLimit, sortOrder, pageNumber, pageSize, withStatus, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, hidden, show), cancellationToken);
    
    /// <summary>List resources of a Knowledge Box</summary>
    public static Task<Result<KnowledgeboxSearchResults, HttpError<HTTPValidationError>>> CatalogPostKbKbidCatalogAsync(
        this HttpClient httpClient,
        string kbid, CatalogRequest body,
        CancellationToken cancellationToken = default
    ) => _catalogPostKbKbidCatalogAsync(httpClient, (kbid, body), cancellationToken);
    
    /// <summary>Current configuration of models assigned to a Knowledge Box</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> ConfigurationKbKbidConfigurationGetAsync(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _configurationKbKbidConfigurationGetAsync(httpClient, kbid, cancellationToken);
    
    /// <summary>Update current configuration of models assigned to a Knowledge Box</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> ConfigurationKbKbidConfigurationPatchAsync(
        this HttpClient httpClient,
        string kbid, object body,
        CancellationToken cancellationToken = default
    ) => _configurationKbKbidConfigurationPatchAsync(httpClient, (kbid, body), cancellationToken);
    
    /// <summary>Create configuration of models assigned to a Knowledge Box</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> SetConfigurationKbKbidConfigurationAsync(
        this HttpClient httpClient,
        string kbid, object body,
        CancellationToken cancellationToken = default
    ) => _setConfigurationKbKbidConfigurationAsync(httpClient, (kbid, body), cancellationToken);
    
    /// <summary>Summary of amount of different things inside a knowledgebox</summary>
    public static Task<Result<KnowledgeboxCounters, HttpError<HTTPValidationError>>> KnowledgeboxCountersKbKbidCountersAsync(
        this HttpClient httpClient,
        string kbid, bool debug = false, string xNUCLIADBROLES = "READER",
        CancellationToken cancellationToken = default
    ) => _knowledgeboxCountersKbKbidCountersAsync(httpClient, (kbid, debug, xNUCLIADBROLES), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> SetCustomSynonymsKbKbidCustomSynonymsAsync(
        this HttpClient httpClient,
        string kbid, KnowledgeBoxSynonyms body,
        CancellationToken cancellationToken = default
    ) => _setCustomSynonymsKbKbidCustomSynonymsAsync(httpClient, (kbid, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<Unit, HttpError<HTTPValidationError>>> CustomSynonymsKbKbidCustomSynonymsDeleteAsync(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _customSynonymsKbKbidCustomSynonymsDeleteAsync(httpClient, kbid, cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    public static Task<Result<KnowledgeBoxSynonyms, HttpError<HTTPValidationError>>> CustomSynonymsKbKbidCustomSynonymsGetAsync(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _customSynonymsKbKbidCustomSynonymsGetAsync(httpClient, kbid, cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> UpdateEntitiesGroupKbKbidEntitiesgroupGroupAsync(
        this HttpClient httpClient,
        string kbid, string group, UpdateEntitiesGroupPayload body,
        CancellationToken cancellationToken = default
    ) => _updateEntitiesGroupKbKbidEntitiesgroupGroupAsync(httpClient, ((kbid, group), body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<Unit, HttpError<HTTPValidationError>>> EntitiesKbKbidEntitiesgroupGroupDeleteAsync(
        this HttpClient httpClient,
        string kbid, string group,
        CancellationToken cancellationToken = default
    ) => _entitiesKbKbidEntitiesgroupGroupDeleteAsync(httpClient, (kbid, group), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    public static Task<Result<EntitiesGroup, HttpError<HTTPValidationError>>> EntityKbKbidEntitiesgroupGroupGetAsync(
        this HttpClient httpClient,
        string kbid, string group,
        CancellationToken cancellationToken = default
    ) => _entityKbKbidEntitiesgroupGroupGetAsync(httpClient, (kbid, group), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> CreateEntitiesGroupKbKbidEntitiesgroupsAsync(
        this HttpClient httpClient,
        string kbid, CreateEntitiesGroupPayload body,
        CancellationToken cancellationToken = default
    ) => _createEntitiesGroupKbKbidEntitiesgroupsAsync(httpClient, (kbid, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    public static Task<Result<KnowledgeBoxEntities, HttpError<HTTPValidationError>>> EntitiesKbKbidEntitiesgroupsGetAsync(
        this HttpClient httpClient,
        string kbid, bool showEntities = false,
        CancellationToken cancellationToken = default
    ) => _entitiesKbKbidEntitiesgroupsGetAsync(httpClient, (kbid, showEntities), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`</summary>
    public static Task<Result<CreateExportResponse, HttpError<HTTPValidationError>>> StartKbExportEndpointKbKbidExportAsync(
        this HttpClient httpClient,
        string kbid, object body,
        CancellationToken cancellationToken = default
    ) => _startKbExportEndpointKbKbidExportAsync(httpClient, (kbid, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> DownloadExportKbEndpointKbKbidExportExportIdAsync(
        this HttpClient httpClient,
        string kbid, string exportId,
        CancellationToken cancellationToken = default
    ) => _downloadExportKbEndpointKbKbidExportExportIdAsync(httpClient, (kbid, exportId), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`</summary>
    public static Task<Result<StatusResponse, HttpError<HTTPValidationError>>> ExportStatusEndpointKbKbidExportExportIdStatusGetAsync(
        this HttpClient httpClient,
        string kbid, string exportId,
        CancellationToken cancellationToken = default
    ) => _exportStatusEndpointKbKbidExportExportIdStatusGetAsync(httpClient, (kbid, exportId), cancellationToken);
    
    /// <summary>Add a extract strategy to a KB</summary>
    public static Task<Result<string, HttpError<HTTPValidationError>>> AddStrategyKbKbidExtractStrategiesAsync(
        this HttpClient httpClient,
        string kbid, ExtractConfig body,
        CancellationToken cancellationToken = default
    ) => _addStrategyKbKbidExtractStrategiesAsync(httpClient, (kbid, body), cancellationToken);
    
    /// <summary>Get available extract strategies</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> ExtractStrategiesKbKbidExtractStrategiesGetAsync(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _extractStrategiesKbKbidExtractStrategiesGetAsync(httpClient, kbid, cancellationToken);
    
    /// <summary>Removes a extract strategy from a KB</summary>
    public static Task<Result<Unit, HttpError<HTTPValidationError>>> StrategyKbKbidExtractStrategiesStrategyStrategyIdDeleteAsync(
        this HttpClient httpClient,
        string kbid, string strategyId,
        CancellationToken cancellationToken = default
    ) => _strategyKbKbidExtractStrategiesStrategyStrategyIdDeleteAsync(httpClient, (kbid, strategyId), cancellationToken);
    
    /// <summary>Get extract strategy for a given id</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> ExtractStrategyFromIdKbKbidExtractStrategiesStrategyStrategyIdGetAsync(
        this HttpClient httpClient,
        string kbid, string strategyId,
        CancellationToken cancellationToken = default
    ) => _extractStrategyFromIdKbKbidExtractStrategiesStrategyStrategyIdGetAsync(httpClient, (kbid, strategyId), cancellationToken);
    
    /// <summary>Send feedback for a search operation in a Knowledge Box</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> SendFeedbackEndpointKbKbidFeedbackAsync(
        this HttpClient httpClient,
        string kbid, FeedbackRequest body, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null,
        CancellationToken cancellationToken = default
    ) => _sendFeedbackEndpointKbKbidFeedbackAsync(httpClient, (kbid, xNdbClient, xNucliadbUser, xForwardedFor, body), cancellationToken);
    
    /// <summary>Find on a Knowledge Box</summary>
    public static Task<Result<KnowledgeboxFindResults, HttpError<HTTPValidationError>>> FindKnowledgeboxKbKbidFindAsync(
        this HttpClient httpClient,
        string kbid, string query = "", object? filterExpression = null, List<string>? fields = null, List<string>? filters = null, object? topK = null, object? minScore = null, object? minScoreSemantic = null, float minScoreBm25 = 0, object? vectorset = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, List<string>? features = null, bool debug = false, bool highlight = false, List<string>? show = null, List<string>? fieldType = null, List<string>? extracted = null, bool withDuplicates = false, bool withSynonyms = false, bool autofilter = false, List<string>? securityGroups = null, bool showHidden = false, string rankFusion = "rrf", object? reranker = null, object? searchConfiguration = null, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null,
        CancellationToken cancellationToken = default
    ) => _findKnowledgeboxKbKbidFindAsync(httpClient, (kbid, query, filterExpression, fields, filters, topK, minScore, minScoreSemantic, minScoreBm25, vectorset, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, features, debug, highlight, show, fieldType, extracted, withDuplicates, withSynonyms, autofilter, securityGroups, showHidden, rankFusion, reranker, searchConfiguration, xNdbClient, xNucliadbUser, xForwardedFor), cancellationToken);
    
    /// <summary>Find on a Knowledge Box</summary>
    public static Task<Result<KnowledgeboxFindResults, HttpError<HTTPValidationError>>> FindPostKnowledgeboxKbKbidFindAsync(
        this HttpClient httpClient,
        string kbid, FindRequest body, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null,
        CancellationToken cancellationToken = default
    ) => _findPostKnowledgeboxKbKbidFindAsync(httpClient, (kbid, xNdbClient, xNucliadbUser, xForwardedFor, body), cancellationToken);
    
    /// <summary>Search on the Knowledge Box graph and retrieve triplets of vertex-edge-vertex</summary>
    public static Task<Result<GraphSearchResponse, HttpError<HTTPValidationError>>> GraphSearchKnowledgeboxKbKbidGraphAsync(
        this HttpClient httpClient,
        string kbid, GraphSearchRequest body, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null,
        CancellationToken cancellationToken = default
    ) => _graphSearchKnowledgeboxKbKbidGraphAsync(httpClient, (kbid, xNdbClient, xNucliadbUser, xForwardedFor, body), cancellationToken);
    
    /// <summary>Search on the Knowledge Box graph and retrieve nodes (vertices)</summary>
    public static Task<Result<GraphNodesSearchResponse, HttpError<HTTPValidationError>>> GraphNodesSearchKnowledgeboxKbKbidGraphNodesAsync(
        this HttpClient httpClient,
        string kbid, GraphNodesSearchRequest body, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null,
        CancellationToken cancellationToken = default
    ) => _graphNodesSearchKnowledgeboxKbKbidGraphNodesAsync(httpClient, (kbid, xNdbClient, xNucliadbUser, xForwardedFor, body), cancellationToken);
    
    /// <summary>Search on the Knowledge Box graph and retrieve relations (edges)</summary>
    public static Task<Result<GraphRelationsSearchResponse, HttpError<HTTPValidationError>>> GraphRelationsSearchKnowledgeboxKbKbidGraphRelationsAsync(
        this HttpClient httpClient,
        string kbid, GraphRelationsSearchRequest body, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null,
        CancellationToken cancellationToken = default
    ) => _graphRelationsSearchKnowledgeboxKbKbidGraphRelationsAsync(httpClient, (kbid, xNdbClient, xNucliadbUser, xForwardedFor, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`</summary>
    public static Task<Result<CreateImportResponse, HttpError<HTTPValidationError>>> StartKbImportEndpointKbKbidImportAsync(
        this HttpClient httpClient,
        string kbid, object body,
        CancellationToken cancellationToken = default
    ) => _startKbImportEndpointKbKbidImportAsync(httpClient, (kbid, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`</summary>
    public static Task<Result<StatusResponse, HttpError<HTTPValidationError>>> ImportStatusEndpointKbKbidImportImportIdStatusGetAsync(
        this HttpClient httpClient,
        string kbid, string importId,
        CancellationToken cancellationToken = default
    ) => _importStatusEndpointKbKbidImportImportIdStatusGetAsync(httpClient, (kbid, importId), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> SetLabelsetEndpointKbKbidLabelsetLabelsetAsync(
        this HttpClient httpClient,
        string kbid, string labelset, LabelSet body,
        CancellationToken cancellationToken = default
    ) => _setLabelsetEndpointKbKbidLabelsetLabelsetAsync(httpClient, ((kbid, labelset), body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<Unit, HttpError<HTTPValidationError>>> LabelsetEndpointKbKbidLabelsetLabelsetDeleteAsync(
        this HttpClient httpClient,
        string kbid, string labelset,
        CancellationToken cancellationToken = default
    ) => _labelsetEndpointKbKbidLabelsetLabelsetDeleteAsync(httpClient, (kbid, labelset), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    public static Task<Result<LabelSet, HttpError<string>>> LabelsetEndpointKbKbidLabelsetLabelsetGetAsync(
        this HttpClient httpClient,
        string kbid, string labelset,
        CancellationToken cancellationToken = default
    ) => _labelsetEndpointKbKbidLabelsetLabelsetGetAsync(httpClient, (kbid, labelset), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    public static Task<Result<KnowledgeBoxLabels, HttpError<HTTPValidationError>>> LabelsetsEndointKbKbidLabelsetsGetAsync(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _labelsetsEndointKbKbidLabelsetsGetAsync(httpClient, kbid, cancellationToken);
    
    /// <summary>Get metadata for a particular model</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> ModelKbKbidModelModelIdGetAsync(
        this HttpClient httpClient,
        string kbid, string modelId,
        CancellationToken cancellationToken = default
    ) => _modelKbKbidModelModelIdGetAsync(httpClient, (kbid, modelId), cancellationToken);
    
    /// <summary>Get available models</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> ModelsKbKbidModelsGetAsync(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _modelsKbKbidModelsGetAsync(httpClient, kbid, cancellationToken);
    
    /// <summary>Download the trained model or any other generated file as a result of a training task on a Knowledge Box.</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> DownloadModelKbKbidModelsModelIdFilenameAsync(
        this HttpClient httpClient,
        string kbid, string modelId, string filename,
        CancellationToken cancellationToken = default
    ) => _downloadModelKbKbidModelsModelIdFilenameAsync(httpClient, (kbid, modelId, filename), cancellationToken);
    
    /// <summary>Provides a stream of activity notifications for the given Knowledge Box. The stream will be automatically closed after 2 minutes.</summary>
    public static Task<Result<object, HttpError<string>>> NotificationsEndpointKbKbidNotificationsAsync(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _notificationsEndpointKbKbidNotificationsAsync(httpClient, kbid, cancellationToken);
    
    /// <summary>Convenience endpoint that proxies requests to the Predict API. It adds the Knowledge Box configuration settings as headers to the predict API request. Refer to the Predict API documentation for more details about the request and response models: https://docs.nuclia.dev/docs/nua-api#tag/Predict</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> PredictProxyEndpointKbKbidPredictEndpointAsync(
        this HttpClient httpClient,
        string kbid, string endpoint, object body, string? xNucliadbUser = null, string xNdbClient = "api", string? xForwardedFor = null,
        CancellationToken cancellationToken = default
    ) => _predictProxyEndpointKbKbidPredictEndpointAsync(httpClient, (kbid, endpoint, xNucliadbUser, xNdbClient, xForwardedFor, body), cancellationToken);
    
    /// <summary>Convenience endpoint that proxies requests to the Predict API. It adds the Knowledge Box configuration settings as headers to the predict API request. Refer to the Predict API documentation for more details about the request and response models: https://docs.nuclia.dev/docs/nua-api#tag/Predict</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> PredictProxyEndpointKbKbidPredictEndpointAsync2(
        this HttpClient httpClient,
        string kbid, string endpoint, string? xNucliadbUser = null, string xNdbClient = "api", string? xForwardedFor = null,
        CancellationToken cancellationToken = default
    ) => _predictProxyEndpointKbKbidPredictEndpointAsync2(httpClient, (kbid, endpoint, xNucliadbUser, xNdbClient, xForwardedFor), cancellationToken);
    
    /// <summary>Provides the status of the processing of the given Knowledge Box.</summary>
    public static Task<Result<RequestsResults, HttpError<string>>> ProcessingStatusKbKbidProcessingStatusAsync(
        this HttpClient httpClient,
        string kbid, object? cursor = null, object? scheduled = null, int limit = 20,
        CancellationToken cancellationToken = default
    ) => _processingStatusKbKbidProcessingStatusAsync(httpClient, (kbid, cursor, scheduled, limit), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> TusPostRidPrefixKbKbidResourcePathRidFileFieldTusuploadAsync(
        this HttpClient httpClient,
        string kbid, string pathRid, string field, object body, object? xExtractStrategy = null, object? xSplitStrategy = null,
        CancellationToken cancellationToken = default
    ) => _tusPostRidPrefixKbKbidResourcePathRidFileFieldTusuploadAsync(httpClient, (kbid, pathRid, field, xExtractStrategy, xSplitStrategy, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> UploadInformationKbKbidResourcePathRidFileFieldTusuploadUploadIdAsync(
        this HttpClient httpClient,
        string kbid, string pathRid, string field, string uploadId,
        CancellationToken cancellationToken = default
    ) => _uploadInformationKbKbidResourcePathRidFileFieldTusuploadUploadIdAsync(httpClient, (kbid, pathRid, field, uploadId), cancellationToken);
    
    /// <summary>Upload a file as a field on an existing resource, if the field exists will return a conflict (419)</summary>
    public static Task<Result<ResourceFileUploaded, HttpError<HTTPValidationError>>> UploadRidPrefixKbKbidResourcePathRidFileFieldUploadAsync(
        this HttpClient httpClient,
        string kbid, string pathRid, string field, object body, object? xFilename = null, object? xPassword = null, object? xLanguage = null, object? xMd5 = null, object? xExtractStrategy = null, object? xSplitStrategy = null,
        CancellationToken cancellationToken = default
    ) => _uploadRidPrefixKbKbidResourcePathRidFileFieldUploadAsync(httpClient, (kbid, pathRid, field, xFilename, xPassword, xLanguage, xMd5, xExtractStrategy, xSplitStrategy, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<ResourceUpdated, HttpError<HTTPValidationError>>> ModifyResourceRidPrefixKbKbidResourceRidAsync(
        this HttpClient httpClient,
        string kbid, string rid, UpdateResourcePayload body, string? xNucliadbUser = null, bool xSkipStore = false, string xNUCLIADBROLES = "WRITER",
        CancellationToken cancellationToken = default
    ) => _modifyResourceRidPrefixKbKbidResourceRidAsync(httpClient, (kbid, rid, xNucliadbUser, xSkipStore, xNUCLIADBROLES, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<Unit, HttpError<HTTPValidationError>>> ResourceRidPrefixKbKbidResourceRidDeleteAsync(
        this HttpClient httpClient,
        string kbid, string rid, string xNUCLIADBROLES = "WRITER",
        CancellationToken cancellationToken = default
    ) => _resourceRidPrefixKbKbidResourceRidDeleteAsync(httpClient, (kbid, rid, xNUCLIADBROLES), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    public static Task<Result<NucliadbModelsResourceResource, HttpError<HTTPValidationError>>> ResourceByUuidKbKbidResourceRidGetAsync(
        this HttpClient httpClient,
        string kbid, string rid, List<string>? show = null, List<string>? fieldType = null, List<string>? extracted = null, string? xNucliadbUser = null, string? xForwardedFor = null, string xNUCLIADBROLES = "READER",
        CancellationToken cancellationToken = default
    ) => _resourceByUuidKbKbidResourceRidGetAsync(httpClient, (kbid, rid, show, fieldType, extracted, xNucliadbUser, xForwardedFor, xNUCLIADBROLES), cancellationToken);
    
    /// <summary>Ask questions to a resource</summary>
    public static Task<Result<SyncAskResponse, HttpError<HTTPValidationError>>> ResourceAskEndpointByUuidKbKbidResourceRidAskAsync(
        this HttpClient httpClient,
        string kbid, string rid, AskRequest body, bool xShowConsumption = false, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null, bool xSynchronous = false,
        CancellationToken cancellationToken = default
    ) => _resourceAskEndpointByUuidKbKbidResourceRidAskAsync(httpClient, (kbid, rid, xShowConsumption, xNdbClient, xNucliadbUser, xForwardedFor, xSynchronous, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<HTTPValidationError>>> AddResourceFieldConversationRidPrefixKbKbidResourceRidConversationFieldIdAsync(
        this HttpClient httpClient,
        string kbid, string rid, string fieldId, InputConversationField body,
        CancellationToken cancellationToken = default
    ) => _addResourceFieldConversationRidPrefixKbKbidResourceRidConversationFieldIdAsync(httpClient, ((kbid, rid, fieldId), body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> DownloadFieldConversationAttachmentRidPrefixKbKbidResourceRidConversationFieldIdDownloadFieldMessageIdFileNumAsync(
        this HttpClient httpClient,
        string kbid, string rid, string fieldId, string messageId, int fileNum,
        CancellationToken cancellationToken = default
    ) => _downloadFieldConversationAttachmentRidPrefixKbKbidResourceRidConversationFieldIdDownloadFieldMessageIdFileNumAsync(httpClient, (kbid, rid, fieldId, messageId, fileNum), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<HTTPValidationError>>> AppendMessagesToConversationFieldRidPrefixKbKbidResourceRidConversationFieldIdMessagesAsync(
        this HttpClient httpClient,
        string kbid, string rid, string fieldId, object body,
        CancellationToken cancellationToken = default
    ) => _appendMessagesToConversationFieldRidPrefixKbKbidResourceRidConversationFieldIdMessagesAsync(httpClient, ((kbid, rid, fieldId), body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<HTTPValidationError>>> AddResourceFieldFileRidPrefixKbKbidResourceRidFileFieldIdAsync(
        this HttpClient httpClient,
        string kbid, string rid, string fieldId, FileField body, bool xSkipStore = false,
        CancellationToken cancellationToken = default
    ) => _addResourceFieldFileRidPrefixKbKbidResourceRidFileFieldIdAsync(httpClient, (kbid, rid, fieldId, xSkipStore, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> DownloadFieldFileRidPrefixKbKbidResourceRidFileFieldIdDownloadFieldAsync(
        this HttpClient httpClient,
        string kbid, string rid, string fieldId, bool inline = false,
        CancellationToken cancellationToken = default
    ) => _downloadFieldFileRidPrefixKbKbidResourceRidFileFieldIdDownloadFieldAsync(httpClient, (kbid, rid, fieldId, inline), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<ResourceUpdated, HttpError<HTTPValidationError>>> ReprocessFileFieldKbKbidResourceRidFileFieldIdReprocessAsync(
        this HttpClient httpClient,
        string kbid, string rid, string fieldId, object body, bool resetTitle = false, string? xNucliadbUser = null, object? xFilePassword = null,
        CancellationToken cancellationToken = default
    ) => _reprocessFileFieldKbKbidResourceRidFileFieldIdReprocessAsync(httpClient, (kbid, rid, fieldId, resetTitle, xNucliadbUser, xFilePassword, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> TusPatchRidPrefixKbKbidResourceRidFileFieldTusuploadUploadIdAsync(
        this HttpClient httpClient,
        string kbid, string rid, string field, string uploadId, object body,
        CancellationToken cancellationToken = default
    ) => _tusPatchRidPrefixKbKbidResourceRidFileFieldTusuploadUploadIdAsync(httpClient, ((kbid, rid, field, uploadId), body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<HTTPValidationError>>> AddResourceFieldLinkRidPrefixKbKbidResourceRidLinkFieldIdAsync(
        this HttpClient httpClient,
        string kbid, string rid, string fieldId, LinkField body,
        CancellationToken cancellationToken = default
    ) => _addResourceFieldLinkRidPrefixKbKbidResourceRidLinkFieldIdAsync(httpClient, ((kbid, rid, fieldId), body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> ReindexResourceRidPrefixKbKbidResourceRidReindexAsync(
        this HttpClient httpClient,
        string kbid, string rid, object body, bool reindexVectors = false,
        CancellationToken cancellationToken = default
    ) => _reindexResourceRidPrefixKbKbidResourceRidReindexAsync(httpClient, (kbid, rid, reindexVectors, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<ResourceUpdated, HttpError<HTTPValidationError>>> ReprocessResourceRidPrefixKbKbidResourceRidReprocessAsync(
        this HttpClient httpClient,
        string kbid, string rid, object body, bool resetTitle = false, string? xNucliadbUser = null,
        CancellationToken cancellationToken = default
    ) => _reprocessResourceRidPrefixKbKbidResourceRidReprocessAsync(httpClient, (kbid, rid, resetTitle, xNucliadbUser, body), cancellationToken);
    
    /// <summary>Run Agents on Resource</summary>
    public static Task<Result<ResourceAgentsResponse, HttpError<HTTPValidationError>>> RunAgentsByUuidKbKbidResourceRidRunAgentsAsync(
        this HttpClient httpClient,
        string kbid, string rid, ResourceAgentsRequest body, string? xNucliadbUser = null,
        CancellationToken cancellationToken = default
    ) => _runAgentsByUuidKbKbidResourceRidRunAgentsAsync(httpClient, (kbid, rid, xNucliadbUser, body), cancellationToken);
    
    /// <summary>Search on a single resource</summary>
    public static Task<Result<ResourceSearchResults, HttpError<HTTPValidationError>>> ResourceSearchKbKbidResourceRidSearchAsync(
        this HttpClient httpClient,
        string kbid, string rid, string query, object? filterExpression = null, List<string>? fields = null, List<string>? filters = null, List<string>? faceted = null, object? sortField = null, string sortOrder = "desc", object? topK = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, bool highlight = false, bool debug = false, string xNdbClient = "api",
        CancellationToken cancellationToken = default
    ) => _resourceSearchKbKbidResourceRidSearchAsync(httpClient, (kbid, rid, query, filterExpression, fields, filters, faceted, sortField, sortOrder, topK, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, highlight, debug, xNdbClient), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<HTTPValidationError>>> AddResourceFieldTextRidPrefixKbKbidResourceRidTextFieldIdAsync(
        this HttpClient httpClient,
        string kbid, string rid, string fieldId, TextField body, string xNUCLIADBROLES = "WRITER",
        CancellationToken cancellationToken = default
    ) => _addResourceFieldTextRidPrefixKbKbidResourceRidTextFieldIdAsync(httpClient, (kbid, rid, fieldId, xNUCLIADBROLES, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<Unit, HttpError<HTTPValidationError>>> ResourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdDeleteAsync(
        this HttpClient httpClient,
        string kbid, string rid, string fieldType, string fieldId,
        CancellationToken cancellationToken = default
    ) => _resourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdDeleteAsync(httpClient, (kbid, rid, fieldType, fieldId), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    public static Task<Result<ResourceField, HttpError<HTTPValidationError>>> ResourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdGetAsync(
        this HttpClient httpClient,
        string kbid, string rid, string fieldType, string fieldId, List<string>? show = null, List<string>? extracted = null, object? page = null,
        CancellationToken cancellationToken = default
    ) => _resourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdGetAsync(httpClient, (kbid, rid, fieldType, fieldId, show, extracted, page), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> DownloadExtractFileRidPrefixKbKbidResourceRidFieldTypeFieldIdDownloadExtractedDownloadFieldAsync(
        this HttpClient httpClient,
        string kbid, string rid, string fieldType, string fieldId, string downloadField,
        CancellationToken cancellationToken = default
    ) => _downloadExtractFileRidPrefixKbKbidResourceRidFieldTypeFieldIdDownloadExtractedDownloadFieldAsync(httpClient, (kbid, rid, fieldType, fieldId, downloadField), cancellationToken);
    
    /// <summary>Create a new Resource in a Knowledge Box</summary>
    public static Task<Result<ResourceCreated, HttpError<HTTPValidationError>>> CreateResourceKbKbidResourcesAsync(
        this HttpClient httpClient,
        string kbid, CreateResourcePayload body, bool xSkipStore = false, string? xNucliadbUser = null, string xNUCLIADBROLES = "WRITER",
        CancellationToken cancellationToken = default
    ) => _createResourceKbKbidResourcesAsync(httpClient, (kbid, xSkipStore, xNucliadbUser, xNUCLIADBROLES, body), cancellationToken);
    
    /// <summary>List of resources of a knowledgebox</summary>
    public static Task<Result<ResourceList, HttpError<HTTPValidationError>>> ListResourcesKbKbidResourcesAsync(
        this HttpClient httpClient,
        string kbid, int page = 0, int size = 20, string xNUCLIADBROLES = "READER",
        CancellationToken cancellationToken = default
    ) => _listResourcesKbKbidResourcesAsync(httpClient, (kbid, page, size, xNUCLIADBROLES), cancellationToken);
    
    /// <summary>Get jsonschema definition to update the `learning_configuration` of your Knowledge Box</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> SchemaForConfigurationUpdatesKbKbidSchemaGetAsync(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _schemaForConfigurationUpdatesKbKbidSchemaGetAsync(httpClient, kbid, cancellationToken);
    
    /// <summary>Search on a Knowledge Box and retrieve separate results for documents, paragraphs, and sentences. Usually, it is better to use `find`</summary>
    public static Task<Result<KnowledgeboxSearchResults, HttpError<HTTPValidationError>>> SearchKnowledgeboxKbKbidSearchAsync(
        this HttpClient httpClient,
        string kbid, string query = "", object? filterExpression = null, List<string>? fields = null, List<string>? filters = null, List<string>? faceted = null, string sortField = "", object? sortLimit = null, string sortOrder = "desc", int topK = 20, object? minScore = null, object? minScoreSemantic = null, float minScoreBm25 = 0, object? vectorset = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, List<string>? features = null, bool debug = false, bool highlight = false, List<string>? show = null, List<string>? fieldType = null, List<string>? extracted = null, bool withDuplicates = false, bool withSynonyms = false, bool autofilter = false, List<string>? securityGroups = null, bool showHidden = false, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null,
        CancellationToken cancellationToken = default
    ) => _searchKnowledgeboxKbKbidSearchAsync(httpClient, (kbid, query, filterExpression, fields, filters, faceted, sortField, sortLimit, sortOrder, topK, minScore, minScoreSemantic, minScoreBm25, vectorset, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, features, debug, highlight, show, fieldType, extracted, withDuplicates, withSynonyms, autofilter, securityGroups, showHidden, xNdbClient, xNucliadbUser, xForwardedFor), cancellationToken);
    
    /// <summary>Search on a Knowledge Box and retrieve separate results for documents, paragraphs, and sentences. Usually, it is better to use `find`</summary>
    public static Task<Result<KnowledgeboxSearchResults, HttpError<HTTPValidationError>>> SearchPostKnowledgeboxKbKbidSearchAsync(
        this HttpClient httpClient,
        string kbid, SearchRequest body, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null,
        CancellationToken cancellationToken = default
    ) => _searchPostKnowledgeboxKbKbidSearchAsync(httpClient, (kbid, xNdbClient, xNucliadbUser, xForwardedFor, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> ListSearchConfigurationsKbKbidSearchConfigurationsAsync(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _listSearchConfigurationsKbKbidSearchConfigurationsAsync(httpClient, kbid, cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> CreateSearchConfigurationKbKbidSearchConfigurationsConfigNameAsync(
        this HttpClient httpClient,
        string kbid, string configName, object body,
        CancellationToken cancellationToken = default
    ) => _createSearchConfigurationKbKbidSearchConfigurationsConfigNameAsync(httpClient, ((kbid, configName), body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> UpdateSearchConfigurationKbKbidSearchConfigurationsConfigNameAsync(
        this HttpClient httpClient,
        string kbid, string configName, object body,
        CancellationToken cancellationToken = default
    ) => _updateSearchConfigurationKbKbidSearchConfigurationsConfigNameAsync(httpClient, ((kbid, configName), body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<Unit, HttpError<HTTPValidationError>>> SearchConfigurationKbKbidSearchConfigurationsConfigNameDeleteAsync(
        this HttpClient httpClient,
        string kbid, string configName,
        CancellationToken cancellationToken = default
    ) => _searchConfigurationKbKbidSearchConfigurationsConfigNameDeleteAsync(httpClient, (kbid, configName), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> SearchConfigurationKbKbidSearchConfigurationsConfigNameGetAsync(
        this HttpClient httpClient,
        string kbid, string configName,
        CancellationToken cancellationToken = default
    ) => _searchConfigurationKbKbidSearchConfigurationsConfigNameGetAsync(httpClient, (kbid, configName), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<ResourceUpdated, HttpError<HTTPValidationError>>> ModifyResourceRslugPrefixKbKbidSlugRslugAsync(
        this HttpClient httpClient,
        string kbid, string rslug, UpdateResourcePayload body, bool xSkipStore = false, string? xNucliadbUser = null,
        CancellationToken cancellationToken = default
    ) => _modifyResourceRslugPrefixKbKbidSlugRslugAsync(httpClient, (kbid, rslug, xSkipStore, xNucliadbUser, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<Unit, HttpError<HTTPValidationError>>> ResourceRslugPrefixKbKbidSlugRslugDeleteAsync(
        this HttpClient httpClient,
        string kbid, string rslug,
        CancellationToken cancellationToken = default
    ) => _resourceRslugPrefixKbKbidSlugRslugDeleteAsync(httpClient, (kbid, rslug), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    public static Task<Result<NucliadbModelsResourceResource, HttpError<HTTPValidationError>>> ResourceBySlugKbKbidSlugRslugGetAsync(
        this HttpClient httpClient,
        string kbid, string rslug, List<string>? show = null, List<string>? fieldType = null, List<string>? extracted = null, string? xNucliadbUser = null, string? xForwardedFor = null,
        CancellationToken cancellationToken = default
    ) => _resourceBySlugKbKbidSlugRslugGetAsync(httpClient, (kbid, rslug, show, fieldType, extracted, xNucliadbUser, xForwardedFor), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<HTTPValidationError>>> AddResourceFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdAsync(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldId, InputConversationField body,
        CancellationToken cancellationToken = default
    ) => _addResourceFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdAsync(httpClient, ((kbid, rslug, fieldId), body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> DownloadFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdDownloadFieldMessageIdFileNumAsync(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldId, string messageId, int fileNum,
        CancellationToken cancellationToken = default
    ) => _downloadFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdDownloadFieldMessageIdFileNumAsync(httpClient, (kbid, rslug, fieldId, messageId, fileNum), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<HTTPValidationError>>> AppendMessagesToConversationFieldRslugPrefixKbKbidSlugRslugConversationFieldIdMessagesAsync(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldId, object body,
        CancellationToken cancellationToken = default
    ) => _appendMessagesToConversationFieldRslugPrefixKbKbidSlugRslugConversationFieldIdMessagesAsync(httpClient, ((kbid, rslug, fieldId), body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<HTTPValidationError>>> AddResourceFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdAsync(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldId, FileField body, bool xSkipStore = false,
        CancellationToken cancellationToken = default
    ) => _addResourceFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdAsync(httpClient, (kbid, rslug, fieldId, xSkipStore, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> DownloadFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdDownloadFieldAsync(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldId, bool inline = false,
        CancellationToken cancellationToken = default
    ) => _downloadFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdDownloadFieldAsync(httpClient, (kbid, rslug, fieldId, inline), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> TusPostRslugPrefixKbKbidSlugRslugFileFieldTusuploadAsync(
        this HttpClient httpClient,
        string kbid, string rslug, string field, object body, object? xExtractStrategy = null, object? xSplitStrategy = null,
        CancellationToken cancellationToken = default
    ) => _tusPostRslugPrefixKbKbidSlugRslugFileFieldTusuploadAsync(httpClient, (kbid, rslug, field, xExtractStrategy, xSplitStrategy, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> TusPatchRslugPrefixKbKbidSlugRslugFileFieldTusuploadUploadIdAsync(
        this HttpClient httpClient,
        string kbid, string rslug, string field, string uploadId, object body,
        CancellationToken cancellationToken = default
    ) => _tusPatchRslugPrefixKbKbidSlugRslugFileFieldTusuploadUploadIdAsync(httpClient, ((kbid, rslug, field, uploadId), body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> UploadInformationKbKbidSlugRslugFileFieldTusuploadUploadIdAsync(
        this HttpClient httpClient,
        string kbid, string rslug, string field, string uploadId,
        CancellationToken cancellationToken = default
    ) => _uploadInformationKbKbidSlugRslugFileFieldTusuploadUploadIdAsync(httpClient, (kbid, rslug, field, uploadId), cancellationToken);
    
    /// <summary>Upload a file as a field on an existing resource, if the field exists will return a conflict (419)</summary>
    public static Task<Result<ResourceFileUploaded, HttpError<HTTPValidationError>>> UploadRslugPrefixKbKbidSlugRslugFileFieldUploadAsync(
        this HttpClient httpClient,
        string kbid, string rslug, string field, object body, object? xFilename = null, object? xPassword = null, object? xLanguage = null, object? xMd5 = null, object? xExtractStrategy = null, object? xSplitStrategy = null,
        CancellationToken cancellationToken = default
    ) => _uploadRslugPrefixKbKbidSlugRslugFileFieldUploadAsync(httpClient, (kbid, rslug, field, xFilename, xPassword, xLanguage, xMd5, xExtractStrategy, xSplitStrategy, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<HTTPValidationError>>> AddResourceFieldLinkRslugPrefixKbKbidSlugRslugLinkFieldIdAsync(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldId, LinkField body,
        CancellationToken cancellationToken = default
    ) => _addResourceFieldLinkRslugPrefixKbKbidSlugRslugLinkFieldIdAsync(httpClient, ((kbid, rslug, fieldId), body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> ReindexResourceRslugPrefixKbKbidSlugRslugReindexAsync(
        this HttpClient httpClient,
        string kbid, string rslug, object body, bool reindexVectors = false,
        CancellationToken cancellationToken = default
    ) => _reindexResourceRslugPrefixKbKbidSlugRslugReindexAsync(httpClient, (kbid, rslug, reindexVectors, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<ResourceUpdated, HttpError<HTTPValidationError>>> ReprocessResourceRslugPrefixKbKbidSlugRslugReprocessAsync(
        this HttpClient httpClient,
        string kbid, string rslug, object body, bool resetTitle = false, string? xNucliadbUser = null,
        CancellationToken cancellationToken = default
    ) => _reprocessResourceRslugPrefixKbKbidSlugRslugReprocessAsync(httpClient, (kbid, rslug, resetTitle, xNucliadbUser, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<HTTPValidationError>>> AddResourceFieldTextRslugPrefixKbKbidSlugRslugTextFieldIdAsync(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldId, TextField body,
        CancellationToken cancellationToken = default
    ) => _addResourceFieldTextRslugPrefixKbKbidSlugRslugTextFieldIdAsync(httpClient, ((kbid, rslug, fieldId), body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<Unit, HttpError<HTTPValidationError>>> ResourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDeleteAsync(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldType, string fieldId,
        CancellationToken cancellationToken = default
    ) => _resourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDeleteAsync(httpClient, (kbid, rslug, fieldType, fieldId), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    public static Task<Result<ResourceField, HttpError<HTTPValidationError>>> ResourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdGetAsync(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldType, string fieldId, List<string>? show = null, List<string>? extracted = null, object? page = null,
        CancellationToken cancellationToken = default
    ) => _resourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdGetAsync(httpClient, (kbid, rslug, fieldType, fieldId, show, extracted, page), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> DownloadExtractFileRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDownloadExtractedDownloadFieldAsync(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldType, string fieldId, string downloadField,
        CancellationToken cancellationToken = default
    ) => _downloadExtractFileRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDownloadExtractedDownloadFieldAsync(httpClient, (kbid, rslug, fieldType, fieldId, downloadField), cancellationToken);
    
    /// <summary>Ask questions to a resource</summary>
    public static Task<Result<SyncAskResponse, HttpError<HTTPValidationError>>> ResourceAskEndpointBySlugKbKbidSlugSlugAskAsync(
        this HttpClient httpClient,
        string kbid, string slug, AskRequest body, bool xShowConsumption = false, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null, bool xSynchronous = false,
        CancellationToken cancellationToken = default
    ) => _resourceAskEndpointBySlugKbKbidSlugSlugAskAsync(httpClient, (kbid, slug, xShowConsumption, xNdbClient, xNucliadbUser, xForwardedFor, xSynchronous, body), cancellationToken);
    
    /// <summary>Run Agents on Resource (by slug)</summary>
    public static Task<Result<ResourceAgentsResponse, HttpError<HTTPValidationError>>> RunAgentsBySlugKbKbidSlugSlugRunAgentsAsync(
        this HttpClient httpClient,
        string kbid, string slug, ResourceAgentsRequest body, string? xNucliadbUser = null,
        CancellationToken cancellationToken = default
    ) => _runAgentsBySlugKbKbidSlugSlugRunAgentsAsync(httpClient, (kbid, slug, xNucliadbUser, body), cancellationToken);
    
    /// <summary>Add a split strategy to a KB</summary>
    public static Task<Result<string, HttpError<HTTPValidationError>>> AddSplitStrategyKbKbidSplitStrategiesAsync(
        this HttpClient httpClient,
        string kbid, SplitConfiguration body,
        CancellationToken cancellationToken = default
    ) => _addSplitStrategyKbKbidSplitStrategiesAsync(httpClient, (kbid, body), cancellationToken);
    
    /// <summary>Get available split strategies</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> SplitStrategiesKbKbidSplitStrategiesGetAsync(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _splitStrategiesKbKbidSplitStrategiesGetAsync(httpClient, kbid, cancellationToken);
    
    /// <summary>Removes a split strategy from a KB</summary>
    public static Task<Result<Unit, HttpError<HTTPValidationError>>> SplitStrategyKbKbidSplitStrategiesStrategyStrategyIdDeleteAsync(
        this HttpClient httpClient,
        string kbid, string strategyId,
        CancellationToken cancellationToken = default
    ) => _splitStrategyKbKbidSplitStrategiesStrategyStrategyIdDeleteAsync(httpClient, (kbid, strategyId), cancellationToken);
    
    /// <summary>Get split strategy for a given id</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> SplitStrategyFromIdKbKbidSplitStrategiesStrategyStrategyIdGetAsync(
        this HttpClient httpClient,
        string kbid, string strategyId,
        CancellationToken cancellationToken = default
    ) => _splitStrategyFromIdKbKbidSplitStrategiesStrategyStrategyIdGetAsync(httpClient, (kbid, strategyId), cancellationToken);
    
    /// <summary>Suggestions on a knowledge box</summary>
    public static Task<Result<KnowledgeboxSuggestResults, HttpError<HTTPValidationError>>> SuggestKnowledgeboxKbKbidSuggestAsync(
        this HttpClient httpClient,
        string kbid, string query, List<string>? fields = null, List<string>? filters = null, List<string>? faceted = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, List<string>? features = null, List<string>? show = null, List<string>? fieldType = null, bool debug = false, bool highlight = false, bool showHidden = false, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null,
        CancellationToken cancellationToken = default
    ) => _suggestKnowledgeboxKbKbidSuggestAsync(httpClient, (kbid, query, fields, filters, faceted, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, features, show, fieldType, debug, highlight, showHidden, xNdbClient, xNucliadbUser, xForwardedFor), cancellationToken);
    
    /// <summary>Summarize Your Documents</summary>
    public static Task<Result<SummarizedResponse, HttpError<HTTPValidationError>>> SummarizeEndpointKbKbidSummarizeAsync(
        this HttpClient httpClient,
        string kbid, SummarizeRequest body, bool xShowConsumption = false,
        CancellationToken cancellationToken = default
    ) => _summarizeEndpointKbKbidSummarizeAsync(httpClient, (kbid, xShowConsumption, body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> TusPostKbKbidTusuploadAsync(
        this HttpClient httpClient,
        string kbid, object body, object? xExtractStrategy = null, object? xSplitStrategy = null,
        CancellationToken cancellationToken = default
    ) => _tusPostKbKbidTusuploadAsync(httpClient, (kbid, xExtractStrategy, xSplitStrategy, body), cancellationToken);
    
    /// <summary>TUS Server information</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> TusOptionsKbKbidTusuploadAsync(
        this HttpClient httpClient,
        string kbid, object? rid = null, object? rslug = null, object? uploadId = null, object? field = null,
        CancellationToken cancellationToken = default
    ) => _tusOptionsKbKbidTusuploadAsync(httpClient, (kbid, rid, rslug, uploadId, field), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> KbKbidTusuploadUploadIdPatchAsync(
        this HttpClient httpClient,
        string kbid, string uploadId, object body,
        CancellationToken cancellationToken = default
    ) => _kbKbidTusuploadUploadIdPatchAsync(httpClient, ((kbid, uploadId), body), cancellationToken);
    
    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> UploadInformationKbKbidTusuploadUploadIdAsync(
        this HttpClient httpClient,
        string kbid, string uploadId,
        CancellationToken cancellationToken = default
    ) => _uploadInformationKbKbidTusuploadUploadIdAsync(httpClient, (kbid, uploadId), cancellationToken);
    
    /// <summary>Upload a file onto a Knowledge Box, field id will be file and rid will be autogenerated.</summary>
    public static Task<Result<ResourceFileUploaded, HttpError<HTTPValidationError>>> UploadKbKbidUploadAsync(
        this HttpClient httpClient,
        string kbid, object body, object? xFilename = null, object? xPassword = null, object? xLanguage = null, object? xMd5 = null, object? xExtractStrategy = null, object? xSplitStrategy = null,
        CancellationToken cancellationToken = default
    ) => _uploadKbKbidUploadAsync(httpClient, (kbid, xFilename, xPassword, xLanguage, xMd5, xExtractStrategy, xSplitStrategy, body), cancellationToken);

    #endregion

    #region Kbs Operations

    /// <summary>Create a new knowledge box</summary>
    public static Task<Result<KnowledgeBoxObj, HttpError<string>>> CreateKnowledgeBoxKbsAsync(
        this HttpClient httpClient,
        object body, string xNUCLIADBROLES = "MANAGER",
        CancellationToken cancellationToken = default
    ) => _createKnowledgeBoxKbsAsync(httpClient, (xNUCLIADBROLES, body), cancellationToken);

    #endregion

    #region Learning Operations

    /// <summary>Get jsonschema definition for `learning_configuration` field of knowledgebox creation payload</summary>
    public static Task<Result<object, HttpError<HTTPValidationError>>> LearningConfigurationSchemaLearningConfigurationSchemaAsync(
        this HttpClient httpClient,
        
        CancellationToken cancellationToken = default
    ) => _learningConfigurationSchemaLearningConfigurationSchemaAsync(httpClient, Unit.Value, cancellationToken);

    #endregion

    private static GetAsync<KnowledgeBoxObj, HTTPValidationError, string> _kbBySlugKbSSlugGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeBoxObj, HTTPValidationError, string>(
            url: BaseUrl,
            buildRequest: static slug => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/s/{slug}"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeBoxObj>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<KnowledgeBoxObj, HTTPValidationError, (string kbid, string xNUCLIADBROLES)> _kbKbKbidGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeBoxObj, HTTPValidationError, (string kbid, string xNUCLIADBROLES)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}"), null, new Dictionary<string, string> { ["X-NUCLIADB-ROLES"] = param.xNUCLIADBROLES.ToString() }),
            deserializeSuccess: DeserializeJson<KnowledgeBoxObj>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<SyncAskResponse, HTTPValidationError, (string kbid, string xNdbClient, bool xShowConsumption, string? xNucliadbUser, string? xForwardedFor, bool xSynchronous, AskRequest Body)> _askKnowledgeboxEndpointKbKbidAskAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<SyncAskResponse, HTTPValidationError, (string kbid, string xNdbClient, bool xShowConsumption, string? xNucliadbUser, string? xForwardedFor, bool xSynchronous, AskRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/ask"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString(), ["x-show-consumption"] = param.xShowConsumption.ToString(), ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor?.ToString() ?? string.Empty, ["x-synchronous"] = param.xSynchronous.ToString() }),
            deserializeSuccess: DeserializeJson<SyncAskResponse>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<KnowledgeboxSearchResults, HTTPValidationError, (string kbid, string query, object? filterExpression, List<string>? filters, List<string>? faceted, string sortField, object? sortLimit, string sortOrder, int pageNumber, int pageSize, object? withStatus, object? rangeCreationStart, object? rangeCreationEnd, object? rangeModificationStart, object? rangeModificationEnd, object? hidden, List<string>? show)> _catalogGetKbKbidCatalogAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeboxSearchResults, HTTPValidationError, (string kbid, string query, object? filterExpression, List<string>? filters, List<string>? faceted, string sortField, object? sortLimit, string sortOrder, int pageNumber, int pageSize, object? withStatus, object? rangeCreationStart, object? rangeCreationEnd, object? rangeModificationStart, object? rangeModificationEnd, object? hidden, List<string>? show)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/catalog{BuildQueryString(("query", param.query), ("filter_expression", param.filterExpression), ("filters", param.filters), ("faceted", param.faceted), ("sort_field", param.sortField), ("sort_limit", param.sortLimit), ("sort_order", param.sortOrder), ("page_number", param.pageNumber), ("page_size", param.pageSize), ("with_status", param.withStatus), ("range_creation_start", param.rangeCreationStart), ("range_creation_end", param.rangeCreationEnd), ("range_modification_start", param.rangeModificationStart), ("range_modification_end", param.rangeModificationEnd), ("hidden", param.hidden), ("show", param.show))}"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeboxSearchResults>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<KnowledgeboxSearchResults, HTTPValidationError, (string Params, CatalogRequest Body)> _catalogPostKbKbidCatalogAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<KnowledgeboxSearchResults, HTTPValidationError, (string Params, CatalogRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/catalog"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<KnowledgeboxSearchResults>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, string> _configurationKbKbidConfigurationGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/configuration"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PatchAsync<object, HTTPValidationError, (string Params, object Body)> _configurationKbKbidConfigurationPatchAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, HTTPValidationError, (string Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/configuration"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<object, HTTPValidationError, (string Params, object Body)> _setConfigurationKbKbidConfigurationAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, HTTPValidationError, (string Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/configuration"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<KnowledgeboxCounters, HTTPValidationError, (string kbid, bool debug, string xNUCLIADBROLES)> _knowledgeboxCountersKbKbidCountersAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeboxCounters, HTTPValidationError, (string kbid, bool debug, string xNUCLIADBROLES)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/counters?debug={param.debug}"), null, new Dictionary<string, string> { ["X-NUCLIADB-ROLES"] = param.xNUCLIADBROLES.ToString() }),
            deserializeSuccess: DeserializeJson<KnowledgeboxCounters>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PutAsync<object, HTTPValidationError, (string Params, KnowledgeBoxSynonyms Body)> _setCustomSynonymsKbKbidCustomSynonymsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<object, HTTPValidationError, (string Params, KnowledgeBoxSynonyms Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/custom-synonyms"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static DeleteAsync<Unit, HTTPValidationError, string> _customSynonymsKbKbidCustomSynonymsDeleteAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, HTTPValidationError, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/custom-synonyms"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<KnowledgeBoxSynonyms, HTTPValidationError, string> _customSynonymsKbKbidCustomSynonymsGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeBoxSynonyms, HTTPValidationError, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/custom-synonyms"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeBoxSynonyms>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PatchAsync<object, HTTPValidationError, ((string kbid, string group) Params, UpdateEntitiesGroupPayload Body)> _updateEntitiesGroupKbKbidEntitiesgroupGroupAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, HTTPValidationError, ((string kbid, string group) Params, UpdateEntitiesGroupPayload Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/entitiesgroup/{param.Params.group}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static DeleteAsync<Unit, HTTPValidationError, (string kbid, string group)> _entitiesKbKbidEntitiesgroupGroupDeleteAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, HTTPValidationError, (string kbid, string group)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/entitiesgroup/{param.group}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<EntitiesGroup, HTTPValidationError, (string kbid, string group)> _entityKbKbidEntitiesgroupGroupGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<EntitiesGroup, HTTPValidationError, (string kbid, string group)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/entitiesgroup/{param.group}"), null, null),
            deserializeSuccess: DeserializeJson<EntitiesGroup>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<object, HTTPValidationError, (string Params, CreateEntitiesGroupPayload Body)> _createEntitiesGroupKbKbidEntitiesgroupsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, HTTPValidationError, (string Params, CreateEntitiesGroupPayload Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/entitiesgroups"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<KnowledgeBoxEntities, HTTPValidationError, (string kbid, bool showEntities)> _entitiesKbKbidEntitiesgroupsGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeBoxEntities, HTTPValidationError, (string kbid, bool showEntities)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/entitiesgroups?show_entities={param.showEntities}"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeBoxEntities>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<CreateExportResponse, HTTPValidationError, (string Params, object Body)> _startKbExportEndpointKbKbidExportAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<CreateExportResponse, HTTPValidationError, (string Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/export"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<CreateExportResponse>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, (string kbid, string exportId)> _downloadExportKbEndpointKbKbidExportExportIdAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, (string kbid, string exportId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/export/{param.exportId}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<StatusResponse, HTTPValidationError, (string kbid, string exportId)> _exportStatusEndpointKbKbidExportExportIdStatusGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<StatusResponse, HTTPValidationError, (string kbid, string exportId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/export/{param.exportId}/status"), null, null),
            deserializeSuccess: DeserializeJson<StatusResponse>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<string, HTTPValidationError, (string Params, ExtractConfig Body)> _addStrategyKbKbidExtractStrategiesAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<string, HTTPValidationError, (string Params, ExtractConfig Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/extract_strategies"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<string>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, string> _extractStrategiesKbKbidExtractStrategiesGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/extract_strategies"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static DeleteAsync<Unit, HTTPValidationError, (string kbid, string strategyId)> _strategyKbKbidExtractStrategiesStrategyStrategyIdDeleteAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, HTTPValidationError, (string kbid, string strategyId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/extract_strategies/strategy/{param.strategyId}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, (string kbid, string strategyId)> _extractStrategyFromIdKbKbidExtractStrategiesStrategyStrategyIdGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, (string kbid, string strategyId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/extract_strategies/strategy/{param.strategyId}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<object, HTTPValidationError, (string kbid, string xNdbClient, string? xNucliadbUser, string? xForwardedFor, FeedbackRequest Body)> _sendFeedbackEndpointKbKbidFeedbackAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, HTTPValidationError, (string kbid, string xNdbClient, string? xNucliadbUser, string? xForwardedFor, FeedbackRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/feedback"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString(), ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<KnowledgeboxFindResults, HTTPValidationError, (string kbid, string query, object? filterExpression, List<string>? fields, List<string>? filters, object? topK, object? minScore, object? minScoreSemantic, float minScoreBm25, object? vectorset, object? rangeCreationStart, object? rangeCreationEnd, object? rangeModificationStart, object? rangeModificationEnd, List<string>? features, bool debug, bool highlight, List<string>? show, List<string>? fieldType, List<string>? extracted, bool withDuplicates, bool withSynonyms, bool autofilter, List<string>? securityGroups, bool showHidden, string rankFusion, object? reranker, object? searchConfiguration, string xNdbClient, string? xNucliadbUser, string? xForwardedFor)> _findKnowledgeboxKbKbidFindAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeboxFindResults, HTTPValidationError, (string kbid, string query, object? filterExpression, List<string>? fields, List<string>? filters, object? topK, object? minScore, object? minScoreSemantic, float minScoreBm25, object? vectorset, object? rangeCreationStart, object? rangeCreationEnd, object? rangeModificationStart, object? rangeModificationEnd, List<string>? features, bool debug, bool highlight, List<string>? show, List<string>? fieldType, List<string>? extracted, bool withDuplicates, bool withSynonyms, bool autofilter, List<string>? securityGroups, bool showHidden, string rankFusion, object? reranker, object? searchConfiguration, string xNdbClient, string? xNucliadbUser, string? xForwardedFor)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/find{BuildQueryString(("query", param.query), ("filter_expression", param.filterExpression), ("fields", param.fields), ("filters", param.filters), ("top_k", param.topK), ("min_score", param.minScore), ("min_score_semantic", param.minScoreSemantic), ("min_score_bm25", param.minScoreBm25), ("vectorset", param.vectorset), ("range_creation_start", param.rangeCreationStart), ("range_creation_end", param.rangeCreationEnd), ("range_modification_start", param.rangeModificationStart), ("range_modification_end", param.rangeModificationEnd), ("features", param.features), ("debug", param.debug), ("highlight", param.highlight), ("show", param.show), ("field_type", param.fieldType), ("extracted", param.extracted), ("with_duplicates", param.withDuplicates), ("with_synonyms", param.withSynonyms), ("autofilter", param.autofilter), ("security_groups", param.securityGroups), ("show_hidden", param.showHidden), ("rank_fusion", param.rankFusion), ("reranker", param.reranker), ("search_configuration", param.searchConfiguration))}"), null, new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString(), ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<KnowledgeboxFindResults>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<KnowledgeboxFindResults, HTTPValidationError, (string kbid, string xNdbClient, string? xNucliadbUser, string? xForwardedFor, FindRequest Body)> _findPostKnowledgeboxKbKbidFindAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<KnowledgeboxFindResults, HTTPValidationError, (string kbid, string xNdbClient, string? xNucliadbUser, string? xForwardedFor, FindRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/find"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString(), ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<KnowledgeboxFindResults>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<GraphSearchResponse, HTTPValidationError, (string kbid, string xNdbClient, string? xNucliadbUser, string? xForwardedFor, GraphSearchRequest Body)> _graphSearchKnowledgeboxKbKbidGraphAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<GraphSearchResponse, HTTPValidationError, (string kbid, string xNdbClient, string? xNucliadbUser, string? xForwardedFor, GraphSearchRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/graph"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString(), ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<GraphSearchResponse>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<GraphNodesSearchResponse, HTTPValidationError, (string kbid, string xNdbClient, string? xNucliadbUser, string? xForwardedFor, GraphNodesSearchRequest Body)> _graphNodesSearchKnowledgeboxKbKbidGraphNodesAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<GraphNodesSearchResponse, HTTPValidationError, (string kbid, string xNdbClient, string? xNucliadbUser, string? xForwardedFor, GraphNodesSearchRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/graph/nodes"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString(), ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<GraphNodesSearchResponse>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<GraphRelationsSearchResponse, HTTPValidationError, (string kbid, string xNdbClient, string? xNucliadbUser, string? xForwardedFor, GraphRelationsSearchRequest Body)> _graphRelationsSearchKnowledgeboxKbKbidGraphRelationsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<GraphRelationsSearchResponse, HTTPValidationError, (string kbid, string xNdbClient, string? xNucliadbUser, string? xForwardedFor, GraphRelationsSearchRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/graph/relations"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString(), ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<GraphRelationsSearchResponse>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<CreateImportResponse, HTTPValidationError, (string Params, object Body)> _startKbImportEndpointKbKbidImportAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<CreateImportResponse, HTTPValidationError, (string Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/import"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<CreateImportResponse>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<StatusResponse, HTTPValidationError, (string kbid, string importId)> _importStatusEndpointKbKbidImportImportIdStatusGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<StatusResponse, HTTPValidationError, (string kbid, string importId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/import/{param.importId}/status"), null, null),
            deserializeSuccess: DeserializeJson<StatusResponse>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<object, HTTPValidationError, ((string kbid, string labelset) Params, LabelSet Body)> _setLabelsetEndpointKbKbidLabelsetLabelsetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, HTTPValidationError, ((string kbid, string labelset) Params, LabelSet Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/labelset/{param.Params.labelset}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static DeleteAsync<Unit, HTTPValidationError, (string kbid, string labelset)> _labelsetEndpointKbKbidLabelsetLabelsetDeleteAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, HTTPValidationError, (string kbid, string labelset)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/labelset/{param.labelset}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<LabelSet, string, (string kbid, string labelset)> _labelsetEndpointKbKbidLabelsetLabelsetGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<LabelSet, string, (string kbid, string labelset)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/labelset/{param.labelset}"), null, null),
            deserializeSuccess: DeserializeJson<LabelSet>,
            deserializeError: DeserializeError
        );

    private static GetAsync<KnowledgeBoxLabels, HTTPValidationError, string> _labelsetsEndointKbKbidLabelsetsGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeBoxLabels, HTTPValidationError, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/labelsets"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeBoxLabels>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, (string kbid, string modelId)> _modelKbKbidModelModelIdGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, (string kbid, string modelId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/model/{param.modelId}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, string> _modelsKbKbidModelsGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/models"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, (string kbid, string modelId, string filename)> _downloadModelKbKbidModelsModelIdFilenameAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, (string kbid, string modelId, string filename)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/models/{param.modelId}/{param.filename}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, string, string> _notificationsEndpointKbKbidNotificationsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/notifications"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PostAsync<object, HTTPValidationError, (string kbid, string endpoint, string? xNucliadbUser, string xNdbClient, string? xForwardedFor, object Body)> _predictProxyEndpointKbKbidPredictEndpointAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, HTTPValidationError, (string kbid, string endpoint, string? xNucliadbUser, string xNdbClient, string? xForwardedFor, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/predict/{param.endpoint}"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["x-ndb-client"] = param.xNdbClient.ToString(), ["x-forwarded-for"] = param.xForwardedFor?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, (string kbid, string endpoint, string? xNucliadbUser, string xNdbClient, string? xForwardedFor)> _predictProxyEndpointKbKbidPredictEndpointAsync2 { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, (string kbid, string endpoint, string? xNucliadbUser, string xNdbClient, string? xForwardedFor)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/predict/{param.endpoint}"), null, new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["x-ndb-client"] = param.xNdbClient.ToString(), ["x-forwarded-for"] = param.xForwardedFor?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<RequestsResults, string, (string kbid, object? cursor, object? scheduled, int limit)> _processingStatusKbKbidProcessingStatusAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<RequestsResults, string, (string kbid, object? cursor, object? scheduled, int limit)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/processing-status{BuildQueryString(("cursor", param.cursor), ("scheduled", param.scheduled), ("limit", param.limit))}"), null, null),
            deserializeSuccess: DeserializeJson<RequestsResults>,
            deserializeError: DeserializeError
        );

    private static PostAsync<object, HTTPValidationError, (string kbid, string pathRid, string field, object? xExtractStrategy, object? xSplitStrategy, object Body)> _tusPostRidPrefixKbKbidResourcePathRidFileFieldTusuploadAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, HTTPValidationError, (string kbid, string pathRid, string field, object? xExtractStrategy, object? xSplitStrategy, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.pathRid}/file/{param.field}/tusupload"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-extract-strategy"] = param.xExtractStrategy?.ToString() ?? string.Empty, ["x-split-strategy"] = param.xSplitStrategy?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static HeadAsync<object, HTTPValidationError, (string kbid, string pathRid, string field, string uploadId)> _uploadInformationKbKbidResourcePathRidFileFieldTusuploadUploadIdAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateHead<object, HTTPValidationError, (string kbid, string pathRid, string field, string uploadId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.pathRid}/file/{param.field}/tusupload/{param.uploadId}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<ResourceFileUploaded, HTTPValidationError, (string kbid, string pathRid, string field, object? xFilename, object? xPassword, object? xLanguage, object? xMd5, object? xExtractStrategy, object? xSplitStrategy, object Body)> _uploadRidPrefixKbKbidResourcePathRidFileFieldUploadAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceFileUploaded, HTTPValidationError, (string kbid, string pathRid, string field, object? xFilename, object? xPassword, object? xLanguage, object? xMd5, object? xExtractStrategy, object? xSplitStrategy, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.pathRid}/file/{param.field}/upload"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-filename"] = param.xFilename?.ToString() ?? string.Empty, ["x-password"] = param.xPassword?.ToString() ?? string.Empty, ["x-language"] = param.xLanguage?.ToString() ?? string.Empty, ["x-md5"] = param.xMd5?.ToString() ?? string.Empty, ["x-extract-strategy"] = param.xExtractStrategy?.ToString() ?? string.Empty, ["x-split-strategy"] = param.xSplitStrategy?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceFileUploaded>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PatchAsync<ResourceUpdated, HTTPValidationError, (string kbid, string rid, string? xNucliadbUser, bool xSkipStore, string xNUCLIADBROLES, UpdateResourcePayload Body)> _modifyResourceRidPrefixKbKbidResourceRidAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<ResourceUpdated, HTTPValidationError, (string kbid, string rid, string? xNucliadbUser, bool xSkipStore, string xNUCLIADBROLES, UpdateResourcePayload Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["x-skip-store"] = param.xSkipStore.ToString(), ["X-NUCLIADB-ROLES"] = param.xNUCLIADBROLES.ToString() }),
            deserializeSuccess: DeserializeJson<ResourceUpdated>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static DeleteAsync<Unit, HTTPValidationError, (string kbid, string rid, string xNUCLIADBROLES)> _resourceRidPrefixKbKbidResourceRidDeleteAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, HTTPValidationError, (string kbid, string rid, string xNUCLIADBROLES)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}"), null, new Dictionary<string, string> { ["X-NUCLIADB-ROLES"] = param.xNUCLIADBROLES.ToString() }),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<NucliadbModelsResourceResource, HTTPValidationError, (string kbid, string rid, List<string>? show, List<string>? fieldType, List<string>? extracted, string? xNucliadbUser, string? xForwardedFor, string xNUCLIADBROLES)> _resourceByUuidKbKbidResourceRidGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<NucliadbModelsResourceResource, HTTPValidationError, (string kbid, string rid, List<string>? show, List<string>? fieldType, List<string>? extracted, string? xNucliadbUser, string? xForwardedFor, string xNUCLIADBROLES)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}{BuildQueryString(("show", param.show), ("field_type", param.fieldType), ("extracted", param.extracted))}"), null, new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor?.ToString() ?? string.Empty, ["X-NUCLIADB-ROLES"] = param.xNUCLIADBROLES.ToString() }),
            deserializeSuccess: DeserializeJson<NucliadbModelsResourceResource>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<SyncAskResponse, HTTPValidationError, (string kbid, string rid, bool xShowConsumption, string xNdbClient, string? xNucliadbUser, string? xForwardedFor, bool xSynchronous, AskRequest Body)> _resourceAskEndpointByUuidKbKbidResourceRidAskAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<SyncAskResponse, HTTPValidationError, (string kbid, string rid, bool xShowConsumption, string xNdbClient, string? xNucliadbUser, string? xForwardedFor, bool xSynchronous, AskRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/ask"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-show-consumption"] = param.xShowConsumption.ToString(), ["x-ndb-client"] = param.xNdbClient.ToString(), ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor?.ToString() ?? string.Empty, ["x-synchronous"] = param.xSynchronous.ToString() }),
            deserializeSuccess: DeserializeJson<SyncAskResponse>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PutAsync<ResourceFieldAdded, HTTPValidationError, ((string kbid, string rid, string fieldId) Params, InputConversationField Body)> _addResourceFieldConversationRidPrefixKbKbidResourceRidConversationFieldIdAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, HTTPValidationError, ((string kbid, string rid, string fieldId) Params, InputConversationField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/resource/{param.Params.rid}/conversation/{param.Params.fieldId}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, (string kbid, string rid, string fieldId, string messageId, int fileNum)> _downloadFieldConversationAttachmentRidPrefixKbKbidResourceRidConversationFieldIdDownloadFieldMessageIdFileNumAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, (string kbid, string rid, string fieldId, string messageId, int fileNum)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/conversation/{param.fieldId}/download/field/{param.messageId}/{param.fileNum}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PutAsync<ResourceFieldAdded, HTTPValidationError, ((string kbid, string rid, string fieldId) Params, object Body)> _appendMessagesToConversationFieldRidPrefixKbKbidResourceRidConversationFieldIdMessagesAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, HTTPValidationError, ((string kbid, string rid, string fieldId) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/resource/{param.Params.rid}/conversation/{param.Params.fieldId}/messages"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PutAsync<ResourceFieldAdded, HTTPValidationError, (string kbid, string rid, string fieldId, bool xSkipStore, FileField Body)> _addResourceFieldFileRidPrefixKbKbidResourceRidFileFieldIdAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, HTTPValidationError, (string kbid, string rid, string fieldId, bool xSkipStore, FileField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/file/{param.fieldId}"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-skip-store"] = param.xSkipStore.ToString() }),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, (string kbid, string rid, string fieldId, bool inline)> _downloadFieldFileRidPrefixKbKbidResourceRidFileFieldIdDownloadFieldAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, (string kbid, string rid, string fieldId, bool inline)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/file/{param.fieldId}/download/field?inline={param.inline}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<ResourceUpdated, HTTPValidationError, (string kbid, string rid, string fieldId, bool resetTitle, string? xNucliadbUser, object? xFilePassword, object Body)> _reprocessFileFieldKbKbidResourceRidFileFieldIdReprocessAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceUpdated, HTTPValidationError, (string kbid, string rid, string fieldId, bool resetTitle, string? xNucliadbUser, object? xFilePassword, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/file/{param.fieldId}/reprocess?reset_title={param.resetTitle}"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["x-file-password"] = param.xFilePassword?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceUpdated>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PatchAsync<object, HTTPValidationError, ((string kbid, string rid, string field, string uploadId) Params, object Body)> _tusPatchRidPrefixKbKbidResourceRidFileFieldTusuploadUploadIdAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, HTTPValidationError, ((string kbid, string rid, string field, string uploadId) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/resource/{param.Params.rid}/file/{param.Params.field}/tusupload/{param.Params.uploadId}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PutAsync<ResourceFieldAdded, HTTPValidationError, ((string kbid, string rid, string fieldId) Params, LinkField Body)> _addResourceFieldLinkRidPrefixKbKbidResourceRidLinkFieldIdAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, HTTPValidationError, ((string kbid, string rid, string fieldId) Params, LinkField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/resource/{param.Params.rid}/link/{param.Params.fieldId}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<object, HTTPValidationError, (string kbid, string rid, bool reindexVectors, object Body)> _reindexResourceRidPrefixKbKbidResourceRidReindexAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, HTTPValidationError, (string kbid, string rid, bool reindexVectors, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/reindex?reindex_vectors={param.reindexVectors}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<ResourceUpdated, HTTPValidationError, (string kbid, string rid, bool resetTitle, string? xNucliadbUser, object Body)> _reprocessResourceRidPrefixKbKbidResourceRidReprocessAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceUpdated, HTTPValidationError, (string kbid, string rid, bool resetTitle, string? xNucliadbUser, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/reprocess?reset_title={param.resetTitle}"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceUpdated>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<ResourceAgentsResponse, HTTPValidationError, (string kbid, string rid, string? xNucliadbUser, ResourceAgentsRequest Body)> _runAgentsByUuidKbKbidResourceRidRunAgentsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceAgentsResponse, HTTPValidationError, (string kbid, string rid, string? xNucliadbUser, ResourceAgentsRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/run-agents"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceAgentsResponse>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<ResourceSearchResults, HTTPValidationError, (string kbid, string rid, string query, object? filterExpression, List<string>? fields, List<string>? filters, List<string>? faceted, object? sortField, string sortOrder, object? topK, object? rangeCreationStart, object? rangeCreationEnd, object? rangeModificationStart, object? rangeModificationEnd, bool highlight, bool debug, string xNdbClient)> _resourceSearchKbKbidResourceRidSearchAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<ResourceSearchResults, HTTPValidationError, (string kbid, string rid, string query, object? filterExpression, List<string>? fields, List<string>? filters, List<string>? faceted, object? sortField, string sortOrder, object? topK, object? rangeCreationStart, object? rangeCreationEnd, object? rangeModificationStart, object? rangeModificationEnd, bool highlight, bool debug, string xNdbClient)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/search{BuildQueryString(("query", param.query), ("filter_expression", param.filterExpression), ("fields", param.fields), ("filters", param.filters), ("faceted", param.faceted), ("sort_field", param.sortField), ("sort_order", param.sortOrder), ("top_k", param.topK), ("range_creation_start", param.rangeCreationStart), ("range_creation_end", param.rangeCreationEnd), ("range_modification_start", param.rangeModificationStart), ("range_modification_end", param.rangeModificationEnd), ("highlight", param.highlight), ("debug", param.debug))}"), null, new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString() }),
            deserializeSuccess: DeserializeJson<ResourceSearchResults>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PutAsync<ResourceFieldAdded, HTTPValidationError, (string kbid, string rid, string fieldId, string xNUCLIADBROLES, TextField Body)> _addResourceFieldTextRidPrefixKbKbidResourceRidTextFieldIdAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, HTTPValidationError, (string kbid, string rid, string fieldId, string xNUCLIADBROLES, TextField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/text/{param.fieldId}"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["X-NUCLIADB-ROLES"] = param.xNUCLIADBROLES.ToString() }),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static DeleteAsync<Unit, HTTPValidationError, (string kbid, string rid, string fieldType, string fieldId)> _resourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdDeleteAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, HTTPValidationError, (string kbid, string rid, string fieldType, string fieldId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/{param.fieldType}/{param.fieldId}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<ResourceField, HTTPValidationError, (string kbid, string rid, string fieldType, string fieldId, List<string>? show, List<string>? extracted, object? page)> _resourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<ResourceField, HTTPValidationError, (string kbid, string rid, string fieldType, string fieldId, List<string>? show, List<string>? extracted, object? page)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/{param.fieldType}/{param.fieldId}{BuildQueryString(("show", param.show), ("extracted", param.extracted), ("page", param.page))}"), null, null),
            deserializeSuccess: DeserializeJson<ResourceField>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, (string kbid, string rid, string fieldType, string fieldId, string downloadField)> _downloadExtractFileRidPrefixKbKbidResourceRidFieldTypeFieldIdDownloadExtractedDownloadFieldAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, (string kbid, string rid, string fieldType, string fieldId, string downloadField)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/{param.fieldType}/{param.fieldId}/download/extracted/{param.downloadField}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<ResourceCreated, HTTPValidationError, (string kbid, bool xSkipStore, string? xNucliadbUser, string xNUCLIADBROLES, CreateResourcePayload Body)> _createResourceKbKbidResourcesAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceCreated, HTTPValidationError, (string kbid, bool xSkipStore, string? xNucliadbUser, string xNUCLIADBROLES, CreateResourcePayload Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resources"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-skip-store"] = param.xSkipStore.ToString(), ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["X-NUCLIADB-ROLES"] = param.xNUCLIADBROLES.ToString() }),
            deserializeSuccess: DeserializeJson<ResourceCreated>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<ResourceList, HTTPValidationError, (string kbid, int page, int size, string xNUCLIADBROLES)> _listResourcesKbKbidResourcesAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<ResourceList, HTTPValidationError, (string kbid, int page, int size, string xNUCLIADBROLES)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resources?page={param.page}&size={param.size}"), null, new Dictionary<string, string> { ["X-NUCLIADB-ROLES"] = param.xNUCLIADBROLES.ToString() }),
            deserializeSuccess: DeserializeJson<ResourceList>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, string> _schemaForConfigurationUpdatesKbKbidSchemaGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/schema"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<KnowledgeboxSearchResults, HTTPValidationError, (string kbid, string query, object? filterExpression, List<string>? fields, List<string>? filters, List<string>? faceted, string sortField, object? sortLimit, string sortOrder, int topK, object? minScore, object? minScoreSemantic, float minScoreBm25, object? vectorset, object? rangeCreationStart, object? rangeCreationEnd, object? rangeModificationStart, object? rangeModificationEnd, List<string>? features, bool debug, bool highlight, List<string>? show, List<string>? fieldType, List<string>? extracted, bool withDuplicates, bool withSynonyms, bool autofilter, List<string>? securityGroups, bool showHidden, string xNdbClient, string? xNucliadbUser, string? xForwardedFor)> _searchKnowledgeboxKbKbidSearchAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeboxSearchResults, HTTPValidationError, (string kbid, string query, object? filterExpression, List<string>? fields, List<string>? filters, List<string>? faceted, string sortField, object? sortLimit, string sortOrder, int topK, object? minScore, object? minScoreSemantic, float minScoreBm25, object? vectorset, object? rangeCreationStart, object? rangeCreationEnd, object? rangeModificationStart, object? rangeModificationEnd, List<string>? features, bool debug, bool highlight, List<string>? show, List<string>? fieldType, List<string>? extracted, bool withDuplicates, bool withSynonyms, bool autofilter, List<string>? securityGroups, bool showHidden, string xNdbClient, string? xNucliadbUser, string? xForwardedFor)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/search{BuildQueryString(("query", param.query), ("filter_expression", param.filterExpression), ("fields", param.fields), ("filters", param.filters), ("faceted", param.faceted), ("sort_field", param.sortField), ("sort_limit", param.sortLimit), ("sort_order", param.sortOrder), ("top_k", param.topK), ("min_score", param.minScore), ("min_score_semantic", param.minScoreSemantic), ("min_score_bm25", param.minScoreBm25), ("vectorset", param.vectorset), ("range_creation_start", param.rangeCreationStart), ("range_creation_end", param.rangeCreationEnd), ("range_modification_start", param.rangeModificationStart), ("range_modification_end", param.rangeModificationEnd), ("features", param.features), ("debug", param.debug), ("highlight", param.highlight), ("show", param.show), ("field_type", param.fieldType), ("extracted", param.extracted), ("with_duplicates", param.withDuplicates), ("with_synonyms", param.withSynonyms), ("autofilter", param.autofilter), ("security_groups", param.securityGroups), ("show_hidden", param.showHidden))}"), null, new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString(), ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<KnowledgeboxSearchResults>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<KnowledgeboxSearchResults, HTTPValidationError, (string kbid, string xNdbClient, string? xNucliadbUser, string? xForwardedFor, SearchRequest Body)> _searchPostKnowledgeboxKbKbidSearchAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<KnowledgeboxSearchResults, HTTPValidationError, (string kbid, string xNdbClient, string? xNucliadbUser, string? xForwardedFor, SearchRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/search"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString(), ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<KnowledgeboxSearchResults>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, string> _listSearchConfigurationsKbKbidSearchConfigurationsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/search_configurations"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<object, HTTPValidationError, ((string kbid, string configName) Params, object Body)> _createSearchConfigurationKbKbidSearchConfigurationsConfigNameAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, HTTPValidationError, ((string kbid, string configName) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/search_configurations/{param.Params.configName}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PatchAsync<object, HTTPValidationError, ((string kbid, string configName) Params, object Body)> _updateSearchConfigurationKbKbidSearchConfigurationsConfigNameAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, HTTPValidationError, ((string kbid, string configName) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/search_configurations/{param.Params.configName}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static DeleteAsync<Unit, HTTPValidationError, (string kbid, string configName)> _searchConfigurationKbKbidSearchConfigurationsConfigNameDeleteAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, HTTPValidationError, (string kbid, string configName)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/search_configurations/{param.configName}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, (string kbid, string configName)> _searchConfigurationKbKbidSearchConfigurationsConfigNameGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, (string kbid, string configName)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/search_configurations/{param.configName}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PatchAsync<ResourceUpdated, HTTPValidationError, (string kbid, string rslug, bool xSkipStore, string? xNucliadbUser, UpdateResourcePayload Body)> _modifyResourceRslugPrefixKbKbidSlugRslugAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<ResourceUpdated, HTTPValidationError, (string kbid, string rslug, bool xSkipStore, string? xNucliadbUser, UpdateResourcePayload Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-skip-store"] = param.xSkipStore.ToString(), ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceUpdated>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static DeleteAsync<Unit, HTTPValidationError, (string kbid, string rslug)> _resourceRslugPrefixKbKbidSlugRslugDeleteAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, HTTPValidationError, (string kbid, string rslug)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<NucliadbModelsResourceResource, HTTPValidationError, (string kbid, string rslug, List<string>? show, List<string>? fieldType, List<string>? extracted, string? xNucliadbUser, string? xForwardedFor)> _resourceBySlugKbKbidSlugRslugGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<NucliadbModelsResourceResource, HTTPValidationError, (string kbid, string rslug, List<string>? show, List<string>? fieldType, List<string>? extracted, string? xNucliadbUser, string? xForwardedFor)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}{BuildQueryString(("show", param.show), ("field_type", param.fieldType), ("extracted", param.extracted))}"), null, new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<NucliadbModelsResourceResource>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PutAsync<ResourceFieldAdded, HTTPValidationError, ((string kbid, string rslug, string fieldId) Params, InputConversationField Body)> _addResourceFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, HTTPValidationError, ((string kbid, string rslug, string fieldId) Params, InputConversationField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/slug/{param.Params.rslug}/conversation/{param.Params.fieldId}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, (string kbid, string rslug, string fieldId, string messageId, int fileNum)> _downloadFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdDownloadFieldMessageIdFileNumAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, (string kbid, string rslug, string fieldId, string messageId, int fileNum)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/conversation/{param.fieldId}/download/field/{param.messageId}/{param.fileNum}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PutAsync<ResourceFieldAdded, HTTPValidationError, ((string kbid, string rslug, string fieldId) Params, object Body)> _appendMessagesToConversationFieldRslugPrefixKbKbidSlugRslugConversationFieldIdMessagesAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, HTTPValidationError, ((string kbid, string rslug, string fieldId) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/slug/{param.Params.rslug}/conversation/{param.Params.fieldId}/messages"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PutAsync<ResourceFieldAdded, HTTPValidationError, (string kbid, string rslug, string fieldId, bool xSkipStore, FileField Body)> _addResourceFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, HTTPValidationError, (string kbid, string rslug, string fieldId, bool xSkipStore, FileField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/file/{param.fieldId}"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-skip-store"] = param.xSkipStore.ToString() }),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, (string kbid, string rslug, string fieldId, bool inline)> _downloadFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdDownloadFieldAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, (string kbid, string rslug, string fieldId, bool inline)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/file/{param.fieldId}/download/field?inline={param.inline}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<object, HTTPValidationError, (string kbid, string rslug, string field, object? xExtractStrategy, object? xSplitStrategy, object Body)> _tusPostRslugPrefixKbKbidSlugRslugFileFieldTusuploadAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, HTTPValidationError, (string kbid, string rslug, string field, object? xExtractStrategy, object? xSplitStrategy, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/file/{param.field}/tusupload"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-extract-strategy"] = param.xExtractStrategy?.ToString() ?? string.Empty, ["x-split-strategy"] = param.xSplitStrategy?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PatchAsync<object, HTTPValidationError, ((string kbid, string rslug, string field, string uploadId) Params, object Body)> _tusPatchRslugPrefixKbKbidSlugRslugFileFieldTusuploadUploadIdAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, HTTPValidationError, ((string kbid, string rslug, string field, string uploadId) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/slug/{param.Params.rslug}/file/{param.Params.field}/tusupload/{param.Params.uploadId}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static HeadAsync<object, HTTPValidationError, (string kbid, string rslug, string field, string uploadId)> _uploadInformationKbKbidSlugRslugFileFieldTusuploadUploadIdAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateHead<object, HTTPValidationError, (string kbid, string rslug, string field, string uploadId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/file/{param.field}/tusupload/{param.uploadId}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<ResourceFileUploaded, HTTPValidationError, (string kbid, string rslug, string field, object? xFilename, object? xPassword, object? xLanguage, object? xMd5, object? xExtractStrategy, object? xSplitStrategy, object Body)> _uploadRslugPrefixKbKbidSlugRslugFileFieldUploadAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceFileUploaded, HTTPValidationError, (string kbid, string rslug, string field, object? xFilename, object? xPassword, object? xLanguage, object? xMd5, object? xExtractStrategy, object? xSplitStrategy, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/file/{param.field}/upload"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-filename"] = param.xFilename?.ToString() ?? string.Empty, ["x-password"] = param.xPassword?.ToString() ?? string.Empty, ["x-language"] = param.xLanguage?.ToString() ?? string.Empty, ["x-md5"] = param.xMd5?.ToString() ?? string.Empty, ["x-extract-strategy"] = param.xExtractStrategy?.ToString() ?? string.Empty, ["x-split-strategy"] = param.xSplitStrategy?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceFileUploaded>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PutAsync<ResourceFieldAdded, HTTPValidationError, ((string kbid, string rslug, string fieldId) Params, LinkField Body)> _addResourceFieldLinkRslugPrefixKbKbidSlugRslugLinkFieldIdAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, HTTPValidationError, ((string kbid, string rslug, string fieldId) Params, LinkField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/slug/{param.Params.rslug}/link/{param.Params.fieldId}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<object, HTTPValidationError, (string kbid, string rslug, bool reindexVectors, object Body)> _reindexResourceRslugPrefixKbKbidSlugRslugReindexAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, HTTPValidationError, (string kbid, string rslug, bool reindexVectors, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/reindex?reindex_vectors={param.reindexVectors}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<ResourceUpdated, HTTPValidationError, (string kbid, string rslug, bool resetTitle, string? xNucliadbUser, object Body)> _reprocessResourceRslugPrefixKbKbidSlugRslugReprocessAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceUpdated, HTTPValidationError, (string kbid, string rslug, bool resetTitle, string? xNucliadbUser, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/reprocess?reset_title={param.resetTitle}"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceUpdated>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PutAsync<ResourceFieldAdded, HTTPValidationError, ((string kbid, string rslug, string fieldId) Params, TextField Body)> _addResourceFieldTextRslugPrefixKbKbidSlugRslugTextFieldIdAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, HTTPValidationError, ((string kbid, string rslug, string fieldId) Params, TextField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/slug/{param.Params.rslug}/text/{param.Params.fieldId}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static DeleteAsync<Unit, HTTPValidationError, (string kbid, string rslug, string fieldType, string fieldId)> _resourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDeleteAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, HTTPValidationError, (string kbid, string rslug, string fieldType, string fieldId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/{param.fieldType}/{param.fieldId}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<ResourceField, HTTPValidationError, (string kbid, string rslug, string fieldType, string fieldId, List<string>? show, List<string>? extracted, object? page)> _resourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<ResourceField, HTTPValidationError, (string kbid, string rslug, string fieldType, string fieldId, List<string>? show, List<string>? extracted, object? page)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/{param.fieldType}/{param.fieldId}{BuildQueryString(("show", param.show), ("extracted", param.extracted), ("page", param.page))}"), null, null),
            deserializeSuccess: DeserializeJson<ResourceField>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, (string kbid, string rslug, string fieldType, string fieldId, string downloadField)> _downloadExtractFileRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDownloadExtractedDownloadFieldAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, (string kbid, string rslug, string fieldType, string fieldId, string downloadField)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/{param.fieldType}/{param.fieldId}/download/extracted/{param.downloadField}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<SyncAskResponse, HTTPValidationError, (string kbid, string slug, bool xShowConsumption, string xNdbClient, string? xNucliadbUser, string? xForwardedFor, bool xSynchronous, AskRequest Body)> _resourceAskEndpointBySlugKbKbidSlugSlugAskAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<SyncAskResponse, HTTPValidationError, (string kbid, string slug, bool xShowConsumption, string xNdbClient, string? xNucliadbUser, string? xForwardedFor, bool xSynchronous, AskRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.slug}/ask"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-show-consumption"] = param.xShowConsumption.ToString(), ["x-ndb-client"] = param.xNdbClient.ToString(), ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor?.ToString() ?? string.Empty, ["x-synchronous"] = param.xSynchronous.ToString() }),
            deserializeSuccess: DeserializeJson<SyncAskResponse>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<ResourceAgentsResponse, HTTPValidationError, (string kbid, string slug, string? xNucliadbUser, ResourceAgentsRequest Body)> _runAgentsBySlugKbKbidSlugSlugRunAgentsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceAgentsResponse, HTTPValidationError, (string kbid, string slug, string? xNucliadbUser, ResourceAgentsRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.slug}/run-agents"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceAgentsResponse>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<string, HTTPValidationError, (string Params, SplitConfiguration Body)> _addSplitStrategyKbKbidSplitStrategiesAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<string, HTTPValidationError, (string Params, SplitConfiguration Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/split_strategies"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<string>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, string> _splitStrategiesKbKbidSplitStrategiesGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/split_strategies"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static DeleteAsync<Unit, HTTPValidationError, (string kbid, string strategyId)> _splitStrategyKbKbidSplitStrategiesStrategyStrategyIdDeleteAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, HTTPValidationError, (string kbid, string strategyId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/split_strategies/strategy/{param.strategyId}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<object, HTTPValidationError, (string kbid, string strategyId)> _splitStrategyFromIdKbKbidSplitStrategiesStrategyStrategyIdGetAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, (string kbid, string strategyId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/split_strategies/strategy/{param.strategyId}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static GetAsync<KnowledgeboxSuggestResults, HTTPValidationError, (string kbid, string query, List<string>? fields, List<string>? filters, List<string>? faceted, object? rangeCreationStart, object? rangeCreationEnd, object? rangeModificationStart, object? rangeModificationEnd, List<string>? features, List<string>? show, List<string>? fieldType, bool debug, bool highlight, bool showHidden, string xNdbClient, string? xNucliadbUser, string? xForwardedFor)> _suggestKnowledgeboxKbKbidSuggestAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeboxSuggestResults, HTTPValidationError, (string kbid, string query, List<string>? fields, List<string>? filters, List<string>? faceted, object? rangeCreationStart, object? rangeCreationEnd, object? rangeModificationStart, object? rangeModificationEnd, List<string>? features, List<string>? show, List<string>? fieldType, bool debug, bool highlight, bool showHidden, string xNdbClient, string? xNucliadbUser, string? xForwardedFor)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/suggest{BuildQueryString(("query", param.query), ("fields", param.fields), ("filters", param.filters), ("faceted", param.faceted), ("range_creation_start", param.rangeCreationStart), ("range_creation_end", param.rangeCreationEnd), ("range_modification_start", param.rangeModificationStart), ("range_modification_end", param.rangeModificationEnd), ("features", param.features), ("show", param.show), ("field_type", param.fieldType), ("debug", param.debug), ("highlight", param.highlight), ("show_hidden", param.showHidden))}"), null, new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString(), ["x-nucliadb-user"] = param.xNucliadbUser?.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<KnowledgeboxSuggestResults>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<SummarizedResponse, HTTPValidationError, (string kbid, bool xShowConsumption, SummarizeRequest Body)> _summarizeEndpointKbKbidSummarizeAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<SummarizedResponse, HTTPValidationError, (string kbid, bool xShowConsumption, SummarizeRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/summarize"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-show-consumption"] = param.xShowConsumption.ToString() }),
            deserializeSuccess: DeserializeJson<SummarizedResponse>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<object, HTTPValidationError, (string kbid, object? xExtractStrategy, object? xSplitStrategy, object Body)> _tusPostKbKbidTusuploadAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, HTTPValidationError, (string kbid, object? xExtractStrategy, object? xSplitStrategy, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/tusupload"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-extract-strategy"] = param.xExtractStrategy?.ToString() ?? string.Empty, ["x-split-strategy"] = param.xSplitStrategy?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static OptionsAsync<object, HTTPValidationError, (string kbid, object? rid, object? rslug, object? uploadId, object? field)> _tusOptionsKbKbidTusuploadAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateOptions<object, HTTPValidationError, (string kbid, object? rid, object? rslug, object? uploadId, object? field)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/tusupload{BuildQueryString(("rid", param.rid), ("rslug", param.rslug), ("upload_id", param.uploadId), ("field", param.field))}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PatchAsync<object, HTTPValidationError, ((string kbid, string uploadId) Params, object Body)> _kbKbidTusuploadUploadIdPatchAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, HTTPValidationError, ((string kbid, string uploadId) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/tusupload/{param.Params.uploadId}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static HeadAsync<object, HTTPValidationError, (string kbid, string uploadId)> _uploadInformationKbKbidTusuploadUploadIdAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateHead<object, HTTPValidationError, (string kbid, string uploadId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/tusupload/{param.uploadId}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<ResourceFileUploaded, HTTPValidationError, (string kbid, object? xFilename, object? xPassword, object? xLanguage, object? xMd5, object? xExtractStrategy, object? xSplitStrategy, object Body)> _uploadKbKbidUploadAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceFileUploaded, HTTPValidationError, (string kbid, object? xFilename, object? xPassword, object? xLanguage, object? xMd5, object? xExtractStrategy, object? xSplitStrategy, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/upload"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-filename"] = param.xFilename?.ToString() ?? string.Empty, ["x-password"] = param.xPassword?.ToString() ?? string.Empty, ["x-language"] = param.xLanguage?.ToString() ?? string.Empty, ["x-md5"] = param.xMd5?.ToString() ?? string.Empty, ["x-extract-strategy"] = param.xExtractStrategy?.ToString() ?? string.Empty, ["x-split-strategy"] = param.xSplitStrategy?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceFileUploaded>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

    private static PostAsync<KnowledgeBoxObj, string, (string xNUCLIADBROLES, object Body)> _createKnowledgeBoxKbsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<KnowledgeBoxObj, string, (string xNUCLIADBROLES, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kbs"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["X-NUCLIADB-ROLES"] = param.xNUCLIADBROLES.ToString() }),
            deserializeSuccess: DeserializeJson<KnowledgeBoxObj>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, HTTPValidationError, Unit> _learningConfigurationSchemaLearningConfigurationSchemaAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, HTTPValidationError, Unit>(
            url: BaseUrl,
            buildRequest: static _ => new HttpRequestParts(new RelativeUrl("/api/v1/learning/configuration/schema"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<HTTPValidationError>
        );

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