
#if (NETCOREAPP3_1)

using Flurl.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClient.Net.Samples.Model;
using RestClient.Net.UnitTests.Model;
using RestClientApiSamples;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{
    [TestClass]
    public class PerformanceTests
    {
        private const int Repeats = 8;
        private const string RestCountriesUrl = "https://localhost:44337/JsonPerson?personKey=10";

        [TestMethod]
        public async Task PerformanceTestFlurlGet()
        {
            var flurlClient = new FlurlClient(RestCountriesUrl);

            var startTime = DateTime.Now;
            var person = await flurlClient.Request().GetJsonAsync<Person>();
            Console.WriteLine($"Flurl.Http Get : Total Milliseconds:{ (DateTime.Now - startTime).TotalMilliseconds}");

            startTime = DateTime.Now;
            for (var i = 0; i < Repeats; i++)
            {
                person = await flurlClient.Request().GetJsonAsync<Person>();
                Assert.IsTrue(person != null);
            }
            Console.WriteLine($"Flurl.Http Get x {Repeats} : Total Milliseconds:{ (DateTime.Now - startTime).TotalMilliseconds}");
        }

        [TestMethod]
        public async Task PerformanceTestRestClientGet()
        {
            var countryCodeClient = new Client(new NewtonsoftSerializationAdapter(), new Uri(RestCountriesUrl));

            var startTime = DateTime.Now;
            Person person = await countryCodeClient.GetAsync<Person>();
            Console.WriteLine($"RestClient Get : Total Milliseconds:{ (DateTime.Now - startTime).TotalMilliseconds}");

            for (var i = 0; i < Repeats; i++)
            {
                person = await countryCodeClient.GetAsync<Person>();
                Assert.IsTrue(person != null);
            }

            Console.WriteLine($"RestClient Get x {Repeats}: Total Milliseconds:{ (DateTime.Now - startTime).TotalMilliseconds}");
        }
    }
}

#endif