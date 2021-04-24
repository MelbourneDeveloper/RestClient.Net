#if !NET45

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClient.Net.Abstractions;
using System;
using System.Threading.Tasks;
using RestClient.Net.DependencyInjection;
using System.Net.Http;
using RestClient.Net.DI;

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
                .AddSingleton<ISomeService, SomeService>()
                .AddRestClient();

            _ = serviceCollection.AddHttpClient(httpClientName, new Action<HttpClient>((c) =>
                c.Timeout = new TimeSpan(0, 0, secondsTimeout)
            ));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var someService = serviceProvider.GetRequiredService<ISomeService>();

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
                .AddSingleton<ISomeService, SomeService2>()
                .AddRestClient();

            _ = serviceCollection.AddHttpClient("Jim", new Action<HttpClient>((c) =>
                c.Timeout = new TimeSpan(0, 0, secondsTimeout)
            ));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var someService = serviceProvider.GetRequiredService<ISomeService>();

            if (someService.Client is not Client client)
            {
                throw new InvalidOperationException("Nah");
            }

            Assert.AreEqual("test", client.Name);


            Assert.AreEqual("Hi", client.BaseUrl.Host);

        }


    }

    public interface ISomeService
    {
        IClient Client { get; }
        Task<string> GetAsync();
    }

    public class SomeService : ISomeService
    {
        public IClient Client { get; }

        public SomeService(IClient client) => Client = client;

        public async Task<string> GetAsync() => await Client.GetAsync<string>();
    }

    public class SomeService2 : ISomeService
    {
        public IClient Client { get; }

        public SomeService2(CreateClient createClient2)
        {
            if (createClient2 == null) throw new ArgumentNullException(nameof(createClient2));
            Client = createClient2("test", (o) => { o.BaseUrl = o.BaseUrl with { Host = "Hi" }; });
        }

        public async Task<string> GetAsync() => await Client.GetAsync<string>();
    }

}
#endif