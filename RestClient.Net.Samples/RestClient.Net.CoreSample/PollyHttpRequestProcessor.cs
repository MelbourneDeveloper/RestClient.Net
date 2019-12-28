using Polly;
using Polly.Extensions.Http;
using RestClientDotNet;
using RestClientDotNet.Abstractions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RESTClient.NET.CoreSample
{
    public class PollyHttpRequestProcessor : DefaultHttpRequestProcessor
    {
        public async override Task<HttpResponseMessage> SendRestRequestAsync<TRequestBody>(HttpClient httpClient, RestRequest<TRequestBody> restRequest, byte[] requestBodyData)
        {
            var tries = 0;

            var policy = HttpPolicyExtensions
              .HandleTransientHttpError()
              .OrResult(response => response.StatusCode == HttpStatusCode.NotFound)
              .RetryAsync(3);

            var func = new Func<Task<HttpResponseMessage>>(async () =>
            {
                if (tries == 2) restRequest.Resource = new Uri("Person", UriKind.Relative);

                tries++;

                var httpRequestMessage = GetHttpRequestMessage(restRequest, requestBodyData);

                return await httpClient.SendAsync(httpRequestMessage, restRequest.CancellationToken);
            });

            return await policy.ExecuteAsync(func);

        }
    }
}
