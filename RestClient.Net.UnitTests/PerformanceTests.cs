
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
        private const int Repeats = 1000;
        private const string RestCountriesUrl = "https://localhost:44337/JsonPerson/people";

        [TestMethod]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        public async Task PerformanceTestFlurlGet()
        {
            var startTime = DateTime.Now;
            var originalStartTime = DateTime.Now;
            var flurlClient = new FlurlClient(RestCountriesUrl);
            var construct = (DateTime.Now - startTime).TotalMilliseconds;

            startTime = DateTime.Now;
            var person = await flurlClient.Request().GetJsonAsync<List<Person>>();
            var timesOne = (DateTime.Now - startTime).TotalMilliseconds;

            startTime = DateTime.Now;
            for (var i = 0; i < Repeats; i++)
            {
                person = await flurlClient.Request().GetJsonAsync<List<Person>>();
                Assert.IsTrue(person != null);
            }

            var timesRepeats = (DateTime.Now - startTime).TotalMilliseconds;
            var total = (DateTime.Now - startTime).TotalMilliseconds;

            Console.WriteLine($"{construct},{timesOne},{timesRepeats},{total}");
        }

        [TestMethod]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        public async Task PerformanceTestRestClientGet()
        {
            var startTime = DateTime.Now;
            var originalStartTime = DateTime.Now;
            var countryCodeClient = new Client(new NewtonsoftSerializationAdapter(), new Uri(RestCountriesUrl));
            var construct = (DateTime.Now - startTime).TotalMilliseconds;

            startTime = DateTime.Now;
            List<Person> person = await countryCodeClient.GetAsync<List<Person>>();
            var timesOne = (DateTime.Now - startTime).TotalMilliseconds;

            for (var i = 0; i < Repeats; i++)
            {
                person = await countryCodeClient.GetAsync<List<Person>>();
                Assert.IsTrue(person != null);
            }

            var timesRepeats = (DateTime.Now - startTime).TotalMilliseconds;
            var total = (DateTime.Now - startTime).TotalMilliseconds;

            Console.WriteLine($"{construct},{timesOne},{timesRepeats},{total}");
        }
    }
}

#endif