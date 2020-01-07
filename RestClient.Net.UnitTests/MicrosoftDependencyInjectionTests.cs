using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClient.Net.DependencyInjection;
using RestClient.Net.UnitTests.Model;
using RestClientApiSamples;
using RestClient.Net.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StructureMap;

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
            serviceCollection.AddHttpClient("test", (c) => { c.BaseAddress = baseUri; });
            serviceCollection.AddDependencyInjectionMapping();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("test");
            Assert.AreEqual(baseUri, httpClient.BaseAddress);
        }

        [TestMethod]
        public async Task TestSendHandler()
        {
            try
            {
                var serviceCollection = new ServiceCollection();
                var baseUri = new Uri("http://www.test.com");
                serviceCollection.AddSingleton(typeof(ISerializationAdapter), typeof(NewtonsoftSerializationAdapter));
                serviceCollection.AddSingleton(typeof(ILogger), typeof(ConsoleLogger));
                serviceCollection.AddSingleton(typeof(IClient), typeof(Client));
                serviceCollection.AddDependencyInjectionMapping();
                serviceCollection.AddTransient<TestHandler>();
                serviceCollection.AddHttpClient("RestClient", (c) => { c.BaseAddress = baseUri; })
                    .AddHttpMessageHandler<TestHandler>();
                var serviceProvider = serviceCollection.BuildServiceProvider();
                var client = serviceProvider.GetService<IClient>();
                await client.GetAsync<object>();
            }
            catch (SendException<object> hse)
            {
                Assert.AreEqual("Ouch", hse.InnerException.Message);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public async Task TestFactoryWithNames()
        {
            var serviceCollection = new ServiceCollection();
            var baseUri = new Uri("https://restcountries.eu/rest/v2/");
            serviceCollection.AddSingleton(typeof(ISerializationAdapter), typeof(NewtonsoftSerializationAdapter));
            serviceCollection.AddSingleton(typeof(IClientFactory), typeof(ClientFactory));
            serviceCollection.AddSingleton(typeof(ILogger), typeof(ConsoleLogger));
            serviceCollection.AddSingleton<MockAspController>();
            serviceCollection.AddHttpClient("test", (c) => { c.BaseAddress = baseUri; });
            serviceCollection.AddDependencyInjectionMapping();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var mockAspController = serviceProvider.GetService<MockAspController>();
            var reponse = await mockAspController.Client.GetAsync<List<RestCountry>>();
        }

        [TestMethod]
        public void TestStructureMap()
        {
            var container = new Container(c =>
            {
                c.Scan(s =>
                {
                    s.TheCallingAssembly();
                    s.WithDefaultConventions();
                });

                c.For<IClientFactory>().Use<ClientFactory>();
                c.For<IHttpClientFactory>().Use<DefaultHttpClientFactory>();
                c.For<ILogger>().Use<ConsoleLogger>();
                c.For<ISerializationAdapter>().Use<NewtonsoftSerializationAdapter>();
            });

            var clientFactory = container.GetInstance<IClientFactory>();
            var client = clientFactory.CreateClient("Test");
            Assert.IsNotNull(client);
            Assert.AreEqual("Test", client.Name);
        }

    }
}