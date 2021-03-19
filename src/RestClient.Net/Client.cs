

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
        #region Internal Fields

        /// <summary>
        /// Delegate used for getting or creating HttpClient instances when the SendAsync call is made
        /// </summary>
        internal readonly CreateHttpClient createHttpClient;

        /// <summary>
        /// Gets the delegate responsible for converting rest requests to http requests
        /// </summary>
        internal readonly IGetHttpRequestMessage getHttpRequestMessage;

        internal readonly ISendHttpRequestMessage sendHttpRequestMessage;

        /// <summary>
        /// Logging abstraction that will trace request/response data and log events
        /// </summary>
        internal readonly ILogger logger;

        #endregion Internal Fields

        #region Private Fields

        /// <summary>
        /// This is an interesting approach to storing minted HttpClients. If the Client is not disposed, this dictionary may pile up and cause memory leaks
        /// TODO: Find a way to store the HttpClient in this class without copying it to cloned clients via createHttpClient
        /// </summary>
        private static readonly Dictionary<Client, HttpClient> HttpClientsByClient = new();

        private bool disposed;

        #endregion Private Fields

        #region Public Constructors
        /// <param name="serializationAdapter">The serialization adapter for serializing/deserializing http content bodies. Defaults to JSON and adds the default Content-Type header for JSON on platforms later than .NET Framework 4.5</param>
        /// <param name="name">The of the client instance. This is also passed to the HttpClient factory to get or create HttpClient instances</param>
        /// <param name="baseUri">The base Url for the client. Specify this if the client will be used for one Url only. This should be an absolute Uri</param>
        /// <param name="defaultRequestHeaders">Default headers to be sent with http requests</param>
        /// <param name="logger">Logging abstraction that will trace request/response data and log events</param>
        /// <param name="createHttpClient">The delegate that is used for getting or creating HttpClient instances when the SendAsync call is made</param>
        /// <param name="sendHttpRequest">The service responsible for performing the SendAsync method on HttpClient. This can replaced in the constructor in order to implement retries and so on.</param>
        /// <param name="getHttpRequestMessage">Service responsible for converting rest requests to http requests</param>
        /// <param name="timeout">Amount of time a request should wait before timing out</param>
        /// <param name="throwExceptionOnFailure">Whether or not to throw an exception on non-successful http calls</param>
        public Client(
#if NET45
            ISerializationAdapter serializationAdapter,
#else
            ISerializationAdapter? serializationAdapter = null,
#endif
            Uri? baseUri = null,
            IHeadersCollection? defaultRequestHeaders = null,
            ILogger<Client>? logger = null,
            CreateHttpClient? createHttpClient = null,
            ISendHttpRequestMessage? sendHttpRequest = null,
            IGetHttpRequestMessage? getHttpRequestMessage = null,
            TimeSpan timeout = default,
            bool throwExceptionOnFailure = true,
            string? name = null)
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

            BaseUri = baseUri;

            Name = name ?? Guid.NewGuid().ToString();

            this.getHttpRequestMessage = getHttpRequestMessage ?? DefaultGetHttpRequestMessage.Instance;

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

            sendHttpRequestMessage = sendHttpRequest ?? DefaultSendHttpRequestMessage.Instance;

            Timeout = timeout;
            ThrowExceptionOnFailure = throwExceptionOnFailure;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Base Uri for the client. Any resources specified on requests will be relative to this.
        /// </summary>
        public Uri? BaseUri { get; }

        /// <summary>
        /// Default headers to be sent with http requests
        /// </summary>
        public IHeadersCollection DefaultRequestHeaders { get; }

        /// <summary>
        /// Name of the client
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Adapter for serialization/deserialization of http body data
        /// </summary>
        public ISerializationAdapter SerializationAdapter { get; }

        /// <summary>
        /// Specifies whether or not the client will throw an exception when non-successful status codes are returned in the http response. The default is true
        /// </summary>
        public bool ThrowExceptionOnFailure { get; } = true;

        /// <summary>
        /// Default timeout for http requests
        /// </summary>
        public TimeSpan Timeout { get; }

        #endregion Public Properties

        #region Public Methods

        public void Dispose()
        {
            if (disposed) return;

            disposed = true;

            if (!HttpClientsByClient.ContainsKey(this)) return;

            HttpClientsByClient[this].Dispose();
            _ = HttpClientsByClient.Remove(this);
        }

        public async Task<Response<TResponseBody>> SendAsync<TResponseBody, TRequestBody>(IRequest<TRequestBody> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            HttpResponseMessage httpResponseMessage;

            try
            {
                logger.LogTrace(Messages.TraceBeginSend, request, TraceEvent.Request);

                var httpClient = createHttpClient(Name);

                logger.LogTrace("Got HttpClient: {httpClient}", httpClient);

                if (httpClient == null) throw new InvalidOperationException("CreateHttpClient returned null");

                if (httpClient.BaseAddress != null)
                {
                    throw new InvalidOperationException($"{nameof(createHttpClient)} returned a {nameof(HttpClient)} with a {nameof(HttpClient.BaseAddress)}. The {nameof(HttpClient)} must never have a {nameof(HttpClient.BaseAddress)}. Fix the {nameof(createHttpClient)} func so that it never creates a {nameof(HttpClient)} with {nameof(HttpClient.BaseAddress)}");
                }

                if (httpClient.DefaultRequestHeaders.Any())
                {
                    throw new InvalidOperationException($"{nameof(createHttpClient)} returned a {nameof(HttpClient)} with at least one item in {nameof(HttpClient.DefaultRequestHeaders)}. The {nameof(HttpClient)} must never have {nameof(HttpClient.DefaultRequestHeaders)}. Fix the {nameof(createHttpClient)} func so that it never creates a {nameof(HttpClient)} with {nameof(HttpClient.DefaultRequestHeaders)}");
                }

                //Note: if HttpClient naming is not handled properly, this may alter the HttpClient of another RestClient
                //TODO: Get rid of this
                if (httpClient.Timeout != Timeout && Timeout != default) httpClient.Timeout = Timeout;

                logger.LogTrace("HttpClient configured. Request: {request} Adapter: {serializationAdapter}", request, SerializationAdapter);

                var requestBodyIsNull = request.BodyData == null;

                logger.LogTrace(!requestBodyIsNull ? "Request body" : "No request body", new object[] { requestBodyIsNull });

                //Note: we do not simply get the HttpRequestMessage here. If we use something like Polly, we may need to send it several times, and you cannot send the same message multiple times
                //This is why we must compose the send func with getHttpRequestMessage

                httpResponseMessage = await sendHttpRequestMessage.SendHttpRequestMessage(
                    httpClient,
                    getHttpRequestMessage,
                    request,
                    logger,
                    SerializationAdapter
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

            return await ProcessResponseAsync<TResponseBody>(request, httpResponseMessage).ConfigureAwait(false);
        }

        #endregion Public Methods

        #region Private Methods

        private async Task<Response<TResponseBody>> ProcessResponseAsync<TResponseBody>(
            IRequest request,
            HttpResponseMessage httpResponseMessage)
        {
            //No idea why this is necessary...
            if (httpResponseMessage == null) throw new ArgumentNullException(nameof(httpResponseMessage));

            var responseData = await httpResponseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

            var httpResponseHeadersCollection = httpResponseMessage.Headers.ToHeadersCollection();
            var contentHeaders = httpResponseMessage.Content?.Headers?.ToHeadersCollection();
            var allHeaders = httpResponseMessage.Headers.ToHeadersCollection().Append(contentHeaders);

            TResponseBody responseBody = default;

            if (httpResponseMessage.IsSuccessStatusCode)
            {

                try
                {
                    responseBody = SerializationAdapter.Deserialize<TResponseBody>(responseData, httpResponseHeadersCollection);
                }
                catch (Exception ex)
                {
                    var deserializationException = new DeserializationException(Messages.ErrorMessageDeserialization, responseData, ex);

                    logger.LogError(deserializationException, Messages.ErrorMessageDeserialization, responseData);

                    throw deserializationException;
                }
            }

            var httpResponseMessageResponse = new Response<TResponseBody>
            (
                allHeaders,
                (int)httpResponseMessage.StatusCode,
                request.HttpRequestMethod,
                responseData,
                responseBody,
                request.Uri
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

        #endregion Private Methods
    }
}