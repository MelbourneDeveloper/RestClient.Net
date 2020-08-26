
#if NET45
using RestClient.Net.Abstractions.Logging;
#else
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


    public delegate Task<HttpResponseMessage> SendHttpRequestMessage(HttpClient httpClient, GetHttpRequestMessage httpRequestMessageFunc, IRequest request);

    /// <summary>
    /// Rest client implementation using Microsoft's HttpClient class. 
    /// </summary>
    public sealed class Client : IClient, IDisposable
    {
        #region Fields
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
        public IZip? Zip { get; set; }

        /// <summary>
        /// Default headers to be sent with http requests
        /// </summary>
        public IHeadersCollection DefaultRequestHeaders { get; }

        /// <summary>
        /// Default timeout for http requests
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Adapter for serialization/deserialization of http body data
        /// </summary>
        public ISerializationAdapter SerializationAdapter { get; }

        /// <summary>
        /// Logging abstraction that will trace request/response data and log events
        /// </summary>
        public ILogger? Logger { get; }

        /// <summary>
        /// Specifies whether or not the client will throw an exception when non-successful status codes are returned in the http response. The default is true
        /// </summary>
        public bool ThrowExceptionOnFailure { get; set; } = true;

        /// <summary>
        /// Base Uri for the client. Any resources specified on requests will be relative to this.
        /// </summary>
        public Uri? BaseUri { get; set; }

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
                var httpRequestMessage = httpRequestMessageFunc(request);

                Logger?.LogTrace(new Trace(HttpRequestMethod.Custom, TraceEvent.Information, message: $"Attempting to send with the HttpClient. HttpClient Null: {httpClient == null}"));

                if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, request.CancellationToken);

                Logger?.LogTrace(new Trace(HttpRequestMethod.Custom, TraceEvent.Information, message: $"SendAsync on HttpClient returned without an exception"));

                return httpResponseMessage;
            }
            catch (Exception ex)
            {
                Logger?.LogException(new Trace(
                HttpRequestMethod.Custom,
                TraceEvent.Error,
                null,
                message: $"Exception: {ex}"),
                ex);

                throw;
            }
        }
        #endregion

        #region Constructors

#if !NET45
        /// <summary>
        /// Construct a client
        /// </summary>
        /// <param name="baseUri">The base Url for the client. Specify this if the client will be used for one Url only</param>
        public Client(
            Uri baseUri)
        : this(
            null,
            null,
            baseUri)
        {
        }
#endif

        /// <summary>
        /// Construct a client
        /// </summary>
        /// <param name="serializationAdapter">The serialization adapter for serializing/deserializing http content bodies</param>
        public Client(
            ISerializationAdapter serializationAdapter)
        : this(
            serializationAdapter,
            default)
        {
        }

        /// <summary>
        /// Construct a client
        /// </summary>
        /// <param name="serializationAdapter">The serialization adapter for serializing/deserializing http content bodies</param>
        /// <param name="baseUri">The base Url for the client. Specify this if the client will be used for one Url only</param>
        public Client(
            ISerializationAdapter serializationAdapter,
            Uri? baseUri)
        : this(
            serializationAdapter,
            null,
            baseUri)
        {
        }

        /// <summary>
        /// Construct a client.
        /// </summary>
        /// <param name="serializationAdapter">The serialization adapter for serializing/deserializing http content bodies</param>
        /// <param name="logger"></param>
        /// <param name="createHttpClient"></param>
        public Client(
            ISerializationAdapter serializationAdapter,
            ILogger logger,
            CreateHttpClient createHttpClient)
        : this(
            serializationAdapter,
            null,
            null,
            logger: logger,
            createHttpClient: createHttpClient)
        {
        }

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
            ILogger? logger = null,
            CreateHttpClient? createHttpClient = null,
            SendHttpRequestMessage? sendHttpRequestFunc = null,
            GetHttpRequestMessage? getHttpRequestMessage = null)
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
            Logger = logger;
            BaseUri = baseUri;
            Name = name ?? Guid.NewGuid().ToString();

            _getHttpRequestMessage = getHttpRequestMessage ?? GetHttpRequestMessage;

            if (createHttpClient == null)
            {
                _httpClient = new HttpClient { BaseAddress = baseUri };
                _createHttpClient = (n) => _httpClient;
            }
            else
            {
                _createHttpClient = createHttpClient;
            }

            _sendHttpRequestFunc = sendHttpRequestFunc ?? DefaultSendHttpRequestMessageFunc;
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
                Logger?.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, request.Resource, message: $"Begin send"));

                httpClient = _createHttpClient(Name);

                Logger?.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, request.Resource, message: $"Got HttpClient null: {httpClient == null}"));

                if (httpClient == null) throw new InvalidOperationException("CreateHttpClient returned null");

                //Note: if HttpClient naming is not handled properly, this may alter the HttpClient of another RestClient
                if (httpClient.Timeout != Timeout && Timeout != default) httpClient.Timeout = Timeout;
                if (httpClient.BaseAddress != BaseUri && BaseUri != null) httpClient.BaseAddress = BaseUri;

                Logger?.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, request.Resource, message: $"HttpClient configured. Request Null: {request == null} Adapter Null: {SerializationAdapter == null}"));

                if (request == null) throw new ArgumentNullException(nameof(request));

                if (_updateHttpRequestMethods.Contains(request.HttpRequestMethod))
                {
                    Logger?.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, request.Resource, request.BodyData, message: $"Request body serialized"));
                }
                else
                {
                    Logger?.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, request.Resource, request.BodyData, message: $"No request body to serialize"));
                }

                httpResponseMessage = await _sendHttpRequestFunc(
                    httpClient,
                    _getHttpRequestMessage,
                    request
                    );
            }
            catch (TaskCanceledException tce)
            {
                Logger?.LogException(new Trace(
                    request.HttpRequestMethod,
                    TraceEvent.Error,
                    request.Resource,
                    message: $"Exception: {tce}"),
                    tce);

                throw;
            }
            catch (OperationCanceledException oce)
            {
                Logger?.LogException(new Trace(
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

                Logger?.LogException(new Trace(
                request.HttpRequestMethod,
                TraceEvent.Error,
                request.Resource,
                message: $"Exception: {ex}"),
                exception);

                throw exception;
            }

            Logger?.LogTrace(new Trace
                (
                    request.HttpRequestMethod,
                    TraceEvent.Request,
                    httpResponseMessage.RequestMessage?.RequestUri,
                    request.BodyData,
                    null,
                    request.Headers,
                    "Request was sent to server"
                ));

            return await ProcessResponseAsync<TResponseBody>(request, httpResponseMessage, httpClient);
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
                    var bytes = await httpResponseMessage.Content.ReadAsByteArrayAsync();
                    responseData = Zip.Unzip(bytes);
                }
            }

            if (responseData == null)
            {
                responseData = await httpResponseMessage.Content.ReadAsByteArrayAsync();
            }

            var httpResponseHeadersCollection = new HttpResponseHeadersCollection(httpResponseMessage.Headers);

            //🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮🤮
#nullable disable
            var httpResponseMessageResponse = new HttpResponseMessageResponse<TResponseBody>
            (
                httpResponseHeadersCollection,
                (int)httpResponseMessage.StatusCode,
                request.HttpRequestMethod,
                responseData,
                default,
                httpResponseMessage,
                httpClient
            );
#nullable enable

            if (!httpResponseMessageResponse.IsSuccess)
            {
                if (!ThrowExceptionOnFailure)
                {
                    return httpResponseMessageResponse;
                }

                throw new HttpStatusException(Messages.GetErrorMessageNonSuccess(httpResponseMessageResponse.StatusCode, httpResponseMessageResponse.RequestUri), httpResponseMessageResponse, this);
            }

            try
            {
                httpResponseMessageResponse.Body = SerializationAdapter.Deserialize<TResponseBody>(httpResponseMessageResponse);
            }
            catch (Exception ex)
            {
                throw new DeserializationException(Messages.ErrorMessageDeserialization, responseData, this, ex);
            }

            Logger?.LogTrace(new Trace
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
        private HttpRequestMessage GetHttpRequestMessage(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            try
            {
                Logger?.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, message: "Converting Request to HttpRequestMethod..."));

                var httpMethod = string.IsNullOrEmpty(request.CustomHttpRequestMethod)
                    ? (request.HttpRequestMethod switch
                    {
                        HttpRequestMethod.Get => HttpMethod.Get,
                        HttpRequestMethod.Post => HttpMethod.Post,
                        HttpRequestMethod.Put => HttpMethod.Put,
                        HttpRequestMethod.Delete => HttpMethod.Delete,
                        HttpRequestMethod.Patch => new HttpMethod("PATCH"),
                        HttpRequestMethod.Custom => throw new NotImplementedException("CustomHttpRequestMethod must be specified for Custom Http Requests"),
                        _ => throw new NotImplementedException(),
                    })
                    : new HttpMethod(request.CustomHttpRequestMethod);

                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = httpMethod,
                    RequestUri = request.Resource
                };

                ByteArrayContent? httpContent = null;
                if (request.BodyData != null && request.BodyData.Length > 0)
                {
                    httpContent = new ByteArrayContent(request.BodyData);
                    httpRequestMessage.Content = httpContent;
                    Logger?.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, message: $"Request content was set. Length: {request.BodyData.Length}"));
                }
                else
                {
                    Logger?.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, message: $"No request content setup on HttpRequestMessage"));
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

                        Logger?.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, message: $"Header: {headerName} processed"));
                    }
                }

                Logger?.LogTrace(new Trace(request.HttpRequestMethod, TraceEvent.Information, message: $"Successfully converted"));
                return httpRequestMessage;
            }
            catch (Exception ex)
            {
                Logger?.LogException(new Trace(
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