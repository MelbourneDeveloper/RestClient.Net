

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RestClient.Net.Abstractions;
using RestClient.Net.Abstractions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

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

        private static readonly List<HttpRequestMethod> _updateHttpRequestMethods = new()
        {
            HttpRequestMethod.Put,
            HttpRequestMethod.Post,
            HttpRequestMethod.Patch
        };

        private readonly SendHttpRequestMessage sendHttpRequestFunc;

        /// <summary>
        /// Delegate used for getting or creating HttpClient instances when the SendAsync call is made
        /// </summary>
        private readonly CreateHttpClient createHttpClient;

        /// <summary>
        /// The http client created by the default factory delegate
        /// TODO: We really shouldn't hang on to this....
        /// </summary>
        private readonly HttpClient? httpClient;

        /// <summary>
        /// Gets the delegate responsible for converting rest requests to http requests
        /// </summary>
        private readonly GetHttpRequestMessage getHttpRequestMessage;
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
        public bool ThrowExceptionOnFailure { get; } = true;

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

                _logger.LogTrace(Messages.InfoAttemptingToSend, request);

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, request.CancellationToken).ConfigureAwait(false);

                _logger.LogInformation(Messages.InfoSendReturnedNoException);

                return httpResponseMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Messages.ErrorOnSend, request);

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
        /// <param name="throwExceptionOnFailure">Whether or not to throw an exception on non-successful http calls</param>
        public Client(
#if NET45
           ISerializationAdapter serializationAdapter,
#else
           ISerializationAdapter? serializationAdapter = null,
#endif
            string? name = null,
            Uri? baseUri = null,
            IHeadersCollection? defaultRequestHeaders = null,
            ILogger<Client>? logger = null,
            CreateHttpClient? createHttpClient = null,
            SendHttpRequestMessage? sendHttpRequestFunc = null,
            GetHttpRequestMessage? getHttpRequestMessage = null,
            TimeSpan timeout = default,
            IZip? zip = null,
            bool throwExceptionOnFailure = true)
        {
            DefaultRequestHeaders = defaultRequestHeaders ?? NullHeadersCollection.Instance;

#if NET45
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
#else
            if (serializationAdapter == null)
            {
                //Use a shared instance for serialization. There should be no reason that this is not thread safe. Unless it's not.
                SerializationAdapter = JsonSerializationAdapter.Instance;
                DefaultRequestHeaders = DefaultRequestHeaders.SetJsonContentTypeHeader();
            }
            else
            {
                SerializationAdapter = serializationAdapter;
            }
#endif

            _logger = (ILogger?)logger ?? NullLogger.Instance;

            if (baseUri != null && !baseUri.ToString().EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                baseUri = new Uri($"{baseUri}/");
            }

            BaseUri = baseUri;

            Name = name ?? Guid.NewGuid().ToString();

            this.getHttpRequestMessage = getHttpRequestMessage ?? DefaultGetHttpRequestMessage;

            if (createHttpClient == null)
            {
                httpClient = new HttpClient();
                this.createHttpClient = n => httpClient;
            }
            else
            {
                this.createHttpClient = createHttpClient;
            }

            this.sendHttpRequestFunc = sendHttpRequestFunc ?? DefaultSendHttpRequestMessageFunc;

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
                _logger.LogTrace(Messages.TraceBeginSend, request, TraceEvent.Request);

                httpClient = createHttpClient(Name);

                _logger.LogTrace("Got HttpClient null: {httpClientNull}", httpClient == null);

                if (httpClient == null) throw new InvalidOperationException("CreateHttpClient returned null");

                //Note: if HttpClient naming is not handled properly, this may alter the HttpClient of another RestClient
                if (httpClient.Timeout != Timeout && Timeout != default) httpClient.Timeout = Timeout;

                //TODO: DefaultRequestHeaders are not necessarily in sync here...

                _logger.LogTrace("HttpClient configured. Request: {request} Adapter: {serializationAdapter}", request, SerializationAdapter);

                if (request == null) throw new ArgumentNullException(nameof(request));

                _logger.LogTrace(IsUpdate(request.HttpRequestMethod) ? "Request body serialized {bodyData}" : "No request body to serialize", new object[] { request.BodyData ?? new byte[0] });

                httpResponseMessage = await sendHttpRequestFunc(
                    httpClient,
                    getHttpRequestMessage,
                    request
                    ).ConfigureAwait(false);
            }
            catch (TaskCanceledException tce)
            {
                _logger.LogError(tce, Messages.ErrorTaskCancelled, request);
                throw;
            }
            catch (OperationCanceledException oce)
            {
                _logger.LogError(oce, "OperationCanceledException {request}", request);
                throw;
            }
            catch (Exception ex)
            {
                var exception = new SendException(Messages.ErrorSendException, request, ex);
                _logger.LogError(exception, Messages.ErrorSendException, request);
                throw exception;
            }

            _logger.LogTrace("Successful request/response {request}", request);

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

            if (httpResponseMessage.IsSuccessStatusCode)
            {

                try
                {
                    responseBody = SerializationAdapter.Deserialize<TResponseBody>(responseData, httpResponseHeadersCollection);
                }
                catch (Exception ex)
                {
                    throw new DeserializationException(Messages.ErrorMessageDeserialization, responseData, this, ex);
                }
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

            _logger.LogTrace(Messages.TraceResponseProcessed, httpResponseMessageResponse, TraceEvent.Response);

            return httpResponseMessageResponse;
        }

        public void Dispose() => httpClient?.Dispose();
        #endregion

        #region Private Methods
        private static bool IsUpdate(HttpRequestMethod httpRequestMethod) => _updateHttpRequestMethods.Contains(httpRequestMethod);

        private HttpRequestMessage DefaultGetHttpRequestMessage(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            try
            {
                _logger.LogTrace("Converting Request to HttpRequestMethod... Event: {event} Request: {request}", TraceEvent.Information, request);

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
                    _logger.LogTrace("Request content was set. Length: {requestBodyLength}", request.BodyData.Length);
                }
                else
                {
                    _logger.LogTrace("No request content set up on HttpRequestMessage");
                }

                if (request.Headers != null)
                {
                    foreach (var headerName in request.Headers.Names)
                    {
                        if (string.Compare(headerName, HeadersExtensions.ContentTypeHeaderName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            //Note: not sure why this is necessary...
                            //The HttpClient class seems to differentiate between content headers and request message headers, but this distinction doesn't exist in the real world...
                            //TODO: Other Content headers
                            httpContent?.Headers.Add(HeadersExtensions.ContentTypeHeaderName, request.Headers[headerName]);
                        }
                        else
                        {
                            httpRequestMessage.Headers.Add(headerName, request.Headers[headerName]);
                        }
                    }

                    _logger.LogTrace("Headers added to request");
                }

                _logger.LogTrace("Successfully converted IRequest to HttpRequestMessage");

                return httpRequestMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception on DefaultGetHttpRequestMessage Event: {event}", TraceEvent.Request);

                throw;
            }
        }

        #endregion
    }
}