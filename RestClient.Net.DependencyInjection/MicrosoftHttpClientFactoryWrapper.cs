using snh = System.Net.Http;

namespace RestClient.Net.DependencyInjection
{
    public class MicrosoftHttpClientFactoryWrapper : IHttpClientFactory
    {
        #region Public Properties
        snh.IHttpClientFactory HttpClientFactory { get; }
        #endregion

        #region Constructor
        public MicrosoftHttpClientFactoryWrapper(snh.IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
        }
        #endregion

        #region Implementation
        public snh.HttpClient CreateClient(string name)
        {
            return HttpClientFactory.CreateClient(name);
        }
        #endregion
    }
}
