
#if (NETCOREAPP3_1)

using Flurl.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClient.Net.Samples.Model;
using RestClient.Net.UnitTests.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{
    [TestClass]
    public class PerformanceTests
    {
        private const int Repeats = 8;
        private const string RestCountriesUrl = "https://restcountries.eu/rest/v2/";

        [TestMethod]
        public async Task PerformanceTestFlurlGet()
        {
            var flurlClient = new FlurlClient(RestCountriesUrl);

            var startTime = DateTime.Now;
            var countryData = await flurlClient.Request().GetJsonAsync<List<RestCountry>>();
            Console.WriteLine($"Flurl.Http Get : Total Milliseconds:{ (DateTime.Now - startTime).TotalMilliseconds}");

            startTime = DateTime.Now;
            for (var i = 0; i < Repeats; i++)
            {
                countryData = await flurlClient.Request().GetJsonAsync<List<RestCountry>>();
                Assert.IsTrue(countryData.Count > 0);
            }
            Console.WriteLine($"Flurl.Http Get x {Repeats} : Total Milliseconds:{ (DateTime.Now - startTime).TotalMilliseconds}");
        }

        [TestMethod]
        public async Task PerformanceTestRestClientGet()
        {
            var countryCodeClient = new Client(new NewtonsoftSerializationAdapter(), new Uri(RestCountriesUrl));

            var startTime = DateTime.Now;
            List<RestCountry> countryData = await countryCodeClient.GetAsync<List<RestCountry>>();
            Console.WriteLine($"RestClient Get : Total Milliseconds:{ (DateTime.Now - startTime).TotalMilliseconds}");

            for (var i = 0; i < Repeats; i++)
            {
                countryData = await countryCodeClient.GetAsync<List<RestCountry>>();
                Assert.IsTrue(countryData.Count > 0);
            }

            Console.WriteLine($"RestClient Get x {Repeats}: Total Milliseconds:{ (DateTime.Now - startTime).TotalMilliseconds}");
        }
    }
}

#endif