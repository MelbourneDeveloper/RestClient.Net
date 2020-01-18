
#if NET45
using RestClient.Net.Abstractions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

#if NETCOREAPP3_0
using RestClient.Net.Abstractions.Extensions;
#endif

using RestClient.Net.Abstractions;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CA2000

namespace RestClient.Net
{
    /// <summary>
    /// Rest client implementation using Microsoft's HttpClient class. 
    /// </summary>
    public sealed class Client : IClient
    {
        #region Fields
        private readonly Func<HttpClient, Func<HttpRequestMessage>, CancellationToken, Task<HttpResponseMessage>> _sendHttpRequestFunc;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the current IHttpClientFactory instance that is used for getting or creating HttpClient instances when the SendAsync call is made
        /// </summary>
        public IHttpClientFactory HttpClientFactory { get; }

        /// <summary>
        /// Compresses and decompresses http requests 
        /// </summary>
        public IZip Zip { get; set; }

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
        public ILogger Logger { get; }

        /// <summary>
        /// Specifies whether or not the client will throw an exception when non-successful status codes are returned in the http response. The default is true
        /// </summary>
        public bool ThrowExceptionOnFailure { get; set; } = true;

        /// <summary>
        /// Base Uri for the client. Any resources specified on requests will be relative to this.
        /// </summary>
        public Uri BaseUri { get; set; }

        /// <summary>
        /// Name of the client
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the current IRequestConverter instance responsible for converting rest requests to http requests
        /// </summary>
        public IRequestConverter RequestConverter { get; }
        #endregion

        #region Func
        private static readonly Func<HttpClient, Func<HttpRequestMessage>, CancellationToken, Task<HttpResponseMessage>> DefaultSendHttpRequestMessageFunc = (httpClient, httpRequestMessageFunc, cancellationToken) =>
        {
            var httpRequestMessage = httpRequestMessageFunc.Invoke();
            return httpClient.SendAsync(httpRequestMessage, cancellationToken);
        };
        #endregion

        #region Constructors

#if NETCOREAPP3_0
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
            Uri baseUri)
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
        /// <param name="httpClientFactory"></param>
        public Client(
            ISerializationAdapter serializationAdapter,
            ILogger logger,
            IHttpClientFactory httpClientFactory)
        : this(
            serializationAdapter,
            null,
            null,
            logger: logger,
            httpClientFactory: httpClientFactory)
        {
        }

        /// <summary>
        /// Construct a client.
        /// </summary>
        /// <param name="serializationAdapter">The serialization adapter for serializing/deserializing http content bodies. Defaults to JSON and adds the default Content-Type header for JSON</param>
        /// <param name="serializationAdapter">The serialization adapter for serializing/deserializing http content bodies. 
#if NETCOREAPP3_0
        /// Defaults to JSON and adds the default Content-Type header for JSON</param>
#endif
        /// <param name="name">The of the client instance. This is also passed to the HttpClient factory to get or create HttpClient instances</param>
        /// <param name="baseUri">The base Url for the client. Specify this if the client will be used for one Url only</param>
        /// <param name="defaultRequestHeaders">Default headers to be sent with http requests</param>
        /// <param name="logger">Logging abstraction that will trace request/response data and log events</param>
        /// <param name="httpClientFactory">The IHttpClientFactory instance that is used for getting or creating HttpClient instances when the SendAsync call is made</param>
        /// <param name="sendHttpRequestFunc">The Func responsible for performing the SendAsync method on HttpClient. This can replaced in the constructor in order to implement retries and so on.</param>
        /// <param name="requestConverter">IRequestConverter instance responsible for converting rest requests to http requests</param>
        public Client(
#if !NETCOREAPP3_0
           ISerializationAdapter serializationAdapter,
#else
           ISerializationAdapter serializationAdapter = null,
#endif
            string name = null,
            Uri baseUri = null,
            IHeadersCollection defaultRequestHeaders = null,
            ILogger logger = null,
            IHttpClientFactory httpClientFactory = null,
            Func<HttpClient, Func<HttpRequestMessage>, CancellationToken, Task<HttpResponseMessage>> sendHttpRequestFunc = null,
            IRequestConverter requestConverter = null)
        {
            DefaultRequestHeaders = defaultRequestHeaders ?? new RequestHeadersCollection();

#if !NETCOREAPP3_0
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
            Name = name ?? "RestClient";
            RequestConverter = requestConverter ?? new DefaultRequestConverter();
            HttpClientFactory = httpClientFactory ?? new DefaultHttpClientFactory();
            _sendHttpRequestFunc = sendHttpRequestFunc ?? DefaultSendHttpRequestMessageFunc;
        }

        #endregion

        #region Implementation
        async Task<Response<TResponseBody>> IClient.SendAsync<TResponseBody, TRequestBody>(Request<TRequestBody> request)
        {
            var httpClient = HttpClientFactory.CreateClient(Name);

            //Note: if HttpClient naming is not handled properly, this may alter the HttpClient of another RestClient
            if (httpClient.Timeout != Timeout && Timeout != default) httpClient.Timeout = Timeout;
            if (httpClient.BaseAddress != BaseUri && BaseUri != null) httpClient.BaseAddress = BaseUri;

            byte[] requestBodyData = null;

            if (DefaultRequestConverter.UpdateHttpRequestMethods.Contains(request.HttpRequestMethod))
            {
                requestBodyData = SerializationAdapter.Serialize(request.Body, request.Headers);
            }

            HttpResponseMessage httpResponseMessage;
            try
            {
                httpResponseMessage = await _sendHttpRequestFunc.Invoke(
                    httpClient,
                    () => RequestConverter.GetHttpRequestMessage(request, requestBodyData),
                    request.CancellationToken
                    );

                //var httpRequestMessage = RequestConverter.GetHttpRequestMessage(request, requestBodyData);
                //httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, request.CancellationToken);


            }
            catch (TaskCanceledException tce)
            {
                Log(LogLevel.Error, null, tce);
                throw;
            }
            catch (OperationCanceledException oce)
            {
                Log(LogLevel.Error, null, oce);
                throw;
            }
            //TODO: Does this need to be handled like this?
            catch (Exception ex)
            {
                var exception = new SendException<TRequestBody>("HttpClient Send Exception", request, ex);
                Log(LogLevel.Error, null, exception);
                throw exception;
            }

            Log(LogLevel.Trace, new Trace
                (
                 request.HttpRequestMethod,
                 httpResponseMessage.RequestMessage.RequestUri,
                 requestBodyData,
                 TraceEvent.Request,
                 null,
                 request.Headers
                ), null);

            return await ProcessResponseAsync<TResponseBody, TRequestBody>(request, httpResponseMessage, httpClient);
        }

        private async Task<Response<TResponseBody>> ProcessResponseAsync<TResponseBody, TRequestBody>(Request<TRequestBody> request, HttpResponseMessage httpResponseMessage, HttpClient httpClient)
        {
            byte[] responseData = null;

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

            TResponseBody responseBody;
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

            Log(LogLevel.Trace, new Trace
            (
             request.HttpRequestMethod,
             httpResponseMessage.RequestMessage.RequestUri,
             responseData,
             TraceEvent.Response,
             (int)httpResponseMessage.StatusCode,
             httpResponseHeadersCollection
            ), null);

            if (httpResponseMessageResponse.IsSuccess || !ThrowExceptionOnFailure)
            {
                return httpResponseMessageResponse;
            }

            throw new HttpStatusException($"Non successful Http Status Code: {httpResponseMessageResponse.StatusCode}.\r\nRequest Uri: {httpResponseMessage.RequestMessage.RequestUri}", httpResponseMessageResponse, this);
        }
        #endregion

        #region Private Methods
        private void Log(LogLevel loglevel, Trace restTrace, Exception exception)
        {
            Logger?.Log(loglevel,
                restTrace != null ?
                new EventId((int)restTrace.RestEvent, restTrace.RestEvent.ToString()) :
                new EventId((int)TraceEvent.Error, TraceEvent.Error.ToString()),
                restTrace, exception, null);
        }
        #endregion
    }
}

#pragma warning restore CA2000