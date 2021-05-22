#if !NET45

using System;
using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{
    public class GetString2 : IGetString
    {
        public IClient Client { get; }

        public GetString2(CreateClient createClient)
        {
            if (createClient == null) throw new ArgumentNullException(nameof(createClient));
            Client = createClient("test", (o) => { o.BaseUrl = o.BaseUrl with { Host = "Hi" }; });
        }

        public async Task<string?> GetStringAsync() => await Client.GetAsync<string>();
    }

}
#endif