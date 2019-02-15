using groupkt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClientDotNet;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{
    [TestClass]
    public class PerformanceTests
    {
        [TestMethod]
        public async Task PerformanceTestOne()
        {
            var countryCodeClient = new RestClientDotNet.RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://services.groupkt.com/country/get/all"));

            groupktResult<CountriesResult> countryData = null;

            var startTime = DateTime.Now;

            for (var i = 0; i < 15; i++)
                countryData = await countryCodeClient.GetAsync<groupktResult<CountriesResult>>();

            var restClientTotalMilliseconds = (DateTime.Now - startTime).TotalMilliseconds;
            Console.WriteLine($"RestClient Get : Total Milliseconds:{ restClientTotalMilliseconds}");


            startTime = DateTime.Now;
            var restSharpClient = new RestSharp.RestClient("http://services.groupkt.com");

            var request = new RestRequest(Method.GET)
            {
                Resource = "/country/get/all"
            };

            for (var i = 0; i < 15; i++)
            {
                var taskCompletionSource = new TaskCompletionSource<groupktResult<CountriesResult>>();
                var response = restSharpClient.ExecuteAsync<groupktResult<CountriesResult>>(request, (a) => { taskCompletionSource.SetResult(a.Data); });
                var result = await taskCompletionSource.Task;
            }

            var restSharpTotalMilliseconds = (DateTime.Now - startTime).TotalMilliseconds;
            Console.WriteLine($"RestSharp Get : Total Milliseconds:{ restSharpTotalMilliseconds}");

            Assert.IsTrue(restClientTotalMilliseconds < restSharpTotalMilliseconds, "😞 RestSharp wins.");

            Console.WriteLine("🏆 RestClient Wins!!!");
        }
    }
}
