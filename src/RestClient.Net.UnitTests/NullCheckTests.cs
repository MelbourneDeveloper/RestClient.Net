using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestClient.Net.Abstractions;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace RestClient.Net.UnitTests
{
    [TestClass]
#pragma warning disable CA1063 // Implement IDisposable Correctly
    public class NullCheckTests : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        private readonly Mock<IGetHttpRequestMessage> getHttpRequestMessageMock = new Mock<IGetHttpRequestMessage>();
        private readonly Mock<IRequest<string>> request = new Mock<IRequest<string>>();
        private readonly Mock<ILogger<Client>> logger = new Mock<ILogger<Client>>();
        private readonly Mock<ISerializationAdapter> serializationAdapterMock = new Mock<ISerializationAdapter>();
        private readonly HttpClient httpClient = new HttpClient();

#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose() => httpClient.Dispose();
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
#pragma warning restore CA1063 // Implement IDisposable Correctly

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
                    serializationAdapterMock.Object).ConfigureAwait(false);

            }).ConfigureAwait(false);

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
                    serializationAdapterMock.Object).ConfigureAwait(false);

            }).ConfigureAwait(false);

            Assert.AreEqual("httpRequestMessageFunc", exception.ParamName);
        }
    }
}