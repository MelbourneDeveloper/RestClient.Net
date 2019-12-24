using RestClientDotNet.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public sealed class RestClient : RestClientBase, IRestClient
    {
        #region Public Properties
        public IResponseProcessor ResponseProcessor { get; }
        public override IRestHeadersCollection DefaultRequestHeaders => ResponseProcessor.DefaultRequestHeaders;
        public override TimeSpan Timeout { get => ResponseProcessor.Timeout; set => ResponseProcessor.Timeout = value; }
        #endregion

        #region Constructors
        public RestClient(
            ISerializationAdapter serializationAdapter)
        : this(
            serializationAdapter,
            default(Uri))
        {
        }

        public RestClient(
            ISerializationAdapter serializationAdapter,
            Uri baseUri)
        : this(
            serializationAdapter,
            baseUri,
            null)
        {
        }

        public RestClient(
            ISerializationAdapter serializationAdapter,
            Uri baseUri,
            TimeSpan timeout)
        : this(
            serializationAdapter,
            new SingletonHttpClientFactory(timeout, baseUri))
        {
        }

        public RestClient(
            ISerializationAdapter serializationAdapter,
            Uri baseUri,
            ITracer tracer)
        : this(
          serializationAdapter,
          new SingletonHttpClientFactory(default, baseUri),
          tracer)
        {
        }

        public RestClient(
            ISerializationAdapter serializationAdapter,
            IHttpClientFactory httpClientFactory)
        : this(
          serializationAdapter,
          httpClientFactory,
          null)
        {
        }

        public RestClient(
            ISerializationAdapter serializationAdapter,
            IHttpClientFactory httpClientFactory,
            ITracer tracer)
        : this(
          serializationAdapter,
          tracer,
          new ResponseProcessor(null, serializationAdapter, tracer, httpClientFactory))

        {
        }

        public RestClient(ISerializationAdapter serializationAdapter,
                          ITracer tracer,
                          IResponseProcessor responseProcessor)
            : base(serializationAdapter, tracer)

        {
            ResponseProcessor = responseProcessor;
        }
        #endregion

        #region Implementation
        async Task<RestResponseBase<TReturn>> IRestClient.SendAsync<TReturn, TBody>(Uri resource, HttpVerb httpVerb, string contentType, TBody body, CancellationToken cancellationToken)
        {
            var response = await ResponseProcessor.SendAsync<TReturn, TBody>(resource, httpVerb, body, contentType, cancellationToken);

            if (response.IsSuccess || !ThrowExceptionOnFailure)
            {
                return response;
            }

            throw new HttpStatusException($"{response.StatusCode}.\r\nResource: {resource}", response);
        }
        #endregion
    }
}
