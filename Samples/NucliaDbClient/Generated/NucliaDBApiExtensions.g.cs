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

    private static readonly AbsoluteUrl BaseUrl = "http://localhost:8080/api/v1".ToAbsoluteUrl();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly Deserialize<Unit> _deserializeUnit = static (_, _) =>
        Task.FromResult(Unit.Value);

    #endregion

    #region Kb Operations

    /// <summary>Get Knowledge Box (by slug)</summary>
    public static Task<Result<KnowledgeBoxObj, HttpError<string>>> GetKbBySlugKbSSlugGet(
        this HttpClient httpClient,
        string slug,
        CancellationToken cancellationToken = default
    ) => _getKbBySlugKbSSlugGet(httpClient, slug, cancellationToken);
    
    /// <summary>Get Knowledge Box</summary>
    public static Task<Result<KnowledgeBoxObj, HttpError<string>>> GetKbKbKbidGet(
        this HttpClient httpClient,
        string kbid, string xNUCLIADBROLES,
        CancellationToken cancellationToken = default
    ) => _getKbKbKbidGet(httpClient, (kbid, xNUCLIADBROLES), cancellationToken);
    
    /// <summary>Ask Knowledge Box</summary>
    public static Task<Result<SyncAskResponse, HttpError<string>>> AskKnowledgeboxEndpointKbKbidAskPost(
        this HttpClient httpClient,
        string kbid, NucliaDBClientType xNdbClient, bool xShowConsumption, string xNucliadbUser, string xForwardedFor, bool xSynchronous, AskRequest body,
        CancellationToken cancellationToken = default
    ) => _askKnowledgeboxEndpointKbKbidAskPost(httpClient, (kbid, xNdbClient, xShowConsumption, xNucliadbUser, xForwardedFor, xSynchronous, body), cancellationToken);
    
    /// <summary>List resources of a Knowledge Box</summary>
    public static Task<Result<KnowledgeboxSearchResults, HttpError<string>>> CatalogGetKbKbidCatalogGet(
        this HttpClient httpClient,
        string kbid, string query, object filterExpression, List<string> filters, List<string> faceted, SortField sortField, object sortLimit, SortOrder sortOrder, int pageNumber, int pageSize, object withStatus, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, object hidden, List<ResourceProperties> show,
        CancellationToken cancellationToken = default
    ) => _catalogGetKbKbidCatalogGet(httpClient, (kbid, query, filterExpression, filters, faceted, sortField, sortLimit, sortOrder, pageNumber, pageSize, withStatus, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, hidden, show), cancellationToken);
    
    /// <summary>List resources of a Knowledge Box</summary>
    public static Task<Result<KnowledgeboxSearchResults, HttpError<string>>> CatalogPostKbKbidCatalogPost(
        this HttpClient httpClient,
        string kbid, CatalogRequest body,
        CancellationToken cancellationToken = default
    ) => _catalogPostKbKbidCatalogPost(httpClient, (kbid, body), cancellationToken);
    
    /// <summary>Get Knowledge Box models configuration</summary>
    public static Task<Result<object, HttpError<string>>> GetConfigurationKbKbidConfigurationGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _getConfigurationKbKbidConfigurationGet(httpClient, kbid, cancellationToken);
    
    /// <summary>Update Knowledge Box models configuration</summary>
    public static Task<Result<object, HttpError<string>>> PatchConfigurationKbKbidConfigurationPatch(
        this HttpClient httpClient,
        string kbid, object body,
        CancellationToken cancellationToken = default
    ) => _patchConfigurationKbKbidConfigurationPatch(httpClient, (kbid, body), cancellationToken);
    
    /// <summary>Create Knowledge Box models configuration</summary>
    public static Task<Result<object, HttpError<string>>> SetConfigurationKbKbidConfigurationPost(
        this HttpClient httpClient,
        string kbid, object body,
        CancellationToken cancellationToken = default
    ) => _setConfigurationKbKbidConfigurationPost(httpClient, (kbid, body), cancellationToken);
    
    /// <summary>Knowledgebox Counters</summary>
    public static Task<Result<KnowledgeboxCounters, HttpError<string>>> KnowledgeboxCountersKbKbidCountersGet(
        this HttpClient httpClient,
        string kbid, bool debug,
        CancellationToken cancellationToken = default
    ) => _knowledgeboxCountersKbKbidCountersGet(httpClient, (kbid, debug), cancellationToken);
    
    /// <summary>Set Knowledge Box Custom Synonyms</summary>
    public static Task<Result<object, HttpError<string>>> SetCustomSynonymsKbKbidCustomSynonymsPut(
        this HttpClient httpClient,
        string kbid, KnowledgeBoxSynonyms body,
        CancellationToken cancellationToken = default
    ) => _setCustomSynonymsKbKbidCustomSynonymsPut(httpClient, (kbid, body), cancellationToken);
    
    /// <summary>Delete Knowledge Box Custom Synonyms</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteCustomSynonymsKbKbidCustomSynonymsDelete(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _deleteCustomSynonymsKbKbidCustomSynonymsDelete(httpClient, kbid, cancellationToken);
    
    /// <summary>Get Knowledge Box Custom Synonyms</summary>
    public static Task<Result<KnowledgeBoxSynonyms, HttpError<string>>> GetCustomSynonymsKbKbidCustomSynonymsGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _getCustomSynonymsKbKbidCustomSynonymsGet(httpClient, kbid, cancellationToken);
    
    /// <summary>Update Knowledge Box Entities Group</summary>
    public static Task<Result<object, HttpError<string>>> UpdateEntitiesGroupKbKbidEntitiesgroupGroupPatch(
        this HttpClient httpClient,
        string kbid, string group, UpdateEntitiesGroupPayload body,
        CancellationToken cancellationToken = default
    ) => _updateEntitiesGroupKbKbidEntitiesgroupGroupPatch(httpClient, ((kbid, group), body), cancellationToken);
    
    /// <summary>Delete Knowledge Box Entities</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteEntitiesKbKbidEntitiesgroupGroupDelete(
        this HttpClient httpClient,
        string kbid, string group,
        CancellationToken cancellationToken = default
    ) => _deleteEntitiesKbKbidEntitiesgroupGroupDelete(httpClient, (kbid, group), cancellationToken);
    
    /// <summary>Get a Knowledge Box Entities Group</summary>
    public static Task<Result<EntitiesGroup, HttpError<string>>> GetEntityKbKbidEntitiesgroupGroupGet(
        this HttpClient httpClient,
        string kbid, string group,
        CancellationToken cancellationToken = default
    ) => _getEntityKbKbidEntitiesgroupGroupGet(httpClient, (kbid, group), cancellationToken);
    
    /// <summary>Create Knowledge Box Entities Group</summary>
    public static Task<Result<object, HttpError<string>>> CreateEntitiesGroupKbKbidEntitiesgroupsPost(
        this HttpClient httpClient,
        string kbid, CreateEntitiesGroupPayload body,
        CancellationToken cancellationToken = default
    ) => _createEntitiesGroupKbKbidEntitiesgroupsPost(httpClient, (kbid, body), cancellationToken);
    
    /// <summary>Get Knowledge Box Entities</summary>
    public static Task<Result<KnowledgeBoxEntities, HttpError<string>>> GetEntitiesKbKbidEntitiesgroupsGet(
        this HttpClient httpClient,
        string kbid, bool showEntities,
        CancellationToken cancellationToken = default
    ) => _getEntitiesKbKbidEntitiesgroupsGet(httpClient, (kbid, showEntities), cancellationToken);
    
    /// <summary>Start an export of a Knowledge Box</summary>
    public static Task<Result<CreateExportResponse, HttpError<string>>> StartKbExportEndpointKbKbidExportPost(
        this HttpClient httpClient,
        string kbid, object body,
        CancellationToken cancellationToken = default
    ) => _startKbExportEndpointKbKbidExportPost(httpClient, (kbid, body), cancellationToken);
    
    /// <summary>Download a Knowledge Box export</summary>
    public static Task<Result<object, HttpError<string>>> DownloadExportKbEndpointKbKbidExportExportIdGet(
        this HttpClient httpClient,
        string kbid, string exportId,
        CancellationToken cancellationToken = default
    ) => _downloadExportKbEndpointKbKbidExportExportIdGet(httpClient, (kbid, exportId), cancellationToken);
    
    /// <summary>Get the status of a Knowledge Box Export</summary>
    public static Task<Result<StatusResponse, HttpError<string>>> GetExportStatusEndpointKbKbidExportExportIdStatusGet(
        this HttpClient httpClient,
        string kbid, string exportId,
        CancellationToken cancellationToken = default
    ) => _getExportStatusEndpointKbKbidExportExportIdStatusGet(httpClient, (kbid, exportId), cancellationToken);
    
    /// <summary>Add a extract strategy to a KB</summary>
    public static Task<Result<string, HttpError<string>>> AddStrategyKbKbidExtractStrategiesPost(
        this HttpClient httpClient,
        string kbid, ExtractConfig body,
        CancellationToken cancellationToken = default
    ) => _addStrategyKbKbidExtractStrategiesPost(httpClient, (kbid, body), cancellationToken);
    
    /// <summary>Learning extract strategies</summary>
    public static Task<Result<object, HttpError<string>>> GetExtractStrategiesKbKbidExtractStrategiesGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _getExtractStrategiesKbKbidExtractStrategiesGet(httpClient, kbid, cancellationToken);
    
    /// <summary>Remove a extract strategy from a KB</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteStrategyKbKbidExtractStrategiesStrategyStrategyIdDelete(
        this HttpClient httpClient,
        string kbid, string strategyId,
        CancellationToken cancellationToken = default
    ) => _deleteStrategyKbKbidExtractStrategiesStrategyStrategyIdDelete(httpClient, (kbid, strategyId), cancellationToken);
    
    /// <summary>Extract strategy configuration</summary>
    public static Task<Result<object, HttpError<string>>> GetExtractStrategyFromIdKbKbidExtractStrategiesStrategyStrategyIdGet(
        this HttpClient httpClient,
        string kbid, string strategyId,
        CancellationToken cancellationToken = default
    ) => _getExtractStrategyFromIdKbKbidExtractStrategiesStrategyStrategyIdGet(httpClient, (kbid, strategyId), cancellationToken);
    
    /// <summary>Send Feedback</summary>
    public static Task<Result<object, HttpError<string>>> SendFeedbackEndpointKbKbidFeedbackPost(
        this HttpClient httpClient,
        string kbid, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, FeedbackRequest body,
        CancellationToken cancellationToken = default
    ) => _sendFeedbackEndpointKbKbidFeedbackPost(httpClient, (kbid, xNdbClient, xNucliadbUser, xForwardedFor, body), cancellationToken);
    
    /// <summary>Find Knowledge Box</summary>
    public static Task<Result<KnowledgeboxFindResults, HttpError<string>>> FindKnowledgeboxKbKbidFindGet(
        this HttpClient httpClient,
        string kbid, string query, object filterExpression, List<string> fields, List<string> filters, object topK, object minScore, object minScoreSemantic, float minScoreBm25, object vectorset, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, List<FindOptions> features, bool debug, bool highlight, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, bool withDuplicates, bool withSynonyms, bool autofilter, List<string> securityGroups, bool showHidden, RankFusionName rankFusion, object reranker, object searchConfiguration, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor,
        CancellationToken cancellationToken = default
    ) => _findKnowledgeboxKbKbidFindGet(httpClient, (kbid, query, filterExpression, fields, filters, topK, minScore, minScoreSemantic, minScoreBm25, vectorset, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, features, debug, highlight, show, fieldType, extracted, withDuplicates, withSynonyms, autofilter, securityGroups, showHidden, rankFusion, reranker, searchConfiguration, xNdbClient, xNucliadbUser, xForwardedFor), cancellationToken);
    
    /// <summary>Find Knowledge Box</summary>
    public static Task<Result<KnowledgeboxFindResults, HttpError<string>>> FindPostKnowledgeboxKbKbidFindPost(
        this HttpClient httpClient,
        string kbid, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, FindRequest body,
        CancellationToken cancellationToken = default
    ) => _findPostKnowledgeboxKbKbidFindPost(httpClient, (kbid, xNdbClient, xNucliadbUser, xForwardedFor, body), cancellationToken);
    
    /// <summary>Search Knowledge Box graph</summary>
    public static Task<Result<GraphSearchResponse, HttpError<string>>> GraphSearchKnowledgeboxKbKbidGraphPost(
        this HttpClient httpClient,
        string kbid, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, GraphSearchRequest body,
        CancellationToken cancellationToken = default
    ) => _graphSearchKnowledgeboxKbKbidGraphPost(httpClient, (kbid, xNdbClient, xNucliadbUser, xForwardedFor, body), cancellationToken);
    
    /// <summary>Search Knowledge Box graph nodes</summary>
    public static Task<Result<GraphNodesSearchResponse, HttpError<string>>> GraphNodesSearchKnowledgeboxKbKbidGraphNodesPost(
        this HttpClient httpClient,
        string kbid, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, GraphNodesSearchRequest body,
        CancellationToken cancellationToken = default
    ) => _graphNodesSearchKnowledgeboxKbKbidGraphNodesPost(httpClient, (kbid, xNdbClient, xNucliadbUser, xForwardedFor, body), cancellationToken);
    
    /// <summary>Search Knowledge Box graph relations</summary>
    public static Task<Result<GraphRelationsSearchResponse, HttpError<string>>> GraphRelationsSearchKnowledgeboxKbKbidGraphRelationsPost(
        this HttpClient httpClient,
        string kbid, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, GraphRelationsSearchRequest body,
        CancellationToken cancellationToken = default
    ) => _graphRelationsSearchKnowledgeboxKbKbidGraphRelationsPost(httpClient, (kbid, xNdbClient, xNucliadbUser, xForwardedFor, body), cancellationToken);
    
    /// <summary>Start an import to a Knowledge Box</summary>
    public static Task<Result<CreateImportResponse, HttpError<string>>> StartKbImportEndpointKbKbidImportPost(
        this HttpClient httpClient,
        string kbid, object body,
        CancellationToken cancellationToken = default
    ) => _startKbImportEndpointKbKbidImportPost(httpClient, (kbid, body), cancellationToken);
    
    /// <summary>Get the status of a Knowledge Box Import</summary>
    public static Task<Result<StatusResponse, HttpError<string>>> GetImportStatusEndpointKbKbidImportImportIdStatusGet(
        this HttpClient httpClient,
        string kbid, string importId,
        CancellationToken cancellationToken = default
    ) => _getImportStatusEndpointKbKbidImportImportIdStatusGet(httpClient, (kbid, importId), cancellationToken);
    
    /// <summary>Set Knowledge Box Labels</summary>
    public static Task<Result<object, HttpError<string>>> SetLabelsetEndpointKbKbidLabelsetLabelsetPost(
        this HttpClient httpClient,
        string kbid, string labelset, LabelSet body,
        CancellationToken cancellationToken = default
    ) => _setLabelsetEndpointKbKbidLabelsetLabelsetPost(httpClient, ((kbid, labelset), body), cancellationToken);
    
    /// <summary>Delete Knowledge Box Label</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteLabelsetEndpointKbKbidLabelsetLabelsetDelete(
        this HttpClient httpClient,
        string kbid, string labelset,
        CancellationToken cancellationToken = default
    ) => _deleteLabelsetEndpointKbKbidLabelsetLabelsetDelete(httpClient, (kbid, labelset), cancellationToken);
    
    /// <summary>Get a Knowledge Box Label Set</summary>
    public static Task<Result<LabelSet, HttpError<string>>> GetLabelsetEndpointKbKbidLabelsetLabelsetGet(
        this HttpClient httpClient,
        string kbid, string labelset,
        CancellationToken cancellationToken = default
    ) => _getLabelsetEndpointKbKbidLabelsetLabelsetGet(httpClient, (kbid, labelset), cancellationToken);
    
    /// <summary>Get Knowledge Box Label Sets</summary>
    public static Task<Result<KnowledgeBoxLabels, HttpError<string>>> GetLabelsetsEndointKbKbidLabelsetsGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _getLabelsetsEndointKbKbidLabelsetsGet(httpClient, kbid, cancellationToken);
    
    /// <summary>Get model metadata</summary>
    public static Task<Result<object, HttpError<string>>> GetModelKbKbidModelModelIdGet(
        this HttpClient httpClient,
        string kbid, string modelId,
        CancellationToken cancellationToken = default
    ) => _getModelKbKbidModelModelIdGet(httpClient, (kbid, modelId), cancellationToken);
    
    /// <summary>Get available models</summary>
    public static Task<Result<object, HttpError<string>>> GetModelsKbKbidModelsGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _getModelsKbKbidModelsGet(httpClient, kbid, cancellationToken);
    
    /// <summary>Download the Knowledege Box model</summary>
    public static Task<Result<object, HttpError<string>>> DownloadModelKbKbidModelsModelIdFilenameGet(
        this HttpClient httpClient,
        string kbid, string modelId, string filename,
        CancellationToken cancellationToken = default
    ) => _downloadModelKbKbidModelsModelIdFilenameGet(httpClient, (kbid, modelId, filename), cancellationToken);
    
    /// <summary>Knowledge Box Notifications Stream</summary>
    public static Task<Result<object, HttpError<string>>> NotificationsEndpointKbKbidNotificationsGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _notificationsEndpointKbKbidNotificationsGet(httpClient, kbid, cancellationToken);
    
    /// <summary>Predict API Proxy</summary>
    public static Task<Result<object, HttpError<string>>> PredictProxyEndpointKbKbidPredictEndpointPost(
        this HttpClient httpClient,
        string kbid, PredictProxiedEndpoints endpoint, string xNucliadbUser, NucliaDBClientType xNdbClient, string xForwardedFor, object body,
        CancellationToken cancellationToken = default
    ) => _predictProxyEndpointKbKbidPredictEndpointPost(httpClient, (kbid, endpoint, xNucliadbUser, xNdbClient, xForwardedFor, body), cancellationToken);
    
    /// <summary>Predict API Proxy</summary>
    public static Task<Result<object, HttpError<string>>> PredictProxyEndpointKbKbidPredictEndpointGet(
        this HttpClient httpClient,
        string kbid, PredictProxiedEndpoints endpoint, string xNucliadbUser, NucliaDBClientType xNdbClient, string xForwardedFor,
        CancellationToken cancellationToken = default
    ) => _predictProxyEndpointKbKbidPredictEndpointGet(httpClient, (kbid, endpoint, xNucliadbUser, xNdbClient, xForwardedFor), cancellationToken);
    
    /// <summary>Knowledge Box Processing Status</summary>
    public static Task<Result<RequestsResults, HttpError<string>>> ProcessingStatusKbKbidProcessingStatusGet(
        this HttpClient httpClient,
        string kbid, object cursor, object scheduled, int limit,
        CancellationToken cancellationToken = default
    ) => _processingStatusKbKbidProcessingStatusGet(httpClient, (kbid, cursor, scheduled, limit), cancellationToken);
    
    /// <summary>Create new upload on a Resource (by id)</summary>
    public static Task<Result<object, HttpError<string>>> TusPostRidPrefixKbKbidResourcePathRidFileFieldTusuploadPost(
        this HttpClient httpClient,
        string kbid, string pathRid, string field, object xExtractStrategy, object xSplitStrategy, object body,
        CancellationToken cancellationToken = default
    ) => _tusPostRidPrefixKbKbidResourcePathRidFileFieldTusuploadPost(httpClient, (kbid, pathRid, field, xExtractStrategy, xSplitStrategy, body), cancellationToken);
    
    /// <summary>Upload information</summary>
    public static Task<Result<object, HttpError<string>>> UploadInformationKbKbidResourcePathRidFileFieldTusuploadUploadIdHead(
        this HttpClient httpClient,
        string kbid, string pathRid, string field, string uploadId,
        CancellationToken cancellationToken = default
    ) => _uploadInformationKbKbidResourcePathRidFileFieldTusuploadUploadIdHead(httpClient, (kbid, pathRid, field, uploadId), cancellationToken);
    
    /// <summary>Upload binary file on a Resource (by id)</summary>
    public static Task<Result<ResourceFileUploaded, HttpError<string>>> UploadRidPrefixKbKbidResourcePathRidFileFieldUploadPost(
        this HttpClient httpClient,
        string kbid, string pathRid, string field, object xFilename, object xPassword, object xLanguage, object xMd5, object xExtractStrategy, object xSplitStrategy, object body,
        CancellationToken cancellationToken = default
    ) => _uploadRidPrefixKbKbidResourcePathRidFileFieldUploadPost(httpClient, (kbid, pathRid, field, xFilename, xPassword, xLanguage, xMd5, xExtractStrategy, xSplitStrategy, body), cancellationToken);
    
    /// <summary>Modify Resource (by id)</summary>
    public static Task<Result<ResourceUpdated, HttpError<string>>> ModifyResourceRidPrefixKbKbidResourceRidPatch(
        this HttpClient httpClient,
        string kbid, string rid, string xNucliadbUser, bool xSkipStore, UpdateResourcePayload body,
        CancellationToken cancellationToken = default
    ) => _modifyResourceRidPrefixKbKbidResourceRidPatch(httpClient, (kbid, rid, xNucliadbUser, xSkipStore, body), cancellationToken);
    
    /// <summary>Delete Resource (by id)</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteResourceRidPrefixKbKbidResourceRidDelete(
        this HttpClient httpClient,
        string kbid, string rid,
        CancellationToken cancellationToken = default
    ) => _deleteResourceRidPrefixKbKbidResourceRidDelete(httpClient, (kbid, rid), cancellationToken);
    
    /// <summary>Get Resource (by id)</summary>
    public static Task<Result<NucliadbModelsResourceResource, HttpError<string>>> GetResourceByUuidKbKbidResourceRidGet(
        this HttpClient httpClient,
        string kbid, string rid, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, string xNucliadbUser, string xForwardedFor,
        CancellationToken cancellationToken = default
    ) => _getResourceByUuidKbKbidResourceRidGet(httpClient, (kbid, rid, show, fieldType, extracted, xNucliadbUser, xForwardedFor), cancellationToken);
    
    /// <summary>Ask a resource (by id)</summary>
    public static Task<Result<SyncAskResponse, HttpError<string>>> ResourceAskEndpointByUuidKbKbidResourceRidAskPost(
        this HttpClient httpClient,
        string kbid, string rid, bool xShowConsumption, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, bool xSynchronous, AskRequest body,
        CancellationToken cancellationToken = default
    ) => _resourceAskEndpointByUuidKbKbidResourceRidAskPost(httpClient, (kbid, rid, xShowConsumption, xNdbClient, xNucliadbUser, xForwardedFor, xSynchronous, body), cancellationToken);
    
    /// <summary>Add resource conversation field (by id)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AddResourceFieldConversationRidPrefixKbKbidResourceRidConversationFieldIdPut(
        this HttpClient httpClient,
        string kbid, string rid, string fieldId, InputConversationField body,
        CancellationToken cancellationToken = default
    ) => _addResourceFieldConversationRidPrefixKbKbidResourceRidConversationFieldIdPut(httpClient, ((kbid, rid, fieldId), body), cancellationToken);
    
    /// <summary>Download conversation binary field (by id)</summary>
    public static Task<Result<object, HttpError<string>>> DownloadFieldConversationAttachmentRidPrefixKbKbidResourceRidConversationFieldIdDownloadFieldMessageIdFileNumGet(
        this HttpClient httpClient,
        string kbid, string rid, string fieldId, string messageId, int fileNum,
        CancellationToken cancellationToken = default
    ) => _downloadFieldConversationAttachmentRidPrefixKbKbidResourceRidConversationFieldIdDownloadFieldMessageIdFileNumGet(httpClient, (kbid, rid, fieldId, messageId, fileNum), cancellationToken);
    
    /// <summary>Append messages to conversation field (by id)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AppendMessagesToConversationFieldRidPrefixKbKbidResourceRidConversationFieldIdMessagesPut(
        this HttpClient httpClient,
        string kbid, string rid, string fieldId, object body,
        CancellationToken cancellationToken = default
    ) => _appendMessagesToConversationFieldRidPrefixKbKbidResourceRidConversationFieldIdMessagesPut(httpClient, ((kbid, rid, fieldId), body), cancellationToken);
    
    /// <summary>Add resource file field (by id)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AddResourceFieldFileRidPrefixKbKbidResourceRidFileFieldIdPut(
        this HttpClient httpClient,
        string kbid, string rid, string fieldId, bool xSkipStore, FileField body,
        CancellationToken cancellationToken = default
    ) => _addResourceFieldFileRidPrefixKbKbidResourceRidFileFieldIdPut(httpClient, (kbid, rid, fieldId, xSkipStore, body), cancellationToken);
    
    /// <summary>Download field binary field (by id)</summary>
    public static Task<Result<object, HttpError<string>>> DownloadFieldFileRidPrefixKbKbidResourceRidFileFieldIdDownloadFieldGet(
        this HttpClient httpClient,
        string kbid, string rid, string fieldId, bool inline,
        CancellationToken cancellationToken = default
    ) => _downloadFieldFileRidPrefixKbKbidResourceRidFileFieldIdDownloadFieldGet(httpClient, (kbid, rid, fieldId, inline), cancellationToken);
    
    /// <summary>Reprocess file field (by id)</summary>
    public static Task<Result<ResourceUpdated, HttpError<string>>> ReprocessFileFieldKbKbidResourceRidFileFieldIdReprocessPost(
        this HttpClient httpClient,
        string kbid, string rid, string fieldId, bool resetTitle, string xNucliadbUser, object xFilePassword, object body,
        CancellationToken cancellationToken = default
    ) => _reprocessFileFieldKbKbidResourceRidFileFieldIdReprocessPost(httpClient, (kbid, rid, fieldId, resetTitle, xNucliadbUser, xFilePassword, body), cancellationToken);
    
    /// <summary>Upload data on a Resource (by id)</summary>
    public static Task<Result<object, HttpError<string>>> TusPatchRidPrefixKbKbidResourceRidFileFieldTusuploadUploadIdPatch(
        this HttpClient httpClient,
        string kbid, string rid, string field, string uploadId, object body,
        CancellationToken cancellationToken = default
    ) => _tusPatchRidPrefixKbKbidResourceRidFileFieldTusuploadUploadIdPatch(httpClient, ((kbid, rid, field, uploadId), body), cancellationToken);
    
    /// <summary>Add resource link field (by id)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AddResourceFieldLinkRidPrefixKbKbidResourceRidLinkFieldIdPut(
        this HttpClient httpClient,
        string kbid, string rid, string fieldId, LinkField body,
        CancellationToken cancellationToken = default
    ) => _addResourceFieldLinkRidPrefixKbKbidResourceRidLinkFieldIdPut(httpClient, ((kbid, rid, fieldId), body), cancellationToken);
    
    /// <summary>Reindex Resource (by id)</summary>
    public static Task<Result<object, HttpError<string>>> ReindexResourceRidPrefixKbKbidResourceRidReindexPost(
        this HttpClient httpClient,
        string kbid, string rid, bool reindexVectors, object body,
        CancellationToken cancellationToken = default
    ) => _reindexResourceRidPrefixKbKbidResourceRidReindexPost(httpClient, (kbid, rid, reindexVectors, body), cancellationToken);
    
    /// <summary>Reprocess resource (by id)</summary>
    public static Task<Result<ResourceUpdated, HttpError<string>>> ReprocessResourceRidPrefixKbKbidResourceRidReprocessPost(
        this HttpClient httpClient,
        string kbid, string rid, bool resetTitle, string xNucliadbUser, object body,
        CancellationToken cancellationToken = default
    ) => _reprocessResourceRidPrefixKbKbidResourceRidReprocessPost(httpClient, (kbid, rid, resetTitle, xNucliadbUser, body), cancellationToken);
    
    /// <summary>Run Agents on Resource</summary>
    public static Task<Result<ResourceAgentsResponse, HttpError<string>>> RunAgentsByUuidKbKbidResourceRidRunAgentsPost(
        this HttpClient httpClient,
        string kbid, string rid, string xNucliadbUser, ResourceAgentsRequest body,
        CancellationToken cancellationToken = default
    ) => _runAgentsByUuidKbKbidResourceRidRunAgentsPost(httpClient, (kbid, rid, xNucliadbUser, body), cancellationToken);
    
    /// <summary>Search on Resource</summary>
    public static Task<Result<ResourceSearchResults, HttpError<string>>> ResourceSearchKbKbidResourceRidSearchGet(
        this HttpClient httpClient,
        string kbid, string rid, string query, object filterExpression, List<string> fields, List<string> filters, List<string> faceted, object sortField, SortOrder sortOrder, object topK, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, bool highlight, bool debug, NucliaDBClientType xNdbClient,
        CancellationToken cancellationToken = default
    ) => _resourceSearchKbKbidResourceRidSearchGet(httpClient, (kbid, rid, query, filterExpression, fields, filters, faceted, sortField, sortOrder, topK, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, highlight, debug, xNdbClient), cancellationToken);
    
    /// <summary>Add resource text field (by id)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AddResourceFieldTextRidPrefixKbKbidResourceRidTextFieldIdPut(
        this HttpClient httpClient,
        string kbid, string rid, string fieldId, TextField body,
        CancellationToken cancellationToken = default
    ) => _addResourceFieldTextRidPrefixKbKbidResourceRidTextFieldIdPut(httpClient, ((kbid, rid, fieldId), body), cancellationToken);
    
    /// <summary>Delete Resource field (by id)</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteResourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdDelete(
        this HttpClient httpClient,
        string kbid, string rid, FieldTypeName fieldType, string fieldId,
        CancellationToken cancellationToken = default
    ) => _deleteResourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdDelete(httpClient, (kbid, rid, fieldType, fieldId), cancellationToken);
    
    /// <summary>Get Resource field (by id)</summary>
    public static Task<Result<ResourceField, HttpError<string>>> GetResourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdGet(
        this HttpClient httpClient,
        string kbid, string rid, FieldTypeName fieldType, string fieldId, List<ResourceFieldProperties> show, List<ExtractedDataTypeName> extracted, object page,
        CancellationToken cancellationToken = default
    ) => _getResourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdGet(httpClient, (kbid, rid, fieldType, fieldId, show, extracted, page), cancellationToken);
    
    /// <summary>Download extracted binary file (by id)</summary>
    public static Task<Result<object, HttpError<string>>> DownloadExtractFileRidPrefixKbKbidResourceRidFieldTypeFieldIdDownloadExtractedDownloadFieldGet(
        this HttpClient httpClient,
        string kbid, string rid, FieldTypeName fieldType, string fieldId, string downloadField,
        CancellationToken cancellationToken = default
    ) => _downloadExtractFileRidPrefixKbKbidResourceRidFieldTypeFieldIdDownloadExtractedDownloadFieldGet(httpClient, (kbid, rid, fieldType, fieldId, downloadField), cancellationToken);
    
    /// <summary>Create Resource</summary>
    public static Task<Result<ResourceCreated, HttpError<string>>> CreateResourceKbKbidResourcesPost(
        this HttpClient httpClient,
        string kbid, bool xSkipStore, string xNucliadbUser, CreateResourcePayload body,
        CancellationToken cancellationToken = default
    ) => _createResourceKbKbidResourcesPost(httpClient, (kbid, xSkipStore, xNucliadbUser, body), cancellationToken);
    
    /// <summary>List Resources</summary>
    public static Task<Result<ResourceList, HttpError<string>>> ListResourcesKbKbidResourcesGet(
        this HttpClient httpClient,
        string kbid, int page, int size, string xNUCLIADBROLES,
        CancellationToken cancellationToken = default
    ) => _listResourcesKbKbidResourcesGet(httpClient, (kbid, page, size, xNUCLIADBROLES), cancellationToken);
    
    /// <summary>Learning configuration schema</summary>
    public static Task<Result<object, HttpError<string>>> GetSchemaForConfigurationUpdatesKbKbidSchemaGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _getSchemaForConfigurationUpdatesKbKbidSchemaGet(httpClient, kbid, cancellationToken);
    
    /// <summary>Search Knowledge Box</summary>
    public static Task<Result<KnowledgeboxSearchResults, HttpError<string>>> SearchKnowledgeboxKbKbidSearchGet(
        this HttpClient httpClient,
        string kbid, string query, object filterExpression, List<string> fields, List<string> filters, List<string> faceted, SortField sortField, object sortLimit, SortOrder sortOrder, int topK, object minScore, object minScoreSemantic, float minScoreBm25, object vectorset, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, List<SearchOptions> features, bool debug, bool highlight, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, bool withDuplicates, bool withSynonyms, bool autofilter, List<string> securityGroups, bool showHidden, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor,
        CancellationToken cancellationToken = default
    ) => _searchKnowledgeboxKbKbidSearchGet(httpClient, (kbid, query, filterExpression, fields, filters, faceted, sortField, sortLimit, sortOrder, topK, minScore, minScoreSemantic, minScoreBm25, vectorset, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, features, debug, highlight, show, fieldType, extracted, withDuplicates, withSynonyms, autofilter, securityGroups, showHidden, xNdbClient, xNucliadbUser, xForwardedFor), cancellationToken);
    
    /// <summary>Search Knowledge Box</summary>
    public static Task<Result<KnowledgeboxSearchResults, HttpError<string>>> SearchPostKnowledgeboxKbKbidSearchPost(
        this HttpClient httpClient,
        string kbid, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, SearchRequest body,
        CancellationToken cancellationToken = default
    ) => _searchPostKnowledgeboxKbKbidSearchPost(httpClient, (kbid, xNdbClient, xNucliadbUser, xForwardedFor, body), cancellationToken);
    
    /// <summary>List search configurations</summary>
    public static Task<Result<object, HttpError<string>>> ListSearchConfigurationsKbKbidSearchConfigurationsGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _listSearchConfigurationsKbKbidSearchConfigurationsGet(httpClient, kbid, cancellationToken);
    
    /// <summary>Create search configuration</summary>
    public static Task<Result<object, HttpError<string>>> CreateSearchConfigurationKbKbidSearchConfigurationsConfigNamePost(
        this HttpClient httpClient,
        string kbid, string configName, object body,
        CancellationToken cancellationToken = default
    ) => _createSearchConfigurationKbKbidSearchConfigurationsConfigNamePost(httpClient, ((kbid, configName), body), cancellationToken);
    
    /// <summary>Update search configuration</summary>
    public static Task<Result<object, HttpError<string>>> UpdateSearchConfigurationKbKbidSearchConfigurationsConfigNamePatch(
        this HttpClient httpClient,
        string kbid, string configName, object body,
        CancellationToken cancellationToken = default
    ) => _updateSearchConfigurationKbKbidSearchConfigurationsConfigNamePatch(httpClient, ((kbid, configName), body), cancellationToken);
    
    /// <summary>Delete search configuration</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteSearchConfigurationKbKbidSearchConfigurationsConfigNameDelete(
        this HttpClient httpClient,
        string kbid, string configName,
        CancellationToken cancellationToken = default
    ) => _deleteSearchConfigurationKbKbidSearchConfigurationsConfigNameDelete(httpClient, (kbid, configName), cancellationToken);
    
    /// <summary>Get search configuration</summary>
    public static Task<Result<object, HttpError<string>>> GetSearchConfigurationKbKbidSearchConfigurationsConfigNameGet(
        this HttpClient httpClient,
        string kbid, string configName,
        CancellationToken cancellationToken = default
    ) => _getSearchConfigurationKbKbidSearchConfigurationsConfigNameGet(httpClient, (kbid, configName), cancellationToken);
    
    /// <summary>Modify Resource (by slug)</summary>
    public static Task<Result<ResourceUpdated, HttpError<string>>> ModifyResourceRslugPrefixKbKbidSlugRslugPatch(
        this HttpClient httpClient,
        string kbid, string rslug, bool xSkipStore, string xNucliadbUser, UpdateResourcePayload body,
        CancellationToken cancellationToken = default
    ) => _modifyResourceRslugPrefixKbKbidSlugRslugPatch(httpClient, (kbid, rslug, xSkipStore, xNucliadbUser, body), cancellationToken);
    
    /// <summary>Delete Resource (by slug)</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteResourceRslugPrefixKbKbidSlugRslugDelete(
        this HttpClient httpClient,
        string kbid, string rslug,
        CancellationToken cancellationToken = default
    ) => _deleteResourceRslugPrefixKbKbidSlugRslugDelete(httpClient, (kbid, rslug), cancellationToken);
    
    /// <summary>Get Resource (by slug)</summary>
    public static Task<Result<NucliadbModelsResourceResource, HttpError<string>>> GetResourceBySlugKbKbidSlugRslugGet(
        this HttpClient httpClient,
        string kbid, string rslug, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, string xNucliadbUser, string xForwardedFor,
        CancellationToken cancellationToken = default
    ) => _getResourceBySlugKbKbidSlugRslugGet(httpClient, (kbid, rslug, show, fieldType, extracted, xNucliadbUser, xForwardedFor), cancellationToken);
    
    /// <summary>Add resource conversation field (by slug)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AddResourceFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdPut(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldId, InputConversationField body,
        CancellationToken cancellationToken = default
    ) => _addResourceFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdPut(httpClient, ((kbid, rslug, fieldId), body), cancellationToken);
    
    /// <summary>Download conversation binary field (by slug)</summary>
    public static Task<Result<object, HttpError<string>>> DownloadFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdDownloadFieldMessageIdFileNumGet(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldId, string messageId, int fileNum,
        CancellationToken cancellationToken = default
    ) => _downloadFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdDownloadFieldMessageIdFileNumGet(httpClient, (kbid, rslug, fieldId, messageId, fileNum), cancellationToken);
    
    /// <summary>Append messages to conversation field (by slug)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AppendMessagesToConversationFieldRslugPrefixKbKbidSlugRslugConversationFieldIdMessagesPut(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldId, object body,
        CancellationToken cancellationToken = default
    ) => _appendMessagesToConversationFieldRslugPrefixKbKbidSlugRslugConversationFieldIdMessagesPut(httpClient, ((kbid, rslug, fieldId), body), cancellationToken);
    
    /// <summary>Add resource file field (by slug)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AddResourceFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdPut(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldId, bool xSkipStore, FileField body,
        CancellationToken cancellationToken = default
    ) => _addResourceFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdPut(httpClient, (kbid, rslug, fieldId, xSkipStore, body), cancellationToken);
    
    /// <summary>Download field binary field (by slug)</summary>
    public static Task<Result<object, HttpError<string>>> DownloadFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdDownloadFieldGet(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldId, bool inline,
        CancellationToken cancellationToken = default
    ) => _downloadFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdDownloadFieldGet(httpClient, (kbid, rslug, fieldId, inline), cancellationToken);
    
    /// <summary>Create new upload on a Resource (by slug)</summary>
    public static Task<Result<object, HttpError<string>>> TusPostRslugPrefixKbKbidSlugRslugFileFieldTusuploadPost(
        this HttpClient httpClient,
        string kbid, string rslug, string field, object xExtractStrategy, object xSplitStrategy, object body,
        CancellationToken cancellationToken = default
    ) => _tusPostRslugPrefixKbKbidSlugRslugFileFieldTusuploadPost(httpClient, (kbid, rslug, field, xExtractStrategy, xSplitStrategy, body), cancellationToken);
    
    /// <summary>Upload data on a Resource (by slug)</summary>
    public static Task<Result<object, HttpError<string>>> TusPatchRslugPrefixKbKbidSlugRslugFileFieldTusuploadUploadIdPatch(
        this HttpClient httpClient,
        string kbid, string rslug, string field, string uploadId, object body,
        CancellationToken cancellationToken = default
    ) => _tusPatchRslugPrefixKbKbidSlugRslugFileFieldTusuploadUploadIdPatch(httpClient, ((kbid, rslug, field, uploadId), body), cancellationToken);
    
    /// <summary>Upload information</summary>
    public static Task<Result<object, HttpError<string>>> UploadInformationKbKbidSlugRslugFileFieldTusuploadUploadIdHead(
        this HttpClient httpClient,
        string kbid, string rslug, string field, string uploadId,
        CancellationToken cancellationToken = default
    ) => _uploadInformationKbKbidSlugRslugFileFieldTusuploadUploadIdHead(httpClient, (kbid, rslug, field, uploadId), cancellationToken);
    
    /// <summary>Upload binary file on a Resource (by slug)</summary>
    public static Task<Result<ResourceFileUploaded, HttpError<string>>> UploadRslugPrefixKbKbidSlugRslugFileFieldUploadPost(
        this HttpClient httpClient,
        string kbid, string rslug, string field, object xFilename, object xPassword, object xLanguage, object xMd5, object xExtractStrategy, object xSplitStrategy, object body,
        CancellationToken cancellationToken = default
    ) => _uploadRslugPrefixKbKbidSlugRslugFileFieldUploadPost(httpClient, (kbid, rslug, field, xFilename, xPassword, xLanguage, xMd5, xExtractStrategy, xSplitStrategy, body), cancellationToken);
    
    /// <summary>Add resource link field (by slug)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AddResourceFieldLinkRslugPrefixKbKbidSlugRslugLinkFieldIdPut(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldId, LinkField body,
        CancellationToken cancellationToken = default
    ) => _addResourceFieldLinkRslugPrefixKbKbidSlugRslugLinkFieldIdPut(httpClient, ((kbid, rslug, fieldId), body), cancellationToken);
    
    /// <summary>Reindex Resource (by slug)</summary>
    public static Task<Result<object, HttpError<string>>> ReindexResourceRslugPrefixKbKbidSlugRslugReindexPost(
        this HttpClient httpClient,
        string kbid, string rslug, bool reindexVectors, object body,
        CancellationToken cancellationToken = default
    ) => _reindexResourceRslugPrefixKbKbidSlugRslugReindexPost(httpClient, (kbid, rslug, reindexVectors, body), cancellationToken);
    
    /// <summary>Reprocess resource (by slug)</summary>
    public static Task<Result<ResourceUpdated, HttpError<string>>> ReprocessResourceRslugPrefixKbKbidSlugRslugReprocessPost(
        this HttpClient httpClient,
        string kbid, string rslug, bool resetTitle, string xNucliadbUser, object body,
        CancellationToken cancellationToken = default
    ) => _reprocessResourceRslugPrefixKbKbidSlugRslugReprocessPost(httpClient, (kbid, rslug, resetTitle, xNucliadbUser, body), cancellationToken);
    
    /// <summary>Add resource text field (by slug)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AddResourceFieldTextRslugPrefixKbKbidSlugRslugTextFieldIdPut(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldId, TextField body,
        CancellationToken cancellationToken = default
    ) => _addResourceFieldTextRslugPrefixKbKbidSlugRslugTextFieldIdPut(httpClient, ((kbid, rslug, fieldId), body), cancellationToken);
    
    /// <summary>Delete Resource field (by slug)</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteResourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDelete(
        this HttpClient httpClient,
        string kbid, string rslug, FieldTypeName fieldType, string fieldId,
        CancellationToken cancellationToken = default
    ) => _deleteResourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDelete(httpClient, (kbid, rslug, fieldType, fieldId), cancellationToken);
    
    /// <summary>Get Resource field (by slug)</summary>
    public static Task<Result<ResourceField, HttpError<string>>> GetResourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdGet(
        this HttpClient httpClient,
        string kbid, string rslug, FieldTypeName fieldType, string fieldId, List<ResourceFieldProperties> show, List<ExtractedDataTypeName> extracted, object page,
        CancellationToken cancellationToken = default
    ) => _getResourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdGet(httpClient, (kbid, rslug, fieldType, fieldId, show, extracted, page), cancellationToken);
    
    /// <summary>Download extracted binary file (by slug)</summary>
    public static Task<Result<object, HttpError<string>>> DownloadExtractFileRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDownloadExtractedDownloadFieldGet(
        this HttpClient httpClient,
        string kbid, string rslug, FieldTypeName fieldType, string fieldId, string downloadField,
        CancellationToken cancellationToken = default
    ) => _downloadExtractFileRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDownloadExtractedDownloadFieldGet(httpClient, (kbid, rslug, fieldType, fieldId, downloadField), cancellationToken);
    
    /// <summary>Ask a resource (by slug)</summary>
    public static Task<Result<SyncAskResponse, HttpError<string>>> ResourceAskEndpointBySlugKbKbidSlugSlugAskPost(
        this HttpClient httpClient,
        string kbid, string slug, bool xShowConsumption, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, bool xSynchronous, AskRequest body,
        CancellationToken cancellationToken = default
    ) => _resourceAskEndpointBySlugKbKbidSlugSlugAskPost(httpClient, (kbid, slug, xShowConsumption, xNdbClient, xNucliadbUser, xForwardedFor, xSynchronous, body), cancellationToken);
    
    /// <summary>Run Agents on Resource (by slug)</summary>
    public static Task<Result<ResourceAgentsResponse, HttpError<string>>> RunAgentsBySlugKbKbidSlugSlugRunAgentsPost(
        this HttpClient httpClient,
        string kbid, string slug, string xNucliadbUser, ResourceAgentsRequest body,
        CancellationToken cancellationToken = default
    ) => _runAgentsBySlugKbKbidSlugSlugRunAgentsPost(httpClient, (kbid, slug, xNucliadbUser, body), cancellationToken);
    
    /// <summary>Add a split strategy to a KB</summary>
    public static Task<Result<string, HttpError<string>>> AddSplitStrategyKbKbidSplitStrategiesPost(
        this HttpClient httpClient,
        string kbid, SplitConfiguration body,
        CancellationToken cancellationToken = default
    ) => _addSplitStrategyKbKbidSplitStrategiesPost(httpClient, (kbid, body), cancellationToken);
    
    /// <summary>Learning split strategies</summary>
    public static Task<Result<object, HttpError<string>>> GetSplitStrategiesKbKbidSplitStrategiesGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken cancellationToken = default
    ) => _getSplitStrategiesKbKbidSplitStrategiesGet(httpClient, kbid, cancellationToken);
    
    /// <summary>Remove a split strategy from a KB</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteSplitStrategyKbKbidSplitStrategiesStrategyStrategyIdDelete(
        this HttpClient httpClient,
        string kbid, string strategyId,
        CancellationToken cancellationToken = default
    ) => _deleteSplitStrategyKbKbidSplitStrategiesStrategyStrategyIdDelete(httpClient, (kbid, strategyId), cancellationToken);
    
    /// <summary>Extract split configuration</summary>
    public static Task<Result<object, HttpError<string>>> GetSplitStrategyFromIdKbKbidSplitStrategiesStrategyStrategyIdGet(
        this HttpClient httpClient,
        string kbid, string strategyId,
        CancellationToken cancellationToken = default
    ) => _getSplitStrategyFromIdKbKbidSplitStrategiesStrategyStrategyIdGet(httpClient, (kbid, strategyId), cancellationToken);
    
    /// <summary>Suggest on a knowledge box</summary>
    public static Task<Result<KnowledgeboxSuggestResults, HttpError<string>>> SuggestKnowledgeboxKbKbidSuggestGet(
        this HttpClient httpClient,
        string kbid, string query, List<string> fields, List<string> filters, List<string> faceted, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, List<SuggestOptions> features, List<ResourceProperties> show, List<FieldTypeName> fieldType, bool debug, bool highlight, bool showHidden, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor,
        CancellationToken cancellationToken = default
    ) => _suggestKnowledgeboxKbKbidSuggestGet(httpClient, (kbid, query, fields, filters, faceted, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, features, show, fieldType, debug, highlight, showHidden, xNdbClient, xNucliadbUser, xForwardedFor), cancellationToken);
    
    /// <summary>Summarize your documents</summary>
    public static Task<Result<SummarizedResponse, HttpError<string>>> SummarizeEndpointKbKbidSummarizePost(
        this HttpClient httpClient,
        string kbid, bool xShowConsumption, SummarizeRequest body,
        CancellationToken cancellationToken = default
    ) => _summarizeEndpointKbKbidSummarizePost(httpClient, (kbid, xShowConsumption, body), cancellationToken);
    
    /// <summary>Create new upload on a Knowledge Box</summary>
    public static Task<Result<object, HttpError<string>>> TusPostKbKbidTusuploadPost(
        this HttpClient httpClient,
        string kbid, object xExtractStrategy, object xSplitStrategy, object body,
        CancellationToken cancellationToken = default
    ) => _tusPostKbKbidTusuploadPost(httpClient, (kbid, xExtractStrategy, xSplitStrategy, body), cancellationToken);
    
    /// <summary>TUS Server information</summary>
    public static Task<Result<object, HttpError<string>>> TusOptionsKbKbidTusuploadOptions(
        this HttpClient httpClient,
        string kbid, object rid, object rslug, object uploadId, object field,
        CancellationToken cancellationToken = default
    ) => _tusOptionsKbKbidTusuploadOptions(httpClient, (kbid, rid, rslug, uploadId, field), cancellationToken);
    
    /// <summary>Upload data on a Knowledge Box</summary>
    public static Task<Result<object, HttpError<string>>> PatchKbKbidTusuploadUploadIdPatch(
        this HttpClient httpClient,
        string kbid, string uploadId, object body,
        CancellationToken cancellationToken = default
    ) => _patchKbKbidTusuploadUploadIdPatch(httpClient, ((kbid, uploadId), body), cancellationToken);
    
    /// <summary>Upload information</summary>
    public static Task<Result<object, HttpError<string>>> UploadInformationKbKbidTusuploadUploadIdHead(
        this HttpClient httpClient,
        string kbid, string uploadId,
        CancellationToken cancellationToken = default
    ) => _uploadInformationKbKbidTusuploadUploadIdHead(httpClient, (kbid, uploadId), cancellationToken);
    
    /// <summary>Upload binary file on a Knowledge Box</summary>
    public static Task<Result<ResourceFileUploaded, HttpError<string>>> UploadKbKbidUploadPost(
        this HttpClient httpClient,
        string kbid, object xFilename, object xPassword, object xLanguage, object xMd5, object xExtractStrategy, object xSplitStrategy, object body,
        CancellationToken cancellationToken = default
    ) => _uploadKbKbidUploadPost(httpClient, (kbid, xFilename, xPassword, xLanguage, xMd5, xExtractStrategy, xSplitStrategy, body), cancellationToken);

    #endregion

    #region Learning Operations

    /// <summary>Learning Configuration Schema</summary>
    public static Task<Result<object, HttpError<string>>> LearningConfigurationSchemaLearningConfigurationSchemaGet(
        this HttpClient httpClient,
        
        CancellationToken cancellationToken = default
    ) => _learningConfigurationSchemaLearningConfigurationSchemaGet(httpClient, Unit.Value, cancellationToken);

    #endregion

    private static GetAsync<KnowledgeBoxObj, string, string> _getKbBySlugKbSSlugGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeBoxObj, string, string>(
            url: BaseUrl,
            buildRequest: static slug => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/s/{slug}"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeBoxObj>,
            deserializeError: DeserializeError
        );

    private static GetAsync<KnowledgeBoxObj, string, (string kbid, string xNUCLIADBROLES)> _getKbKbKbidGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeBoxObj, string, (string kbid, string xNUCLIADBROLES)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}"), null, new Dictionary<string, string> { ["X-NUCLIADB-ROLES"] = param.xNUCLIADBROLES.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<KnowledgeBoxObj>,
            deserializeError: DeserializeError
        );

    private static PostAsync<SyncAskResponse, string, (string kbid, NucliaDBClientType xNdbClient, bool xShowConsumption, string xNucliadbUser, string xForwardedFor, bool xSynchronous, AskRequest Body)> _askKnowledgeboxEndpointKbKbidAskPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<SyncAskResponse, string, (string kbid, NucliaDBClientType xNdbClient, bool xShowConsumption, string xNucliadbUser, string xForwardedFor, bool xSynchronous, AskRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/ask"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString() ?? string.Empty, ["x-show-consumption"] = param.xShowConsumption.ToString() ?? string.Empty, ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor.ToString() ?? string.Empty, ["x-synchronous"] = param.xSynchronous.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<SyncAskResponse>,
            deserializeError: DeserializeError
        );

    private static GetAsync<KnowledgeboxSearchResults, string, (string kbid, string query, object filterExpression, List<string> filters, List<string> faceted, SortField sortField, object sortLimit, SortOrder sortOrder, int pageNumber, int pageSize, object withStatus, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, object hidden, List<ResourceProperties> show)> _catalogGetKbKbidCatalogGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeboxSearchResults, string, (string kbid, string query, object filterExpression, List<string> filters, List<string> faceted, SortField sortField, object sortLimit, SortOrder sortOrder, int pageNumber, int pageSize, object withStatus, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, object hidden, List<ResourceProperties> show)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/catalog?query={param.query}&filter_expression={param.filterExpression}&filters={param.filters}&faceted={param.faceted}&sort_field={param.sortField}&sort_limit={param.sortLimit}&sort_order={param.sortOrder}&page_number={param.pageNumber}&page_size={param.pageSize}&with_status={param.withStatus}&range_creation_start={param.rangeCreationStart}&range_creation_end={param.rangeCreationEnd}&range_modification_start={param.rangeModificationStart}&range_modification_end={param.rangeModificationEnd}&hidden={param.hidden}&show={param.show}"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeboxSearchResults>,
            deserializeError: DeserializeError
        );

    private static PostAsync<KnowledgeboxSearchResults, string, (string Params, CatalogRequest Body)> _catalogPostKbKbidCatalogPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<KnowledgeboxSearchResults, string, (string Params, CatalogRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/catalog"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<KnowledgeboxSearchResults>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, string> _getConfigurationKbKbidConfigurationGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/configuration"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PatchAsync<object, string, (string Params, object Body)> _patchConfigurationKbKbidConfigurationPatch { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, string, (string Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/configuration"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PostAsync<object, string, (string Params, object Body)> _setConfigurationKbKbidConfigurationPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, (string Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/configuration"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static GetAsync<KnowledgeboxCounters, string, (string kbid, bool debug)> _knowledgeboxCountersKbKbidCountersGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeboxCounters, string, (string kbid, bool debug)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/counters?debug={param.debug}"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeboxCounters>,
            deserializeError: DeserializeError
        );

    private static PutAsync<object, string, (string Params, KnowledgeBoxSynonyms Body)> _setCustomSynonymsKbKbidCustomSynonymsPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<object, string, (string Params, KnowledgeBoxSynonyms Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/custom-synonyms"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static DeleteAsync<Unit, string, string> _deleteCustomSynonymsKbKbidCustomSynonymsDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/custom-synonyms"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );

    private static GetAsync<KnowledgeBoxSynonyms, string, string> _getCustomSynonymsKbKbidCustomSynonymsGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeBoxSynonyms, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/custom-synonyms"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeBoxSynonyms>,
            deserializeError: DeserializeError
        );

    private static PatchAsync<object, string, ((string kbid, string group) Params, UpdateEntitiesGroupPayload Body)> _updateEntitiesGroupKbKbidEntitiesgroupGroupPatch { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, string, ((string kbid, string group) Params, UpdateEntitiesGroupPayload Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/entitiesgroup/{param.Params.group}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static DeleteAsync<Unit, string, (string kbid, string group)> _deleteEntitiesKbKbidEntitiesgroupGroupDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, (string kbid, string group)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/entitiesgroup/{param.group}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );

    private static GetAsync<EntitiesGroup, string, (string kbid, string group)> _getEntityKbKbidEntitiesgroupGroupGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<EntitiesGroup, string, (string kbid, string group)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/entitiesgroup/{param.group}"), null, null),
            deserializeSuccess: DeserializeJson<EntitiesGroup>,
            deserializeError: DeserializeError
        );

    private static PostAsync<object, string, (string Params, CreateEntitiesGroupPayload Body)> _createEntitiesGroupKbKbidEntitiesgroupsPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, (string Params, CreateEntitiesGroupPayload Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/entitiesgroups"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static GetAsync<KnowledgeBoxEntities, string, (string kbid, bool showEntities)> _getEntitiesKbKbidEntitiesgroupsGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeBoxEntities, string, (string kbid, bool showEntities)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/entitiesgroups?show_entities={param.showEntities}"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeBoxEntities>,
            deserializeError: DeserializeError
        );

    private static PostAsync<CreateExportResponse, string, (string Params, object Body)> _startKbExportEndpointKbKbidExportPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<CreateExportResponse, string, (string Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/export"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<CreateExportResponse>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, (string kbid, string exportId)> _downloadExportKbEndpointKbKbidExportExportIdGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string exportId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/export/{param.exportId}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static GetAsync<StatusResponse, string, (string kbid, string exportId)> _getExportStatusEndpointKbKbidExportExportIdStatusGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<StatusResponse, string, (string kbid, string exportId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/export/{param.exportId}/status"), null, null),
            deserializeSuccess: DeserializeJson<StatusResponse>,
            deserializeError: DeserializeError
        );

    private static PostAsync<string, string, (string Params, ExtractConfig Body)> _addStrategyKbKbidExtractStrategiesPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<string, string, (string Params, ExtractConfig Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/extract_strategies"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<string>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, string> _getExtractStrategiesKbKbidExtractStrategiesGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/extract_strategies"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static DeleteAsync<Unit, string, (string kbid, string strategyId)> _deleteStrategyKbKbidExtractStrategiesStrategyStrategyIdDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, (string kbid, string strategyId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/extract_strategies/strategy/{param.strategyId}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, (string kbid, string strategyId)> _getExtractStrategyFromIdKbKbidExtractStrategiesStrategyStrategyIdGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string strategyId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/extract_strategies/strategy/{param.strategyId}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PostAsync<object, string, (string kbid, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, FeedbackRequest Body)> _sendFeedbackEndpointKbKbidFeedbackPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, (string kbid, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, FeedbackRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/feedback"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString() ?? string.Empty, ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static GetAsync<KnowledgeboxFindResults, string, (string kbid, string query, object filterExpression, List<string> fields, List<string> filters, object topK, object minScore, object minScoreSemantic, float minScoreBm25, object vectorset, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, List<FindOptions> features, bool debug, bool highlight, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, bool withDuplicates, bool withSynonyms, bool autofilter, List<string> securityGroups, bool showHidden, RankFusionName rankFusion, object reranker, object searchConfiguration, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor)> _findKnowledgeboxKbKbidFindGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeboxFindResults, string, (string kbid, string query, object filterExpression, List<string> fields, List<string> filters, object topK, object minScore, object minScoreSemantic, float minScoreBm25, object vectorset, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, List<FindOptions> features, bool debug, bool highlight, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, bool withDuplicates, bool withSynonyms, bool autofilter, List<string> securityGroups, bool showHidden, RankFusionName rankFusion, object reranker, object searchConfiguration, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/find?query={param.query}&filter_expression={param.filterExpression}&fields={param.fields}&filters={param.filters}&top_k={param.topK}&min_score={param.minScore}&min_score_semantic={param.minScoreSemantic}&min_score_bm25={param.minScoreBm25}&vectorset={param.vectorset}&range_creation_start={param.rangeCreationStart}&range_creation_end={param.rangeCreationEnd}&range_modification_start={param.rangeModificationStart}&range_modification_end={param.rangeModificationEnd}&features={param.features}&debug={param.debug}&highlight={param.highlight}&show={param.show}&field_type={param.fieldType}&extracted={param.extracted}&with_duplicates={param.withDuplicates}&with_synonyms={param.withSynonyms}&autofilter={param.autofilter}&security_groups={param.securityGroups}&show_hidden={param.showHidden}&rank_fusion={param.rankFusion}&reranker={param.reranker}&search_configuration={param.searchConfiguration}"), null, new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString() ?? string.Empty, ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<KnowledgeboxFindResults>,
            deserializeError: DeserializeError
        );

    private static PostAsync<KnowledgeboxFindResults, string, (string kbid, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, FindRequest Body)> _findPostKnowledgeboxKbKbidFindPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<KnowledgeboxFindResults, string, (string kbid, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, FindRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/find"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString() ?? string.Empty, ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<KnowledgeboxFindResults>,
            deserializeError: DeserializeError
        );

    private static PostAsync<GraphSearchResponse, string, (string kbid, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, GraphSearchRequest Body)> _graphSearchKnowledgeboxKbKbidGraphPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<GraphSearchResponse, string, (string kbid, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, GraphSearchRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/graph"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString() ?? string.Empty, ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<GraphSearchResponse>,
            deserializeError: DeserializeError
        );

    private static PostAsync<GraphNodesSearchResponse, string, (string kbid, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, GraphNodesSearchRequest Body)> _graphNodesSearchKnowledgeboxKbKbidGraphNodesPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<GraphNodesSearchResponse, string, (string kbid, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, GraphNodesSearchRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/graph/nodes"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString() ?? string.Empty, ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<GraphNodesSearchResponse>,
            deserializeError: DeserializeError
        );

    private static PostAsync<GraphRelationsSearchResponse, string, (string kbid, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, GraphRelationsSearchRequest Body)> _graphRelationsSearchKnowledgeboxKbKbidGraphRelationsPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<GraphRelationsSearchResponse, string, (string kbid, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, GraphRelationsSearchRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/graph/relations"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString() ?? string.Empty, ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<GraphRelationsSearchResponse>,
            deserializeError: DeserializeError
        );

    private static PostAsync<CreateImportResponse, string, (string Params, object Body)> _startKbImportEndpointKbKbidImportPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<CreateImportResponse, string, (string Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/import"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<CreateImportResponse>,
            deserializeError: DeserializeError
        );

    private static GetAsync<StatusResponse, string, (string kbid, string importId)> _getImportStatusEndpointKbKbidImportImportIdStatusGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<StatusResponse, string, (string kbid, string importId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/import/{param.importId}/status"), null, null),
            deserializeSuccess: DeserializeJson<StatusResponse>,
            deserializeError: DeserializeError
        );

    private static PostAsync<object, string, ((string kbid, string labelset) Params, LabelSet Body)> _setLabelsetEndpointKbKbidLabelsetLabelsetPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, ((string kbid, string labelset) Params, LabelSet Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/labelset/{param.Params.labelset}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static DeleteAsync<Unit, string, (string kbid, string labelset)> _deleteLabelsetEndpointKbKbidLabelsetLabelsetDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, (string kbid, string labelset)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/labelset/{param.labelset}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );

    private static GetAsync<LabelSet, string, (string kbid, string labelset)> _getLabelsetEndpointKbKbidLabelsetLabelsetGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<LabelSet, string, (string kbid, string labelset)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/labelset/{param.labelset}"), null, null),
            deserializeSuccess: DeserializeJson<LabelSet>,
            deserializeError: DeserializeError
        );

    private static GetAsync<KnowledgeBoxLabels, string, string> _getLabelsetsEndointKbKbidLabelsetsGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeBoxLabels, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/labelsets"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeBoxLabels>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, (string kbid, string modelId)> _getModelKbKbidModelModelIdGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string modelId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/model/{param.modelId}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, string> _getModelsKbKbidModelsGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/models"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, (string kbid, string modelId, string filename)> _downloadModelKbKbidModelsModelIdFilenameGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string modelId, string filename)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/models/{param.modelId}/{param.filename}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, string> _notificationsEndpointKbKbidNotificationsGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/notifications"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PostAsync<object, string, (string kbid, PredictProxiedEndpoints endpoint, string xNucliadbUser, NucliaDBClientType xNdbClient, string xForwardedFor, object Body)> _predictProxyEndpointKbKbidPredictEndpointPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, (string kbid, PredictProxiedEndpoints endpoint, string xNucliadbUser, NucliaDBClientType xNdbClient, string xForwardedFor, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/predict/{param.endpoint}"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty, ["x-ndb-client"] = param.xNdbClient.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, (string kbid, PredictProxiedEndpoints endpoint, string xNucliadbUser, NucliaDBClientType xNdbClient, string xForwardedFor)> _predictProxyEndpointKbKbidPredictEndpointGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, PredictProxiedEndpoints endpoint, string xNucliadbUser, NucliaDBClientType xNdbClient, string xForwardedFor)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/predict/{param.endpoint}"), null, new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty, ["x-ndb-client"] = param.xNdbClient.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static GetAsync<RequestsResults, string, (string kbid, object cursor, object scheduled, int limit)> _processingStatusKbKbidProcessingStatusGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<RequestsResults, string, (string kbid, object cursor, object scheduled, int limit)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/processing-status?cursor={param.cursor}&scheduled={param.scheduled}&limit={param.limit}"), null, null),
            deserializeSuccess: DeserializeJson<RequestsResults>,
            deserializeError: DeserializeError
        );

    private static PostAsync<object, string, (string kbid, string pathRid, string field, object xExtractStrategy, object xSplitStrategy, object Body)> _tusPostRidPrefixKbKbidResourcePathRidFileFieldTusuploadPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, (string kbid, string pathRid, string field, object xExtractStrategy, object xSplitStrategy, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.pathRid}/file/{param.field}/tusupload"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-extract-strategy"] = param.xExtractStrategy.ToString() ?? string.Empty, ["x-split-strategy"] = param.xSplitStrategy.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static HeadAsync<object, string, (string kbid, string pathRid, string field, string uploadId)> _uploadInformationKbKbidResourcePathRidFileFieldTusuploadUploadIdHead { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateHead<object, string, (string kbid, string pathRid, string field, string uploadId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.pathRid}/file/{param.field}/tusupload/{param.uploadId}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PostAsync<ResourceFileUploaded, string, (string kbid, string pathRid, string field, object xFilename, object xPassword, object xLanguage, object xMd5, object xExtractStrategy, object xSplitStrategy, object Body)> _uploadRidPrefixKbKbidResourcePathRidFileFieldUploadPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceFileUploaded, string, (string kbid, string pathRid, string field, object xFilename, object xPassword, object xLanguage, object xMd5, object xExtractStrategy, object xSplitStrategy, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.pathRid}/file/{param.field}/upload"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-filename"] = param.xFilename.ToString() ?? string.Empty, ["x-password"] = param.xPassword.ToString() ?? string.Empty, ["x-language"] = param.xLanguage.ToString() ?? string.Empty, ["x-md5"] = param.xMd5.ToString() ?? string.Empty, ["x-extract-strategy"] = param.xExtractStrategy.ToString() ?? string.Empty, ["x-split-strategy"] = param.xSplitStrategy.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceFileUploaded>,
            deserializeError: DeserializeError
        );

    private static PatchAsync<ResourceUpdated, string, (string kbid, string rid, string xNucliadbUser, bool xSkipStore, UpdateResourcePayload Body)> _modifyResourceRidPrefixKbKbidResourceRidPatch { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<ResourceUpdated, string, (string kbid, string rid, string xNucliadbUser, bool xSkipStore, UpdateResourcePayload Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty, ["x-skip-store"] = param.xSkipStore.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceUpdated>,
            deserializeError: DeserializeError
        );

    private static DeleteAsync<Unit, string, (string kbid, string rid)> _deleteResourceRidPrefixKbKbidResourceRidDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, (string kbid, string rid)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );

    private static GetAsync<NucliadbModelsResourceResource, string, (string kbid, string rid, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, string xNucliadbUser, string xForwardedFor)> _getResourceByUuidKbKbidResourceRidGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<NucliadbModelsResourceResource, string, (string kbid, string rid, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, string xNucliadbUser, string xForwardedFor)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}?show={param.show}&field_type={param.fieldType}&extracted={param.extracted}"), null, new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<NucliadbModelsResourceResource>,
            deserializeError: DeserializeError
        );

    private static PostAsync<SyncAskResponse, string, (string kbid, string rid, bool xShowConsumption, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, bool xSynchronous, AskRequest Body)> _resourceAskEndpointByUuidKbKbidResourceRidAskPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<SyncAskResponse, string, (string kbid, string rid, bool xShowConsumption, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, bool xSynchronous, AskRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/ask"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-show-consumption"] = param.xShowConsumption.ToString() ?? string.Empty, ["x-ndb-client"] = param.xNdbClient.ToString() ?? string.Empty, ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor.ToString() ?? string.Empty, ["x-synchronous"] = param.xSynchronous.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<SyncAskResponse>,
            deserializeError: DeserializeError
        );

    private static PutAsync<ResourceFieldAdded, string, ((string kbid, string rid, string fieldId) Params, InputConversationField Body)> _addResourceFieldConversationRidPrefixKbKbidResourceRidConversationFieldIdPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, ((string kbid, string rid, string fieldId) Params, InputConversationField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/resource/{param.Params.rid}/conversation/{param.Params.fieldId}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, (string kbid, string rid, string fieldId, string messageId, int fileNum)> _downloadFieldConversationAttachmentRidPrefixKbKbidResourceRidConversationFieldIdDownloadFieldMessageIdFileNumGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string rid, string fieldId, string messageId, int fileNum)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/conversation/{param.fieldId}/download/field/{param.messageId}/{param.fileNum}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PutAsync<ResourceFieldAdded, string, ((string kbid, string rid, string fieldId) Params, object Body)> _appendMessagesToConversationFieldRidPrefixKbKbidResourceRidConversationFieldIdMessagesPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, ((string kbid, string rid, string fieldId) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/resource/{param.Params.rid}/conversation/{param.Params.fieldId}/messages"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );

    private static PutAsync<ResourceFieldAdded, string, (string kbid, string rid, string fieldId, bool xSkipStore, FileField Body)> _addResourceFieldFileRidPrefixKbKbidResourceRidFileFieldIdPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, (string kbid, string rid, string fieldId, bool xSkipStore, FileField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/file/{param.fieldId}"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-skip-store"] = param.xSkipStore.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, (string kbid, string rid, string fieldId, bool inline)> _downloadFieldFileRidPrefixKbKbidResourceRidFileFieldIdDownloadFieldGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string rid, string fieldId, bool inline)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/file/{param.fieldId}/download/field?inline={param.inline}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PostAsync<ResourceUpdated, string, (string kbid, string rid, string fieldId, bool resetTitle, string xNucliadbUser, object xFilePassword, object Body)> _reprocessFileFieldKbKbidResourceRidFileFieldIdReprocessPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceUpdated, string, (string kbid, string rid, string fieldId, bool resetTitle, string xNucliadbUser, object xFilePassword, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/file/{param.fieldId}/reprocess?reset_title={param.resetTitle}"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty, ["x-file-password"] = param.xFilePassword.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceUpdated>,
            deserializeError: DeserializeError
        );

    private static PatchAsync<object, string, ((string kbid, string rid, string field, string uploadId) Params, object Body)> _tusPatchRidPrefixKbKbidResourceRidFileFieldTusuploadUploadIdPatch { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, string, ((string kbid, string rid, string field, string uploadId) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/resource/{param.Params.rid}/file/{param.Params.field}/tusupload/{param.Params.uploadId}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PutAsync<ResourceFieldAdded, string, ((string kbid, string rid, string fieldId) Params, LinkField Body)> _addResourceFieldLinkRidPrefixKbKbidResourceRidLinkFieldIdPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, ((string kbid, string rid, string fieldId) Params, LinkField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/resource/{param.Params.rid}/link/{param.Params.fieldId}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );

    private static PostAsync<object, string, (string kbid, string rid, bool reindexVectors, object Body)> _reindexResourceRidPrefixKbKbidResourceRidReindexPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, (string kbid, string rid, bool reindexVectors, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/reindex?reindex_vectors={param.reindexVectors}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PostAsync<ResourceUpdated, string, (string kbid, string rid, bool resetTitle, string xNucliadbUser, object Body)> _reprocessResourceRidPrefixKbKbidResourceRidReprocessPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceUpdated, string, (string kbid, string rid, bool resetTitle, string xNucliadbUser, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/reprocess?reset_title={param.resetTitle}"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceUpdated>,
            deserializeError: DeserializeError
        );

    private static PostAsync<ResourceAgentsResponse, string, (string kbid, string rid, string xNucliadbUser, ResourceAgentsRequest Body)> _runAgentsByUuidKbKbidResourceRidRunAgentsPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceAgentsResponse, string, (string kbid, string rid, string xNucliadbUser, ResourceAgentsRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/run-agents"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceAgentsResponse>,
            deserializeError: DeserializeError
        );

    private static GetAsync<ResourceSearchResults, string, (string kbid, string rid, string query, object filterExpression, List<string> fields, List<string> filters, List<string> faceted, object sortField, SortOrder sortOrder, object topK, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, bool highlight, bool debug, NucliaDBClientType xNdbClient)> _resourceSearchKbKbidResourceRidSearchGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<ResourceSearchResults, string, (string kbid, string rid, string query, object filterExpression, List<string> fields, List<string> filters, List<string> faceted, object sortField, SortOrder sortOrder, object topK, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, bool highlight, bool debug, NucliaDBClientType xNdbClient)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/search?query={param.query}&filter_expression={param.filterExpression}&fields={param.fields}&filters={param.filters}&faceted={param.faceted}&sort_field={param.sortField}&sort_order={param.sortOrder}&top_k={param.topK}&range_creation_start={param.rangeCreationStart}&range_creation_end={param.rangeCreationEnd}&range_modification_start={param.rangeModificationStart}&range_modification_end={param.rangeModificationEnd}&highlight={param.highlight}&debug={param.debug}"), null, new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceSearchResults>,
            deserializeError: DeserializeError
        );

    private static PutAsync<ResourceFieldAdded, string, ((string kbid, string rid, string fieldId) Params, TextField Body)> _addResourceFieldTextRidPrefixKbKbidResourceRidTextFieldIdPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, ((string kbid, string rid, string fieldId) Params, TextField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/resource/{param.Params.rid}/text/{param.Params.fieldId}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );

    private static DeleteAsync<Unit, string, (string kbid, string rid, FieldTypeName fieldType, string fieldId)> _deleteResourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, (string kbid, string rid, FieldTypeName fieldType, string fieldId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/{param.fieldType}/{param.fieldId}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );

    private static GetAsync<ResourceField, string, (string kbid, string rid, FieldTypeName fieldType, string fieldId, List<ResourceFieldProperties> show, List<ExtractedDataTypeName> extracted, object page)> _getResourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<ResourceField, string, (string kbid, string rid, FieldTypeName fieldType, string fieldId, List<ResourceFieldProperties> show, List<ExtractedDataTypeName> extracted, object page)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/{param.fieldType}/{param.fieldId}?show={param.show}&extracted={param.extracted}&page={param.page}"), null, null),
            deserializeSuccess: DeserializeJson<ResourceField>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, (string kbid, string rid, FieldTypeName fieldType, string fieldId, string downloadField)> _downloadExtractFileRidPrefixKbKbidResourceRidFieldTypeFieldIdDownloadExtractedDownloadFieldGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string rid, FieldTypeName fieldType, string fieldId, string downloadField)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/{param.fieldType}/{param.fieldId}/download/extracted/{param.downloadField}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PostAsync<ResourceCreated, string, (string kbid, bool xSkipStore, string xNucliadbUser, CreateResourcePayload Body)> _createResourceKbKbidResourcesPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceCreated, string, (string kbid, bool xSkipStore, string xNucliadbUser, CreateResourcePayload Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resources"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-skip-store"] = param.xSkipStore.ToString() ?? string.Empty, ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceCreated>,
            deserializeError: DeserializeError
        );

    private static GetAsync<ResourceList, string, (string kbid, int page, int size, string xNUCLIADBROLES)> _listResourcesKbKbidResourcesGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<ResourceList, string, (string kbid, int page, int size, string xNUCLIADBROLES)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resources?page={param.page}&size={param.size}"), null, new Dictionary<string, string> { ["X-NUCLIADB-ROLES"] = param.xNUCLIADBROLES.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceList>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, string> _getSchemaForConfigurationUpdatesKbKbidSchemaGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/schema"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static GetAsync<KnowledgeboxSearchResults, string, (string kbid, string query, object filterExpression, List<string> fields, List<string> filters, List<string> faceted, SortField sortField, object sortLimit, SortOrder sortOrder, int topK, object minScore, object minScoreSemantic, float minScoreBm25, object vectorset, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, List<SearchOptions> features, bool debug, bool highlight, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, bool withDuplicates, bool withSynonyms, bool autofilter, List<string> securityGroups, bool showHidden, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor)> _searchKnowledgeboxKbKbidSearchGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeboxSearchResults, string, (string kbid, string query, object filterExpression, List<string> fields, List<string> filters, List<string> faceted, SortField sortField, object sortLimit, SortOrder sortOrder, int topK, object minScore, object minScoreSemantic, float minScoreBm25, object vectorset, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, List<SearchOptions> features, bool debug, bool highlight, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, bool withDuplicates, bool withSynonyms, bool autofilter, List<string> securityGroups, bool showHidden, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/search?query={param.query}&filter_expression={param.filterExpression}&fields={param.fields}&filters={param.filters}&faceted={param.faceted}&sort_field={param.sortField}&sort_limit={param.sortLimit}&sort_order={param.sortOrder}&top_k={param.topK}&min_score={param.minScore}&min_score_semantic={param.minScoreSemantic}&min_score_bm25={param.minScoreBm25}&vectorset={param.vectorset}&range_creation_start={param.rangeCreationStart}&range_creation_end={param.rangeCreationEnd}&range_modification_start={param.rangeModificationStart}&range_modification_end={param.rangeModificationEnd}&features={param.features}&debug={param.debug}&highlight={param.highlight}&show={param.show}&field_type={param.fieldType}&extracted={param.extracted}&with_duplicates={param.withDuplicates}&with_synonyms={param.withSynonyms}&autofilter={param.autofilter}&security_groups={param.securityGroups}&show_hidden={param.showHidden}"), null, new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString() ?? string.Empty, ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<KnowledgeboxSearchResults>,
            deserializeError: DeserializeError
        );

    private static PostAsync<KnowledgeboxSearchResults, string, (string kbid, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, SearchRequest Body)> _searchPostKnowledgeboxKbKbidSearchPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<KnowledgeboxSearchResults, string, (string kbid, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, SearchRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/search"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString() ?? string.Empty, ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<KnowledgeboxSearchResults>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, string> _listSearchConfigurationsKbKbidSearchConfigurationsGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/search_configurations"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PostAsync<object, string, ((string kbid, string configName) Params, object Body)> _createSearchConfigurationKbKbidSearchConfigurationsConfigNamePost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, ((string kbid, string configName) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/search_configurations/{param.Params.configName}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PatchAsync<object, string, ((string kbid, string configName) Params, object Body)> _updateSearchConfigurationKbKbidSearchConfigurationsConfigNamePatch { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, string, ((string kbid, string configName) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/search_configurations/{param.Params.configName}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static DeleteAsync<Unit, string, (string kbid, string configName)> _deleteSearchConfigurationKbKbidSearchConfigurationsConfigNameDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, (string kbid, string configName)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/search_configurations/{param.configName}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, (string kbid, string configName)> _getSearchConfigurationKbKbidSearchConfigurationsConfigNameGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string configName)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/search_configurations/{param.configName}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PatchAsync<ResourceUpdated, string, (string kbid, string rslug, bool xSkipStore, string xNucliadbUser, UpdateResourcePayload Body)> _modifyResourceRslugPrefixKbKbidSlugRslugPatch { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<ResourceUpdated, string, (string kbid, string rslug, bool xSkipStore, string xNucliadbUser, UpdateResourcePayload Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-skip-store"] = param.xSkipStore.ToString() ?? string.Empty, ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceUpdated>,
            deserializeError: DeserializeError
        );

    private static DeleteAsync<Unit, string, (string kbid, string rslug)> _deleteResourceRslugPrefixKbKbidSlugRslugDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, (string kbid, string rslug)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );

    private static GetAsync<NucliadbModelsResourceResource, string, (string kbid, string rslug, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, string xNucliadbUser, string xForwardedFor)> _getResourceBySlugKbKbidSlugRslugGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<NucliadbModelsResourceResource, string, (string kbid, string rslug, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, string xNucliadbUser, string xForwardedFor)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}?show={param.show}&field_type={param.fieldType}&extracted={param.extracted}"), null, new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<NucliadbModelsResourceResource>,
            deserializeError: DeserializeError
        );

    private static PutAsync<ResourceFieldAdded, string, ((string kbid, string rslug, string fieldId) Params, InputConversationField Body)> _addResourceFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, ((string kbid, string rslug, string fieldId) Params, InputConversationField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/slug/{param.Params.rslug}/conversation/{param.Params.fieldId}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, (string kbid, string rslug, string fieldId, string messageId, int fileNum)> _downloadFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdDownloadFieldMessageIdFileNumGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string rslug, string fieldId, string messageId, int fileNum)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/conversation/{param.fieldId}/download/field/{param.messageId}/{param.fileNum}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PutAsync<ResourceFieldAdded, string, ((string kbid, string rslug, string fieldId) Params, object Body)> _appendMessagesToConversationFieldRslugPrefixKbKbidSlugRslugConversationFieldIdMessagesPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, ((string kbid, string rslug, string fieldId) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/slug/{param.Params.rslug}/conversation/{param.Params.fieldId}/messages"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );

    private static PutAsync<ResourceFieldAdded, string, (string kbid, string rslug, string fieldId, bool xSkipStore, FileField Body)> _addResourceFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, (string kbid, string rslug, string fieldId, bool xSkipStore, FileField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/file/{param.fieldId}"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-skip-store"] = param.xSkipStore.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, (string kbid, string rslug, string fieldId, bool inline)> _downloadFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdDownloadFieldGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string rslug, string fieldId, bool inline)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/file/{param.fieldId}/download/field?inline={param.inline}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PostAsync<object, string, (string kbid, string rslug, string field, object xExtractStrategy, object xSplitStrategy, object Body)> _tusPostRslugPrefixKbKbidSlugRslugFileFieldTusuploadPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, (string kbid, string rslug, string field, object xExtractStrategy, object xSplitStrategy, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/file/{param.field}/tusupload"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-extract-strategy"] = param.xExtractStrategy.ToString() ?? string.Empty, ["x-split-strategy"] = param.xSplitStrategy.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PatchAsync<object, string, ((string kbid, string rslug, string field, string uploadId) Params, object Body)> _tusPatchRslugPrefixKbKbidSlugRslugFileFieldTusuploadUploadIdPatch { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, string, ((string kbid, string rslug, string field, string uploadId) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/slug/{param.Params.rslug}/file/{param.Params.field}/tusupload/{param.Params.uploadId}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static HeadAsync<object, string, (string kbid, string rslug, string field, string uploadId)> _uploadInformationKbKbidSlugRslugFileFieldTusuploadUploadIdHead { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateHead<object, string, (string kbid, string rslug, string field, string uploadId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/file/{param.field}/tusupload/{param.uploadId}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PostAsync<ResourceFileUploaded, string, (string kbid, string rslug, string field, object xFilename, object xPassword, object xLanguage, object xMd5, object xExtractStrategy, object xSplitStrategy, object Body)> _uploadRslugPrefixKbKbidSlugRslugFileFieldUploadPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceFileUploaded, string, (string kbid, string rslug, string field, object xFilename, object xPassword, object xLanguage, object xMd5, object xExtractStrategy, object xSplitStrategy, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/file/{param.field}/upload"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-filename"] = param.xFilename.ToString() ?? string.Empty, ["x-password"] = param.xPassword.ToString() ?? string.Empty, ["x-language"] = param.xLanguage.ToString() ?? string.Empty, ["x-md5"] = param.xMd5.ToString() ?? string.Empty, ["x-extract-strategy"] = param.xExtractStrategy.ToString() ?? string.Empty, ["x-split-strategy"] = param.xSplitStrategy.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceFileUploaded>,
            deserializeError: DeserializeError
        );

    private static PutAsync<ResourceFieldAdded, string, ((string kbid, string rslug, string fieldId) Params, LinkField Body)> _addResourceFieldLinkRslugPrefixKbKbidSlugRslugLinkFieldIdPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, ((string kbid, string rslug, string fieldId) Params, LinkField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/slug/{param.Params.rslug}/link/{param.Params.fieldId}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );

    private static PostAsync<object, string, (string kbid, string rslug, bool reindexVectors, object Body)> _reindexResourceRslugPrefixKbKbidSlugRslugReindexPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, (string kbid, string rslug, bool reindexVectors, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/reindex?reindex_vectors={param.reindexVectors}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PostAsync<ResourceUpdated, string, (string kbid, string rslug, bool resetTitle, string xNucliadbUser, object Body)> _reprocessResourceRslugPrefixKbKbidSlugRslugReprocessPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceUpdated, string, (string kbid, string rslug, bool resetTitle, string xNucliadbUser, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/reprocess?reset_title={param.resetTitle}"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceUpdated>,
            deserializeError: DeserializeError
        );

    private static PutAsync<ResourceFieldAdded, string, ((string kbid, string rslug, string fieldId) Params, TextField Body)> _addResourceFieldTextRslugPrefixKbKbidSlugRslugTextFieldIdPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, ((string kbid, string rslug, string fieldId) Params, TextField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/slug/{param.Params.rslug}/text/{param.Params.fieldId}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );

    private static DeleteAsync<Unit, string, (string kbid, string rslug, FieldTypeName fieldType, string fieldId)> _deleteResourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, (string kbid, string rslug, FieldTypeName fieldType, string fieldId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/{param.fieldType}/{param.fieldId}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );

    private static GetAsync<ResourceField, string, (string kbid, string rslug, FieldTypeName fieldType, string fieldId, List<ResourceFieldProperties> show, List<ExtractedDataTypeName> extracted, object page)> _getResourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<ResourceField, string, (string kbid, string rslug, FieldTypeName fieldType, string fieldId, List<ResourceFieldProperties> show, List<ExtractedDataTypeName> extracted, object page)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/{param.fieldType}/{param.fieldId}?show={param.show}&extracted={param.extracted}&page={param.page}"), null, null),
            deserializeSuccess: DeserializeJson<ResourceField>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, (string kbid, string rslug, FieldTypeName fieldType, string fieldId, string downloadField)> _downloadExtractFileRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDownloadExtractedDownloadFieldGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string rslug, FieldTypeName fieldType, string fieldId, string downloadField)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/{param.fieldType}/{param.fieldId}/download/extracted/{param.downloadField}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PostAsync<SyncAskResponse, string, (string kbid, string slug, bool xShowConsumption, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, bool xSynchronous, AskRequest Body)> _resourceAskEndpointBySlugKbKbidSlugSlugAskPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<SyncAskResponse, string, (string kbid, string slug, bool xShowConsumption, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor, bool xSynchronous, AskRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.slug}/ask"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-show-consumption"] = param.xShowConsumption.ToString() ?? string.Empty, ["x-ndb-client"] = param.xNdbClient.ToString() ?? string.Empty, ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor.ToString() ?? string.Empty, ["x-synchronous"] = param.xSynchronous.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<SyncAskResponse>,
            deserializeError: DeserializeError
        );

    private static PostAsync<ResourceAgentsResponse, string, (string kbid, string slug, string xNucliadbUser, ResourceAgentsRequest Body)> _runAgentsBySlugKbKbidSlugSlugRunAgentsPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceAgentsResponse, string, (string kbid, string slug, string xNucliadbUser, ResourceAgentsRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.slug}/run-agents"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceAgentsResponse>,
            deserializeError: DeserializeError
        );

    private static PostAsync<string, string, (string Params, SplitConfiguration Body)> _addSplitStrategyKbKbidSplitStrategiesPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<string, string, (string Params, SplitConfiguration Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/split_strategies"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<string>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, string> _getSplitStrategiesKbKbidSplitStrategiesGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/split_strategies"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static DeleteAsync<Unit, string, (string kbid, string strategyId)> _deleteSplitStrategyKbKbidSplitStrategiesStrategyStrategyIdDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, (string kbid, string strategyId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/split_strategies/strategy/{param.strategyId}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, (string kbid, string strategyId)> _getSplitStrategyFromIdKbKbidSplitStrategiesStrategyStrategyIdGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string strategyId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/split_strategies/strategy/{param.strategyId}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static GetAsync<KnowledgeboxSuggestResults, string, (string kbid, string query, List<string> fields, List<string> filters, List<string> faceted, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, List<SuggestOptions> features, List<ResourceProperties> show, List<FieldTypeName> fieldType, bool debug, bool highlight, bool showHidden, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor)> _suggestKnowledgeboxKbKbidSuggestGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeboxSuggestResults, string, (string kbid, string query, List<string> fields, List<string> filters, List<string> faceted, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, List<SuggestOptions> features, List<ResourceProperties> show, List<FieldTypeName> fieldType, bool debug, bool highlight, bool showHidden, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/suggest?query={param.query}&fields={param.fields}&filters={param.filters}&faceted={param.faceted}&range_creation_start={param.rangeCreationStart}&range_creation_end={param.rangeCreationEnd}&range_modification_start={param.rangeModificationStart}&range_modification_end={param.rangeModificationEnd}&features={param.features}&show={param.show}&field_type={param.fieldType}&debug={param.debug}&highlight={param.highlight}&show_hidden={param.showHidden}"), null, new Dictionary<string, string> { ["x-ndb-client"] = param.xNdbClient.ToString() ?? string.Empty, ["x-nucliadb-user"] = param.xNucliadbUser.ToString() ?? string.Empty, ["x-forwarded-for"] = param.xForwardedFor.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<KnowledgeboxSuggestResults>,
            deserializeError: DeserializeError
        );

    private static PostAsync<SummarizedResponse, string, (string kbid, bool xShowConsumption, SummarizeRequest Body)> _summarizeEndpointKbKbidSummarizePost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<SummarizedResponse, string, (string kbid, bool xShowConsumption, SummarizeRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/summarize"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-show-consumption"] = param.xShowConsumption.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<SummarizedResponse>,
            deserializeError: DeserializeError
        );

    private static PostAsync<object, string, (string kbid, object xExtractStrategy, object xSplitStrategy, object Body)> _tusPostKbKbidTusuploadPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, (string kbid, object xExtractStrategy, object xSplitStrategy, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/tusupload"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-extract-strategy"] = param.xExtractStrategy.ToString() ?? string.Empty, ["x-split-strategy"] = param.xSplitStrategy.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static OptionsAsync<object, string, (string kbid, object rid, object rslug, object uploadId, object field)> _tusOptionsKbKbidTusuploadOptions { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateOptions<object, string, (string kbid, object rid, object rslug, object uploadId, object field)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/tusupload?rid={param.rid}&rslug={param.rslug}&upload_id={param.uploadId}&field={param.field}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PatchAsync<object, string, ((string kbid, string uploadId) Params, object Body)> _patchKbKbidTusuploadUploadIdPatch { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, string, ((string kbid, string uploadId) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params.kbid}/tusupload/{param.Params.uploadId}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static HeadAsync<object, string, (string kbid, string uploadId)> _uploadInformationKbKbidTusuploadUploadIdHead { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateHead<object, string, (string kbid, string uploadId)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/tusupload/{param.uploadId}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PostAsync<ResourceFileUploaded, string, (string kbid, object xFilename, object xPassword, object xLanguage, object xMd5, object xExtractStrategy, object xSplitStrategy, object Body)> _uploadKbKbidUploadPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceFileUploaded, string, (string kbid, object xFilename, object xPassword, object xLanguage, object xMd5, object xExtractStrategy, object xSplitStrategy, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/upload"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["x-filename"] = param.xFilename.ToString() ?? string.Empty, ["x-password"] = param.xPassword.ToString() ?? string.Empty, ["x-language"] = param.xLanguage.ToString() ?? string.Empty, ["x-md5"] = param.xMd5.ToString() ?? string.Empty, ["x-extract-strategy"] = param.xExtractStrategy.ToString() ?? string.Empty, ["x-split-strategy"] = param.xSplitStrategy.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<ResourceFileUploaded>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, string, Unit> _learningConfigurationSchemaLearningConfigurationSchemaGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, Unit>(
            url: BaseUrl,
            buildRequest: static _ => new HttpRequestParts(new RelativeUrl("/api/v1/learning/configuration/schema"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

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