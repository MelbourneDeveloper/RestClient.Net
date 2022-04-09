#if !NET45

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net.Http;
using Urls;

namespace RestClient.Net.UnitTests
{

    [TestClass]
    public class MicrosoftDependencyInjectionTests2
    {
        private const string DefaultRestClientName = "RestClient";

        [TestMethod]
        [DataRow(DefaultRestClientName, true)]
        [DataRow("123", false)]
        public void TestDIMapping(string httpClientName, bool isEqual)
        {
            const int secondsTimeout = 123;

            var serviceCollection = new ServiceCollection()
                .AddSingleton<IGetString, GetString1>()
                .AddRestClient();

            _ = serviceCollection.AddHttpClient(httpClientName, new Action<HttpClient>((c) =>
                c.Timeout = new TimeSpan(0, 0, secondsTimeout)
            ));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var someService = serviceProvider.GetRequiredService<IGetString>();

            if (someService.Client is not Client client)
            {
                throw new InvalidOperationException("Nah");
            }

            Assert.AreEqual(DefaultRestClientName, client.Name);

            if (isEqual)
            {
                //Make sure we got the HttpClient that the Microsft DI put in the container
                Assert.AreEqual(secondsTimeout, client.lazyHttpClient.Value.Timeout.TotalSeconds);
            }
            else
            {
                Assert.AreNotEqual(secondsTimeout, client.lazyHttpClient.Value.Timeout.TotalSeconds);
            }
        }

        [TestMethod]
        public void TestDIMapping2()
        {
            const int secondsTimeout = 123;

            var serviceCollection = new ServiceCollection()
                .AddSingleton<IGetString, GetString2>()
                .AddRestClient();

            _ = serviceCollection.AddHttpClient("Jim", new Action<HttpClient>((c) =>
                c.Timeout = new TimeSpan(0, 0, secondsTimeout)
            ));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var someService = serviceProvider.GetRequiredService<IGetString>();

            if (someService.Client is not Client client)
            {
                throw new InvalidOperationException("Nah");
            }

            Assert.AreEqual("test", client.Name);
            Assert.AreEqual("Hi", client.BaseUrl.Host);
        }

        [TestMethod]
        public void DIConfigureWithInjectedService()
        {
            const int secondsTimeout = 123;
            var testUrl = "http://example.org";

            var urlProvider = new Mock<IUrlProvider>();
            urlProvider.Setup(x => x.GetUrl())
                .Returns(testUrl)
                .Verifiable();

            var serviceCollection = new ServiceCollection()
                .AddRestClient(createClient: (n, o, sp) =>
                new Client(
                    new AbsoluteUrl(sp.GetRequiredService<IUrlProvider>().GetUrl()),
                    name: n))
                .AddSingleton(urlProvider.Object);

            _ = serviceCollection.AddHttpClient("Jim", new Action<HttpClient>((c) =>
                c.Timeout = new TimeSpan(0, 0, secondsTimeout)
            ));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var c = serviceProvider.GetRequiredService<IClient>();

            if (c is not Client client)
            {
                throw new InvalidOperationException("Nah");
            }

            Assert.AreEqual("RestClient", client.Name);

            Assert.AreEqual(new AbsoluteUrl(testUrl), client.BaseUrl);
            urlProvider.VerifyAll();
        }


        [TestMethod]
        public void TestCreateClientFunc()
        {
            var newtonsoftSerializationAdapter = new NewtonsoftSerializationAdapter();
            var baseUrl = new AbsoluteUrl("http://www.di.com");
            const string clientName = "tim";

            var serviceCollection = new ServiceCollection()
                .AddSingleton(baseUrl)
                .AddRestClient(createClient: (name, options, sp) =>
                new Client(
                    serializationAdapter: options.SerializationAdapter,
                    sp.GetRequiredService<AbsoluteUrl>(),
                    createHttpClient: options.CreateHttpClient,
                    name: name));

            _ = serviceCollection.AddHttpClient(clientName);

            var sp = serviceCollection.BuildServiceProvider();

            var expectedHttpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(clientName); ;

            var createClient = sp.GetRequiredService<CreateClient>();

            var client = (Client)createClient(clientName, (o) => { o.SerializationAdapter = newtonsoftSerializationAdapter; });

            Assert.IsTrue(ReferenceEquals(newtonsoftSerializationAdapter, client.SerializationAdapter));

            //Make sure the correct http client handler gets used in the Client
            Assert.IsTrue(ReferenceEquals(
                PollyTests.HttpClientHandlerField.GetValue(expectedHttpClient),
                PollyTests.HttpClientHandlerField.GetValue(client.lazyHttpClient.Value)));

            Assert.AreEqual(baseUrl, client.BaseUrl);
        }

    }
    public interface IUrlProvider
    {
#pragma warning disable CA1055 // URI-like return values should not be strings
        string GetUrl();
#pragma warning restore CA1055 // URI-like return values should not be strings
    }

}
#endif