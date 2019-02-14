using groupkt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClientDotNet;
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

            for (var i = 0; i < 10; i++)
                countryData = await countryCodeClient.GetAsync<groupktResult<CountriesResult>>();

            Console.WriteLine($"RestClient Get : Total Milliseconds:{ (DateTime.Now - startTime).TotalMilliseconds}");
        }
    }
}
