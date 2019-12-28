using Polly;
using RestClientDotNet;
using RestClientDotNet.Abstractions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RESTClient.NET.CoreSample
{
    public class PollyHttpRequestProcessor : DefaultHttpRequestProcessor
    {
        public async override Task<HttpResponseMessage> SendRestRequestAsync<TRequestBody>(HttpClient httpClient, RestRequest<TRequestBody> restRequest, byte[] requestBodyData)
        {
            var retries = 0;

            var policy = Policy.Handle<Exception>().RetryAsync(3, (exception, attempt) =>
            {
                retries++;
            });

            var task = await policy.ExecuteAsync(async () =>
            {
                if (retries == 2) restRequest.Resource = new Uri("Person", UriKind.Relative);

                var httpRequestMessage = GetHttpRequestMessage(restRequest, requestBodyData);

                var httpResponseMessage = await  httpClient.SendAsync(httpRequestMessage, restRequest.CancellationToken);

                if (!httpResponseMessage.IsSuccessStatusCode) throw new Exception("Ouch");

                //This is a hacky workaround. The call has already finished so there should be not need to return a task
                var taskCompletionSource = new TaskCompletionSource<HttpResponseMessage>();
                taskCompletionSource.SetResult(httpResponseMessage);
                return taskCompletionSource.Task;
            });

            return await task;
        }
    }
}
