using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Urls;

#if !NET45
using System.Linq;
#endif

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

namespace RestClient.Net.UnitTests
{
    [TestClass]
    public class NullCheckTests : IDisposable
    {
        private readonly Mock<IGetHttpRequestMessage> getHttpRequestMessageMock = new();
        private readonly Mock<IRequest<string>> request = new();
        private readonly Mock<ILogger<Client>> logger = new();
        private readonly Mock<ISerializationAdapter> serializationAdapterMock = new();
        private readonly HttpClient httpClient = new();

        public void Dispose() => httpClient.Dispose();

        [TestMethod]
        public async Task TestDefaultSendHttpRequestMessagehttpClient()
        {

            var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                _ = await DefaultSendHttpRequestMessage.Instance.SendHttpRequestMessage(
                    null,
                    getHttpRequestMessageMock.Object,
                    request.Object,
                    logger.Object,
                    serializationAdapterMock.Object);

            });

            Assert.AreEqual("httpClient", exception.ParamName);
        }

        [TestMethod]
        public async Task TestDefaultSendHttpRequestMessagehttpRequestMessageFunc()
        {

            var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                _ = await DefaultSendHttpRequestMessage.Instance.SendHttpRequestMessage(
                    httpClient,
                    null,
                    request.Object,
                    logger.Object,
                    serializationAdapterMock.Object);

            });

            Assert.AreEqual("httpRequestMessageFunc", exception.ParamName);
        }

        [TestMethod]
        public async Task TestDefaultSendHttpRequestMessageRequest()
        {

            var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                _ = await DefaultSendHttpRequestMessage.Instance.SendHttpRequestMessage(
                    httpClient,
                    getHttpRequestMessageMock.Object,
                    default(Request<string>),
                    logger.Object,
                    serializationAdapterMock.Object);

            });

            Assert.AreEqual("request", exception.ParamName);
        }

        [TestMethod]
        public void TestGetHttpRequestMessage()
        {

            var exception = Assert.ThrowsException<ArgumentNullException>(() =>
           {
               _ = DefaultGetHttpRequestMessage.Instance.GetHttpRequestMessage(
                   default(Request<string>),
                   logger.Object,
                   serializationAdapterMock.Object);

           });

            Assert.AreEqual("request", exception.ParamName);
        }

        [TestMethod]
        public void TestGetHttpRequestMessageLogger()
        {

            var exception = Assert.ThrowsException<ArgumentNullException>(() =>
           {
               _ = DefaultGetHttpRequestMessage.Instance.GetHttpRequestMessage(
                   request.Object,
                   logger.Object,
                   null);

           });

            Assert.AreEqual("serializationAdapter", exception.ParamName);
        }

        [TestMethod]
        public async Task TestSendAsyncClient()
        {
            var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                _ = await CallExtensions.SendAsync<string, string>(null, request.Object);

            });

            Assert.AreEqual("client", exception.ParamName);
        }

        [TestMethod]
        public void TestSendAsyncClient2() =>
    Assert.AreEqual("client", Assert.ThrowsException<ArgumentNullException>(() =>
        _ = CallExtensions.SendAsync<string, string>(null, HttpRequestMethod.Delete, "Asd", new Uri("http://www.testing.com").ToAbsoluteUrl().RelativeUrl)).ParamName);


        [TestMethod]
        public async Task TestDeleteAsyncClient()
        {
            var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                _ = await CallExtensions.DeleteAsync(null);

            });

            Assert.AreEqual("client", exception.ParamName);
        }

        [TestMethod]
        public void TestHeadersExtensionsAppendHeadersCollection()
        {
            var exception = Assert.ThrowsException<ArgumentNullException>(() =>
            _ = HeadersExtensions.Append(default, "Asd", new List<string> { }));
            Assert.AreEqual("headersCollection", exception.ParamName);
        }

        [TestMethod]
        public void TestHeadersExtensionsAppendKey() =>
            Assert.AreEqual("key", Assert.ThrowsException<ArgumentNullException>(() =>
            _ = HeadersExtensions.Append(NullHeadersCollection.Instance, null, new List<string> { })).ParamName);

        [TestMethod]
        public void TestHeadersExtensionsAppendValue() =>
            Assert.AreEqual("value", Assert.ThrowsException<ArgumentNullException>(() =>
            _ = HeadersExtensions.Append(NullHeadersCollection.Instance, "asdAS", default(IEnumerable<string>))).ParamName);


        [TestMethod]
        public void TestAppendDefaultRequestHeaders() =>
            Assert.AreEqual("client", Assert.ThrowsException<ArgumentNullException>(() =>
                _ = HeadersExtensions.AppendDefaultRequestHeaders(null, NullHeadersCollection.Instance)).ParamName);

        [TestMethod]
        public void TestRequestUri() =>
            Assert.AreEqual("uri", Assert.ThrowsException<ArgumentNullException>(() =>
                _ = new Request<string>(default, "asd", NullHeadersCollection.Instance, HttpRequestMethod.Get)).ParamName);

        [TestMethod]
        public void TestAppendDefaultRequestHeadersheadersCollection() =>
            Assert.AreEqual("headersCollection", Assert.ThrowsException<ArgumentNullException>(() =>
                _ = HeadersExtensions.AppendDefaultRequestHeaders(
                    new Client(
                        new Mock<ISerializationAdapter>().Object),
                        null
                    )).ParamName);


        [TestMethod]
        public async Task TestClientValidateHttpClientNull()
        {
            using var client = new Client(new Mock<ISerializationAdapter>().Object, baseUrl: new AbsoluteUrl("http://www.test.com"), createHttpClient: (n) => null);
            var exception = await Assert.ThrowsExceptionAsync<SendException>(client.GetAsync<string>);
            Assert.IsTrue(exception.InnerException is InvalidOperationException);
        }

        [TestMethod]
        public async Task TestSendNoRequest()
        => Assert.AreEqual("request", (await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => new Client(
              serializationAdapter: new NewtonsoftSerializationAdapter(),
              createHttpClient: (n) => new HttpClient()
              ).SendAsync<string, object>(null))).ParamName);


#if !NET45
#pragma warning disable IDE0072 // Add missing cases
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        [TestMethod]
        [DataRow(HttpRequestMethod.Get)]
        [DataRow(HttpRequestMethod.Patch)]
        [DataRow(HttpRequestMethod.Post)]
        [DataRow(HttpRequestMethod.Put)]
        [DataRow(HttpRequestMethod.Delete)]
        public async Task TestThatNullUrlRaisesException(HttpRequestMethod httpRequestMethod)
        {
            AbsoluteUrl absoluteUrl = null;
            var requestThing = new RequestThing();

            _ = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
              {

                  var clientAndResponse = httpRequestMethod switch
                  {
                      HttpRequestMethod.Delete => await absoluteUrl.DeleteAsync(),
                      HttpRequestMethod.Get => await absoluteUrl.GetAsync<ResponseThing>(),
                      HttpRequestMethod.Patch => await absoluteUrl.PatchAsync<ResponseThing, RequestThing>(requestThing),
                      HttpRequestMethod.Post => await absoluteUrl.PostAsync<ResponseThing, RequestThing>(requestThing),
                      HttpRequestMethod.Put => await absoluteUrl.PutAsync<ResponseThing, RequestThing>(requestThing),
                      _ => throw new NotImplementedException()
                  };
              });
        }
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore IDE0072 // Add missing cases
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        [TestMethod]
        public void TestCreateFromUrlStringAndWith()
        {
            using var client = new Client("http://www.test.com/test/test2?test=test#frag");
            using var client2 =
                client
                .WithDefaultRequestHeaders("a", "b")
                .With(client.BaseUrl)
                .With(client.SerializationAdapter)
                .With(client.Name)
                .With(client.ThrowExceptionOnFailure);

            Assert.AreEqual("frag", client2.BaseUrl.RelativeUrl.Fragment);
            Assert.AreEqual("www.test.com", client2.BaseUrl.Host);
            Assert.IsTrue(client2.BaseUrl.RelativeUrl.Path.SequenceEqual(new List<string> { "test", "test2" }));
            Assert.AreEqual("test", client.BaseUrl.RelativeUrl.QueryParameters[0].FieldName);
            Assert.AreEqual(client.SerializationAdapter, client2.SerializationAdapter);
            Assert.AreEqual(client.ThrowExceptionOnFailure, client2.ThrowExceptionOnFailure);
            Assert.AreEqual(client.Name, client2.Name);
            Assert.AreEqual(client.BaseUrl, client2.BaseUrl);
        }

        [TestMethod]
        public void TestJsonSerializationAdapterDeserialize() =>
            Assert.AreEqual("responseData", Assert.ThrowsException<ArgumentNullException>(() =>
                _ = new JsonSerializationAdapter().Deserialize<string>(null, NullHeadersCollection.Instance)).ParamName);
#endif

    }
}