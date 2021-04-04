using Http.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
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
            var url = ServerExtensions.GetLocalhostAddress();

            using var server = new HttpServer(url);

            var task = server.ServeAsync((context) =>
            {
                var writer = new StreamWriter(context.Response.OutputStream);
                writer.Write(outputHtml);
                writer.Close();
            });


            using var myhttpclient = new HttpClient() { BaseAddress = new Uri(url) };

            // Act
            using var request = new HttpRequestMessage();

            var response = await myhttpclient.SendAsync(request).ConfigureAwait(false);

            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(outputHtml, content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            await task;

        }

    }
}