
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using ApiExamples;
#endif

#if NET45
using RestClientDotNet.Abstractions.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

namespace RestClientDotNet.UnitTests
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
        public async Task TestGetRestCountries()
        {
            var baseUri = new Uri("https://restcountries.eu/rest/v2/");
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), baseUri: baseUri, logger: _logger.Object);
            List<RestCountry> countries = await restClient.GetAsync<List<RestCountry>>();
            Assert.IsNotNull(countries);
            Assert.IsTrue(countries.Count > 0);

            VerifyLog(baseUri, HttpRequestMethod.Get, RestEvent.Request);
            VerifyLog(baseUri, HttpRequestMethod.Get, RestEvent.Response, (int)HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task TestDelete()
        {
            var baseUri = new Uri("https://jsonplaceholder.typicode.com");
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), baseUri: baseUri, logger: _logger.Object);
            await restClient.DeleteAsync("posts/1");

            var requestUri = new Uri("https://jsonplaceholder.typicode.com/posts/1");

            VerifyLog(requestUri, HttpRequestMethod.Delete, RestEvent.Request, null, null);
            VerifyLog(requestUri, HttpRequestMethod.Delete, RestEvent.Response, (int)HttpStatusCode.OK, null);
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

                var task = restClient.PostAsync<UserPost, UserPost>(new UserPost { title = "Moops" }, new Uri("/posts", UriKind.Relative), cancellationToken: token);

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
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com")) { Timeout = new TimeSpan(0, 0, 0, 0, 1) };
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

            //TODO: Verify the log
        }

        [TestMethod]
        [DataRow(HttpRequestMethod.Patch)]
        [DataRow(HttpRequestMethod.Post)]
        [DataRow(HttpRequestMethod.Put)]
        public async Task TestUpdate(HttpRequestMethod httpRequestMethod)
        {
            var baseUri = new Uri("https://jsonplaceholder.typicode.com");

            var restClient = new RestClient(
                new NewtonsoftSerializationAdapter(),
                baseUri: new Uri("https://jsonplaceholder.typicode.com"),
                logger: _logger.Object);
            restClient.SetJsonContentTypeHeader();
            var requestUserPost = new UserPost { title = "foo", userId = 10, body = "testbody" };
            UserPost responseUserPost = null;

            var expectedStatusCode = HttpStatusCode.OK;

            switch (httpRequestMethod)
            {
                case HttpRequestMethod.Patch:
                    responseUserPost = await restClient.PatchAsync<UserPost, UserPost>(requestUserPost, new Uri("/posts/1", UriKind.Relative));
                    break;
                case HttpRequestMethod.Post:
                    responseUserPost = await restClient.PostAsync<UserPost, UserPost>(requestUserPost, "/posts");
                    expectedStatusCode = HttpStatusCode.Created;
                    break;
                case HttpRequestMethod.Put:
                    responseUserPost = await restClient.PutAsync<UserPost, UserPost>(requestUserPost, new Uri("/posts/1", UriKind.Relative));
                    break;
            }

            Assert.AreEqual(requestUserPost.userId, responseUserPost.userId);
            Assert.AreEqual(requestUserPost.title, responseUserPost.title);

            VerifyLog(It.IsAny<Uri>(), httpRequestMethod, RestEvent.Request, null, null);
            VerifyLog(It.IsAny<Uri>(), httpRequestMethod, RestEvent.Response, (int)expectedStatusCode, null);
        }

        [TestMethod]
        public async Task TestConsoleLogging()
        {
            var logger = new ConsoleLogger();
            var restClient = new RestClient(
                new NewtonsoftSerializationAdapter(),
                baseUri: new Uri("https://jsonplaceholder.typicode.com"),
                logger: logger);
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

            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            var responsePerson = await restClient.PostAsync<Person, Person>(requestPerson, new Uri($"{LocalBaseUriString}/person"));
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

            var restClient = new RestClient(new ProtobufSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            const string personKey = "123";
            restClient.DefaultRequestHeaders.Add("PersonKey", personKey);
            Person responsePerson = await restClient.PutAsync<Person, Person>(requestPerson, new Uri($"{LocalBaseUriString}/person"));
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
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            var headers = GetHeaders(useDefault, restClient);
            Person responsePerson = await restClient.GetAsync<Person>(new Uri("headers", UriKind.Relative), headers);
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersResponseLocalGet()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
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

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestHeadersTraceLocalGet(bool useDefault)
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), null, null, logger: _logger.Object, httpClientFactory: _testServerHttpClientFactory);
            var headers = GetHeaders(useDefault, restClient);
            var response = await restClient.GetAsync<Person>(new Uri("headers", UriKind.Relative), requestHeaders: headers);

            VerifyLog(It.IsAny<Uri>(), HttpRequestMethod.Get, RestEvent.Request, null, null, CheckRequestHeaders);
            VerifyLog(It.IsAny<Uri>(), HttpRequestMethod.Get, RestEvent.Response, (int)HttpStatusCode.OK, null, CheckResponseHeaders);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestHeadersLocalPost(bool useDefault)
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            restClient.SetJsonContentTypeHeader();
            var headers = GetHeaders(useDefault, restClient);
            var responsePerson = await restClient.PostAsync<Person, Person>(
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
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);

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
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);

                //The server expects the value of "Test"
                restClient.SetJsonContentTypeHeader();
                restClient.DefaultRequestHeaders.Add("Test", "Tests");

                var responsePerson = await restClient.PostAsync<Person, Person>(new Person(), new Uri("headers", UriKind.Relative));
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
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestHeadersLocalPut(bool useDefault)
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            restClient.SetJsonContentTypeHeader();
            var headers = GetHeaders(useDefault, restClient);
            var responsePerson = await restClient.PutAsync<Person, Person>(
                new Person { FirstName = "Bob" },
                new Uri("headers", UriKind.Relative),
                requestHeaders: headers
                );
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalPutStringOverload()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            restClient.SetJsonContentTypeHeader();
            restClient.DefaultRequestHeaders.Add("Test", "Test");
            var responsePerson = await restClient.PutAsync<Person, Person>(new Person { FirstName = "Bob" }, "headers");
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectPut()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);

                //The server expects the value of "Test"
                restClient.SetJsonContentTypeHeader();
                restClient.DefaultRequestHeaders.Add("Test", "Tests");

                var responsePerson = await restClient.PutAsync<Person, Person>(new Person(), new Uri("headers", UriKind.Relative));
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
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestHeadersLocalPatch(bool useDefault)
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            restClient.SetJsonContentTypeHeader();
            var headers = GetHeaders(useDefault, restClient);
            var responsePerson = await restClient.PatchAsync<Person, Person>(
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
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);

                //The server expects the value of "Test"
                restClient.SetJsonContentTypeHeader();
                restClient.DefaultRequestHeaders.Add("Test", "Tests");

                var responsePerson = await restClient.PatchAsync<Person, Person>(new Person(), new Uri("headers", UriKind.Relative));
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
        [DataRow(true)]
        [DataRow(false)]

        public async Task TestHeadersLocalDelete(bool useDefault)
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            var headers = GetHeaders(useDefault, restClient);
            await restClient.DeleteAsync(new Uri("headers/1", UriKind.Relative), headers);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectDelete()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
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
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            var restRequestHeaders = new RestRequestHeadersCollection();
            restRequestHeaders.Add("Test", "Test");
            Person responsePerson = await restClient.SendAsync<Person, object>
                (
                new RestRequest<object>(new Uri("headers", UriKind.Relative), null, restRequestHeaders, HttpRequestMethod.Get, restClient, default)
                ); ;
            Assert.IsNotNull(responsePerson);
        }
        #endregion

        #region Local Errors
        [TestMethod]
        public async Task TestErrorsLocalGet()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            restClient.ThrowExceptionOnFailure = false;
            var response = (RestResponse<Person>)await restClient.GetAsync<Person>("error");
            Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
            var apiResult = restClient.DeserializeResponseBody<ApiResult>(response);
            Assert.AreEqual(ApiMessages.ErrorControllerErrorMessage, apiResult.Errors.First());

            //Check that the response values are getting set correctly
            Assert.AreEqual(new Uri($"{LocalBaseUriString}/error"), response.RequestUri);
            Assert.AreEqual(HttpRequestMethod.Get, response.HttpRequestMethod);
        }

        [TestMethod]
        public async Task TestErrorsLocalGetThrowException()
        {
            RestClient restClient = null;
            try
            {
                restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
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
        public async Task TestBearerTokenAuthenticationLocal()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            restClient.SetJsonContentTypeHeader();
            var response = await restClient.PostAsync<AuthenticationResult, AuthenticationRequest>(
                new AuthenticationRequest { ClientId = "a", ClientSecret = "b" },
                new Uri("secure/authenticate", UriKind.Relative)
                );

            restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + response.Body.BearerToken);
            Person person = await restClient.GetAsync<Person>(new Uri("secure/bearer", UriKind.Relative));
            Assert.AreEqual("Bear", person.FirstName);
        }

        [TestMethod]
        public async Task TestBasicAuthenticationLocalWithError()
        {
            RestClient restClient = null;
            try
            {
                restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
                restClient.SetBasicAuthenticationHeader("Bob", "WrongPassword");
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
        public async Task TestBasicAuthenticationLocal()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            restClient.SetBasicAuthenticationHeader("Bob", "ANicePassword");
            Person person = await restClient.GetAsync<Person>(new Uri("secure/basic", UriKind.Relative));
            Assert.AreEqual("Sam", person.FirstName);
        }

        [TestMethod]
        public async Task TestBearerTokenAuthenticationLocalWithError()
        {
            RestClient restClient = null;
            try
            {
                restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
                restClient.SetBearerTokenuthenticationHeader("321");
                Person person = await restClient.GetAsync<Person>(new Uri("secure/bearer", UriKind.Relative));
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
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            restClient.SetBasicAuthenticationHeader("Bob", "ANicePassword");
            restClient.SetJsonContentTypeHeader();
            Person person = await restClient.PostAsync<Person, Person>(new Person { FirstName = "Sam" }, new Uri("secure/basic", UriKind.Relative));
            Assert.AreEqual("Sam", person.FirstName);
        }

        [TestMethod]
        public async Task TestBasicAuthenticationPostLocalWithError()
        {
            RestClient restClient = null;
            try
            {
                restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
                restClient.SetBasicAuthenticationHeader("Bob", "WrongPassword");
                restClient.SetJsonContentTypeHeader();
                Person person = await restClient.PostAsync<Person, Person>(new Person { FirstName = "Sam" }, new Uri("secure/basic", UriKind.Relative));
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
            Person responsePerson = await restClient.GetAsync<Person>(new Uri("JsonPerson", UriKind.Relative), cancellationToken: new CancellationToken());
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
            var response = await restClient.DeleteAsync(new Uri("?personKey=abc", UriKind.Relative), cancellationToken: new CancellationToken());
            Assert.AreEqual(200, response.StatusCode);

            //TODO: Verify the log
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
            Person responsePerson = await restClient.PostAsync<Person, Person>(requestPerson, new Uri("jsonperson/save", UriKind.Relative), null, new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }
        #endregion

        #region Put
        [TestMethod]
        public async Task TestLocalPutBody()
        {
            var restClient = GetJsonClient(new Uri($"{LocalBaseUriString}/jsonperson/save"));
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PutAsync<Person, Person>(body: requestPerson);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

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
            Person responsePerson = await restClient.PutAsync<Person, Person>(requestPerson, new Uri("jsonperson/save", UriKind.Relative), cancellationToken: new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }
        #endregion

        #region Patch
        [TestMethod]
        public async Task TestLocalPatchBody()
        {
            var restClient = GetJsonClient(new Uri($"{LocalBaseUriString}/jsonperson/save"));
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PatchAsync<Person, Person>(requestPerson);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

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
            Person responsePerson = await restClient.PatchAsync<Person, Person>(requestPerson, new Uri("jsonperson/save", UriKind.Relative), cancellationToken: new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyUriCancellationTokenContentType()
        {
            var restClient = GetJsonClient();
            var requestPerson = new Person { FirstName = "Bob" };
            Person responsePerson = await restClient.PatchAsync<Person, Person>(requestPerson, new Uri("jsonperson/save", UriKind.Relative), cancellationToken: new CancellationToken());
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
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), _logger.Object, null);
                var requestPerson = new Person();
                Person responsePerson = await restClient.PostAsync<Person, Person>(requestPerson);
            }
            catch (SendException<Person>)
            {
                _logger.Verify(l => l.Log<RestTrace>(LogLevel.Error, It.IsAny<EventId>(), null,
                    It.Is<SendException<Person>>(e => e.InnerException != null), null));
                return;
            }
            Assert.Fail();
        }
        #endregion

        #endregion

        #region Helpers
        private static IRestHeadersCollection GetHeaders(bool useDefault, RestClient restClient)
        {
            IRestHeadersCollection headers = null;
            if (useDefault)
            {
                restClient.DefaultRequestHeaders.Add("Test", "Test");
            }
            else
            {
                headers = new RestRequestHeadersCollection { new KeyValuePair<string, IEnumerable<string>>("Test", new List<string> { "Test" }) };
            }

            return headers;
        }

        private void VerifyLog(
            Uri uri,
            HttpRequestMethod httpRequestMethod,
            RestEvent traceType,
            int? httpStatusCode = null,
            Exception exception = null,
            Func<IRestHeadersCollection, bool> checkHeadersFunc = null)
        {
            _logger.Verify(t => t.Log(
                exception == null ? LogLevel.Trace : LogLevel.Error,
                It.Is<EventId>(
                    e => e.Id == (int)traceType && e.Name == traceType.ToString()),
                It.Is<RestTrace>(
                rt =>
                    DebugTraceExpression(rt) &&
                    rt.RestEvent == traceType &&
                    rt.RequestUri == uri &&
                    rt.HttpRequestMethod == httpRequestMethod &&
                    (rt.RestEvent == RestEvent.Response || new List<HttpRequestMethod> { HttpRequestMethod.Patch, HttpRequestMethod.Post, HttpRequestMethod.Patch }.Contains(rt.HttpRequestMethod))
                    ? rt.BodyData != null && rt.BodyData.Length > 0 : true &&
                    rt.HttpStatusCode == httpStatusCode &&
                    checkHeadersFunc != null ? checkHeadersFunc(rt.RestHeadersCollection) : true
                ), exception, It.IsAny<Func<RestTrace, Exception, string>>()));
        }

        private bool DebugTraceExpression(RestTrace restTrace)
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

        private static HttpClient MintClient()
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
                restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: testClientFactory);
            }
            else
            {
                restClient = new RestClient(new NewtonsoftSerializationAdapter(), httpClientFactory: _testServerHttpClientFactory);
            }

            restClient.SetJsonContentTypeHeader();

            return restClient;
        }

        private static bool CheckRequestHeaders(IRestHeadersCollection restRequestHeadersCollection)
        {
            return restRequestHeadersCollection.Contains("Test") && restRequestHeadersCollection["Test"].First() == "Test";
        }

        private static bool CheckResponseHeaders(IRestHeadersCollection restResponseHeadersCollection)
        {
            return restResponseHeadersCollection.Contains("Test1") && restResponseHeadersCollection["Test1"].First() == "a";
        }
        #endregion
    }
}
