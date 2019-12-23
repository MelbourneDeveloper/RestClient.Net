using RestClientDotNet.Abstractions;
using System;

namespace RestClientDotNet
{
    public class RestClient : RestClientBase, IRestClient
    {
        public RestClient(ISerializationAdapter serializationAdapter) : this(serializationAdapter, default(Uri))
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter, Uri baseUri) : this(serializationAdapter, baseUri, default(ITracer))
        {
        }

#pragma warning disable CA2000 // Dispose objects before losing scope
        public RestClient(ISerializationAdapter serializationAdapter, Uri baseUri, ITracer tracer) : this(serializationAdapter, tracer, new SingletonHttpClientFactory(default, baseUri), null)
#pragma warning restore CA2000 // Dispose objects before losing scope
        {
        }

#pragma warning disable CA2000 // Dispose objects before losing scope
        public RestClient(ISerializationAdapter serializationAdapter, Uri baseUri, TimeSpan timeout) : this(serializationAdapter, new SingletonHttpClientFactory(timeout, baseUri))
#pragma warning restore CA2000 // Dispose objects before losing scope
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter, IHttpClientFactory httpClientFactory) : this(serializationAdapter, null, httpClientFactory, null)
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter, IHttpClientFactory httpClientFactory, ITracer tracer) : this(serializationAdapter, tracer, httpClientFactory, null)
        {
        }

        public RestClient(ISerializationAdapter serializationAdapter, ITracer tracer, IHttpClientFactory httpClientFactory, IZip zip) : base(serializationAdapter, new ResponseProcessorFactory(serializationAdapter, tracer, httpClientFactory, zip), tracer)
        {
        }
    }
}
