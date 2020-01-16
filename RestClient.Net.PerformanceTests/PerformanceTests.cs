
#if (NETCOREAPP3_1)

using ApiExamples.Model.JsonModel;
using Flurl.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestClient.Net.Abstractions.Extensions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Net.PerformanceTests
{
    [TestClass]
    public class PerformanceTests
    {
        #region Misc
        [AssemblyInitialize]
        public static void Initialize(TestContext testContext)
        {
            //Load all the assemblies in to the app domain so this loading doesn't skew results
            var flurlClient = new FlurlClient(PeopleUrl);
            var countryCodeClient = new Client(new NewtonsoftSerializationAdapter(), new Uri(PeopleUrl));
            var restSharpClient = new RestSharp.RestClient(PeopleUrl);
            var dalSoftClient = new DalSoft.RestClient.RestClient(PeopleUrl);
            var personJson = JsonConvert.SerializeObject(new Person());
            personJson = System.Text.Json.JsonSerializer.Serialize(new Person());
        }

        private const int Repeats = 250;
        private const string PeopleUrl = "https://localhost:44337/JsonPerson/people";
        private const string Path = "Results.csv";
        private static FileStream stream;

        static PerformanceTests()
        {
            if (File.Exists(Path)) File.Delete(Path);
            stream = new FileStream(Path, FileMode.Append);
            WriteText("Client,Method,First Call,All Calls,Total\r\n");
        }

        private static void WriteText(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }

        [AssemblyCleanup()]
        public static void AssemblyCleanup() 
        {
            stream.Close();
        }
        #endregion

        #region Flurl
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

            var message = $"Flurl,GET,{timesOne},{timesRepeats},{total}\r\n";
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
        public async Task TestPostFlurl()
        {
            var startTime = DateTime.Now;
            var originalStartTime = DateTime.Now;
            var countryCodeClient = new FlurlClient(PeopleUrl);

            var peopleRequest = new List<Person>();
            for (var i = 0; i < 10; i++)
            {
                peopleRequest.Add(new Person { FirstName = "Test" + i });
            }

            startTime = DateTime.Now;
            var people = await ReadPostResponseAsync(countryCodeClient, peopleRequest);
            var timesOne = (DateTime.Now - startTime).TotalMilliseconds;

            for (var i = 0; i < Repeats; i++)
            {
                people = await ReadPostResponseAsync(countryCodeClient, peopleRequest);
                Assert.IsTrue(people != null);
                Assert.IsTrue(people.Count > 0);
            }

            var timesRepeats = (DateTime.Now - startTime).TotalMilliseconds;
            var total = (DateTime.Now - originalStartTime).TotalMilliseconds;

            var message = $"Flurl,POST,{timesOne},{timesRepeats},{total}\r\n";
            WriteText(message);
            Console.WriteLine(message);
        }

        private static async Task<List<Person>> ReadPostResponseAsync(FlurlClient countryCodeClient, List<Person> peopleRequest)
        {
            var response = await countryCodeClient.Request().PostJsonAsync(peopleRequest);
            var json = await response.Content.ReadAsStringAsync();
            var people = JsonConvert.DeserializeObject<List<Person>>(json);
            return people;
        }
        #endregion

        #region RestClient.Net
        [TestMethod]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        [DataRow]
        public async Task TestGetRestClientNewtonSoft()
        {
            var startTime = DateTime.Now;
            var originalStartTime = DateTime.Now;
            var countryCodeClient = new Client(new NewtonsoftSerializationAdapter(), new Uri(PeopleUrl));

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

            var message = $"RestClient.Net Newtonsoft,GET,{timesOne},{timesRepeats},{total}\r\n";
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

            var message = $"RestClient.Net,GET,{timesOne},{timesRepeats},{total}\r\n";
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
                peopleRequest.Add(new Person { FirstName = "Test" + i });
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

            var message = $"RestClient.Net,POST,{timesOne},{timesRepeats},{total}\r\n";
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
        public async Task TestPostRestClientNewtonsoft()
        {
            var startTime = DateTime.Now;
            var originalStartTime = DateTime.Now;
            var countryCodeClient = new Client(new NewtonsoftSerializationAdapter(), new Uri(PeopleUrl));
            countryCodeClient.SetJsonContentTypeHeader();

            var peopleRequest = new List<Person>();
            for (var i = 0; i < 10; i++)
            {
                peopleRequest.Add(new Person { FirstName = "Test" + i });
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

            var message = $"RestClient.Net Newtonsoft,POST,{timesOne},{timesRepeats},{total}\r\n";
            WriteText(message);
            Console.WriteLine(message);
        }
        #endregion

        #region RestSharp
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

            var message = $"RestSharp,GET,{timesOne},{timesRepeats},{total}\r\n";
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
        public async Task TestPostRestSharp()
        {
            var startTime = DateTime.Now;
            var originalStartTime = DateTime.Now;
            var countryCodeClient = new RestSharp.RestClient(new Uri(PeopleUrl));

            var peopleRequest = new List<Person>();
            for (var i = 0; i < 10; i++)
            {
                peopleRequest.Add(new Person { FirstName = "Test" + i });
            }

            startTime = DateTime.Now;
            var peopleRestRequest = new RestRequest { Method = Method.POST, Body = new RequestBody("application/json", "Person", peopleRequest) };
            var people = await countryCodeClient.ExecutePostTaskAsync<List<Person>>(peopleRestRequest);
            var timesOne = (DateTime.Now - startTime).TotalMilliseconds;

            for (var i = 0; i < Repeats; i++)
            {
                people = await countryCodeClient.ExecutePostTaskAsync<List<Person>>(peopleRestRequest);
                Assert.IsTrue(people != null);
                Assert.IsTrue(people.Data.Count > 0);
            }

            var timesRepeats = (DateTime.Now - startTime).TotalMilliseconds;
            var total = (DateTime.Now - originalStartTime).TotalMilliseconds;

            var message = $"RestSharp,POST,{timesOne},{timesRepeats},{total}\r\n";
            WriteText(message);
            Console.WriteLine(message);
        }
        #endregion

        #region DALSoft
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

            var message = $"DalSoft,GET,{timesOne},{timesRepeats},{total}\r\n";
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
        public async Task TestPostDALSoft()
        {
            var startTime = DateTime.Now;
            var originalStartTime = DateTime.Now;
            var countryCodeClient = new DalSoft.RestClient.RestClient(PeopleUrl);

            var peopleRequest = new List<Person>();
            for (var i = 0; i < 10; i++)
            {
                peopleRequest.Add(new Person { FirstName = "Test" + i });
            }

            startTime = DateTime.Now;
            var people = await countryCodeClient.Post<List<Person>, List<Person>>(peopleRequest);
            var timesOne = (DateTime.Now - startTime).TotalMilliseconds;

            for (var i = 0; i < Repeats; i++)
            {
                people = await countryCodeClient.Post<List<Person>, List<Person>>(peopleRequest);
                Assert.IsTrue(people != null);
                Assert.IsTrue(people.Count > 0);
            }

            var timesRepeats = (DateTime.Now - startTime).TotalMilliseconds;
            var total = (DateTime.Now - originalStartTime).TotalMilliseconds;

            var message = $"DalSoft,POST,{timesOne},{timesRepeats},{total}\r\n";
            WriteText(message);
            Console.WriteLine(message);
        }
        #endregion
    }
}

#endif