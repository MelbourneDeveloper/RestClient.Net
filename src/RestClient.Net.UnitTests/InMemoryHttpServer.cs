#if !NET45

using Http.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestClient.Net.Abstractions;
using System;
using System.Net;
using System.Threading.Tasks;
using Urls;

namespace RestClient.Net.UnitTests
{
    [TestClass]
    public class InMemoryHttpServerTests
    {
        [TestMethod]
        public async Task TestGetFromUrl()
        {
            var testThing2 = new TestThing2 { TestPropery1 = "1", TestPropery2 = 2 };
            var responseJson = JsonConvert.SerializeObject(testThing2);
            const string headerKey = "key";
            const string headerValue = "value";

            using var server = ServerExtensions
                .GetLocalhostAddress()
                .GetSingleRequestServer(async (context) =>
            {
                Assert.AreEqual("seg1/", context?.Request?.Url?.Segments?[1]);
                Assert.AreEqual("seg2", context?.Request?.Url?.Segments?[2]);
                Assert.AreEqual("?Id=1", context?.Request?.Url?.Query);
                Assert.AreEqual(headerValue, context?.Request?.Headers[headerKey]);


                if (context == null) throw new InvalidOperationException();

                await context.WriteContentAndCloseAsync(responseJson).ConfigureAwait(false);
            });

            var clientAndResponse = await
                server
                .AbsoluteUrl
                .WithRelativeUrl(
                    new RelativeUrl("seg1/seg2")
                    .WithQueryParamers(new { Id = 1 }))
                .GetAsync<TestThing2>(new HeadersCollection(headerKey, headerValue));

            // Assert
            Assert.IsNotNull(clientAndResponse.Response.Body);
            Assert.AreEqual(testThing2.TestPropery1, clientAndResponse.Response.Body.TestPropery1);
            Assert.AreEqual(testThing2.TestPropery2, clientAndResponse.Response.Body.TestPropery2);
            Assert.AreEqual((int)HttpStatusCode.OK, clientAndResponse.Response.StatusCode);
        }
    }

    public class TestThing2
    {
        public string TestPropery1 { get; set; } = "";
        public int TestPropery2 { get; set; }
    }
}

#endif
