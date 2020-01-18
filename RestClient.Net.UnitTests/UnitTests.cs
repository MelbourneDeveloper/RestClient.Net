
using ApiExamples.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using RestClient.Net.Samples.Model;
using RestClient.Net.UnitTests.Model;
using RestClientApiSamples;
using RestClient.Net.Abstractions;
using RestClient.Net.Abstractions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xml2CSharp;
using jsonperson = ApiExamples.Model.JsonModel.Person;

#if (NETCOREAPP3_1)
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using ApiExamples;
#endif

#if NET45
using RestClient.Net.Abstractions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

namespace RestClient.Net.UnitTests
{

    [TestClass]
    public class UnitTests
    {
        #region Fields
#if (NETCOREAPP3_1)
        public const string LocalBaseUriString = "http://localhost";
        private static TestServer _testServer;
#else
        public const string LocalBaseUriString = "https://localhost:44337";
#endif
        private static TestClientFactory _testServerHttpClientFactory;
        private static Mock<ILogger> _logger;
        #endregion

        #region Setup
        [TestInitialize]
        public void Initialize()
        {
            var testServerHttpClientFactory = GetTestClientFactory();
            _testServerHttpClientFactory = testServerHttpClientFactory;
            _logger = new Mock<ILogger>();
        }
        #endregion

        #region Public Static Methods
        public static TestClientFactory GetTestClientFactory()
        {
#if (NETCOREAPP3_1)
            if (_testServer == null)
            {
                var hostBuilder = new WebHostBuilder();
                hostBuilder.UseStartup<Startup>();
                _testServer = new TestServer(hostBuilder);
            }
#endif

            var testClient = MintClient();
            var testServerHttpClientFactory = new TestClientFactory(testClient);
            return testServerHttpClientFactory;
        }
        #endregion

        #region Tests

        #region External Api Tests
        [TestMethod]
        public async Task TestHead()
        {
            var baseUri = new Uri("https://www.google.com");
            var client = new Client(new NewtonsoftSerializationAdapter(), baseUri);
            var response = await client.SendAsync<string, object>(new Request<object>(
                null,
                null,
                null,
                HttpRequestMethod.Custom,
                client,
                default)
            { CustomHttpRequestMethod = "HEAD" });
            Assert.IsTrue(response.Headers.Contains("Cache-Control"));
        }

#if NETCOREAPP3_1
        [TestMethod]
        public async Task TestGetRestCountriesSystemTextJson()
        {
            var baseUri = new Uri("https://restcountries.eu/rest/v2/");
            var client = new Client(baseUri);
            List<RestCountry> countries = await client.GetAsync<List<RestCountry>>();
            Assert.IsNotNull(countries);
            Assert.IsTrue(countries.Count > 0);
        }
#endif

        [TestMethod]
        public async Task TestGetRestCountries()
        {
            var baseUri = new Uri("https://restcountries.eu/rest/v2/");
            var client = new Client(new NewtonsoftSerializationAdapter(), baseUri: baseUri, logger: _logger.Object);
            List<RestCountry> countries = await client.GetAsync<List<RestCountry>>();
            Assert.IsNotNull(countries);
            Assert.IsTrue(countries.Count > 0);

            VerifyLog(baseUri, HttpRequestMethod.Get, TraceEvent.Request);
            VerifyLog(baseUri, HttpRequestMethod.Get, TraceEvent.Response, (int)HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task TestDelete()
        {
            var baseUri = new Uri("https://jsonplaceholder.typicode.com");
            var client = new Client(new NewtonsoftSerializationAdapter(), baseUri: baseUri, logger: _logger.Object);
            await client.DeleteAsync("posts/1");

            var requestUri = new Uri("https://jsonplaceholder.typicode.com/posts/1");

            VerifyLog(requestUri, HttpRequestMethod.Delete, TraceEvent.Request, null, null);
            VerifyLog(requestUri, HttpRequestMethod.Delete, TraceEvent.Response, (int)HttpStatusCode.OK, null);
        }

        [TestMethod]
        public async Task TestGetRestCountriesAsJson()
        {
            var client = new Client(new NewtonsoftSerializationAdapter(), new Uri("https://restcountries.eu/rest/v2/name/australia"));
            var json = await client.GetAsync<string>();
            var country = JsonConvert.DeserializeObject<List<RestCountry>>(json).FirstOrDefault();
            Assert.AreEqual("Australia", country.name);
        }

        [TestMethod]
        public async Task TestGetRestCountriesNoBaseUri()
        {
            var client = new Client(new NewtonsoftSerializationAdapter());
            List<RestCountry> countries = await client.GetAsync<List<RestCountry>>(new Uri("https://restcountries.eu/rest/v2/name/australia"));
            var country = countries.FirstOrDefault();
            Assert.AreEqual("Australia", country.name);
        }

        [TestMethod]
        public async Task TestAbsoluteUriAsStringThrowsException()
        {
            try
            {
                var client = new Client(new NewtonsoftSerializationAdapter());
                List<RestCountry> countries = await client.GetAsync<List<RestCountry>>("https://restcountries.eu/rest/v2/name/australia");
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
                var client = new Client(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"));

                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;

                var task = client.PostAsync<UserPost, UserPost>(new UserPost { title = "Moops" }, new Uri("/posts", UriKind.Relative), cancellationToken: token);

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
                var client = new Client(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com")) { Timeout = new TimeSpan(0, 0, 0, 0, 1) };
                await client.PostAsync<UserPost, UserPost>(new UserPost { title = "Moops" }, new Uri("/posts", UriKind.Relative));
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

            //TODO: Verify the log
        }

        [TestMethod]
        [DataRow(HttpRequestMethod.Patch)]
        [DataRow(HttpRequestMethod.Post)]
        [DataRow(HttpRequestMethod.Put)]
        public async Task TestUpdate(HttpRequestMethod httpRequestMethod)
        {
            var baseUri = new Uri("https://jsonplaceholder.typicode.com");

            var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: new Uri("https://jsonplaceholder.typicode.com"),
                logger: _logger.Object);
            client.SetJsonContentTypeHeader();
            var requestUserPost = new UserPost { title = "foo", userId = 10, body = "testbody" };
            UserPost responseUserPost = null;

            var expectedStatusCode = HttpStatusCode.OK;

            switch (httpRequestMethod)
            {
                case HttpRequestMethod.Patch:
                    responseUserPost = await client.PatchAsync<UserPost, UserPost>(requestUserPost, new Uri("/posts/1", UriKind.Relative));
                    break;
                case HttpRequestMethod.Post:
                    responseUserPost = await client.PostAsync<UserPost, UserPost>(requestUserPost, "/posts");
                    expectedStatusCode = HttpStatusCode.Created;
                    break;
                case HttpRequestMethod.Put:
                    responseUserPost = await client.PutAsync<UserPost, UserPost>(requestUserPost, new Uri("/posts/1", UriKind.Relative));
                    break;
            }

            Assert.AreEqual(requestUserPost.userId, responseUserPost.userId);
            Assert.AreEqual(requestUserPost.title, responseUserPost.title);

            VerifyLog(It.IsAny<Uri>(), httpRequestMethod, TraceEvent.Request, null, null);
            VerifyLog(It.IsAny<Uri>(), httpRequestMethod, TraceEvent.Response, (int)expectedStatusCode, null);
        }

        [TestMethod]
        public async Task TestConsoleLogging()
        {
            var logger = new ConsoleLogger();
            var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: new Uri("https://jsonplaceholder.typicode.com"),
                logger: logger);
            var requestUserPost = new UserPost { title = "foo", userId = 10, body = "testbody" };
            await client.PostAsync<UserPost, UserPost>(requestUserPost, "/posts");
        }

        [TestMethod]
        public async Task TestGetWithXmlSerialization()
        {
            var client = new Client(new XmlSerializationAdapter(), new Uri("http://www.geoplugin.net/xml.gp"));
            var geoPlugin = await client.GetAsync<GeoPlugin>();
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

            var client = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            var responsePerson = await client.PostAsync<Person, Person>(requestPerson, new Uri($"{LocalBaseUriString}/person"));
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

            var client = new Client(new ProtobufSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            const string personKey = "123";
            client.DefaultRequestHeaders.Add("PersonKey", personKey);
            Person responsePerson = await client.PutAsync<Person, Person>(requestPerson, new Uri($"{LocalBaseUriString}/person"));
            Assert.AreEqual(requestPerson.BillingAddress.Street, responsePerson.BillingAddress.Street);
            Assert.AreEqual(personKey, responsePerson.PersonKey);
        }
        #endregion

        #region Local Headers
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]

        public async Task TestHeadersLocalGet(bool useDefault)
        {
            var client = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            var headers = GetHeaders(useDefault, client);
            Person responsePerson = await client.GetAsync<Person>(new Uri("headers", UriKind.Relative), headers);
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersResponseLocalGet()
        {
            var client = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            client.DefaultRequestHeaders.Add("Test", "Test");
            var response = await client.GetAsync<Person>("headers");

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

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestHeadersTraceLocalGet(bool useDefault)
        {
            var client = new Client(new NewtonsoftSerializationAdapter(), null, null, logger: _logger.Object, httpClientFactory: _testServerHttpClientFactory);
            var headers = GetHeaders(useDefault, client);
            var response = await client.GetAsync<Person>(new Uri("headers", UriKind.Relative), requestHeaders: headers);

            VerifyLog(It.IsAny<Uri>(), HttpRequestMethod.Get, TraceEvent.Request, null, null, CheckRequestHeaders);
            VerifyLog(It.IsAny<Uri>(), HttpRequestMethod.Get, TraceEvent.Response, (int)HttpStatusCode.OK, null, CheckResponseHeaders);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestHeadersLocalPost(bool useDefault)
        {
            var client = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            client.SetJsonContentTypeHeader();
            var headers = GetHeaders(useDefault, client);
            var responsePerson = await client.PostAsync<Person, Person>(
                new Person { FirstName = "Bob" },
                new Uri("headers", UriKind.Relative),
                requestHeaders: headers
                );
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectGet()
        {
            try
            {
                var client = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);

                //The server expects the value of "Test"
                client.DefaultRequestHeaders.Add("Test", "Tests");

                var responsePerson = await client.GetAsync<Person>(new Uri("headers", UriKind.Relative));
                Assert.Fail();
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.Response.StatusCode);
                var apiResult = hex.Client.DeserializeResponseBody<ApiResult>(hex.Response);
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
                var client = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);

                //The server expects the value of "Test"
                client.SetJsonContentTypeHeader();
                client.DefaultRequestHeaders.Add("Test", "Tests");

                var responsePerson = await client.PostAsync<Person, Person>(new Person(), new Uri("headers", UriKind.Relative));
                Assert.Fail();
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.Response.StatusCode);
                var apiResult = hex.Client.DeserializeResponseBody<ApiResult>(hex.Response);
                Assert.AreEqual(ApiMessages.HeadersControllerExceptionMessage, apiResult.Errors[0]);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestHeadersLocalPut(bool useDefault)
        {
            var client = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            client.SetJsonContentTypeHeader();
            var headers = GetHeaders(useDefault, client);
            var responsePerson = await client.PutAsync<Person, Person>(
                new Person { FirstName = "Bob" },
                new Uri("headers", UriKind.Relative),
                requestHeaders: headers
                );
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalPutStringOverload()
        {
            var client = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            client.SetJsonContentTypeHeader();
            client.DefaultRequestHeaders.Add("Test", "Test");
            var responsePerson = await client.PutAsync<Person, Person>(new Person { FirstName = "Bob" }, "headers");
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectPut()
        {
            try
            {
                var client = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);

                //The server expects the value of "Test"
                client.SetJsonContentTypeHeader();
                client.DefaultRequestHeaders.Add("Test", "Tests");

                var responsePerson = await client.PutAsync<Person, Person>(new Person(), new Uri("headers", UriKind.Relative));
                Assert.Fail();
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.Response.StatusCode);
                var apiResult = hex.Client.DeserializeResponseBody<ApiResult>(hex.Response);
                Assert.AreEqual(ApiMessages.HeadersControllerExceptionMessage, apiResult.Errors[0]);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestHeadersLocalPatch(bool useDefault)
        {
            var client = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            client.SetJsonContentTypeHeader();
            var headers = GetHeaders(useDefault, client);
            var responsePerson = await client.PatchAsync<Person, Person>(
                new Person { FirstName = "Bob" },
                new Uri("headers", UriKind.Relative),
                requestHeaders: headers
                );
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectPatch()
        {
            try
            {
                var client = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);

                //The server expects the value of "Test"
                client.SetJsonContentTypeHeader();
                client.DefaultRequestHeaders.Add("Test", "Tests");

                var responsePerson = await client.PatchAsync<Person, Person>(new Person(), new Uri("headers", UriKind.Relative));
                Assert.Fail();
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.Response.StatusCode);
                var apiResult = hex.Client.DeserializeResponseBody<ApiResult>(hex.Response);
                Assert.AreEqual(ApiMessages.HeadersControllerExceptionMessage, apiResult.Errors[0]);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]

        public async Task TestHeadersLocalDelete(bool useDefault)
        {
            var client = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            var headers = GetHeaders(useDefault, client);
            await client.DeleteAsync(new Uri("headers/1", UriKind.Relative), headers);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectDelete()
        {
            try
            {
                var client = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
                await client.DeleteAsync(new Uri("headers/1", UriKind.Relative));
                Assert.Fail();
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.Response.StatusCode);
                var apiResult = hex.Client.DeserializeResponseBody<ApiResult>(hex.Response);
                Assert.AreEqual(ApiMessages.HeadersControllerExceptionMessage, apiResult.Errors[0]);
                return;
            }

            Assert.Fail();
        }
        #endregion

        #region Local Headers In Request
        [TestMethod]
        public async Task TestHeadersLocalInRequest()
        {
            var client = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            var requestHeadersCollection = new RequestHeadersCollection();
            requestHeadersCollection.Add("Test", "Test");
            Person responsePerson = await client.SendAsync<Person, object>
                (
                new Request<object>(new Uri("headers", UriKind.Relative), null, requestHeadersCollection, HttpRequestMethod.Get, client, default)
                ); ;
            Assert.IsNotNull(responsePerson);
        }
        #endregion

        #region Local Errors
        [TestMethod]
        public async Task TestErrorsLocalGet()
        {
            var client = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            client.ThrowExceptionOnFailure = false;
            var response = await client.GetAsync<Person>("error");
            Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
            var apiResult = client.DeserializeResponseBody<ApiResult>(response);
            Assert.AreEqual(ApiMessages.ErrorControllerErrorMessage, apiResult.Errors.First());

            //Check that the response values are getting set correctly
            Assert.AreEqual(new Uri($"{LocalBaseUriString}/error"), response.RequestUri);
            Assert.AreEqual(HttpRequestMethod.Get, response.HttpRequestMethod);
        }

        [TestMethod]
        public async Task TestErrorsLocalGetThrowException()
        {
            Client restClient = null;
            try
            {
                restClient = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
                var response = await restClient.GetAsync<Person>("error");
                Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
            }
            catch (HttpStatusException hex)
            {
                var apiResult = restClient.DeserializeResponseBody<ApiResult>(hex.Response);
                Assert.AreEqual(ApiMessages.ErrorControllerErrorMessage, apiResult.Errors.First());
                return;
            }

            Assert.Fail();
        }
        #endregion

        #region Local Authentication
        [TestMethod]
        public async Task TestBearerTokenAuthenticationLocal()
        {
            var client = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            client.SetJsonContentTypeHeader();
            var response = await client.PostAsync<AuthenticationResult, AuthenticationRequest>(
                new AuthenticationRequest { ClientId = "a", ClientSecret = "b" },
                new Uri("secure/authenticate", UriKind.Relative)
                );

            client.SetBearerTokenuthenticationHeader(response.Body.BearerToken);

            Person person = await client.GetAsync<Person>(new Uri("secure/bearer", UriKind.Relative));
            Assert.AreEqual("Bear", person.FirstName);
        }

        [TestMethod]
        public async Task TestBasicAuthenticationLocalWithError()
        {
            Client restClient = null;
            try
            {
                restClient = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
                restClient.SetBasicAuthenticationHeader("Bob", "WrongPassword");
                Person person = await restClient.GetAsync<Person>(new Uri("secure/basic", UriKind.Relative));
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.Unauthorized, hex.Response.StatusCode);
                var apiResult = restClient.DeserializeResponseBody<ApiResult>(hex.Response);
                Assert.AreEqual(ApiMessages.SecureControllerNotAuthorizedMessage, apiResult.Errors.First());
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public async Task TestBasicAuthenticationLocal()
        {
            var client = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            client.SetBasicAuthenticationHeader("Bob", "ANicePassword");
            Person person = await client.GetAsync<Person>(new Uri("secure/basic", UriKind.Relative));
            Assert.AreEqual("Sam", person.FirstName);
        }

        [TestMethod]
        public async Task TestBearerTokenAuthenticationLocalWithError()
        {
            Client restClient = null;
            try
            {
                restClient = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
                restClient.SetBearerTokenuthenticationHeader("321");
                Person person = await restClient.GetAsync<Person>(new Uri("secure/bearer", UriKind.Relative));
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.Unauthorized, hex.Response.StatusCode);
                var apiResult = restClient.DeserializeResponseBody<ApiResult>(hex.Response);
                Assert.AreEqual(ApiMessages.SecureControllerNotAuthorizedMessage, apiResult.Errors.First());
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public async Task TestBasicAuthenticationPostLocal()
        {
            var client = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            client.SetBasicAuthenticationHeader("Bob", "ANicePassword");
            client.SetJsonContentTypeHeader();
            Person person = await client.PostAsync<Person, Person>(new Person { FirstName = "Sam" }, new Uri("secure/basic", UriKind.Relative));
            Assert.AreEqual("Sam", person.FirstName);
        }

        [TestMethod]
        public async Task TestBasicAuthenticationPostLocalWithError()
        {
            Client restClient = null;
            try
            {
                restClient = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
                restClient.SetBasicAuthenticationHeader("Bob", "WrongPassword");
                restClient.SetJsonContentTypeHeader();
                Person person = await restClient.PostAsync<Person, Person>(new Person { FirstName = "Sam" }, new Uri("secure/basic", UriKind.Relative));
            }
            catch (HttpStatusException ex)
            {
                Assert.AreEqual((int)HttpStatusCode.Unauthorized, ex.Response.StatusCode);
                var apiResult = restClient.DeserializeResponseBody<ApiResult>(ex.Response);
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
            var client = GetJsonClient(new Uri($"{LocalBaseUriString}/JsonPerson"));
            jsonperson responsePerson = await client.GetAsync<jsonperson>();
            Assert.IsNotNull(responsePerson);
            Assert.IsNotNull("Sam", responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalGetStringUri()
        {
            var client = GetJsonClient();
            jsonperson responsePerson = await client.GetAsync<jsonperson>("JsonPerson");
            Assert.IsNotNull(responsePerson);
            Assert.IsNotNull("Sam", responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalGetUri()
        {
            var client = GetJsonClient();
            jsonperson responsePerson = await client.GetAsync<jsonperson>(new Uri("JsonPerson", UriKind.Relative));
            Assert.IsNotNull(responsePerson);
            Assert.IsNotNull("Sam", responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalGetUriCancellationToken()
        {
            var client = GetJsonClient();
            jsonperson responsePerson = await client.GetAsync<jsonperson>(new Uri("JsonPerson", UriKind.Relative), cancellationToken: new CancellationToken());
            Assert.IsNotNull(responsePerson);
            Assert.IsNotNull("Sam", responsePerson.FirstName);
        }
        #endregion

        #region Delete
        [TestMethod]
        public async Task TestLocalDeleteStringUri()
        {
            var client = GetJsonClient(new Uri($"{LocalBaseUriString}/JsonPerson"));
            var response = await client.DeleteAsync("?personKey=abc");
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public async Task TestLocalDeleteUri()
        {
            var client = GetJsonClient(new Uri($"{LocalBaseUriString}/JsonPerson"));
            var response = await client.DeleteAsync(new Uri("?personKey=abc", UriKind.Relative));
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public async Task TestLocalDeleteUriCancellationToken()
        {
            var client = GetJsonClient(new Uri($"{LocalBaseUriString}/JsonPerson"));
            var response = await client.DeleteAsync(new Uri("?personKey=abc", UriKind.Relative), cancellationToken: new CancellationToken());
            Assert.AreEqual(200, response.StatusCode);

            //TODO: Verify the log
        }
        #endregion

        #region Post
        [TestMethod]
        public async Task TestLocalPostBody()
        {
            var client = GetJsonClient(new Uri($"{LocalBaseUriString}/JsonPerson/save"));
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PostAsync<jsonperson, jsonperson>(requestPerson);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPostBodyStringUri()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PostAsync<jsonperson, jsonperson>(requestPerson, "jsonperson/save");
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPostBodyUri()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PostAsync<jsonperson, jsonperson>(requestPerson, new Uri("jsonperson/save", UriKind.Relative));
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPostBodyUriCancellationToken()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PostAsync<jsonperson, jsonperson>(requestPerson, new Uri("jsonperson/save", UriKind.Relative), null, new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }
        #endregion

        #region Put
        [TestMethod]
        public async Task TestLocalPutBody()
        {
            var client = GetJsonClient(new Uri($"{LocalBaseUriString}/jsonperson/save"));
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PutAsync<jsonperson, jsonperson>(requestBody: requestPerson);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPutBodyStringUri()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PutAsync<jsonperson, jsonperson>(requestPerson, "jsonperson/save");
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPutBodyUri()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PutAsync<jsonperson, jsonperson>(requestPerson, new Uri("jsonperson/save", UriKind.Relative));
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPutBodyUriCancellationToken()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PutAsync<jsonperson, jsonperson>(requestPerson, new Uri("jsonperson/save", UriKind.Relative), cancellationToken: new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }
        #endregion

        #region Patch
        [TestMethod]
        public async Task TestLocalPatchBody()
        {
            var client = GetJsonClient(new Uri($"{LocalBaseUriString}/jsonperson/save"));
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PatchAsync<jsonperson, jsonperson>(requestPerson);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyStringUri()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PatchAsync<jsonperson, jsonperson>(requestPerson, "jsonperson/save");
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyUri()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PatchAsync<jsonperson, jsonperson>(requestPerson, new Uri("jsonperson/save", UriKind.Relative));
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyUriCancellationToken()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PatchAsync<jsonperson, jsonperson>(requestPerson, new Uri("jsonperson/save", UriKind.Relative), cancellationToken: new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyUriCancellationTokenContentType()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PatchAsync<jsonperson, jsonperson>(requestPerson, new Uri("jsonperson/save", UriKind.Relative), cancellationToken: new CancellationToken());
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

        [TestMethod]
        public async Task TestErrorLogging()
        {
            try
            {
                var client = new Client(new NewtonsoftSerializationAdapter(), _logger.Object, null);
                var requestPerson = new Person();
                Person responsePerson = await client.PostAsync<Person, Person>(requestPerson);
            }
            catch (SendException<Person>)
            {
                _logger.Verify(l => l.Log<Trace>(LogLevel.Error, It.IsAny<EventId>(), null,
                    It.Is<SendException<Person>>(e => e.InnerException != null), null));
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public async Task TestFactoryCreationWithUri()
        {
            IClientFactory clientFactory = new ClientFactory(new NewtonsoftSerializationAdapter());
            var baseUri = new Uri("https://restcountries.eu/rest/v2/");
            var client = clientFactory.CreateClient("test", baseUri);
            var response = await client.GetAsync<List<RestCountry>>();
            Assert.IsTrue(response.Body.Count > 0);
        }
        #endregion

        #endregion

        #region Helpers
        private static IHeadersCollection GetHeaders(bool useDefault, Client client)
        {
            IHeadersCollection headers = null;
            if (useDefault)
            {
                client.DefaultRequestHeaders.Add("Test", "Test");
            }
            else
            {
                headers = new RequestHeadersCollection { new KeyValuePair<string, IEnumerable<string>>("Test", new List<string> { "Test" }) };
            }

            return headers;
        }

        private void VerifyLog(
            Uri uri,
            HttpRequestMethod httpRequestMethod,
            TraceEvent traceType,
            int? httpStatusCode = null,
            Exception exception = null,
            Func<IHeadersCollection, bool> checkHeadersFunc = null)
        {
            _logger.Verify(t => t.Log(
                exception == null ? LogLevel.Trace : LogLevel.Error,
                It.Is<EventId>(
                    e => e.Id == (int)traceType && e.Name == traceType.ToString()),
                It.Is<Trace>(
                rt =>
                    DebugTraceExpression(rt) &&
                    rt.RestEvent == traceType &&
                    rt.RequestUri == uri &&
                    rt.HttpRequestMethod == httpRequestMethod &&
                    (rt.RestEvent == TraceEvent.Response || new List<HttpRequestMethod> { HttpRequestMethod.Patch, HttpRequestMethod.Post, HttpRequestMethod.Patch }.Contains(rt.HttpRequestMethod))
                    ? rt.BodyData != null && rt.BodyData.Length > 0 : true &&
                    rt.HttpStatusCode == httpStatusCode &&
                    checkHeadersFunc != null ? checkHeadersFunc(rt.HeadersCollection) : true
                ), exception, It.IsAny<Func<Trace, Exception, string>>()));
        }

        private bool DebugTraceExpression(Trace restTrace)
        {
            return true;
        }

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

            var clientFactory = new ClientFactory(
                new NewtonsoftSerializationAdapter(),
                httpClientFactory,
                null);
            var clients = new List<IClient>();

            var tasks = new List<Task<Response<jsonperson>>>();
            const int maxCalls = 100;

            Parallel.For(0, maxCalls, (i) =>
            {
                var client = clientFactory.CreateClient();
                clients.Add(client);
                tasks.Add(client.GetAsync<jsonperson>(new Uri("JsonPerson", UriKind.Relative)));
            });

            var results = await Task.WhenAll(tasks);

            //Ensure only one http client is created
            var expectedCreated = useDefaultFactory ? 1 : maxCalls;
            Assert.AreEqual(expectedCreated, createdClients);
        }

        private static HttpClient MintClient()
        {
#if (NETCOREAPP3_1)
            return _testServer.CreateClient();
#else
            return new HttpClient { BaseAddress = new Uri(LocalBaseUriString) };
#endif
        }

        private IClient GetJsonClient(Uri baseUri = null)
        {
            IClient restClient;

            if (baseUri != null)
            {
                var httpClient = MintClient();
                httpClient.BaseAddress = baseUri;
                var testClientFactory = new TestClientFactory(httpClient);
                restClient = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: testClientFactory);
            }
            else
            {
                restClient = new Client(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            }

            restClient.SetJsonContentTypeHeader();

            return restClient;
        }

        private static bool CheckRequestHeaders(IHeadersCollection requestHeadersCollection)
        {
            return requestHeadersCollection.Contains("Test") && requestHeadersCollection["Test"].First() == "Test";
        }

        private static bool CheckResponseHeaders(IHeadersCollection responseHeadersCollection)
        {
            return responseHeadersCollection.Contains("Test1") && responseHeadersCollection["Test1"].First() == "a";
        }
        #endregion
    }
}
