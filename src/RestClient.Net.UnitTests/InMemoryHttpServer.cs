using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

#pragma warning disable CA1054 // URI-like parameters should not be strings

namespace RestClient.Net.UnitTests
{
    //Thanks! https://gist.github.com/yetanotherchris/fb50071bced8bf0849ecd2cbbc3e9dce

    [TestClass]
    public class InMemoryHttpServerTests
    {
        public static Task BasicHttpServer(string url,
            string outputHtml)
        {
            return Task.Run(() =>
            {
                var listener = new HttpListener();
                listener.Prefixes.Add(url);
                listener.Start();

                // GetContext method blocks while waiting for a request. 
                var context = listener.GetContext();
                var response = context.Response;

                var stream = response.OutputStream;
                var writer = new StreamWriter(stream);
                writer.Write(outputHtml);
                writer.Close();
            });
        }

        //
        // Example
        //

        [TestMethod]
        public async Task Shouldreturnhtml()
        {
            // Arrange
            var url = GetLocalhostAddress();

            const string OutputHtml = "some html";

            using (BasicHttpServer(url, OutputHtml))
            {
                using var myhttpclient = new HttpClient() { BaseAddress = new Uri(url) };

                // Act
                using var request = new HttpRequestMessage();

                var response = await myhttpclient.SendAsync(request).ConfigureAwait(false);

                var content = await response.Content.ReadAsStringAsync();

                // Assert
                Assert.AreEqual(OutputHtml, content);
            }
        }

        private static string GetLocalhostAddress()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            return $"http://localhost:{port}/";
        }
    }
}
