#if !NET45

using Moq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace RestClient.Net.UnitTests
{
    public class TestHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken) => throw new HttpStatusException("Ouch", new Mock<IResponse>().Object);
    }
}
#endif