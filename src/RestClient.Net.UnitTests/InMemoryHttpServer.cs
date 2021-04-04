#if !NET45

using Http.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{
    [TestClass]
    public class InMemoryHttpServerTests
    {
        [TestMethod]
        public async Task TestReturnsHtmlText()
        {
            var testThing2 = new TestThing2 { TestPropery1 = "1", TestPropery2 = 2 };
            var responseJson = JsonConvert.SerializeObject(testThing2);

            using var server = ServerExtensions
                .GetLocalhostAddress()
                .GetSingleRequestServer(async (context) =>
            {
                await context.WriteContentAndCloseAsync(responseJson).ConfigureAwait(false);
            });

            var clientAndResponse = await server.AbsoluteUrl.GetAsync<TestThing2>();

            // Assert
            Assert.IsNotNull(clientAndResponse.Response.Body);
            Assert.AreEqual(testThing2.TestPropery1, clientAndResponse.Response.Body.TestPropery1);
            Assert.AreEqual(testThing2.TestPropery2, clientAndResponse.Response.Body.TestPropery2);
            Assert.AreEqual((int)HttpStatusCode.OK, clientAndResponse.Response.StatusCode);
        }
    }

    public class TestThing2
    {
        public string TestPropery1 { get; set; } = "";
        public int TestPropery2 { get; set; }
    }
}

#endif
