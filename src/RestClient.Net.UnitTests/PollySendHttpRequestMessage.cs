using Microsoft.Extensions.Logging;
using Polly.Retry;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Urls;

namespace RestClient.Net.UnitTests
{
    //It sucks that we have to create a class in this way. The old version was far less verbose. 
    //TODO: Look in to another way to achieve this

    public class PollySendHttpRequestMessage : ISendHttpRequestMessage
    {

        private readonly AsyncRetryPolicy<HttpResponseMessage> policy;

        public PollySendHttpRequestMessage(AsyncRetryPolicy<HttpResponseMessage> policy) => this.policy = policy;

        public int Tries { get; private set; }

        public Task<HttpResponseMessage> SendHttpRequestMessage<TRequestBody>(
            HttpClient httpClient,
            IGetHttpRequestMessage httpRequestMessageFunc,
            IRequest<TRequestBody> request,
            ILogger logger,
            ISerializationAdapter serializationAdapter) =>
            policy.ExecuteAsync(() =>
            {
                if (httpRequestMessageFunc == null) throw new ArgumentNullException(nameof(httpRequestMessageFunc));
                if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
                if (request == null) throw new ArgumentNullException(nameof(request));

                var httpRequestMessage = httpRequestMessageFunc.GetHttpRequestMessage(request, logger, serializationAdapter);

                //On the third try change the Url to a the correct one
                if (Tries == 2)
                {
                    httpRequestMessage.RequestUri =
                    new AbsoluteUrl(MainUnitTests.LocalBaseUriString)
                    .WithRelativeUrl(new RelativeUrl("Person"))
                    .ToUri();
                }

                Tries++;
                return httpClient.SendAsync(httpRequestMessage, request.CancellationToken);
            });
    }
}
