using RestClient.Net.Abstractions;
using System;
using System.Threading.Tasks;
using Urls;

namespace RestClient.Net.UnitTests
{
    public class TestService : ITestService
    {
        public static AbsoluteUrl Uri { get; } = new("http://www.test.com");

        public IClient Client { get; }

        public TestService(CreateClient createClient)
            => Client = createClient != null ?
            createClient("TestServiceClient", (o) => o.BaseUrl = Uri) :
            throw new ArgumentNullException(nameof(createClient));

        public async Task<TestThing?> GetTestThingAsync() => await Client.GetAsync<TestThing>().ConfigureAwait(false);
    }

}
