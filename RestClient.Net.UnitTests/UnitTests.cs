using ApiExamples;
using ApiExamples.Controllers;
using ApiExamples.Model;
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
        private static TestClientFactory _testServerHttpClientFactory;
        private static Mock<ITracer> _tracer;
        #endregion

        #region Setup
        [TestInitialize]
        public void Initialize()
        {
            var hostBuilder = new WebHostBuilder();
            hostBuilder.UseStartup<Startup>();
            _testServer = new TestServer(hostBuilder);
            var testClient = _testServer.CreateClient();
            _testServerHttpClientFactory = new TestClientFactory(testClient);
            _tracer = new Mock<ITracer>();
        }
        #endregion

        #region Tests

        #region External Api Tests
        [TestMethod]
        public async Task TestGetRestCountries()
        {
            var tracer = new Mock<ITracer>();
            var baseUri = new Uri("https://restcountries.eu/rest/v2/");
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), baseUri, tracer.Object);
            List<RestCountry> countries = await restClient.GetAsync<List<RestCountry>>();
            Assert.IsNotNull(countries);
            Assert.IsTrue(countries.Count > 0);

            tracer.Verify(t => t.Trace(HttpVerb.Get, baseUri, It.IsAny<Uri>(), It.IsAny<byte[]>(), TraceType.Request, null, It.IsAny<IRestHeaders>()));
            tracer.Verify(t => t.Trace(HttpVerb.Get, baseUri, It.IsAny<Uri>(), It.Is<byte[]>(d => d != null && d.Length > 0), TraceType.Response, (int)HttpStatusCode.OK, It.IsAny<IRestHeaders>()));
        }

        [TestMethod]
        public async Task TestDelete()
        {
            var tracer = new Mock<ITracer>();
            var baseUri = new Uri("https://jsonplaceholder.typicode.com");
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), baseUri, tracer.Object);
            await restClient.DeleteAsync("posts/1");

            tracer.Verify(t => t.Trace(HttpVerb.Delete, baseUri, It.IsAny<Uri>(), null, TraceType.Request, null, It.IsAny<IRestHeaders>()));
            tracer.Verify(t => t.Trace(HttpVerb.Delete, baseUri, It.IsAny<Uri>(), It.IsAny<byte[]>(), TraceType.Response, (int)HttpStatusCode.OK, It.IsAny<IRestHeaders>()));
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
        public async Task TestPostUserTimeout()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"), new TimeSpan(0, 0, 0, 0, 1));
                await restClient.PostAsync<UserPost, UserPost>(new UserPost { title = "Moops" }, new Uri("/posts", UriKind.Relative));
            }
            catch (TaskCanceledException ex)
            {
                //Success
                return;
            }
            catch (Exception)
            {
                Assert.Fail("The operation threw an exception that was not an TaskCanceledException");
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

            _tracer.Verify(t => t.Trace(verb, baseUri, It.IsAny<Uri>(), It.Is<byte[]>(d => d.Length > 0), TraceType.Request, null, It.IsAny<IRestHeaders>()));
            _tracer.Verify(t => t.Trace(verb, baseUri, It.IsAny<Uri>(), It.Is<byte[]>(d => d.Length > 0), TraceType.Response, (int)expectedStatusCode, It.IsAny<IRestHeaders>()));
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
        #endregion

        #region Local Protobuf
        [TestMethod]
        public async Task TestProtobufPostLocal()
        {
            var requestPerson = new Person
            {
                FirstName = "Bob",
                Surname = "Smith",
                BillingAddress = new Address { Street = "Test St" }
            };

            var restClient = new RestClient(new ProtobufSerializationAdapter(), _testServerHttpClientFactory);
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

            var restClient = new RestClient(new ProtobufSerializationAdapter(), _testServerHttpClientFactory);
            const string personKey = "123";
            restClient.DefaultRequestHeaders.Add("PersonKey", personKey);
            Person responsePerson = await restClient.PutAsync<Person, Person>(requestPerson, new Uri("http://localhost:42908/person"));
            Assert.AreEqual(requestPerson.BillingAddress.Street, responsePerson.BillingAddress.Street);
            Assert.AreEqual(personKey, responsePerson.PersonKey);
        }
        #endregion

        #region Local Headers
        [TestMethod]
        public async Task TestHeadersLocalGet()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            Person responsePerson = await restClient.GetAsync<Person>("headers");
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersResponseLocalGet()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
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
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory, _tracer.Object, null, default, null);
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            var response = await restClient.GetAsync<Person>("headers");

            _tracer.Verify(t => t.Trace(HttpVerb.Get, It.IsAny<Uri>(), It.IsAny<Uri>(), It.IsAny<byte[]>(), TraceType.Request, null,
                It.Is<IRestHeaders>(c => CheckRequestHeaders(c))
                ));

            _tracer.Verify(t => t.Trace(HttpVerb.Get, It.IsAny<Uri>(), It.IsAny<Uri>(), It.IsAny<byte[]>(), TraceType.Response, It.IsAny<int?>(),
                It.Is<RestResponseHeaders>(c => CheckResponseHeaders(c))
                ));
        }

        [TestMethod]
        public async Task TestHeadersLocalPost()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            var responsePerson = await restClient.PostAsync<Person, Person>(new Person { FirstName = "Bob" }, new Uri("headers", UriKind.Relative));
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectGet()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);

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
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);

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
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            var responsePerson = await restClient.PutAsync<Person, Person>(new Person { FirstName = "Bob" }, new Uri("headers", UriKind.Relative));
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalPutStringOverload()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            var responsePerson = await restClient.PutAsync<Person, Person>(new Person { FirstName = "Bob" }, "headers");
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectPut()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);

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
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            var responsePerson = await restClient.PatchAsync<Person, Person>(new Person { FirstName = "Bob" }, new Uri("headers", UriKind.Relative));
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectPatch()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);

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
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            await restClient.DeleteAsync(new Uri("headers/1", UriKind.Relative));
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectDelete()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
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
        #endregion

        #region Local Headers In RestRequest
        [TestMethod]
        public async Task TestHeadersLocalInRestRequest()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
            var restRequestHeaders = new RestRequestHeaders();
            restRequestHeaders.Add("Test", "Test");
            Person responsePerson = await restClient.SendAsync<Person, object>
                (
                new RestRequest<object>(new Uri("headers", UriKind.Relative), null, restRequestHeaders, HttpVerb.Get, restClient, null, default)
                ); ;
            Assert.IsNotNull(responsePerson);
        }
        #endregion

        #region Local Errors
        [TestMethod]
        public async Task TestErrorsLocalGet()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
            restClient.ThrowExceptionOnFailure = false;
            var response = (RestResponse<Person>)await restClient.GetAsync<Person>("error");
            Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
            var apiResult = await restClient.DeserializeResponseBodyAsync<ApiResult>(response);
            Assert.AreEqual(ErrorController.ErrorMessage, apiResult.Errors.First());

            //Check that the response values are getting set correctly
            Assert.AreEqual(response.BaseUri, response.BaseUri);
            Assert.AreEqual(HttpVerb.Get, response.HttpVerb);
            Assert.AreEqual(new Uri("error", UriKind.Relative), response.Resource);
        }

        [TestMethod]
        public async Task TestErrorsLocalGetThrowException()
        {
            RestClient restClient = null;
            try
            {
                restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
                var response = await restClient.GetAsync<Person>("error");
                Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
            }
            catch (HttpStatusException hex)
            {
                var apiResult = await restClient.DeserializeResponseBodyAsync<ApiResult>(hex.RestResponse);
                Assert.AreEqual(ErrorController.ErrorMessage, apiResult.Errors.First());
                return;
            }

            Assert.Fail();
        }
        #endregion

        #region Local Authentication
        [TestMethod]
        public async Task TestBasicAuthenticationLocal()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
            restClient.UseBasicAuthentication("Bob", "ANicePassword");
            Person person = await restClient.GetAsync<Person>(new Uri("secure/basic", UriKind.Relative));
            Assert.AreEqual("Sam", person.FirstName);
        }

        [TestMethod]
        public async Task TestBasicAuthenticationLocalWithError()
        {
            RestClient restClient = null;
            try
            {
                restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
                restClient.UseBasicAuthentication("Bob", "WrongPassword");
                Person person = await restClient.GetAsync<Person>(new Uri("secure/basic", UriKind.Relative));
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.Unauthorized, hex.RestResponse.StatusCode);
                var apiResult = await restClient.DeserializeResponseBodyAsync<ApiResult>(hex.RestResponse);
                Assert.AreEqual(SecureController.NotAuthorizedMessage, apiResult.Errors.First());
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public async Task TestBasicAuthenticationPostLocal()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
            restClient.UseBasicAuthentication("Bob", "ANicePassword");
            Person person = await restClient.PostAsync<Person, Person>(new Person { FirstName = "Sam" }, new Uri("secure/basic", UriKind.Relative));
            Assert.AreEqual("Sam", person.FirstName);
        }

        [TestMethod]
        public async Task TestBasicAuthenticationPostLocalWithError()
        {
            RestClient restClient = null;
            try
            {
                restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
                restClient.UseBasicAuthentication("Bob", "WrongPassword");
                Person person = await restClient.PostAsync<Person, Person>(new Person { FirstName = "Sam" }, new Uri("secure/basic", UriKind.Relative));
            }
            catch (HttpStatusException ex)
            {
                Assert.AreEqual((int)HttpStatusCode.Unauthorized, ex.RestResponse.StatusCode);
                var apiResult = await restClient.DeserializeResponseBodyAsync<ApiResult>(ex.RestResponse);
                Assert.AreEqual(SecureController.NotAuthorizedMessage, apiResult.Errors.First());
                return;
            }
            Assert.Fail();
        }
        #endregion

        #region All Extension Overloads

        #region Get
        [TestMethod]
        public async Task TestLocalGetNoArgs()
        {
            var restClient = GetJsonClient(new Uri("http://localhost/JsonPerson"));
            Person responsePerson = await restClient.GetAsync<Person>();
            Assert.IsNotNull(responsePerson);
            Assert.IsNotNull("Sam", responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalGetStringUri()
        {
            var restClient = GetJsonClient();
            Person responsePerson = await restClient.GetAsync<Person>("JsonPerson");
            Assert.IsNotNull(responsePerson);
            Assert.IsNotNull("Sam", responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalGetUri()
        {
            var restClient = GetJsonClient();
            Person responsePerson = await restClient.GetAsync<Person>(new Uri("JsonPerson", UriKind.Relative));
            Assert.IsNotNull(responsePerson);
            Assert.IsNotNull("Sam", responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalGetUriCancellationToken()
        {
            var restClient = GetJsonClient();
            Person responsePerson = await restClient.GetAsync<Person>(new Uri("JsonPerson", UriKind.Relative), new CancellationToken());
            Assert.IsNotNull(responsePerson);
            Assert.IsNotNull("Sam", responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalGetUriContentType()
        {
            var restClient = GetJsonClient();
            Person responsePerson = await restClient.GetAsync<Person>(new Uri("JsonPerson", UriKind.Relative), "application/json");
            Assert.IsNotNull(responsePerson);
            Assert.IsNotNull("Sam", responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalGetUriContentTypeCancellationToken()
        {
            var restClient = GetJsonClient();
            Person responsePerson = await restClient.GetAsync<Person>(new Uri("JsonPerson", UriKind.Relative), "application/json", new CancellationToken());
            Assert.IsNotNull(responsePerson);
            Assert.IsNotNull("Sam", responsePerson.FirstName);
        }
        #endregion

        #region Delete
        [TestMethod]
        public async Task TestLocalDeleteStringUri()
        {
            var restClient = GetJsonClient(new Uri("http://localhost/JsonPerson"));
            var response = await restClient.DeleteAsync("?personKey=abc");
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public async Task TestLocalDeleteUri()
        {
            var restClient = GetJsonClient(new Uri("http://localhost/JsonPerson"));
            var response = await restClient.DeleteAsync(new Uri("?personKey=abc", UriKind.Relative));
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public async Task TestLocalDeleteUriCancellationToken()
        {
            var restClient = GetJsonClient(new Uri("http://localhost/JsonPerson"));
            var response = await restClient.DeleteAsync(new Uri("?personKey=abc", UriKind.Relative), new CancellationToken());
            Assert.AreEqual(200, response.StatusCode);
        }
        #endregion

        #region Post
        [TestMethod]
        public async Task TestLocalPostBody()
        {
            var restClient = GetJsonClient(new Uri("http://localhost/JsonPerson/save"));
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PostAsync<Person, Person>(requestPerson);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPostBodyStringUri()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PostAsync<Person, Person>(requestPerson, "jsonperson/save");
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPostBodyUri()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PostAsync<Person, Person>(requestPerson, new Uri("jsonperson/save", UriKind.Relative));
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPostBodyUriCancellationToken()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PostAsync<Person, Person>(requestPerson, new Uri("jsonperson/save", UriKind.Relative), new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPostBodyUriCancellationTokenContentType()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PostAsync<Person, Person>(requestPerson, new Uri("jsonperson/save", UriKind.Relative), "application/json", new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }
        #endregion

        #region Put
        [TestMethod]
        public async Task TestLocalPutBodyStringUri()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PutAsync<Person, Person>(requestPerson, "jsonperson/save");
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPutBodyUri()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PutAsync<Person, Person>(requestPerson, new Uri("jsonperson/save", UriKind.Relative));
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPutBodyUriCancellationToken()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PutAsync<Person, Person>(requestPerson, new Uri("jsonperson/save", UriKind.Relative), new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPutBodyUriCancellationTokenContentType()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PutAsync<Person, Person>(requestPerson, new Uri("jsonperson/save", UriKind.Relative), "application/json", new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }
        #endregion

        #region Patch
        [TestMethod]
        public async Task TestLocalPatchBodyStringUri()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PatchAsync<Person, Person>(requestPerson, "jsonperson/save");
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyUri()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PatchAsync<Person, Person>(requestPerson, new Uri("jsonperson/save", UriKind.Relative));
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyUriCancellationToken()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PatchAsync<Person, Person>(requestPerson, new Uri("jsonperson/save", UriKind.Relative), new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyUriCancellationTokenContentType()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PatchAsync<Person, Person>(requestPerson, new Uri("jsonperson/save", UriKind.Relative), "application/json", new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }
        #endregion
        #endregion

        #region Misc
        [TestMethod]
        public async Task TestConcurrentCallsLocal()
        {
            var createdClients = 0;
            var restClientFactory = new RestClientFactory(
                new NewtonsoftSerializationAdapter(),
                new DefaultHttpClientFactory
                (
                    (name) =>
                    {
                        return new Lazy<HttpClient>(() =>
                        {
                            createdClients++;
                            return _testServer.CreateClient();
                        }, LazyThreadSafetyMode.ExecutionAndPublication);
                    }
                ),
                null);
            var clients = new List<IRestClient>();

            var tasks = new List<Task<RestResponseBase<Person>>>();
            const int maxCalls = 100;

            for (var i = 0; i < maxCalls; i++)
            {
                var restClient = restClientFactory.CreateRestClient();
                restClient.DefaultRequestHeaders.Add("Test", "Test");
                clients.Add(restClient);
                tasks.Add(restClient.GetAsync<Person>(new Uri("headers", UriKind.Relative)));
            }

            var results = await Task.WhenAll(tasks);

            //Ensure only one http client is created
            Assert.AreEqual(1, createdClients);
        }
        #endregion

        //TODO: Test exceptions

        //TODO: Test all constructor overloads

        #endregion

        #region Helpers
        private IRestClient GetJsonClient(Uri baseUri = null)
        {
            IRestClient restClient;

            if (baseUri != null)
            {
                var httpClient = _testServer.CreateClient();
                httpClient.BaseAddress = baseUri;
                var testClientFactory = new TestClientFactory(httpClient);
                restClient = new RestClient(new NewtonsoftSerializationAdapter(), testClientFactory);
            }
            else
            {
                restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
            }

            return restClient;
        }

        private static bool CheckRequestHeaders(IRestHeaders restRequestHeadersCollection)
        {
            return restRequestHeadersCollection.Contains("Test") && restRequestHeadersCollection["Test"].First() == "Test";
        }

        private static bool CheckResponseHeaders(RestResponseHeaders restResponseHeadersCollection)
        {
            return restResponseHeadersCollection.Contains("Test1") && restResponseHeadersCollection["Test1"].First() == "a";
        }
        #endregion
    }
}
