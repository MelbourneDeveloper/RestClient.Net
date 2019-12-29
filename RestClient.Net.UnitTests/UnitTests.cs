
using ApiExamples.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using RestClient.Net.Samples.Model;
using RestClient.Net.UnitTests.Model;
using RestClientApiSamples;
using RestClientDotNet.Abstractions;
using RestClientDotNet.Abstractions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xml2CSharp;

#if (NETCOREAPP3_1)
using Polly.Extensions.Http;
using Polly;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using ApiExamples;
using ApiExamples.Controllers;
#endif

namespace RestClientDotNet.UnitTests
{

    [TestClass]
    public class UnitTests
    {
        #region Fields
#if (NETCOREAPP3_1)
        private const string LocalBaseUriString = "https://localhost";
        private static TestServer _testServer;
#else
        private const string LocalBaseUriString = "https://localhost:44337";
#endif
        private static TestClientFactory _testServerHttpClientFactory;
        private static Mock<ITracer> _tracer;
        #endregion

        #region Setup
        [TestInitialize]
        public void Initialize()
        {
#if (NETCOREAPP3_1)
            var hostBuilder = new WebHostBuilder();
            hostBuilder.UseStartup<Startup>();
            _testServer = new TestServer(hostBuilder);
#endif
            var testClient = MintClient();
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

            tracer.Verify(t => t.Trace(HttpVerb.Get, baseUri, It.IsAny<byte[]>(), TraceType.Request, null, It.IsAny<IRestHeaders>()));
            tracer.Verify(t => t.Trace(HttpVerb.Get, baseUri, It.Is<byte[]>(d => d != null && d.Length > 0), TraceType.Response, (int)HttpStatusCode.OK, It.IsAny<IRestHeaders>()));
        }

        [TestMethod]
        public async Task TestDelete()
        {
            var tracer = new Mock<ITracer>();
            var baseUri = new Uri("https://jsonplaceholder.typicode.com");
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), baseUri, tracer.Object);
            await restClient.DeleteAsync("posts/1");

            var requestUri = new Uri("https://jsonplaceholder.typicode.com/posts/1");
            tracer.Verify(t => t.Trace(HttpVerb.Delete, requestUri, null, TraceType.Request, null, It.IsAny<IRestHeaders>()));
            tracer.Verify(t => t.Trace(HttpVerb.Delete, requestUri, It.IsAny<byte[]>(), TraceType.Response, (int)HttpStatusCode.OK, It.IsAny<IRestHeaders>()));
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

                var task = restClient.PostAsync<UserPost, UserPost>(new Uri("/posts", UriKind.Relative), new UserPost { title = "Moops" }, token);

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
                await restClient.PostAsync<UserPost, UserPost>(new Uri("/posts", UriKind.Relative), new UserPost { title = "Moops" });
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
            restClient.UseJsonContentType();
            var requestUserPost = new UserPost { title = "foo", userId = 10, body = "testbody" };
            UserPost responseUserPost = null;

            var expectedStatusCode = HttpStatusCode.OK;

            switch (verb)
            {
                case HttpVerb.Patch:
                    responseUserPost = await restClient.PatchAsync<UserPost, UserPost>(new Uri("/posts/1", UriKind.Relative), requestUserPost);
                    break;
                case HttpVerb.Post:
                    responseUserPost = await restClient.PostAsync<UserPost, UserPost>("/posts", requestUserPost);
                    expectedStatusCode = HttpStatusCode.Created;
                    break;
                case HttpVerb.Put:
                    responseUserPost = await restClient.PutAsync<UserPost, UserPost>(new Uri("/posts/1", UriKind.Relative), requestUserPost);
                    break;
            }

            Assert.AreEqual(requestUserPost.userId, responseUserPost.userId);
            Assert.AreEqual(requestUserPost.title, responseUserPost.title);

            _tracer.Verify(t => t.Trace(verb, It.Is<Uri>(a => a.ToString().Contains(baseUri.ToString())), It.Is<byte[]>(d => d.Length > 0), TraceType.Request, null, It.IsAny<IRestHeaders>()));
            _tracer.Verify(t => t.Trace(verb, It.Is<Uri>(a => a.ToString().Contains(baseUri.ToString())), It.Is<byte[]>(d => d.Length > 0), TraceType.Response, (int)expectedStatusCode, It.IsAny<IRestHeaders>()));
        }

        [TestMethod]
        public async Task TestConsoleLogging()
        {
            var tracer = new ConsoleTracer();
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"), tracer);
            var requestUserPost = new UserPost { title = "foo", userId = 10, body = "testbody" };
            await restClient.PostAsync<UserPost, UserPost>("/posts", requestUserPost);
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

        //TODO: Danger. This method was pointing to the physical local port. Why was this method passing the tests?
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
            var responsePerson = await restClient.PostAsync<Person, Person>(new Uri($"{LocalBaseUriString}/person"), requestPerson);
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
            Person responsePerson = await restClient.PutAsync<Person, Person>(new Uri($"{LocalBaseUriString}/person"), requestPerson);
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
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory, _tracer.Object, null, default, null, null);
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            var response = await restClient.GetAsync<Person>("headers");

            _tracer.Verify(t => t.Trace(HttpVerb.Get, It.IsAny<Uri>(), It.IsAny<byte[]>(), TraceType.Request, null,
                It.Is<IRestHeaders>(c => CheckRequestHeaders(c))
                ));

            _tracer.Verify(t => t.Trace(HttpVerb.Get, It.IsAny<Uri>(), It.IsAny<byte[]>(), TraceType.Response, It.IsAny<int?>(),
                It.Is<RestResponseHeaders>(c => CheckResponseHeaders(c))
                ));
        }

        [TestMethod]
        public async Task TestHeadersLocalPost()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
            restClient.UseJsonContentType();
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            var responsePerson = await restClient.PostAsync<Person, Person>(new Uri("headers", UriKind.Relative), new Person { FirstName = "Bob" });
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
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.RestResponse.StatusCode);
                var apiResult = hex.RestClient.DeserializeResponseBody<ApiResult>(hex.RestResponse);
                Assert.AreEqual(ApiMessages.HeadersControllerExceptionMessage, apiResult.Errors[0]);
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
                restClient.UseJsonContentType();
                restClient.DefaultRequestHeaders.Add("Test", "Tests");

                var responsePerson = await restClient.PostAsync<Person, Person>(new Uri("headers", UriKind.Relative), new Person());
                Assert.Fail();
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.RestResponse.StatusCode);
                var apiResult = hex.RestClient.DeserializeResponseBody<ApiResult>(hex.RestResponse);
                Assert.AreEqual(ApiMessages.HeadersControllerExceptionMessage, apiResult.Errors[0]);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public async Task TestHeadersLocalPut()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
            restClient.UseJsonContentType();
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            var responsePerson = await restClient.PutAsync<Person, Person>(new Uri("headers", UriKind.Relative), new Person { FirstName = "Bob" });
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalPutStringOverload()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
            restClient.UseJsonContentType();
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            var responsePerson = await restClient.PutAsync<Person, Person>("headers", new Person { FirstName = "Bob" });
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectPut()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);

                //The server expects the value of "Test"
                restClient.UseJsonContentType();
                restClient.DefaultRequestHeaders.Add("Test", "Tests");

                var responsePerson = await restClient.PutAsync<Person, Person>(new Uri("headers", UriKind.Relative), new Person());
                Assert.Fail();
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.RestResponse.StatusCode);
                var apiResult = hex.RestClient.DeserializeResponseBody<ApiResult>(hex.RestResponse);
                Assert.AreEqual(ApiMessages.HeadersControllerExceptionMessage, apiResult.Errors[0]);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public async Task TestHeadersLocalPatch()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
            restClient.UseJsonContentType();
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            var responsePerson = await restClient.PatchAsync<Person, Person>(new Uri("headers", UriKind.Relative), new Person { FirstName = "Bob" });
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectPatch()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);

                //The server expects the value of "Test"
                restClient.UseJsonContentType();
                restClient.DefaultRequestHeaders.Add("Test", "Tests");

                var responsePerson = await restClient.PatchAsync<Person, Person>(new Uri("headers", UriKind.Relative), new Person());
                Assert.Fail();
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.RestResponse.StatusCode);
                var apiResult = hex.RestClient.DeserializeResponseBody<ApiResult>(hex.RestResponse);
                Assert.AreEqual(ApiMessages.HeadersControllerExceptionMessage, apiResult.Errors[0]);
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
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.RestResponse.StatusCode);
                var apiResult = hex.RestClient.DeserializeResponseBody<ApiResult>(hex.RestResponse);
                Assert.AreEqual(ApiMessages.HeadersControllerExceptionMessage, apiResult.Errors[0]);
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
                new RestRequest<object>(new Uri("headers", UriKind.Relative), null, restRequestHeaders, HttpVerb.Get, restClient, default)
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
            var apiResult = restClient.DeserializeResponseBody<ApiResult>(response);
            Assert.AreEqual(ApiMessages.ErrorControllerErrorMessage, apiResult.Errors.First());

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
                var apiResult = restClient.DeserializeResponseBody<ApiResult>(hex.RestResponse);
                Assert.AreEqual(ApiMessages.ErrorControllerErrorMessage, apiResult.Errors.First());
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
                var apiResult = restClient.DeserializeResponseBody<ApiResult>(hex.RestResponse);
                Assert.AreEqual(ApiMessages.SecureControllerNotAuthorizedMessage, apiResult.Errors.First());
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public async Task TestBasicAuthenticationPostLocal()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
            restClient.UseBasicAuthentication("Bob", "ANicePassword");
            restClient.UseJsonContentType();
            Person person = await restClient.PostAsync<Person, Person>(new Uri("secure/basic", UriKind.Relative), new Person { FirstName = "Sam" });
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
                restClient.UseJsonContentType();
                Person person = await restClient.PostAsync<Person, Person>(new Uri("secure/basic", UriKind.Relative), new Person { FirstName = "Sam" });
            }
            catch (HttpStatusException ex)
            {
                Assert.AreEqual((int)HttpStatusCode.Unauthorized, ex.RestResponse.StatusCode);
                var apiResult = restClient.DeserializeResponseBody<ApiResult>(ex.RestResponse);
                Assert.AreEqual(ApiMessages.SecureControllerNotAuthorizedMessage, apiResult.Errors.First());
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
            var restClient = GetJsonClient(new Uri($"{LocalBaseUriString}/JsonPerson"));
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
        #endregion

        #region Delete
        [TestMethod]
        public async Task TestLocalDeleteStringUri()
        {
            var restClient = GetJsonClient(new Uri($"{LocalBaseUriString}/JsonPerson"));
            var response = await restClient.DeleteAsync("?personKey=abc");
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public async Task TestLocalDeleteUri()
        {
            var restClient = GetJsonClient(new Uri($"{LocalBaseUriString}/JsonPerson"));
            var response = await restClient.DeleteAsync(new Uri("?personKey=abc", UriKind.Relative));
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public async Task TestLocalDeleteUriCancellationToken()
        {
            var restClient = GetJsonClient(new Uri($"{LocalBaseUriString}/JsonPerson"));
            var response = await restClient.DeleteAsync(new Uri("?personKey=abc", UriKind.Relative), new CancellationToken());
            Assert.AreEqual(200, response.StatusCode);
        }
        #endregion

        #region Post
        [TestMethod]
        public async Task TestLocalPostBody()
        {
            var restClient = GetJsonClient(new Uri($"{LocalBaseUriString}/JsonPerson/save"));
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PostAsync<Person, Person>(requestPerson);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPostBodyStringUri()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PostAsync<Person, Person>("jsonperson/save", requestPerson);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPostBodyUri()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PostAsync<Person, Person>(new Uri("jsonperson/save", UriKind.Relative), requestPerson);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPostBodyUriCancellationToken()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PostAsync<Person, Person>(new Uri("jsonperson/save", UriKind.Relative), requestPerson, new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }
        #endregion

        #region Put
        [TestMethod]
        public async Task TestLocalPutBodyStringUri()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PutAsync<Person, Person>("jsonperson/save", requestPerson);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPutBodyUri()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PutAsync<Person, Person>(new Uri("jsonperson/save", UriKind.Relative), requestPerson);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPutBodyUriCancellationToken()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PutAsync<Person, Person>(new Uri("jsonperson/save", UriKind.Relative), requestPerson, new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }
        #endregion

        #region Patch
        [TestMethod]
        public async Task TestLocalPatchBodyStringUri()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PatchAsync<Person, Person>("jsonperson/save", requestPerson);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyUri()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PatchAsync<Person, Person>(new Uri("jsonperson/save", UriKind.Relative), requestPerson);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyUriCancellationToken()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PatchAsync<Person, Person>(new Uri("jsonperson/save", UriKind.Relative), requestPerson, new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyUriCancellationTokenContentType()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PatchAsync<Person, Person>(new Uri("jsonperson/save", UriKind.Relative), requestPerson, new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }
        #endregion
        #endregion

        #region Misc

        //TODO: This test occasionally fails. It seems to mint only 98 clients. Why?
        [TestMethod]
        public async Task TestConcurrentCallsLocalSingleton()
        {
            await DoTestConcurrentCalls(true);
        }

        [TestMethod]
        public async Task TestConcurrentCallsLocal()
        {
            await DoTestConcurrentCalls(false);
        }

#if (NETCOREAPP3_1)
        [TestMethod]
        public async Task TestPollyIncorrectUri()
        {
            var tries = 0;

            var policy = HttpPolicyExtensions
              .HandleTransientHttpError()
              .OrResult(response => response.StatusCode == HttpStatusCode.NotFound)
              .RetryAsync(3);


            var restClient = new RestClient(
                new ProtobufSerializationAdapter(),
                _testServerHttpClientFactory,
                null,
                new Uri(LocalBaseUriString),
                default,
                null,
                null,
                (httpClient, httpRequestMessageFunc, cancellationToken) =>
                {
                    return policy.ExecuteAsync(()=> 
                    {
                        var httpRequestMessage = httpRequestMessageFunc.Invoke();

                        //On the third try change the Url to a the correct one
                        if (tries == 2) httpRequestMessage.RequestUri = new Uri("Person", UriKind.Relative);
                        tries++;
                        return httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    });
                }
                );

            var person = new Person { FirstName = "Bob", Surname = "Smith" };

            //Note the Uri here is deliberately incorrect. It will cause a 404 Not found response. This is to make sure that polly is working
            person = await restClient.PostAsync<Person, Person>(new Uri("person2", UriKind.Relative), person);
        }
#endif
        #endregion

        //TODO: Test exceptions

        //TODO: Test all constructor overloads

        #endregion

        #region Helpers
        /// <summary>
        /// There were issues with DataRow so doing this instead. These sometimes fail but no idea why...
        /// </summary>
        private async Task DoTestConcurrentCalls(bool useDefaultFactory)
        {
            var createdClients = 0;

            IHttpClientFactory httpClientFactory;
            if (useDefaultFactory)
                httpClientFactory = new DefaultHttpClientFactory
                (
                    (name) =>
                    {
                        return new Lazy<HttpClient>(() =>
                        {
                            createdClients++;
                            return MintClient();
                        }, LazyThreadSafetyMode.ExecutionAndPublication);
                    }
                );
            else
            {
                httpClientFactory = new OverzealousHttpClientFactory
                    (
                        (name) =>
                        {
                            createdClients++;
                            return MintClient();
                        }
                    );
            }

            var restClientFactory = new RestClientFactory(
                new NewtonsoftSerializationAdapter(),
                httpClientFactory,
                null);
            var clients = new List<IRestClient>();

            var tasks = new List<Task<RestResponseBase<Person>>>();
            const int maxCalls = 100;

            Parallel.For(0, maxCalls, (i) =>
            {
                var restClient = restClientFactory.CreateRestClient();
                restClient.DefaultRequestHeaders.Add("Test", "Test");
                clients.Add(restClient);
                tasks.Add(restClient.GetAsync<Person>(new Uri("headers", UriKind.Relative)));
            });

            var results = await Task.WhenAll(tasks);

            //Ensure only one http client is created
            var expectedCreated = useDefaultFactory ? 1 : maxCalls;
            Assert.AreEqual(expectedCreated, createdClients);
        }

        private HttpClient MintClient()
        {
#if (NETCOREAPP3_1)
            return _testServer.CreateClient();
#else
            return new HttpClient { BaseAddress = new Uri(LocalBaseUriString) };
#endif
        }

        private IRestClient GetJsonClient(Uri baseUri = null)
        {
            IRestClient restClient;

            if (baseUri != null)
            {
                var httpClient = MintClient();
                httpClient.BaseAddress = baseUri;
                var testClientFactory = new TestClientFactory(httpClient);
                restClient = new RestClient(new NewtonsoftSerializationAdapter(), testClientFactory);
            }
            else
            {
                restClient = new RestClient(new NewtonsoftSerializationAdapter(), _testServerHttpClientFactory);
            }

            restClient.UseJsonContentType();

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
