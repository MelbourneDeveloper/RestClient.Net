#if NETCOREAPP3_1

using RestClient.Net.Abstractions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{
    public class TestHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken) => throw new HttpStatusException("Ouch", null, null);
    }
}
#endif