using Http.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{
    [TestClass]
    public class InMemoryHttpServerTests
    {
        [TestMethod]
        public async Task TestReturnsHtmlText()
        {
            var outputHtml = "Hi";

            using var server = ServerExtensions.GetLocalhostAddress().GetSingleRequestServer(async (context) =>
            {
                await context.WriteContentAndCloseAsync(outputHtml).ConfigureAwait(false);
            });

            using var myhttpclient = new HttpClient() { BaseAddress = server.AbsoluteUrl };

            // Act
            using var request = new HttpRequestMessage();

            var response = await myhttpclient.SendAsync(request).ConfigureAwait(false);

            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(outputHtml, content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        }
    }
}