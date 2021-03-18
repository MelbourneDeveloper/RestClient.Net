using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestClient.Net.Abstractions;

namespace RestClient.Net.UnitTests
{
    [TestClass]
    public class NullCheckTests
    {
        [TestMethod]
        public async Task TestDefaultSendHttpRequestMessagehttpClient()
        {
            var getHttpRequestMessageMock = new Mock<IGetHttpRequestMessage>();
            var request = new Mock<IRequest<string>>();
            var logger = new Mock<ILogger<Client>>();
            var serializationAdapterMock = new Mock<ISerializationAdapter>();

            var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                _ = await DefaultSendHttpRequestMessage.Instance.SendHttpRequestMessage(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    null,
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    getHttpRequestMessageMock.Object,
                    request.Object,
                    logger.Object,
                    serializationAdapterMock.Object).ConfigureAwait(false);

            }).ConfigureAwait(false);

            Assert.AreEqual("httpClient", exception.ParamName);
        }
    }
}