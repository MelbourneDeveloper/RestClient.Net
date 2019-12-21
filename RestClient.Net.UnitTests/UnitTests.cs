using ApiExamples;
using ApiExamples.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using RestClient.Net.Samples.Model;
using RestClient.Net.UnitTests.Model;
using RestClientApiSamples;
using RestClientDotNet.Abstractions;
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
        private static TestServer _testServer;
        private static HttpClient _testServerHttpClient;
        private static Mock<ITracer> _tracer;
        #endregion

        #region Setup
        [TestInitialize]
        public void Initialize()
        {
            var hostBuilder = new WebHostBuilder();
            hostBuilder.UseStartup<Startup>();
            _testServer = new TestServer(hostBuilder);
            _testServerHttpClient = _testServer.CreateClient();
            _tracer = new Mock<ITracer>();
        }
        #endregion

        #region Tests
        [TestMethod]
        public async Task TestGetRestCountries()
        {
            var tracer = new Mock<ITracer>();
            var baseUri = new Uri("https://restcountries.eu/rest/v2/");
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), baseUri, tracer.Object);
            List<RestCountry> countries = await restClient.GetAsync<List<RestCountry>>();
            Assert.IsNotNull(countries);
            Assert.IsTrue(countries.Count > 0);

            tracer.Verify(t => t.Trace(HttpVerb.Get, baseUri, It.IsAny<Uri>(), It.IsAny<byte[]>(), TraceType.Request, null, It.IsAny<IRestHeadersCollection>()));
            tracer.Verify(t => t.Trace(HttpVerb.Get, baseUri, It.IsAny<Uri>(), It.Is<byte[]>(d => d != null && d.Length > 0), TraceType.Response, HttpStatusCode.OK, It.IsAny<IRestHeadersCollection>()));
        }

        [TestMethod]
        public async Task TestDelete()
        {
            var tracer = new Mock<ITracer>();
            var baseUri = new Uri("https://jsonplaceholder.typicode.com");
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), baseUri, tracer.Object);
            await restClient.DeleteAsync("posts/1");

            tracer.Verify(t => t.Trace(HttpVerb.Delete, baseUri, It.IsAny<Uri>(), null, TraceType.Request, null, It.IsAny<IRestHeadersCollection>()));
            tracer.Verify(t => t.Trace(HttpVerb.Delete, baseUri, It.IsAny<Uri>(), It.IsAny<byte[]>(), TraceType.Response, HttpStatusCode.OK, It.IsAny<IRestHeadersCollection>()));
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
            List<RestCountry> countries = await restClient.GetAsync<List<RestCountry>>(new Uri("https://restcountries.eu/rest/v2/name/australia"));
            var country = countries.FirstOrDefault();
            Assert.AreEqual("Australia", country.name);
        }

        [TestMethod]
        public async Task TestAbsoluteUriAsStringThrowsException()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter());
                List<RestCountry> countries = await restClient.GetAsync<List<RestCountry>>("https://restcountries.eu/rest/v2/name/australia");
                var country = countries.FirstOrDefault();
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
            var baseUri = new Uri("https://jsonplaceholder.typicode.com");

            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"), _tracer.Object);
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

            _tracer.Verify(t => t.Trace(verb, baseUri, It.IsAny<Uri>(), It.Is<byte[]>(d => d.Length > 0), TraceType.Request, null, It.IsAny<IRestHeadersCollection>()));
            _tracer.Verify(t => t.Trace(verb, baseUri, It.IsAny<Uri>(), It.Is<byte[]>(d => d.Length > 0), TraceType.Response, expectedStatusCode, It.IsAny<IRestHeadersCollection>()));
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

            var restClient = new RestClient(new ProtobufSerializationAdapter(), new Uri("http://localhost"), default, _testServerHttpClient);
            var responsePerson = await restClient.PostAsync<Person, Person>(requestPerson, new Uri("http://localhost:42908/person"));
            Assert.AreEqual(requestPerson.BillingAddress.Street, responsePerson.Body.BillingAddress.Street);
        }

        [TestMethod]
        public async Task TestProtobufPutWithHeaderLocal()
        {
            var requestPerson = new Person
            {
                FirstName = "Bob",
                Surname = "Smith",
                BillingAddress = new Address { Street = "Test St" }
            };

            var restClient = new RestClient(new ProtobufSerializationAdapter(), new Uri("http://localhost"), default, _testServerHttpClient);
            const string personKey = "123";
            restClient.DefaultRequestHeaders.Add("PersonKey", personKey);
            Person responsePerson = await restClient.PutAsync<Person, Person>(requestPerson, new Uri("http://localhost:42908/person"));
            Assert.AreEqual(requestPerson.BillingAddress.Street, responsePerson.BillingAddress.Street);
            Assert.AreEqual(personKey, responsePerson.PersonKey);
        }

        [TestMethod]
        public async Task TestHeadersLocalGet()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://localhost"), default, _testServerHttpClient);
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            Person responsePerson = await restClient.GetAsync<Person>("headers");
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersResponseLocalGet()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://localhost"), default, _testServerHttpClient);
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            var response = await restClient.GetAsync<Person>("headers");

            Assert.IsTrue(response.Headers.Contains("Test1"));
            Assert.IsTrue(response.Headers.Contains("Test2"));
            Assert.IsFalse(response.Headers.Contains("Test3"));

            var header1 = response.Headers["Test1"].ToList();
            var header2 = response.Headers["Test2"].ToList();

            Assert.IsNotNull(header1);
            Assert.IsNotNull(header2);

            Assert.AreEqual(1, header1.Count);
            Assert.AreEqual(2, header2.Count);

            Assert.AreEqual("b", header2[1]);
        }

        /// <summary>
        /// TODO: PUT, POST, PATCH etc.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestHeadersTraceLocalGet()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://localhost"), default, _testServerHttpClient, _tracer.Object);
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            var response = await restClient.GetAsync<Person>("headers");

            _tracer.Verify(t => t.Trace(HttpVerb.Get, It.IsAny<Uri>(), It.IsAny<Uri>(), It.IsAny<byte[]>(), TraceType.Request, null,
                It.Is<RestRequestHeadersCollection>(c => CheckRequestHeaders(c))
                ));

            _tracer.Verify(t => t.Trace(HttpVerb.Get, It.IsAny<Uri>(), It.IsAny<Uri>(), It.IsAny<byte[]>(), TraceType.Response, It.IsAny<HttpStatusCode?>(),
                It.Is<RestResponseHeadersCollection>(c => CheckResponseHeaders(c))
                ));
        }

        [TestMethod]
        public async Task TestHeadersLocalPost()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://localhost"), default, _testServerHttpClient);
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            var responsePerson = await restClient.PostAsync<Person, Person>(new Person { FirstName = "Bob" }, new Uri("headers", UriKind.Relative));
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectGet()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://localhost"), default, _testServerHttpClient);

                //The server expects the value of "Test"
                restClient.DefaultRequestHeaders.Add("Test", "Tests");

                var responsePerson = await restClient.GetAsync<Person>(new Uri("headers", UriKind.Relative));
                Assert.Fail();
            }
            catch (Exception ex)
            {
                //TODO: This is not a good test. The server is throwing a simple exception but we should be handling a HttpStatusException here. 
                //This is only the case because it's use a Test HttpClient
                Assert.AreEqual(HeadersController.ExceptionMessage, ex.Message);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectPost()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://localhost"), default, _testServerHttpClient);

                //The server expects the value of "Test"
                restClient.DefaultRequestHeaders.Add("Test", "Tests");

                var responsePerson = await restClient.PostAsync<Person, Person>(new Person(), new Uri("headers", UriKind.Relative));
                Assert.Fail();
            }
            catch (Exception ex)
            {
                //TODO: This is not a good test. The server is throwing a simple exception but we should be handling a HttpStatusException here. 
                //This is only the case because it's use a Test HttpClient
                Assert.AreEqual(HeadersController.ExceptionMessage, ex.Message);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public async Task TestHeadersLocalPut()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://localhost"), default, _testServerHttpClient);
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            var responsePerson = await restClient.PutAsync<Person, Person>(new Person { FirstName = "Bob" }, new Uri("headers", UriKind.Relative));
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectPut()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://localhost"), default, _testServerHttpClient);

                //The server expects the value of "Test"
                restClient.DefaultRequestHeaders.Add("Test", "Tests");

                var responsePerson = await restClient.PutAsync<Person, Person>(new Person(), new Uri("headers", UriKind.Relative));
                Assert.Fail();
            }
            catch (Exception ex)
            {
                //TODO: This is not a good test. The server is throwing a simple exception but we should be handling a HttpStatusException here. 
                //This is only the case because it's use a Test HttpClient
                Assert.AreEqual(HeadersController.ExceptionMessage, ex.Message);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public async Task TestHeadersLocalPatch()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://localhost"), default, _testServerHttpClient);
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            var responsePerson = await restClient.PatchAsync<Person, Person>(new Person { FirstName = "Bob" }, new Uri("headers", UriKind.Relative));
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectPatch()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://localhost"), default, _testServerHttpClient);

                //The server expects the value of "Test"
                restClient.DefaultRequestHeaders.Add("Test", "Tests");

                var responsePerson = await restClient.PatchAsync<Person, Person>(new Person(), new Uri("headers", UriKind.Relative));
                Assert.Fail();
            }
            catch (Exception ex)
            {
                //TODO: This is not a good test. The server is throwing a simple exception but we should be handling a HttpStatusException here. 
                //This is only the case because it's use a Test HttpClient
                Assert.AreEqual(HeadersController.ExceptionMessage, ex.Message);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public async Task TestHeadersLocalDelete()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://localhost"), default, _testServerHttpClient);
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            await restClient.DeleteAsync(new Uri("headers/1", UriKind.Relative));
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectDelete()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://localhost"), default, _testServerHttpClient);
                await restClient.DeleteAsync(new Uri("headers/1", UriKind.Relative));
                Assert.Fail();
            }
            catch (Exception ex)
            {
                //TODO: This is not a good test. The server is throwing a simple exception but we should be handling a HttpStatusException here. 
                //This is only the case because it's use a Test HttpClient
                Assert.AreEqual(HeadersController.ExceptionMessage, ex.Message);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public async Task TestErrorsLocalGet()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://localhost"), default, _testServerHttpClient);
            restClient.ThrowExceptionOnFailure = false;
            var response = await restClient.GetAsync<string>("error");
            var httpResponseMessage = response.UnderlyingResponse as HttpResponseMessage;
            var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
        }

        //TODO: Test error models

        //TODO: Test exceptions

        #endregion

        #region Helpers
        private static bool CheckRequestHeaders(RestRequestHeadersCollection restRequestHeadersCollection)
        {
            return restRequestHeadersCollection.Contains("Test") && restRequestHeadersCollection["Test"].First() == "Test";
        }

        private static bool CheckResponseHeaders(RestResponseHeadersCollection restResponseHeadersCollection)
        {
            return restResponseHeadersCollection.Contains("Test1") && restResponseHeadersCollection["Test1"].First() == "a";
        }
        #endregion
    }
}
