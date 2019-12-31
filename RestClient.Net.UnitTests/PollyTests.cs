using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using Polly.Extensions.Http;
using RestClientApiSamples;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RestClientDotNet.UnitTests
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


            var restClient = new Client(
                new ProtobufSerializationAdapter(),
                null,
                new Uri(UnitTests.LocalBaseUriString),
                logger: null,
                httpClientFactory: UnitTests.GetTestClientFactory(),
                sendHttpRequestFunc: (httpClient, httpRequestMessageFunc, cancellationToken) =>
 {
     return policy.ExecuteAsync(() =>
     {
         var httpRequestMessage = httpRequestMessageFunc.Invoke();

         //On the third try change the Url to a the correct one
         if (tries == 2) httpRequestMessage.RequestUri = new Uri("Person", UriKind.Relative);
         tries++;
         return httpClient.SendAsync(httpRequestMessage, cancellationToken);
     });
 });

            var person = new Person { FirstName = "Bob", Surname = "Smith" };

            //Note the Uri here is deliberately incorrect. It will cause a 404 Not found response. This is to make sure that polly is working
            person = await restClient.PostAsync<Person, Person>(person, new Uri("person2", UriKind.Relative));
            Assert.AreEqual("Bob", person.FirstName);
            Assert.AreEqual(3, tries);

        }
    }
}
