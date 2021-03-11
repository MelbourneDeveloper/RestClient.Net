
#if NET45
using RestClient.Net.Abstractions.Logging;
#else
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
#endif

using RestClient.Net.Abstractions;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using RestClient.Net.Abstractions.Extensions;
using System.Collections.Generic;

namespace RestClient.Net
{
    //TODO: Tests to make sure that null logger is OK

    /// <summary>
    /// Rest client implementation using Microsoft's HttpClient class. 
    /// </summary>
    public sealed class Client : IClient, IDisposable
    {
        #region Fields
        /// <summary>
        /// Logging abstraction that will trace request/response data and log events
        /// </summary>
        private readonly ILogger _logger;

        private static readonly List<HttpRequestMethod> _updateHttpRequestMethods = new List<HttpRequestMethod>
        {
            HttpRequestMethod.Put,
            HttpRequestMethod.Post,
            HttpRequestMethod.Patch
        };

        private readonly SendHttpRequestMessage _sendHttpRequestFunc;

        /// <summary>
        /// Delegate used for getting or creating HttpClient instances when the SendAsync call is made
        /// </summary>
        private readonly CreateHttpClient _createHttpClient;

        /// <summary>
        /// The http client created by the default factory delegate
        /// TODO: We really shouldn't hang on to this....
        /// </summary>
        private readonly HttpClient? _httpClient;

        /// <summary>
        /// Gets the delegate responsible for converting rest requests to http requests
        /// </summary>
        private readonly GetHttpRequestMessage _getHttpRequestMessage;
        #endregion

        #region Public Properties

        /// <summary>
        /// Compresses and decompresses http requests 
        /// </summary>
        public IZip? Zip { get; }

        /// <summary>
        /// Default headers to be sent with http requests
        /// </summary>
        public IHeadersCollection DefaultRequestHeaders { get; }

        /// <summary>
        /// Default timeout for http requests
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <summary>
        /// Adapter for serialization/deserialization of http body data
        /// </summary>
        public ISerializationAdapter SerializationAdapter { get; }

        /// <summary>
        /// Specifies whether or not the client will throw an exception when non-successful status codes are returned in the http response. The default is true
        /// </summary>
        public bool ThrowExceptionOnFailure { get; set; } = true;

        /// <summary>
        /// Base Uri for the client. Any resources specified on requests will be relative to this.
        /// </summary>
        public Uri? BaseUri { get; }

        /// <summary>
        /// Name of the client
        /// </summary>
        public string Name { get; }
        #endregion

        #region Func
        private async Task<HttpResponseMessage> DefaultSendHttpRequestMessageFunc(HttpClient httpClient, GetHttpRequestMessage httpRequestMessageFunc, IRequest request)
        {
            try
            {
                if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

                var httpRequestMessage = httpRequestMessageFunc(request);

                _logger.LogTrace("Attempting to send with the HttpClient. {request}", request);

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, request.CancellationToken).ConfigureAwait(false);

                _logger.LogInformation("SendAsync on HttpClient returned without an exception");

                return httpResponseMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on SendAsync. {request}", request);

                throw;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Construct a client.
        /// </summary>
        /// <param name="serializationAdapter">The serialization adapter for serializing/deserializing http content bodies. Defaults to JSON and adds the default Content-Type header for JSON</param>
        /// <param name="serializationAdapter">The serialization adapter for serializing/deserializing http content bodies. 
#if !NET45
        /// Defaults to JSON and adds the default Content-Type header for JSON</param>
#endif
        /// <param name="name">The of the client instance. This is also passed to the HttpClient factory to get or create HttpClient instances</param>
        /// <param name="baseUri">The base Url for the client. Specify this if the client will be used for one Url only</param>
        /// <param name="defaultRequestHeaders">Default headers to be sent with http requests</param>
        /// <param name="logger">Logging abstraction that will trace request/response data and log events</param>
        /// <param name="createHttpClient">The delegate that is used for getting or creating HttpClient instances when the SendAsync call is made</param>
        /// <param name="sendHttpRequestFunc">The Func responsible for performing the SendAsync method on HttpClient. This can replaced in the constructor in order to implement retries and so on.</param>
        /// <param name="getHttpRequestMessage">Delegate responsible for converting rest requests to http requests</param>
        public Client(
#if NET45
           ISerializationAdapter serializationAdapter,
#else
           ISerializationAdapter? serializationAdapter = null,
#endif
            string? name = null,
            Uri? baseUri = null,
            IHeadersCollection? defaultRequestHeaders = null,
#if NET45
            ILogger? logger = null,
#else
            ILogger<Client>? logger = null,
#endif
            CreateHttpClient? createHttpClient = null,
            SendHttpRequestMessage? sendHttpRequestFunc = null,
            GetHttpRequestMessage? getHttpRequestMessage = null,
            TimeSpan timeout = default,
            IZip? zip = null,
            bool throwExceptionOnFailure = true)
        {
            DefaultRequestHeaders = defaultRequestHeaders ?? new RequestHeadersCollection();

#if NET45
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
#else
            if (serializationAdapter == null)
            {
                SerializationAdapter = new JsonSerializationAdapter();
                this.SetJsonContentTypeHeader();
            }
            else
            {
                SerializationAdapter = serializationAdapter;
            }
#endif
            _logger =
#if !NET45
                (ILogger?)
#endif
                logger ?? NullLogger.Instance;

            if (baseUri != null && !baseUri.ToString().EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                baseUri = new Uri($"{baseUri}/");
            }

            BaseUri = baseUri;

            Name = name ?? Guid.NewGuid().ToString();

            _getHttpRequestMessage = getHttpRequestMessage ?? DefaultGetHttpRequestMessage;

            if (createHttpClient == null)
            {
                _httpClient = new HttpClient();
                _createHttpClient = n => _httpClient;
            }
            else
            {
                _createHttpClient = createHttpClient;
            }

            _sendHttpRequestFunc = sendHttpRequestFunc ?? DefaultSendHttpRequestMessageFunc;

            Timeout = timeout;
            Zip = zip;
            ThrowExceptionOnFailure = throwExceptionOnFailure;
        }

        #endregion

        #region Implementation
        async Task<Response<TResponseBody>> IClient.SendAsync<TResponseBody>(IRequest request)
        {
            //Why do we need to check for null? Nullable is turned on....
            if (request == null) throw new ArgumentNullException(nameof(request));

            HttpResponseMessage httpResponseMessage;
            HttpClient httpClient;

            try
            {
                _logger.LogTrace("Begin send {request}", request);

                httpClient = _createHttpClient(Name);

                _logger.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, request.Resource, message: $"Got HttpClient null: {httpClient == null}"));

                if (httpClient == null) throw new InvalidOperationException("CreateHttpClient returned null");

                //Note: if HttpClient naming is not handled properly, this may alter the HttpClient of another RestClient
                if (httpClient.Timeout != Timeout && Timeout != default) httpClient.Timeout = Timeout;

                _logger.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, request.Resource, message: $"HttpClient configured. Request Null: {request == null} Adapter Null: {SerializationAdapter == null}"));

                if (request == null) throw new ArgumentNullException(nameof(request));

                _logger.LogTrace(_updateHttpRequestMethods.Contains(request.HttpRequestMethod)
                    ? new Trace(request.HttpRequestMethod, TraceEvent.Information, request.Resource, request.BodyData,
                        message: $"Request body serialized")
                    : new Trace(request.HttpRequestMethod, TraceEvent.Information, request.Resource, request.BodyData,
                        message: "No request body to serialize"));

                httpResponseMessage = await _sendHttpRequestFunc(
                    httpClient,
                    _getHttpRequestMessage,
                    request
                    ).ConfigureAwait(false);
            }
            catch (TaskCanceledException tce)
            {
                _logger.LogException(new Trace(
                    request.HttpRequestMethod,
                    TraceEvent.Error,
                    request.Resource,
                    message: $"Exception: {tce}"),
                    tce);

                throw;
            }
            catch (OperationCanceledException oce)
            {
                _logger.LogException(new Trace(
                    request.HttpRequestMethod,
                    TraceEvent.Error,
                    request.Resource,
                    message: $"Exception: {oce}"),
                    oce);

                throw;
            }
            //TODO: Does this need to be handled like this?
            catch (Exception ex)
            {
                var exception = new SendException("HttpClient Send Exception", request, ex);

                _logger.LogException(new Trace(
                request.HttpRequestMethod,
                TraceEvent.Error,
                request.Resource,
                message: $"Exception: {ex}"),
                exception);

                throw exception;
            }

            _logger.LogTrace(new Trace
                (
                    request.HttpRequestMethod,
                    TraceEvent.Request,
                    httpResponseMessage.RequestMessage?.RequestUri,
                    request.BodyData,
                    null,
                    request.Headers,
                    "Request was sent to server"
                ));

            return await ProcessResponseAsync<TResponseBody>(request, httpResponseMessage, httpClient).ConfigureAwait(false);
        }

        private async Task<Response<TResponseBody>> ProcessResponseAsync<TResponseBody>(IRequest request, HttpResponseMessage httpResponseMessage, HttpClient httpClient)
        {
            byte[]? responseData = null;

            if (Zip != null)
            {
                //This is for cases where an unzipping utility needs to be used to unzip the content. This is actually a bug in UWP
                var gzipHeader = httpResponseMessage.Content.Headers.ContentEncoding.FirstOrDefault(h =>
                    !string.IsNullOrEmpty(h) && h.Equals("gzip", StringComparison.OrdinalIgnoreCase));
                if (gzipHeader != null)
                {
                    var bytes = await httpResponseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                    responseData = Zip.Unzip(bytes);
                }
            }

            responseData ??= await httpResponseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

            var httpResponseHeadersCollection = new HttpResponseHeadersCollection(httpResponseMessage.Headers);

            TResponseBody responseBody = default;

            try
            {
                responseBody = SerializationAdapter.Deserialize<TResponseBody>(responseData, httpResponseHeadersCollection);
            }
            catch (Exception ex)
            {
                throw new DeserializationException(Messages.ErrorMessageDeserialization, responseData, this, ex);
            }

            var httpResponseMessageResponse = new HttpResponseMessageResponse<TResponseBody>
            (
                httpResponseHeadersCollection,
                (int)httpResponseMessage.StatusCode,
                request.HttpRequestMethod,
                responseData,
                responseBody,
                httpResponseMessage,
                httpClient
            );

            if (!httpResponseMessageResponse.IsSuccess)
            {
                return !ThrowExceptionOnFailure
                    ? httpResponseMessageResponse
                    : throw new HttpStatusException(
                        Messages.GetErrorMessageNonSuccess(httpResponseMessageResponse.StatusCode,
                        httpResponseMessageResponse.RequestUri),
                        httpResponseMessageResponse,
                        this);
            }

            _logger.LogTrace(new Trace
            (
             request.HttpRequestMethod,
                TraceEvent.Response,
                httpResponseMessage.RequestMessage?.RequestUri,
                responseData,
                (int)httpResponseMessage.StatusCode,
                httpResponseHeadersCollection
            ));

            return httpResponseMessageResponse;
        }

        public void Dispose() => _httpClient?.Dispose();
        #endregion

        #region Private Methods
        private HttpRequestMessage DefaultGetHttpRequestMessage(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            try
            {
                _logger.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, message: "Converting Request to HttpRequestMethod..."));

                var httpMethod = string.IsNullOrEmpty(request.CustomHttpRequestMethod)
                    ? request.HttpRequestMethod switch
                    {
                        HttpRequestMethod.Get => HttpMethod.Get,
                        HttpRequestMethod.Post => HttpMethod.Post,
                        HttpRequestMethod.Put => HttpMethod.Put,
                        HttpRequestMethod.Delete => HttpMethod.Delete,
                        HttpRequestMethod.Patch => new HttpMethod("PATCH"),
                        HttpRequestMethod.Custom => throw new NotImplementedException("CustomHttpRequestMethod must be specified for Custom Http Requests"),
                        _ => throw new NotImplementedException()
                    }
                    : new HttpMethod(request.CustomHttpRequestMethod);

                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = httpMethod,
                    RequestUri = BaseUri != null && request.Resource != null ? new Uri(BaseUri, request.Resource) : BaseUri ?? request.Resource
                };

                ByteArrayContent? httpContent = null;
                if (request.BodyData != null && request.BodyData.Length > 0)
                {
                    httpContent = new ByteArrayContent(request.BodyData);
                    httpRequestMessage.Content = httpContent;
                    _logger.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, message: $"Request content was set. Length: {request.BodyData.Length}"));
                }
                else
                {
                    _logger.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, message: $"No request content setup on HttpRequestMessage"));
                }

                if (request.Headers != null)
                {
                    foreach (var headerName in request.Headers.Names)
                    {
                        if (string.Compare(headerName, MiscExtensions.ContentTypeHeaderName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            //Note: not sure why this is necessary...
                            //The HttpClient class seems to differentiate between content headers and request message headers, but this distinction doesn't exist in the real world...
                            //TODO: Other Content headers
                            httpContent?.Headers.Add(MiscExtensions.ContentTypeHeaderName, request.Headers[headerName]);
                        }
                        else
                        {
                            httpRequestMessage.Headers.Add(headerName, request.Headers[headerName]);
                        }

                        _logger.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, message: $"Header: {headerName} processed"));
                    }
                }

                _logger.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, message: $"Successfully converted"));
                return httpRequestMessage;
            }
            catch (Exception ex)
            {
                _logger.LogException(new Trace(
                request.HttpRequestMethod,
                TraceEvent.Error,
                request.Resource,
                message: $"Exception: {ex}"),
                ex);

                throw;
            }
        }

        #endregion
    }
}