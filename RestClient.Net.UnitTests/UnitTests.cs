using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClient.Net.UnitTests.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestClientDotNet.UnitTests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public async Task TestRestCountries()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("https://restcountries.eu/rest/v2/"));
            var countries = await restClient.GetAsync<List<RestCountry>>();
            Assert.IsNotNull(countries);
            Assert.IsTrue(countries.Count > 0);
        }
    }
}
