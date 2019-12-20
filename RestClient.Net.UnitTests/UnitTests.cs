using ApiExamples;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using RestClient.Net.Samples.Model;
using RestClient.Net.UnitTests.Model;
using RestClientApiSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xml2CSharp;

namespace RestClientDotNet.UnitTests
{
    [TestClass]
    public class UnitTests
    {
        #region Fields
        private static TestServer _TestServer;
        private static HttpClient _TestServerHttpClient;
        #endregion

        #region Setup
        [TestInitialize]
        public void Initialize()
        {
            var hostBuilder = new WebHostBuilder();
            hostBuilder.UseStartup<Startup>();
            _TestServer = new TestServer(hostBuilder);
            _TestServerHttpClient = _TestServer.CreateClient();
        }
        #endregion

        #region Tests
        [TestMethod]
        public async Task TestGetRestCountries()
        {
            var tracer = new Mock<ITracer>();
            var baseUri = new Uri("https://restcountries.eu/rest/v2/");
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), baseUri, tracer.Object);
            var countries = await restClient.GetAsync<List<RestCountry>>();
            Assert.IsNotNull(countries);
            Assert.IsTrue(countries.Count > 0);

            tracer.Verify(t => t.Trace(HttpVerb.Get, baseUri, It.IsAny<Uri>(), It.IsAny<byte[]>(), TraceType.Request, null));
            tracer.Verify(t => t.Trace(HttpVerb.Get, baseUri, It.IsAny<Uri>(), It.Is<byte[]>(d => d != null && d.Length > 0), TraceType.Response, HttpStatusCode.OK));
        }

        [TestMethod]
        public async Task TestDelete()
        {
            var tracer = new Mock<ITracer>();
            var baseUri = new Uri("https://jsonplaceholder.typicode.com");
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), baseUri, tracer.Object);
            await restClient.DeleteAsync("posts/1");

            tracer.Verify(t => t.Trace(HttpVerb.Delete, baseUri, It.IsAny<Uri>(), null, TraceType.Request, null));
            tracer.Verify(t => t.Trace(HttpVerb.Delete, baseUri, It.IsAny<Uri>(), It.IsAny<byte[]>(), TraceType.Response, HttpStatusCode.OK));
        }

        [TestMethod]
        public async Task TestGetRestCountriesAsJson()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("https://restcountries.eu/rest/v2/name/australia"));
            var json = await restClient.GetAsync<string>();
            var country = JsonConvert.DeserializeObject<List<RestCountry>>(json).FirstOrDefault();
            Assert.AreEqual("Australia", country.name);
        }

        [TestMethod]
        public async Task TestGetRestCountriesNoBaseUri()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter());
            var country = (await restClient.GetAsync<List<RestCountry>>(new Uri("https://restcountries.eu/rest/v2/name/australia"))).FirstOrDefault();
            Assert.AreEqual("Australia", country.name);
        }

        [TestMethod]
        public async Task TestAbsoluteUriAsStringThrowsException()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter());
                var country = (await restClient.GetAsync<List<RestCountry>>("https://restcountries.eu/rest/v2/name/australia")).FirstOrDefault();
            }
            catch (UriFormatException ufe)
            {
                Assert.AreEqual(ufe.Message, Messages.ErrorMessageAbsoluteUriAsString);
                return;
            }

            Assert.Fail("Incorrect error message returned");
        }


        [TestMethod]
        public async Task TestPostUserWithCancellation()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"));

                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;

                var task = restClient.PostAsync<UserPost, UserPost>(new UserPost { title = "Moops" }, new Uri("/posts", UriKind.Relative), token);

                tokenSource.Cancel();

                await task;
            }
            catch (OperationCanceledException ex)
            {
                //Success
                return;
            }
            catch (Exception)
            {
                Assert.Fail("The operation threw an exception that was not an OperationCanceledException");
            }

            Assert.Fail("The operation completed successfully");
        }

        [TestMethod]
        [DataRow(HttpVerb.Patch)]
        [DataRow(HttpVerb.Post)]
        [DataRow(HttpVerb.Put)]
        public async Task TestUpdate(HttpVerb verb)
        {
            var tracer = new Mock<ITracer>();
            var baseUri = new Uri("https://jsonplaceholder.typicode.com");

            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"), tracer.Object);
            var requestUserPost = new UserPost { title = "foo", userId = 10, body = "testbody" };
            UserPost responseUserPost = null;

            var expectedStatusCode = HttpStatusCode.OK;

            switch (verb)
            {
                case HttpVerb.Patch:
                    responseUserPost = await restClient.PatchAsync<UserPost, UserPost>(requestUserPost, new Uri("/posts/1", UriKind.Relative));
                    break;
                case HttpVerb.Post:
                    responseUserPost = await restClient.PostAsync<UserPost, UserPost>(requestUserPost, "/posts");
                    expectedStatusCode = HttpStatusCode.Created;
                    break;
                case HttpVerb.Put:
                    responseUserPost = await restClient.PutAsync<UserPost, UserPost>(requestUserPost, new Uri("/posts/1", UriKind.Relative));
                    break;
            }

            Assert.AreEqual(requestUserPost.userId, responseUserPost.userId);
            Assert.AreEqual(requestUserPost.title, responseUserPost.title);

            tracer.Verify(t => t.Trace(verb, baseUri, It.IsAny<Uri>(), It.Is<byte[]>(d => d.Length > 0), TraceType.Request, null));
            tracer.Verify(t => t.Trace(verb, baseUri, It.IsAny<Uri>(), It.Is<byte[]>(d => d.Length > 0), TraceType.Response, expectedStatusCode));
        }

        [TestMethod]
        public async Task TestConsoleLogging()
        {
            var tracer = new ConsoleTracer();
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"), tracer);
            var requestUserPost = new UserPost { title = "foo", userId = 10, body = "testbody" };
            await restClient.PostAsync<UserPost, UserPost>(requestUserPost, "/posts");
        }

        [TestMethod]
        public async Task TestGetWithXmlSerialization()
        {
            var restClient = new RestClient(new XmlSerializationAdapter(), new Uri("http://www.geoplugin.net/xml.gp"));
            var geoPlugin = await restClient.GetAsync<GeoPlugin>();
            Assert.IsNotNull(geoPlugin);
        }

        [TestMethod]
        public async Task TestProtobufPostLocal()
        {
            var requestPerson = new Person
            {
                FirstName = "Bob",
                Surname = "Smith",
                BillingAddress = new Address { Street = "Test St" }
            };

            var restClient = new RestClient(new ProtobufSerializationAdapter(), new Uri("http://localhost"), default, _TestServerHttpClient);
            var responsePerson = await restClient.PostAsync<Person, Person>(requestPerson, new Uri("http://localhost:42908/person"));
            Assert.AreEqual(requestPerson.BillingAddress.Street, responsePerson.BillingAddress.Street);
        }

        [TestMethod]
        public async Task TestHeadersGet()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://localhost"), default, _TestServerHttpClient);
            restClient.Headers.Add("Test", "Test");
            var responsePerson = await restClient.GetAsync<Person>("headers");
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersPost()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://localhost"), default, _TestServerHttpClient);
            restClient.Headers.Add("Test", "Test");
            var responsePerson = await restClient.PostAsync<Person, Person>(new Person { FirstName = "Bob" }, new Uri("headers", UriKind.Relative));
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersIncorrectGet()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://localhost"), default, _TestServerHttpClient);

                //The server expects the value of "Test"
                restClient.Headers.Add("Test", "Tests");

                var responsePerson = await restClient.GetAsync<Person>(new Uri("headers", UriKind.Relative));
                Assert.Fail();
            }
            catch(Exception ex)
            {
                //TODO: This is not a good test. The server is throwing a simple exception but we should be handling a HttpStatusException here. 
                //This is only the case because it's use a Test HttpClient

                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public async Task TestHeadersIncorrectPost()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://localhost"), default, _TestServerHttpClient);

                //The server expects the value of "Test"
                restClient.Headers.Add("Test", "Tests");

                var responsePerson = await restClient.PostAsync<Person, Person>(new Person(), new Uri("headers", UriKind.Relative));
                Assert.Fail();
            }
            catch (Exception ex)
            {
                //TODO: This is not a good test. The server is throwing a simple exception but we should be handling a HttpStatusException here. 
                //This is only the case because it's use a Test HttpClient

                return;
            }

            Assert.Fail();
        }
        #endregion




    }


}
