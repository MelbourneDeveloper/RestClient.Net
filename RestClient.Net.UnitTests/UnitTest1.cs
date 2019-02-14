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
            var countryData = await countryCodeClient.GetAsync<groupktResult<CountriesResult>>();
        }
    }
}
