using RestClientDotNet.Abstractions;

namespace RestClientDotNet
{
    public class StandardRestClientFactory : IRestClientFactory
    {
        #region Public Properties
        public ISerializationAdapter SerializationAdapter { get; }
        public IHttpClientFactory HttpClientFactory { get; }
        public ITracer Tracer { get; }
        #endregion

        #region Constructor
        public StandardRestClientFactory(ISerializationAdapter serializationAdapter,
                                         IHttpClientFactory httpClientFactory,
                                         ITracer tracer)
        {
            SerializationAdapter = serializationAdapter;
            HttpClientFactory = httpClientFactory;
            Tracer = tracer;
        }
        #endregion

        #region Implementation
        public IRestClient CreateRestClient()
        {
            return new RestClient(SerializationAdapter,
                Tracer,
                new ResponseProcessor(
                    null,
                    SerializationAdapter,
                    Tracer,
                    HttpClientFactory));
        }
        #endregion
    }
}
