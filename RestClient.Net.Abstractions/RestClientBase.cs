using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet.Abstractions
{
    public class RestClientBase : IDisposable, IRestClient
    {
        #region Fields
        private bool disposed;
        #endregion

        #region Public Properties
        public IRestHeadersCollection DefaultRequestHeaders => ResponseProcessorFactory.DefaultRequestHeaders;
        public IResponseProcessorFactory ResponseProcessorFactory { get; }
        public bool ThrowExceptionOnFailure { get; set; } = true;
        public string DefaultContentType { get; set; } = "application/json";
        public Uri BaseUri => ResponseProcessorFactory.BaseAddress;
        public ISerializationAdapter SerializationAdapter { get; }
        public TimeSpan Timeout
        {
            get => ResponseProcessorFactory.Timeout;
            set => ResponseProcessorFactory.Timeout = value;
        }
        public ITracer Tracer { get; }
        #endregion

        #region Constructor
        public RestClientBase(ISerializationAdapter serializationAdapter, IResponseProcessorFactory responseProcessorFactory, ITracer tracer)
        {
            ResponseProcessorFactory = responseProcessorFactory;
            SerializationAdapter = serializationAdapter;
            Tracer = tracer;
        }
        #endregion

        #region Private Methods
#pragma warning disable CA1033 // Interface methods should be callable by child types
        Task<RestResponse<TReturn>> IRestClient.SendAsync<TReturn, TBody>(Uri queryString, HttpVerb httpVerb, string contentType, TBody body, CancellationToken cancellationToken)
#pragma warning restore CA1033 // Interface methods should be callable by child types
        {
            return Call<TReturn, TBody>(queryString, httpVerb, contentType, body, cancellationToken);
        }

        private async Task<RestResponse<TReturn>> Call<TReturn, TBody>(Uri queryString, HttpVerb httpVerb, string contentType, TBody body, CancellationToken cancellationToken)
        {
            var responseProcessor = await ResponseProcessorFactory.CreateResponseProcessorAsync
                (
                httpVerb,
                BaseUri,
                queryString,
                body,
                contentType,
                cancellationToken);

            if (responseProcessor.IsSuccess)
            {
                return await responseProcessor.ProcessRestResponseAsync<TReturn>(BaseUri, queryString, httpVerb);
            }

            var errorResponse = new RestResponse<TReturn>(
                default,
                responseProcessor.Headers,
                responseProcessor.StatusCode,
                responseProcessor,
                BaseUri,
                queryString,
                httpVerb
                );

            if (ThrowExceptionOnFailure)
            {
                throw new HttpStatusException(
                    $"{responseProcessor.StatusCode}.\r\nBase Uri: {BaseUri}. Querystring: {queryString}", errorResponse);
            }

            return errorResponse;
        }
        #endregion

        #region Public Methods
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            GC.SuppressFinalize(this);

            ResponseProcessorFactory.Dispose();
        }
        #endregion
    }
}
