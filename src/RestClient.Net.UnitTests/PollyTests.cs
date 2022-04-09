

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using Polly.Extensions.Http;
using RestClientApiSamples;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{

    [TestClass]
    public class PollyTests
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        public static readonly FieldInfo HttpClientHandlerField = typeof(HttpMessageInvoker).GetField("_handler", BindingFlags.Instance | BindingFlags.NonPublic);
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

            var handler1 = HttpClientHandlerField.GetValue(expectedHttpClient);
            var handler2 = HttpClientHandlerField.GetValue(actualHttpClient);

            Assert.IsTrue(ReferenceEquals(handler1, handler2));
        }

    }
}
