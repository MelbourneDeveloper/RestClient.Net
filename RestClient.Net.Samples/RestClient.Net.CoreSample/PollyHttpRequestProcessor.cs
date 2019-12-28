using Polly;
using RestClientDotNet;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RESTClient.NET.CoreSample
{
    public class PollyHttpRequestProcessor : DefaultHttpRequestProcessor
    {
        public async override Task<HttpResponseMessage> SendAsync(HttpClient httpClient, HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            var retries = 0;

            var policy = Policy.Handle<Exception>().RetryAsync(3, (exception, attempt) =>
            {
                retries++;
            });

            var task = await policy.ExecuteAsync(async () =>
            {
                return httpClient.SendAsync(httpRequestMessage, cancellationToken);
            });

            return await task;
        }
    }
}
