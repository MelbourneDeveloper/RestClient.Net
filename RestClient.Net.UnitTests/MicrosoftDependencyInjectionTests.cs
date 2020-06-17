using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClient.Net.Abstractions;
using RestClient.Net.DependencyInjection;
using RestClient.Net.UnitTests.Model;
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
            serviceCollection.AddHttpClient("test", (c) => { c.BaseAddress = baseUri; });
            serviceCollection.AddDependencyInjectionMapping();
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

                var serviceCollection = new ServiceCollection();
                var baseUri = new Uri("http://www.test.com");
                serviceCollection.AddSingleton(typeof(ISerializationAdapter), typeof(NewtonsoftSerializationAdapter));
                serviceCollection.AddSingleton(typeof(ILogger), typeof(ConsoleLogger));
                serviceCollection.AddSingleton(typeof(IClient), typeof(Client));
                serviceCollection.AddDependencyInjectionMapping();
                serviceCollection.AddTransient<TestHandler>();

                //Make sure the HttpClient is named the same as the Rest Client
                serviceCollection.AddSingleton<IClient>(x => new Client(name: clientName, httpClientFactory: x.GetRequiredService<CreateHttpClient>()));
                serviceCollection.AddHttpClient(clientName, (c) => { c.BaseAddress = baseUri; })
                    .AddHttpMessageHandler<TestHandler>();

                var serviceProvider = serviceCollection.BuildServiceProvider();
                var client = serviceProvider.GetService<IClient>();
                await client.GetAsync<object>();
                serviceCollection.Configure<string>((s) => { });
            }
            catch (SendException<object> hse)
            {
                Assert.AreEqual("Ouch", hse.InnerException.Message);
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
                serviceCollection.AddSingleton(typeof(ISerializationAdapter), typeof(NewtonsoftSerializationAdapter));
                serviceCollection.AddSingleton(typeof(ILogger), typeof(ConsoleLogger));
                serviceCollection.AddSingleton(typeof(CreateClient), typeof(ClientFactory));
                serviceCollection.AddDependencyInjectionMapping();
                serviceCollection.AddTransient<TestHandler>();
                serviceCollection.AddHttpClient(clientName, (c) => { c.BaseAddress = baseUri; })
                    .AddHttpMessageHandler<TestHandler>();
                var serviceProvider = serviceCollection.BuildServiceProvider();
                var clientFactory = serviceProvider.GetService<CreateClient>();
                var client = clientFactory(clientName);
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
            serviceCollection.AddSingleton(typeof(ILogger), typeof(ConsoleLogger));
            serviceCollection.AddSingleton<MockAspController>();
            serviceCollection.AddHttpClient("test", (c) => { c.BaseAddress = baseUri; });
            serviceCollection.AddDependencyInjectionMapping();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var mockAspController = serviceProvider.GetService<MockAspController>();
            var response = await mockAspController.Client.GetAsync<List<RestCountry>>();
            Assert.AreEqual(250, response.Body.Count);
        }

        [TestMethod]
        public void TestStructureMap()
        {
            throw new NotImplementedException();
            /*
           var container = new Container(c =>
           {
               c.Scan(s =>
               {
                   s.TheCallingAssembly();
                   s.WithDefaultConventions();
               });

               c.For<CreateClient>().Use(() => new ClientFactory().CreateClient);
               c.For<CreateHttpClient>().Use<DefaultHttpClientFactory>();
               c.For<ILogger>().Use<ConsoleLogger>();
               c.For<ISerializationAdapter>().Use<NewtonsoftSerializationAdapter>();
           });

           var clientFactory = container.GetInstance<CreateClient>();
           var client = clientFactory("Test");
           Assert.IsNotNull(client);
           Assert.AreEqual("Test", client.Name);
         */
        }

    }

    //public static class CreateClientExtensions
    //{
    //    public static void NewMethod(this IServiceCollection serviceCollection)
    //    {
    //        serviceCollection.AddSingleton<CreateHttpClient>((sp) =>
    //        {
    //            var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    //            return clientFactory.CreateClient;
    //        });

    //        serviceCollection.AddSingleton<CreateClient>((sp) =>
    //        {
    //            var clientFactory = new ClientFactory(sp.GetRequiredService<ISerializationAdapter>(), sp.GetRequiredService<CreateHttpClient>());
    //            return clientFactory.CreateClient;
    //        });
    //    }
    //}

}