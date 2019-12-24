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

        #region Explicit Implementation
#pragma warning disable CA1033 // Interface methods should be callable by child types
        async Task<RestResponseBase<TReturn>> IRestClient.SendAsync<TReturn, TBody>(Uri resource, HttpVerb httpVerb, string contentType, TBody body, CancellationToken cancellationToken)
#pragma warning restore CA1033 // Interface methods should be callable by child types
        {
            var responseProcessor = await ResponseProcessorFactory.CreateResponseProcessorAsync
                (
                httpVerb,
                BaseUri,
                resource,
                body,
                contentType,
                cancellationToken);


            var response = await responseProcessor.ProcessRestResponseAsync<TReturn>(BaseUri, resource, httpVerb);

            if (response.IsSuccess || !ThrowExceptionOnFailure)
            {
                return response;
            }


            throw new HttpStatusException(
                $"{responseProcessor.StatusCode}.\r\nBase Uri: {BaseUri}. Resource: {resource}", response);

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
