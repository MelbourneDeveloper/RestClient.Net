/*

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using Polly.Extensions.Http;
using RestClient.Net.Abstractions;
using RestClient.Net.DependencyInjection;
using RestClient.Net.UnitTests.Model;
using RestClientApiSamples;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{
    [TestClass]
    public class PollyTests
    {
        [TestMethod]
        public async Task TestPollyManualIncorrectUri()
        {
            var tries = 0;

            var policy = HttpPolicyExtensions
              .HandleTransientHttpError()
              .OrResult(response => response.StatusCode == HttpStatusCode.NotFound)
              .RetryAsync(3);

            var client = new Client(
                new ProtobufSerializationAdapter(),
                null,
                new Uri(UnitTests.LocalBaseUriString),
                logger: null,
                createHttpClient: UnitTests.GetTestClientFactory().CreateClient,
                sendHttpRequestFunc: (httpClient, httpRequestMessageFunc, request) => policy.ExecuteAsync(() =>
                    {
                        var httpRequestMessage = httpRequestMessageFunc(request);

                        //On the third try change the Url to a the correct one
                        if (tries == 2) httpRequestMessage.RequestUri = new Uri("Person", UriKind.Relative);
                        tries++;
                        return httpClient.SendAsync(httpRequestMessage, request.CancellationToken);
                    }));

            var person = new Person { FirstName = "Bob", Surname = "Smith" };

            //Note the Uri here is deliberately incorrect. It will cause a 404 Not found response. This is to make sure that polly is working
            person = await client.PostAsync<Person, Person>(person, new Uri("person2", UriKind.Relative));
            Assert.AreEqual("Bob", person.FirstName);
            Assert.AreEqual(3, tries);

        }


        [TestMethod]
        public async Task TestPollyWithDependencyInjection()
        {
            //Configure a Polly policy
            var policy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            //Create a Microsoft IoC Container
            var serviceCollection = new ServiceCollection();
            var baseUri = new Uri("https://restcountries.eu/rest/v2/");
            _ = serviceCollection.AddSingleton(typeof(ISerializationAdapter), typeof(NewtonsoftSerializationAdapter))
            .AddSingleton(typeof(ILogger), typeof(ConsoleLogger))
            //Add the Polly policy to the named HttpClient instance
            .AddHttpClient("rc", (c) => c.BaseAddress = baseUri).
                SetHandlerLifetime(TimeSpan.FromMinutes(5)).
                AddPolicyHandler(policy);

            //Provides mapping for Microsoft's IHttpClientFactory (This is what makes the magic happen)
            _ = serviceCollection.AddDependencyInjectionMapping();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var clientFactory = serviceProvider.GetService<CreateClient>();

            //Create a Rest Client that will get the HttpClient by the name of rc
            var client = clientFactory("rc");

            //Make the call
            var response = await client.GetAsync<List<RestCountry>>();

            //TODO: Implement this completely to ensure that the policy is being applied
        }

    }
}

*/
