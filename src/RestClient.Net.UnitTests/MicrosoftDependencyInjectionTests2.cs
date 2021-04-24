#if !NET45

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClient.Net.Abstractions;
using System;
using System.Threading.Tasks;
using RestClient.Net.DependencyInjection;

namespace RestClient.Net.UnitTests
{

    [TestClass]
    public class MicrosoftDependencyInjectionTests
    {

        [TestMethod]
        public void TestDIMapping()
        {
            const string expectedName = "Jim";

            var serviceCollection = new ServiceCollection()
                .AddSingleton<ISomeService, SomeService>()
                .AddRestClient((o) => { });

            _ = serviceCollection.AddHttpClient("test", (c) => c.Timeout = new TimeSpan(0, 0, 1));


            var serviceProvider = serviceCollection.BuildServiceProvider();

            var someService = serviceProvider.GetRequiredService<ISomeService>();

            if (someService.Client is not Client client)
            {
                throw new InvalidOperationException("Nah");
            }

            Assert.AreEqual(expectedName, client.Name);
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

}
#endif