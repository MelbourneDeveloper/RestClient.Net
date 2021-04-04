#if !NET45

using Http.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestClient.Net.Abstractions;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Urls;

namespace RestClient.Net.UnitTests
{
    [TestClass]
    public class InMemoryHttpServerTests
    {
        [TestMethod]
        [DataRow(HttpRequestMethod.Get, false)]
        [DataRow(HttpRequestMethod.Patch, true)]
        [DataRow(HttpRequestMethod.Post, true)]
        public async Task TestGetFromUrl(HttpRequestMethod httpRequestMethod, bool hasRequestBody)
        {
            var responseThing = new ResponseThing { TestPropery1 = "1", TestPropery2 = 2 };
            var requestThing = new RequestThing { TestPropery3 = "asdasd", TestPropery4 = 321 };
            var responseJson = JsonConvert.SerializeObject(responseThing);
            var requestJson = JsonConvert.SerializeObject(requestThing);
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

                if (hasRequestBody)
                {
                    var length = context?.Request?.ContentLength64;
                    if (!length.HasValue) throw new InvalidOperationException();
                    var buffer = new byte[length.Value];
                    _ = (context?.Request?.InputStream.ReadAsync(buffer, 0, (int)length.Value));
                    Assert.AreEqual(requestJson, Encoding.UTF8.GetString(buffer));
                }

                if (context == null) throw new InvalidOperationException();

                await context.WriteContentAndCloseAsync(responseJson).ConfigureAwait(false);
            });

            var absoluteUrl = server
                            .AbsoluteUrl
                            .WithRelativeUrl(
                                new RelativeUrl("seg1/seg2")
                                .WithQueryParamers(new { Id = 1 }));

            var clientAndResponse = httpRequestMethod switch
            {
                HttpRequestMethod.Get => await absoluteUrl.GetAsync<ResponseThing>(new HeadersCollection(headerKey, headerValue)),
                HttpRequestMethod.Patch => await absoluteUrl.PatchAsync<ResponseThing, RequestThing>(requestThing, new HeadersCollection(headerKey, headerValue)),
                HttpRequestMethod.Post => await absoluteUrl.PostAsync<ResponseThing, RequestThing>(requestThing, new HeadersCollection(headerKey, headerValue)),
                HttpRequestMethod.Put => throw new NotImplementedException(),
                HttpRequestMethod.Delete => throw new NotImplementedException(),
                HttpRequestMethod.Custom => throw new NotImplementedException(),
                _ => throw new NotImplementedException()
            };

            // Assert
            Assert.IsNotNull(clientAndResponse.Response.Body);
            Assert.AreEqual(responseThing.TestPropery1, clientAndResponse.Response.Body.TestPropery1);
            Assert.AreEqual(responseThing.TestPropery2, clientAndResponse.Response.Body.TestPropery2);
            Assert.AreEqual((int)HttpStatusCode.OK, clientAndResponse.Response.StatusCode);
            Assert.AreEqual(httpRequestMethod, clientAndResponse.Response.HttpRequestMethod);
        }
    }

    public class ResponseThing
    {
        public string TestPropery1 { get; set; } = "";
        public int TestPropery2 { get; set; }
    }

    public class RequestThing
    {
        public string TestPropery3 { get; set; } = "";
        public int TestPropery4 { get; set; }
    }
}

#endif
