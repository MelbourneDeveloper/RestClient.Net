using RestClientDotNet.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public class RestClient : RestClientBase, IRestClient
    {
        public IResponseProcessor ResponseProcessor { get; }

        public override IRestHeadersCollection DefaultRequestHeaders => ResponseProcessor.Headers;

        public override TimeSpan Timeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public RestClient(ISerializationAdapter serializationAdapter) : this(serializationAdapter, default(Uri))
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter, Uri baseUri) : this(serializationAdapter, baseUri, default(ITracer))
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter, Uri baseUri, ITracer tracer) : this(serializationAdapter, tracer, new SingletonHttpClientFactory(default, baseUri), null)
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter, Uri baseUri, TimeSpan timeout) : this(serializationAdapter, new SingletonHttpClientFactory(timeout, baseUri))
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter, IHttpClientFactory httpClientFactory) : this(serializationAdapter, null, httpClientFactory, null)
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter, IHttpClientFactory httpClientFactory, ITracer tracer) : this(serializationAdapter, tracer, httpClientFactory, null)
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter, ITracer tracer, IHttpClientFactory httpClientFactory, IZip zip) : base(serializationAdapter, tracer)
        {
        }

        async Task<RestResponseBase<TReturn>> IRestClient.SendAsync<TReturn, TBody>(Uri resource, HttpVerb httpVerb, string contentType, TBody body, CancellationToken cancellationToken)
#pragma warning restore CA1033 // Interface methods should be callable by child types
        {
            var response = await ResponseProcessor.ProcessRestResponseAsync<TReturn>(resource, httpVerb);

            if (response.IsSuccess || !ThrowExceptionOnFailure)
            {
                return response;
            }

            throw new HttpStatusException($"{response.StatusCode}.\r\nResource: {resource}", response);
        }
    }
}
