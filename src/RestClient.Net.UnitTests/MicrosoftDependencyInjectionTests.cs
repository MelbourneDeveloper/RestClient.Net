using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StructureMap;
using System;
using System.Threading.Tasks;
using Urls;

namespace RestClient.Net.UnitTests
{
    //https://github.com/microsoft/aspnet-api-versioning/blob/master/src/Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer/Microsoft.Extensions.DependencyInjection/IServiceCollectionExtensions.cs

    [TestClass]
    public class MicrosoftDependencyInjectionTests
    {
        private readonly TimeSpan timeout = new(1, 0, 0);

        [TestMethod]
        public void TestDIMapping()
        {
            var serviceCollection = new ServiceCollection();
            _ = serviceCollection.AddHttpClient("test", (c) => c.Timeout = timeout);
            _ = serviceCollection.AddRestClient();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetRequiredService<CreateHttpClient>();
            var httpClient = httpClientFactory("test");
            Assert.AreEqual(timeout, httpClient.Timeout);
        }

        [TestMethod]
        public async Task TestSendHandler()
        {
            try
            {
                const string clientName = "Test";

                var serviceCollection = new ServiceCollection()
                    .AddSingleton(typeof(ISerializationAdapter), typeof(NewtonsoftSerializationAdapter))
                    .AddLogging()
                    .AddSingleton(typeof(IClient), typeof(Client))
                    .AddRestClient()
                    .AddTransient<TestHandler>()
                    //Make sure the HttpClient is named the same as the Rest Client
                    .AddSingleton<IClient>(x => new Client(new AbsoluteUrl("http://www.test.com"), name: clientName, createHttpClient: x.GetRequiredService<CreateHttpClient>()));

                _ = serviceCollection.AddHttpClient(clientName)
                .AddHttpMessageHandler<TestHandler>();

                var serviceProvider = serviceCollection.BuildServiceProvider();
                var client = serviceProvider.GetRequiredService<IClient>();
                _ = await client.GetAsync<object>();
                _ = serviceCollection.Configure<string>((s) => { });
            }
            catch (SendException hse)
            {
                Assert.AreEqual("Ouch", hse.InnerException?.Message);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public async Task TestSendHandlerWithFactory()
        {
            try
            {
                const string clientName = "Test";
                var serviceCollection = new ServiceCollection();
                _ = serviceCollection.AddSingleton(typeof(ISerializationAdapter), typeof(NewtonsoftSerializationAdapter))
                    .AddLogging()
                    .AddRestClient()
                    .AddTransient<TestHandler>()
                    .AddHttpClient(clientName)
                    .AddHttpMessageHandler<TestHandler>();

                var serviceProvider = serviceCollection.BuildServiceProvider();
                var clientFactory = serviceProvider.GetRequiredService<CreateClient>();
                var client = clientFactory(clientName, (o) => o.BaseUrl = new("http://www.test.com"));
                _ = await client.GetAsync<object>();
            }
            catch (SendException hse)
            {
                Assert.AreEqual("Ouch", hse.InnerException?.Message);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void TestFactoryWithNames()
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton(typeof(ISerializationAdapter), typeof(NewtonsoftSerializationAdapter))
                .AddLogging()
                .AddSingleton<MockAspController>()
                .AddRestClient();

            _ = serviceCollection.AddHttpClient("test");

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var mockAspController = serviceProvider.GetRequiredService<MockAspController>();
            Assert.IsNotNull(mockAspController);
        }

        [TestMethod]
        public void TestStructureMap()
        {
            using var container = new Container(c =>
            {
                c.Scan(s =>
                {
                    s.TheCallingAssembly();
                    _ = s.WithDefaultConventions();
                });

                _ = c.For<CreateClient>().Use<CreateClient>(con => new ClientFactory(con.GetInstance<CreateHttpClient>(), null, null).CreateClient);
                _ = c.For<CreateHttpClient>().Use<CreateHttpClient>(con => new DefaultHttpClientFactory(null, null).CreateClient);
                _ = c.For<ISerializationAdapter>().Use<NewtonsoftSerializationAdapter>();
            });

            var clientFactory = container.GetInstance<CreateClient>();
            var client = clientFactory("Test");
            Assert.IsNotNull(client);
            Assert.AreEqual("Test", ((Client)client).Name);
        }


        [TestMethod]
        public void TestClientGetsCorrectHttpClient()
        {
            var serviceCollection = new ServiceCollection();
            _ = serviceCollection.AddHttpClient("test", (c) => c.Timeout = timeout);
            _ = serviceCollection.AddRestClient();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var createClient = serviceProvider.GetRequiredService<CreateClient>();
            var client = (Client)createClient("test");

            Assert.AreEqual(timeout, client.lazyHttpClient.Value.Timeout);

            Assert.IsFalse(ReferenceEquals(NullLogger.Instance, client.logger));
        }

        [TestMethod]
        public void TestClientGetsUnnamedHttpClient()
        {
            var serviceCollection = new ServiceCollection();
            _ = serviceCollection.AddHttpClient();
            _ = serviceCollection.AddRestClient();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var createClient = serviceProvider.GetRequiredService<CreateClient>();
            var client = (Client)createClient("test");

            Assert.IsNotNull(client.lazyHttpClient.Value.Timeout);
        }

        [TestMethod]
        public void TestClientGetsCorrectLogger()
        {
            var loggerProviderMock = new Mock<ILoggerProvider>();
            var loggerMock = new Mock<ILogger<Client>>();
            _ = loggerProviderMock.Setup(p => p.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);

            var client = (Client)new ServiceCollection()
            .AddLogging((builder) => builder.AddProvider(loggerProviderMock.Object))
            .AddHttpClient()
            .AddRestClient()
            .BuildServiceProvider()
            .GetRequiredService<CreateClient>()("test");

            var logger = (ILogger<Client>)client.logger;

            logger.LogInformation("Hi");

            loggerMock.VerifyLog((state, t) =>
            state.CheckValue("{OriginalFormat}", "Hi")
            , LogLevel.Information, 1);
        }

        [TestMethod]
        public void TestCanGetClient()
        {
            var testService = new ServiceCollection()
            .AddHttpClient()
            .AddRestClient()
            .AddSingleton<ITestService, TestService>()
            .BuildServiceProvider()
            .GetRequiredService<ITestService>();

            Assert.AreEqual(TestService.Uri, testService?.Client?.BaseUrl?.ToUri());
        }

    }

}
