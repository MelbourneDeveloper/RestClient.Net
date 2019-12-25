using System;
using System.Net.Http;
using RestClientDotNet.Abstractions;

namespace RestClientDotNet
{
    public interface IHttpClientFactory : IDisposable
    {
        TimeSpan Timeout { get; set; }
        IRestHeaders DefaultRequestHeaders { get; }
        HttpClient CreateHttpClient(Uri baseUri);
    }

}
