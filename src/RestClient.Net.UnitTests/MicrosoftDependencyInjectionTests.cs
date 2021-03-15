using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClient.Net.Abstractions;
using RestClient.Net.DependencyInjection;
using RestClient.Net.UnitTests.Model;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{
    //https://github.com/microsoft/aspnet-api-versioning/blob/master/src/Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer/Microsoft.Extensions.DependencyInjection/IServiceCollectionExtensions.cs

    [TestClass]
    public class MicrosoftDependencyInjectionTests
    {
        [TestMethod]
        public void TestDIMapping()
        {
            var serviceCollection = new ServiceCollection();
            var baseUri = new Uri("http://www.test.com");
            _ = serviceCollection.AddHttpClient("test", (c) => c.BaseAddress = baseUri);
            _ = serviceCollection.AddDependencyInjectionMapping();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetService<CreateHttpClient>();
            var httpClient = httpClientFactory("test");
            Assert.AreEqual(baseUri, httpClient.BaseAddress);
        }

        [TestMethod]
        public async Task TestSendHandler()
        {
            try
            {
                const string clientName = "Test";

                var baseUri = new Uri("http://www.test.com");
                var serviceCollection = new ServiceCollection()
                    .AddSingleton(typeof(ISerializationAdapter), typeof(NewtonsoftSerializationAdapter))
                    .AddLogging()
                    .AddSingleton(typeof(IClient), typeof(Client))
                    .AddDependencyInjectionMapping()
                    .AddTransient<TestHandler>()
                    //Make sure the HttpClient is named the same as the Rest Client
                    .AddSingleton<IClient>(x => new Client(baseUri: baseUri, name: clientName, createHttpClient: x.GetRequiredService<CreateHttpClient>()));

                _ = serviceCollection.AddHttpClient(clientName)
                .AddHttpMessageHandler<TestHandler>();

                var serviceProvider = serviceCollection.BuildServiceProvider();
                var client = serviceProvider.GetService<IClient>();
                _ = await client.GetAsync<object>().ConfigureAwait(false);
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
                var baseUri = new Uri("http://www.test.com");
                _ = serviceCollection.AddSingleton(typeof(ISerializationAdapter), typeof(NewtonsoftSerializationAdapter))
                    .AddLogging()
                    .AddDependencyInjectionMapping()
                    .AddTransient<TestHandler>()
                    .AddHttpClient(clientName)
                    .AddHttpMessageHandler<TestHandler>();

                var serviceProvider = serviceCollection.BuildServiceProvider();
                var clientFactory = serviceProvider.GetService<CreateClient>();
                var client = clientFactory(clientName, baseUri);
                _ = await client.GetAsync<object>().ConfigureAwait(false);
            }
            catch (SendException hse)
            {
                Assert.AreEqual("Ouch", hse.InnerException?.Message);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public async Task TestFactoryWithNames()
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton(typeof(ISerializationAdapter), typeof(NewtonsoftSerializationAdapter))
                .AddLogging()
                .AddSingleton<MockAspController>()
                .AddDependencyInjectionMapping();

            _ = serviceCollection.AddHttpClient("test");

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var mockAspController = serviceProvider.GetService<MockAspController>();
            var response = await mockAspController.Client.GetAsync<List<RestCountry>>().ConfigureAwait(false);
            Assert.AreEqual(250, response.Body?.Count);
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

                _ = c.For<CreateClient>().Use<CreateClient>(con => new ClientFactory(con.GetInstance<CreateHttpClient>(), con.GetInstance<ISerializationAdapter>(), null).CreateClient);
                _ = c.For<CreateHttpClient>().Use<CreateHttpClient>(con => new DefaultHttpClientFactory().CreateClient);
                _ = c.For<ISerializationAdapter>().Use<NewtonsoftSerializationAdapter>();
            });

            var clientFactory = container.GetInstance<CreateClient>();
            var client = clientFactory("Test");
            Assert.IsNotNull(client);
            Assert.AreEqual("Test", client.Name);
        }
    }
}
