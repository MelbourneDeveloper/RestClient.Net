using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClient.Net.Polly;
using RestClient.Net.UnitTests.Model;
using RestClientApiSamples;
using RestClientDotNet.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestClientDotNet.UnitTests
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
                var restClient = serviceProvider.GetService<IClient>();
                await restClient.GetAsync<object>();
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
            serviceCollection.AddSingleton(typeof(IRestClientFactory), typeof(RestClientFactory));
            serviceCollection.AddSingleton(typeof(ILogger), typeof(ConsoleLogger));
            serviceCollection.AddSingleton<MockAspController>();
            serviceCollection.AddHttpClient("test", (c) => { c.BaseAddress = baseUri; });
            serviceCollection.AddDependencyInjectionMapping();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var mockASpController = serviceProvider.GetService<MockAspController>();
            var reponse = await mockASpController.RestClient.GetAsync<List<RestCountry>>();
        }

    }
}