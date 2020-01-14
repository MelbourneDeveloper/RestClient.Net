
#if (NETCOREAPP3_1)

using ApiExamples.Model.JsonModel;
using Flurl.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using RestClientApiSamples;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Net.PerformanceTests
{


    [TestClass]
    public class PerformanceTests : IDisposable
    {
        private const int Repeats = 500;
        private const string PeopleUrl = "https://localhost:44337/JsonPerson/people";

        FileStream stream;

        public PerformanceTests()
        {
            stream = new FileStream("Results.csv", FileMode.Append);
        }

        private void WriteText(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }

        [TestMethod]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        public async Task TestGetFlurl()
        {
            var startTime = DateTime.Now;
            var originalStartTime = DateTime.Now;
            var flurlClient = new FlurlClient(PeopleUrl);

            startTime = DateTime.Now;
            var people = await flurlClient.Request().GetJsonAsync<List<Person>>();
            var timesOne = (DateTime.Now - startTime).TotalMilliseconds;

            startTime = DateTime.Now;
            for (var i = 0; i < Repeats; i++)
            {
                people = await flurlClient.Request().GetJsonAsync<List<Person>>();
                Assert.IsTrue(people != null);
                Assert.IsTrue(people.Count > 0);
            }

            var timesRepeats = (DateTime.Now - startTime).TotalMilliseconds;
            var total = (DateTime.Now - originalStartTime).TotalMilliseconds;

            var message = $"Flurl,{timesOne},{timesRepeats},{total}\r\n";
            WriteText(message);
            Console.WriteLine(message);
        }

        [TestMethod]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        public async Task TestGetRestClient()
        {
            var startTime = DateTime.Now;
            var originalStartTime = DateTime.Now;
            var countryCodeClient = new Client(new Uri(PeopleUrl));

            startTime = DateTime.Now;
            List<Person> people = await countryCodeClient.GetAsync<List<Person>>();
            var timesOne = (DateTime.Now - startTime).TotalMilliseconds;

            for (var i = 0; i < Repeats; i++)
            {
                people = await countryCodeClient.GetAsync<List<Person>>();
                Assert.IsTrue(people != null);
                Assert.IsTrue(people.Count > 0);
            }

            var timesRepeats = (DateTime.Now - startTime).TotalMilliseconds;
            var total = (DateTime.Now - originalStartTime).TotalMilliseconds;

            var message = $"RestClient.Net,{timesOne},{timesRepeats},{total}\r\n";
            WriteText(message);
            Console.WriteLine(message);
        }

        [TestMethod]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        public async Task TestPostRestClient()
        {
            var startTime = DateTime.Now;
            var originalStartTime = DateTime.Now;
            var countryCodeClient = new Client(new Uri(PeopleUrl));

            var peopleRequest = new List<Person>();
            for (var i = 0; i < 10; i++)
            {
                peopleRequest.Add(new Person { firstName = "Test" + i });
            }

            startTime = DateTime.Now;
            List<Person> people = await countryCodeClient.PostAsync<List<Person>, List<Person>>(peopleRequest);
            var timesOne = (DateTime.Now - startTime).TotalMilliseconds;

            for (var i = 0; i < Repeats; i++)
            {
                people = await countryCodeClient.PostAsync<List<Person>, List<Person>>(peopleRequest);
                Assert.IsTrue(people != null);
                Assert.IsTrue(people.Count > 0);
            }

            var timesRepeats = (DateTime.Now - startTime).TotalMilliseconds;
            var total = (DateTime.Now - originalStartTime).TotalMilliseconds;

            var message = $"RestClient.Net,{timesOne},{timesRepeats},{total}\r\n";
            WriteText(message);
            Console.WriteLine(message);
        }

        [TestMethod]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        public async Task TestGetRestSharp()
        {
            var startTime = DateTime.Now;
            var originalStartTime = DateTime.Now;
            var countryCodeClient = new RestSharp.RestClient(PeopleUrl);

            startTime = DateTime.Now;
            var people = await countryCodeClient.ExecuteGetTaskAsync<List<Person>>(new RestRequest { Method = Method.GET });
            var timesOne = (DateTime.Now - startTime).TotalMilliseconds;

            for (var i = 0; i < Repeats; i++)
            {
                people = await countryCodeClient.ExecuteGetTaskAsync<List<Person>>(new RestRequest { Method = Method.GET });
                Assert.IsTrue(people != null);
                Assert.IsTrue(people.Data.Count > 0);
            }

            var timesRepeats = (DateTime.Now - startTime).TotalMilliseconds;
            var total = (DateTime.Now - originalStartTime).TotalMilliseconds;

            var message = $"RestSharp,{timesOne},{timesRepeats},{total}\r\n";
            WriteText(message);
            Console.WriteLine(message);
        }

        [TestMethod]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        public async Task TestGetDALSoft()
        {
            var startTime = DateTime.Now;
            var originalStartTime = DateTime.Now;
            var countryCodeClient = new DalSoft.RestClient.RestClient(PeopleUrl);

            startTime = DateTime.Now;
            var people = await countryCodeClient.Get<List<Person>>();
            var timesOne = (DateTime.Now - startTime).TotalMilliseconds;

            for (var i = 0; i < Repeats; i++)
            {
                people = await countryCodeClient.Get<List<Person>>();
                Assert.IsTrue(people != null);
                Assert.IsTrue(people.Count > 0);
            }

            var timesRepeats = (DateTime.Now - startTime).TotalMilliseconds;
            var total = (DateTime.Now - originalStartTime).TotalMilliseconds;

            var message = $"RestSharp,{timesOne},{timesRepeats},{total}\r\n";
            WriteText(message);
            Console.WriteLine(message);
        }


        public void Dispose()
        {
            stream.Dispose();
        }
    }
}

#endif