

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RestClient.Net.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

#if !NET45
using RestClient.Net.Abstractions.Extensions;
#endif

namespace RestClient.Net
{
    //TODO: Tests to make sure that null logger is OK

    /// <summary>
    /// Rest client implementation using Microsoft's HttpClient class. 
    /// </summary>
    public sealed class Client : IClient, IDisposable
    {
        #region Fields
        private bool disposed;

        /// <summary>
        /// Compresses and decompresses http requests 
        /// </summary>
        internal readonly IZip? zip;

        /// <summary>
        /// Logging abstraction that will trace request/response data and log events
        /// </summary>
        internal ILogger logger;

        private static readonly List<HttpRequestMethod> _updateHttpRequestMethods = new()
        {
            HttpRequestMethod.Put,
            HttpRequestMethod.Post,
            HttpRequestMethod.Patch
        };

        internal readonly SendHttpRequestMessage sendHttpRequestFunc;

        /// <summary>
        /// Delegate used for getting or creating HttpClient instances when the SendAsync call is made
        /// </summary>
        internal readonly CreateHttpClient createHttpClient;

        /// <summary>
        /// Gets the delegate responsible for converting rest requests to http requests
        /// </summary>
        internal readonly GetHttpRequestMessage getHttpRequestMessage;
        #endregion

        #region Public Properties

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

            this.logger = (ILogger?)logger ?? NullLogger.Instance;

            if (baseUri != null && !baseUri.ToString().EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                baseUri = new Uri($"{baseUri}/");
            }

            BaseUri = baseUri;

            Name = name ?? Guid.NewGuid().ToString();

            this.getHttpRequestMessage = getHttpRequestMessage ?? ClientExtensions.DefaultGetHttpRequestMessage;

            if (createHttpClient == null)
            {
                if (!HttpClientsByClient.ContainsKey(this))
                {
                    HttpClientsByClient.Add(this, new HttpClient());
                }

                this.createHttpClient = n => HttpClientsByClient[this];
            }
            else
            {
                this.createHttpClient = createHttpClient;
            }

            this.sendHttpRequestFunc = sendHttpRequestFunc ?? ClientExtensions.DefaultSendHttpRequestMessageFunc;

            Timeout = timeout;
            this.zip = zip;
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
                logger.LogTrace(Messages.TraceBeginSend, request, TraceEvent.Request);

                httpClient = createHttpClient(Name);

                logger.LogTrace("Got HttpClient null: {httpClientNull}", httpClient == null);

                if (httpClient == null) throw new InvalidOperationException("CreateHttpClient returned null");

                //Note: if HttpClient naming is not handled properly, this may alter the HttpClient of another RestClient
                if (httpClient.Timeout != Timeout && Timeout != default) httpClient.Timeout = Timeout;

                //TODO: DefaultRequestHeaders are not necessarily in sync here...

                logger.LogTrace("HttpClient configured. Request: {request} Adapter: {serializationAdapter}", request, SerializationAdapter);

                if (request == null) throw new ArgumentNullException(nameof(request));

                logger.LogTrace(IsUpdate(request.HttpRequestMethod) ? "Request body serialized {bodyData}" : "No request body to serialize", new object[] { request.BodyData ?? new byte[0] });

                //Note: we do not simply get the HttpRequestMessage here. If we use something like Polly, we may need to send it several times, and you cannot send the same message multiple times
                //This is why we must compose the send func with getHttpRequestMessage

                httpResponseMessage = await sendHttpRequestFunc(
                    httpClient,
                    getHttpRequestMessage,
                    request,
                    logger,
                    BaseUri
                    ).ConfigureAwait(false);
            }
            catch (TaskCanceledException tce)
            {
                logger.LogError(tce, Messages.ErrorTaskCancelled, request);
                throw;
            }
            catch (OperationCanceledException oce)
            {
                logger.LogError(oce, "OperationCanceledException {request}", request);
                throw;
            }
            catch (Exception ex)
            {
                var exception = new SendException(Messages.ErrorSendException, request, ex);
                logger.LogError(exception, Messages.ErrorSendException, request);
                throw exception;
            }

            logger.LogTrace("Successful request/response {request}", request);

            return await ProcessResponseAsync<TResponseBody>(request, httpResponseMessage, httpClient).ConfigureAwait(false);
        }

        private async Task<Response<TResponseBody>> ProcessResponseAsync<TResponseBody>(IRequest request, HttpResponseMessage httpResponseMessage, HttpClient httpClient)
        {
            byte[]? responseData = null;

            if (zip != null)
            {
                //This is for cases where an unzipping utility needs to be used to unzip the content. This is actually a bug in UWP
                var gzipHeader = httpResponseMessage.Content.Headers.ContentEncoding.FirstOrDefault(h =>
                    !string.IsNullOrEmpty(h) && h.Equals("gzip", StringComparison.OrdinalIgnoreCase));
                if (gzipHeader != null)
                {
                    var bytes = await httpResponseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                    responseData = zip.Unzip(bytes);
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
                    throw new DeserializationException(Messages.ErrorMessageDeserialization, responseData, ex);
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
                        httpResponseMessageResponse);
            }

            logger.LogTrace(Messages.TraceResponseProcessed, httpResponseMessageResponse, TraceEvent.Response);

            return httpResponseMessageResponse;
        }

        public void Dispose()
        {
            if (disposed) return;

            disposed = true;

            if (HttpClientsByClient.ContainsKey(this))
            {
                HttpClientsByClient[this].Dispose();
                _ = HttpClientsByClient.Remove(this);
            }
        }
        #endregion

        #region Private Methods
        private static bool IsUpdate(HttpRequestMethod httpRequestMethod) => _updateHttpRequestMethods.Contains(httpRequestMethod);

        /// <summary>
        /// This is an interesting approach to storing minted HttpClients. If the Client is not disposed, this dictionary may pile up and cause memory leaks
        /// TODO: Find a way to store the HttpClient in this class without copying it to cloned clients via createHttpClient
        /// </summary>
        private static readonly Dictionary<Client, HttpClient> HttpClientsByClient = new Dictionary<Client, HttpClient>();
        #endregion
    }
}