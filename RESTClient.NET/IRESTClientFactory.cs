using System;

namespace RestClientDotNet
{
    public interface IRestClientFactory
    {
        RestClient CreateRESTClient(Uri baseUri);
    }
}
