

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using RestClientApiSamples;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
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

    [TestClass]
    public class PollyTests
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        private static readonly FieldInfo _httpClientHandlerField = typeof(HttpMessageInvoker).GetField("_handler", BindingFlags.Instance | BindingFlags.NonPublic);
#pragma warning restore CS8601 // Possible null reference assignment.

        [TestMethod]
        public async Task TestPollyManualIncorrectUri()
        {
            var policy = HttpPolicyExtensions
              .HandleTransientHttpError()
              .OrResult(response => response.StatusCode == HttpStatusCode.NotFound)
              .RetryAsync(3);

            var sendHttpRequestFunc = new PollySendHttpRequestMessage(policy);

            using var client = new Client(
                new ProtobufSerializationAdapter(),
                new(MainUnitTests.LocalBaseUriString),
                logger: null,
                createHttpClient: MainUnitTests.GetTestClientFactory().CreateClient,
                sendHttpRequest: sendHttpRequestFunc,
                name: null);

            var person = new Person { FirstName = "Bob", Surname = "Smith" };

            //Note the Uri here is deliberately incorrect. It will cause a 404 Not found response. This is to make sure that polly is working
            person = await client.PostAsync<Person, Person>(person, new("person2"));
            Assert.AreEqual("Bob", person?.FirstName);
            Assert.AreEqual(3, sendHttpRequestFunc.Tries);
        }


        [TestMethod]
        public void TestPollyWithDependencyInjection()
        {
            const string httpClientName = "rc";

            //Configure a Polly policy
            var policy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            //Create a Microsoft IoC Container
            var serviceCollection = new ServiceCollection();
            _ = serviceCollection.AddSingleton(typeof(ISerializationAdapter), typeof(NewtonsoftSerializationAdapter))
            .AddLogging()
            //Add the Polly policy to the named HttpClient instance
            .AddHttpClient(httpClientName).
                SetHandlerLifetime(TimeSpan.FromMinutes(5)).
                AddPolicyHandler(policy);

            //Provides mapping for Microsoft's IHttpClientFactory (This is what makes the magic happen)
            _ = serviceCollection.AddRestClient();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var clientFactory = serviceProvider.GetRequiredService<CreateClient>();

            //Get the client
            var client = (Client)clientFactory(httpClientName);

            //Get the Http client from the Microsoft IHttpClientFactory implementation wihch will have the Polly handler
            var expectedHttpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(httpClientName);

            //Get the actual HttpClient inside the Client
            var actualHttpClient = client.lazyHttpClient.Value;

            var handler1 = _httpClientHandlerField.GetValue(expectedHttpClient);
            var handler2 = _httpClientHandlerField.GetValue(actualHttpClient);

            Assert.IsTrue(ReferenceEquals(handler1, handler2));
        }

    }
}
