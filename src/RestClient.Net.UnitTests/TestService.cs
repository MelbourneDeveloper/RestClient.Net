using RestClient.Net.Abstractions;
using System;
using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{
    public class TestService : ITestService
    {
        public static Uri Uri { get; } = new Uri("http://www.test.com");

        public IClient Client { get; }

        public TestService(CreateClient createClient)
            => Client = createClient != null ? createClient("TestServiceClient",
               Uri) :
            throw new ArgumentNullException(nameof(createClient));

        public async Task<TestThing> GetTestThingAsync() => await Client.GetAsync<TestThing>().ConfigureAwait(false);
    }

}
