using RestClientDotNet.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public sealed class RestClient : RestClientBase, IRestClient
    {
        public IResponseProcessor ResponseProcessor { get; }

        public override IRestHeadersCollection DefaultRequestHeaders => ResponseProcessor.DefaultRequestHeaders;

        public override TimeSpan Timeout { get => ResponseProcessor.Timeout; set => ResponseProcessor.Timeout = value; }

        public RestClient(ISerializationAdapter serializationAdapter) : this(serializationAdapter, default(Uri))
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter, Uri baseUri) : this(serializationAdapter, baseUri, default(ITracer))
        {
        }

#pragma warning disable CA2000 // Dispose objects before losing scope
#pragma warning disable IDE0067 // Dispose objects before losing scope
        public RestClient(ISerializationAdapter serializationAdapter, Uri baseUri, ITracer tracer) : this(serializationAdapter, tracer, new SingletonHttpClientFactory(default, baseUri), null)
#pragma warning restore IDE0067 // Dispose objects before losing scope
#pragma warning restore CA2000 // Dispose objects before losing scope
        {
        }

#pragma warning disable CA2000 // Dispose objects before losing scope
#pragma warning disable IDE0067 // Dispose objects before losing scope
        public RestClient(ISerializationAdapter serializationAdapter, Uri baseUri, TimeSpan timeout) : this(serializationAdapter, new SingletonHttpClientFactory(timeout, baseUri))
#pragma warning restore IDE0067 // Dispose objects before losing scope
#pragma warning restore CA2000 // Dispose objects before losing scope
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter, IHttpClientFactory httpClientFactory) : this(serializationAdapter, null, httpClientFactory, null)
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter, IHttpClientFactory httpClientFactory, ITracer tracer) : this(serializationAdapter, tracer, httpClientFactory, null)
        {
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public RestClient(ISerializationAdapter serializationAdapter, ITracer tracer, IHttpClientFactory httpClientFactory, IZip zip) : base(serializationAdapter, tracer)
#pragma warning restore IDE0060 // Remove unused parameter
        {
        }

        async Task<RestResponseBase<TReturn>> IRestClient.SendAsync<TReturn, TBody>(Uri resource, HttpVerb httpVerb, string contentType, TBody body, CancellationToken cancellationToken)
#pragma warning restore CA1033 // Interface methods should be callable by child types
        {
            var response = await ResponseProcessor.ProcessRestResponseAsync<TReturn, TBody>(resource, httpVerb, body, contentType, cancellationToken);

            if (response.IsSuccess || !ThrowExceptionOnFailure)
            {
                return response;
            }

            throw new HttpStatusException($"{response.StatusCode}.\r\nResource: {resource}", response);
        }
    }
}
