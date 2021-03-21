using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestClient.Net.Abstractions;
using RestClient.Net.Abstractions.Extensions;

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
        _ = CallExtensions.SendAsync<string, string>(null, HttpRequestMethod.Delete, "Asd", new Uri("http://www.testing.com"))).ParamName);


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
        public void TestClientFactoryExtensionsCreateClientClient()
        {
            var exception = Assert.ThrowsException<ArgumentNullException>(() => _ = ClientFactoryExtensions.CreateClient(default, "Asd"));

            Assert.AreEqual("createClient", exception.ParamName);
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
        public void TestRequestUri2() =>
            Assert.ThrowsException<InvalidOperationException>(() =>
                _ = new Request<string>(new Uri("Hi", UriKind.Relative), "asd", NullHeadersCollection.Instance, HttpRequestMethod.Get));

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
            using var client = new Client(new Mock<ISerializationAdapter>().Object, baseUri: new Uri("http://www.test.com"), createHttpClient: (n) => null);
            var exception = await Assert.ThrowsExceptionAsync<SendException>(() => client.GetAsync<string>());
            Assert.IsTrue(exception.InnerException is InvalidOperationException);
        }

        [TestMethod]
        public async Task TestSendNoRequest()
        => Assert.AreEqual("request", (await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => new Client(
              serializationAdapter: new NewtonsoftSerializationAdapter(),
              createHttpClient: (n) => new HttpClient()
              ).SendAsync<string, object>(null))).ParamName);


#if !NET472
        [TestMethod]
        public void TestJsonSerializationAdapterDeserialize() =>
            Assert.AreEqual("responseData", Assert.ThrowsException<ArgumentNullException>(() =>
                _ = new JsonSerializationAdapter().Deserialize<string>(null, NullHeadersCollection.Instance)).ParamName);
#endif

    }
}