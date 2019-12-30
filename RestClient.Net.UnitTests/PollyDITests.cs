#if NETCOREAPP3_1

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClient.Net.Polly;
using RestClientDotNet.Abstractions;
using System;
using System.Threading.Tasks;

namespace RestClientDotNet.UnitTests
{
    //https://github.com/microsoft/aspnet-api-versioning/blob/master/src/Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer/Microsoft.Extensions.DependencyInjection/IServiceCollectionExtensions.cs

    [TestClass]
    public class PollyDITests
    {
        [TestMethod]
        public void TestDIMapping()
        {
            var serviceCollection = new ServiceCollection();
            var baseUri = new Uri("http://www.test.com");
            serviceCollection.AddHttpClient("test", (c)=> { c.BaseAddress = baseUri; });
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
                serviceCollection.AddSingleton(typeof(ITracer), typeof(ConsoleTracer));
                serviceCollection.AddSingleton(typeof(IRestClient), typeof(RestClient));
                serviceCollection.AddDependencyInjectionMapping();
                serviceCollection.AddTransient<TestHandler>();
                serviceCollection.AddHttpClient("RestClient", (c) => { c.BaseAddress = baseUri; })
                    .AddHttpMessageHandler<TestHandler>();
                var serviceProvider = serviceCollection.BuildServiceProvider();
                var restClient = serviceProvider.GetService<IRestClient>();
                await restClient.GetAsync<object>();
            }
            catch (HttpStatusException hse)
            {
                Assert.AreEqual("Ouch", hse.Message);
                return;
            }
            Assert.Fail();
        }

    }
}
#endif