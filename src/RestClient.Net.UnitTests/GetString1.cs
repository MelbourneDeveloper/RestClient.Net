#if !NET45

using RestClient.Net.Abstractions;
using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{
    public class GetString1 : IGetString
    {
        public IClient Client { get; }

        public GetString1(IClient client) => Client = client;

        public async Task<string?> GetStringAsync() => await Client.GetAsync<string>();
    }

}
#endif