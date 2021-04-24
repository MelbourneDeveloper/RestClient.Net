using RestClient.Net.Abstractions;
using System;
using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{
    public class TestService : ITestService
    {
        public IClient Client { get; }

        public TestService(CreateClient createClient)
            => Client = createClient != null ? createClient("TestServiceClient") :
            throw new ArgumentNullException(nameof(createClient));

        public async Task<TestThing> GetTestThingAsync() => await Client.GetAsync<TestThing>().ConfigureAwait(false);
    }

}
