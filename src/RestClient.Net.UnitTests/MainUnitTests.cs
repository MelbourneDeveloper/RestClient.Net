
using ApiExamples.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using RestClient.Net;
using RestClient.Net.Samples.Model;
using RestClient.Net.UnitTests.Model;
using RestClientApiSamples;
using RestClient.Net.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xml2CSharp;
using jsonperson = ApiExamples.Model.JsonModel.Person;
using RichardSzalay.MockHttp;
using System.IO;
using Microsoft.Extensions.Logging;
using RestClient.Net.Abstractions.Extensions;
using System.Reflection;
using System.Text;
using System.Collections.Immutable;
using System.Collections;

#if NETCOREAPP3_1
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using ApiExamples;
#endif

#if NET472
using Microsoft.Extensions.Logging.Abstractions;
#else
using Moq.Protected;
#endif

#pragma warning disable CA1810 // Initialize reference type static fields inline
#pragma warning disable CA1506 // Initialize reference type static fields inline

namespace RestClient.Net.UnitTests
{
    [TestClass]
    public class MainUnitTests
    {
        #region Fields
        private static Uri testServerBaseUri;
        private static readonly IHeadersCollection DefaultJsonContentHeaderCollection = HeadersExtensions.CreateHeadersCollectionWithJsonContentType();
        private static readonly ILoggerFactory consoleLoggerFactory =
#if NET472
        consoleLoggerFactory = NullLoggerFactory.Instance;
#else
        LoggerFactory.Create(builder => _ = builder.AddConsole().SetMinimumLevel(LogLevel.Trace));
#endif

        private const string StandardContentTypeToString = "application/json; charset=utf-8";
        private const string GoogleUrlString = "https://www.google.com";
        private const string RestCountriesAllUriString = "https://restcountries.eu/rest/v2/";
        private const string RestCountriesAustraliaUriString = "https://restcountries.eu/rest/v2/name/australia/";
        private const string JsonPlaceholderBaseUriString = "https://jsonplaceholder.typicode.com";
        private const string JsonPlaceholderFirstPostSlug = "/posts/1";
        private const string JsonPlaceholderPostsSlug = "/posts";
        private readonly Uri RestCountriesAllUri = new(RestCountriesAllUriString);
        private readonly Uri RestCountriesAustraliaUri = new(RestCountriesAustraliaUriString);
        private readonly Uri JsonPlaceholderBaseUri = new(JsonPlaceholderBaseUriString);
        private const string TransferEncodingHeaderName = "Transfer-Encoding";
        private const string SetCookieHeaderName = "Set-Cookie";
        private const string CacheControlHeaderName = "Cache-Control";
        private const string XRatelimitLimitHeaderName = "X-Ratelimit-Limit";

        private static readonly UserPost _userRequestBody = new() { title = "foo", userId = 10, body = "testbody" };

        private static readonly string _userRequestBodyJson = "{\r\n" +
                $"  \"userId\": {_userRequestBody.userId},\r\n" +
                "  \"id\": 0,\r\n" +
                "  \"title\": \"foo\",\r\n" +
                "  \"body\": \"testbody\"\r\n" +
                "}";

        private readonly Dictionary<string, string> RestCountriesAllHeaders = new()
        {
            { "Date", "Wed, 17 Jun 2020 22:51:03 GMT" },
            { TransferEncodingHeaderName, "chunked" },
            { "Connection", "keep-alive" },
            { "Set-Cookie", "__cfduid=dde664b010195275c339e4b049626e6261592434261; expires=Fri, 17-Jul-20 22:51:03 GMT; path=/; domain=.restcountries.eu; HttpOnly; SameSite=Lax" },
            { "Access-Control-Allow-Origin", "*" },
            { "Access-Control-Allow-Methods", "GET" },
            { "Access-Control-Allow-Headers", "Accept, X-Requested-With" },
            { CacheControlHeaderName, "public, max-age=86400" },
            { "CF-Cache-Status", "DYNAMIC" },
            { "cf-request-id", "0366139e2100001258170ec200000001" },
            { "Expect-CT", "max-age=604800, report-uri=\"https://report-uri.cloudflare.com/cdn-cgi/beacon/expect-ct\"" },
            { "Server", "cloudflare" },
            { "CF-RAY", "5a50554368bf1258-HKG" },
        };

        private readonly Dictionary<string, string> JsonPlaceholderDeleteHeaders = new()
        {
            { "Date", "Thu, 18 Jun 2020 09:17:40 GMT" },
            { "Connection", "keep-alive" },
            { SetCookieHeaderName, "__cfduid=d4048d349d1b9a8c70f8eb26dbf91e9a91592471851; expires=Sat, 18-Jul-20 09:17:36 GMT; path=/; domain=.typicode.com; HttpOnly; SameSite=Lax" },
            { "X-Powered-By", "Express" },
            { "Vary", "Origin, Accept-Encoding" },
            { "Access-Control-Allow-Credentials", "true" },
            { CacheControlHeaderName, "no-cache" },
            { "Pragma", "no-cache" },
            { "Expires", "1" },
            { "X-Content-Type-Options", "nosniff" },
            { "Etag", "W/\"2-vyGp6PvFo4RvsFtPoIWeCReyIC1\"" },
            { "Via", "1.1 vegur" },
            { "CF-Cache-Status", "DYNAMIC" },
            { "cf-request-id", "0368513dc10000ed3f0020a200000001" },
            { "Expect-CT", "max-age=604800, report-uri=\"https://report-uri.cloudflare.com/cdn-cgi/beacon/expect-ct\"" },
            { "Server", "cloudflare" },
            { "CF-RAY", "5a52eb0f9d0bed3f-SJC" },
        };

        private readonly Dictionary<string, string> JsonPlaceholderPostHeaders = new()
        {
            { "Date", "Thu, 18 Jun 2020 09:17:40 GMT" },
            { "Connection", "keep-alive" },
            { SetCookieHeaderName, "__cfduid=d4048d349d1b9a8c70f8eb26dbf91e9a91592471851; expires=Sat, 18-Jul-20 09:17:36 GMT; path=/; domain=.typicode.com; HttpOnly; SameSite=Lax" },
            { "X-Powered-By", "Express" },
            { XRatelimitLimitHeaderName, "10000" },
            { "X-Ratelimit-Remaining", "9990" },
            { "X-Ratelimit-Reset", "1592699847" },
            { "Vary", "Origin, Accept-Encoding" },
            { "Access-Control-Allow-Credentials", "true" },
            { CacheControlHeaderName, "no-cache" },
            { "Pragma", "no-cache" },
            { "Expires", "1" },
            { "Location", "http://jsonplaceholder.typicode.com/posts/101" },
            { "X-Content-Type-Options", "nosniff" },
            { "Etag", "W/\"2-vyGp6PvFo4RvsFtPoIWeCReyIC1\"" },
            { "Via", "1.1 vegur" },
            { "CF-Cache-Status", "DYNAMIC" },
            { "cf-request-id", "0368513dc10000ed3f0020a200000002" },
            { "Expect-CT", "max-age=604800, report-uri=\"https://report-uri.cloudflare.com/cdn-cgi/beacon/expect-ct\"" },
            { "Server", "cloudflare" },
            { "CF-RAY", "5a52eb0f9d0bed3f-SJC" },
        };

        private readonly Dictionary<string, string> GoogleHeadHeaders = new()
        {
            { "P3P", "CP=\"This is not a P3P policy! See g.co/p3phelp for more info.\"" },
            { "Date", "Sun, 21 Jun 2020 02:38:45 GMT" },
            { SetCookieHeaderName, "1P_JAR=2020-06-21-02; expires=Tue, 21-Jul-2020 02:38:45 GMT; path=/; domain=.google.com; Secure" },
            //TODO: there should be two lines of cookie here but mock http doesn't seem to allow for this...
            { "Server", "gws" },
            { "X-XSS-Protection", "0" },
            { "X-Frame-Options", "SAMEORIGIN" },
            { "Transfer-Encoding", "SAMEORIGIN" },
            { "Expires", "Sun, 21 Jun 2020 02:38:45 GMT" },
            { CacheControlHeaderName, "private" },
        };


        //For realises - with factory
        //private static readonly CreateHttpClient _createHttpClient = (n) => new HttpClient();
        //For realsies - no factory
        //private CreateHttpClient _createHttpClient = null;

        //Mock the httpclient
        private static readonly MockHttpMessageHandler _mockHttpMessageHandler = new();
        private static readonly CreateHttpClient _createHttpClient = (n) => _mockHttpMessageHandler.ToHttpClient();
        private static readonly TestClientFactory _testServerHttpClientFactory;
        private static Mock<ILogger<Client>> _logger = new();

#if NETCOREAPP3_1
        private readonly Func<string, Lazy<HttpClient>> _createLazyHttpClientFunc = (n) =>
        {
            var client = _createHttpClient(n);
            return new Lazy<HttpClient>(() => client);
        };

        public const string LocalBaseUriString = "http://localhost";
        private static readonly TestServer _testServer;
#else
        public const string LocalBaseUriString = "https://localhost:44337";
#endif


        #endregion

        #region Setup
        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<ILogger<Client>>();


            _mockHttpMessageHandler.When(RestCountriesAustraliaUriString)
            .Respond(HeadersExtensions.JsonMediaType, File.ReadAllText("JSON/Australia.json"));

            _mockHttpMessageHandler.When(HttpMethod.Delete, JsonPlaceholderBaseUriString + JsonPlaceholderFirstPostSlug).
            //TODO: The curly braces make all the difference here. However, the lack of curly braces should be handled.
            Respond(
                JsonPlaceholderDeleteHeaders,
                HeadersExtensions.JsonMediaType,
                "{}"
                );

            _mockHttpMessageHandler.
                When(HttpMethod.Post, JsonPlaceholderBaseUriString + JsonPlaceholderPostsSlug).
                With(request => request.Content.Headers.ContentType == null).
                Respond(
                HttpStatusCode.Created,
                JsonPlaceholderPostHeaders,
                null,
                //This is the JSON that gets returned when the content type is empty
                "{\r\n" +
                "  \"id\": 101\r\n" +
                "}"
                );

#if NETCOREAPP3_1
            _mockHttpMessageHandler.When(HttpMethod.Patch, JsonPlaceholderBaseUriString + JsonPlaceholderFirstPostSlug).
            Respond(
                HttpStatusCode.OK,
                JsonPlaceholderPostHeaders,
                HeadersExtensions.JsonMediaType,
                _userRequestBodyJson
                );
#endif

            _mockHttpMessageHandler.
                When(HttpMethod.Post, JsonPlaceholderBaseUriString + JsonPlaceholderPostsSlug).
                With(request => request.Content.Headers.ContentType.MediaType == HeadersExtensions.JsonMediaType).
                Respond(
                HttpStatusCode.OK,
                JsonPlaceholderPostHeaders,
                HeadersExtensions.JsonMediaType,
                _userRequestBodyJson
                );

            _mockHttpMessageHandler.
            When(HttpMethod.Put, JsonPlaceholderBaseUriString + JsonPlaceholderFirstPostSlug).
            With(request => request.Content.Headers.ContentType.MediaType == HeadersExtensions.JsonMediaType).
            Respond(
            HttpStatusCode.OK,
            JsonPlaceholderPostHeaders,
            HeadersExtensions.JsonMediaType,
            _userRequestBodyJson
            );

            _mockHttpMessageHandler.
            When(HttpMethod.Head, GoogleUrlString).
            Respond(
            HttpStatusCode.OK,
            GoogleHeadHeaders,
            null,
            ""
            //TODO: Allow for null
            //default(string)
            );

            //Return all rest countries with a status code of 200
            _mockHttpMessageHandler.When(RestCountriesAllUriString)
                    .Respond(
                RestCountriesAllHeaders,
                HeadersExtensions.JsonMediaType,
                File.ReadAllText("JSON/RestCountries.json"));
        }
        #endregion

        #region Public Static Methods
        public static TestClientFactory GetTestClientFactory()
        {
            var testClient = MintClient();
            return new TestClientFactory(testClient);
        }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        static MainUnitTests()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
#if NETCOREAPP3_1
            if (_testServer == null)
            {
                var hostBuilder = new WebHostBuilder();
                hostBuilder.UseStartup<Startup>();
                _testServer = new TestServer(hostBuilder);
            }
#endif

#pragma warning disable IDE0021 // Use expression body for constructors
            _testServerHttpClientFactory = GetTestClientFactory();
#pragma warning restore IDE0021 // Use expression body for constructors
        }
        #endregion

        #region Tests

        #region External Api Tests
        [TestMethod]
        public async Task TestHead()
        {
            var baseUri = new Uri(GoogleUrlString);
            using var client = new Client(
                serializationAdapter: new NewtonsoftSerializationAdapter(),
                createHttpClient: _createHttpClient
                );

            var response = await client.SendAsync<string, object>(new Request<object>(
                baseUri,
                null,
                NullHeadersCollection.Instance,
                HttpRequestMethod.Custom,
                "HEAD",
                default));

            Assert.AreEqual(GoogleHeadHeaders[CacheControlHeaderName], response.Headers[CacheControlHeaderName].Single());
        }

#if !NET472


        /// <summary>
        /// TODO: fix this for the real scenario. It fails when actually contacting the server
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestResendHeaders()
        {
            var headers = new HeadersCollection(new Dictionary<string, IEnumerable<string>>());

            using var client = new Client(baseUri: RestCountriesAllUri, createHttpClient: _createHttpClient);

            var parameters = new object();

            _ = await client.PostAsync<List<RestCountry>, object>(parameters, null, headers);
            _ = await client.PostAsync<List<RestCountry>, object>(parameters, null, headers);
        }

        /// <summary>
        /// This method tests to make sure that all headers end up in the correct location on the request, and making a call twice doesn't confuse the client
        /// </summary>
        [TestMethod]
        public async Task TestCallHeadersMergeWithDefaultHeaders()
        {
            //Arrange

            using var value = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new List<RestCountry>())),
            };

#pragma warning disable CA2000 // Dispose objects before losing scope
            GetHttpClientMoq(out var handlerMock, out var httpClient, value);
#pragma warning restore CA2000 // Dispose objects before losing scope
            HttpClient createHttpClient(string name) => httpClient;

            var testKvp = new KeyValuePair<string, IEnumerable<string>>("test", new List<string> { "test", "test2" });
            var testDefaultKvp = new KeyValuePair<string, IEnumerable<string>>("default", new List<string> { "test", "test2" });


            using var client = new Client(baseUri: RestCountriesAllUri, createHttpClient: createHttpClient, defaultRequestHeaders: testDefaultKvp.CreateHeadersCollection());


            //Act
            _ = await client.PostAsync<List<RestCountry>, object>(new object(), null, testKvp.CreateHeadersCollection());

            //Make sure we can call it twice
            _ = await client.PostAsync<List<RestCountry>, object>(new object(), null, testKvp.CreateHeadersCollection());

            var expectedHeaders = new List<KeyValuePair<string, IEnumerable<string>>>
            {
                testKvp,
                testDefaultKvp
            };

            //Assert
            handlerMock.Protected()
                .Verify(
                "SendAsync",
                Times.Exactly(2),
                ItExpr.Is<HttpRequestMessage>(h => CheckRequestMessage(h, RestCountriesAllUri, expectedHeaders, true)),
                ItExpr.IsAny<CancellationToken>()
                );
        }

        [TestMethod]
        public async Task TestGetDefaultSerializationRestCountries()
        {
            using var client = new Client(baseUri: RestCountriesAllUri, createHttpClient: _createHttpClient);
            List<RestCountry> countries = await client.GetAsync<List<RestCountry>>();
            Assert.IsNotNull(countries);
            Assert.IsTrue(countries.Count > 0);
        }

        [TestMethod]
        public async Task TestGetDefaultSerializationRestCountriesAsJson()
        {
            using var client = new Client(
                baseUri: RestCountriesAustraliaUri,
                createHttpClient: _createHttpClient);
            var json = await client.GetAsync<string>();

            var country = JsonConvert.DeserializeObject<List<RestCountry>>(json).First();
            Assert.AreEqual("Australia", country.name);
        }
#endif

        [TestMethod]
        public async Task TestBadRequestThrowsHttpStatusCodeException()
        {
            using var mockHttp = new MockHttpMessageHandler();

            const HttpStatusCode statusCode = HttpStatusCode.BadRequest;

            //In this case, return an error object
            _ = mockHttp.When(RestCountriesAllUriString)
                    .Respond(statusCode, HeadersExtensions.JsonMediaType, JsonConvert.SerializeObject(new Error { Message = "Test", ErrorCode = 100 }));

            var httpClient = mockHttp.ToHttpClient();

            using var factory = new SingletonHttpClientFactory(httpClient);

            using var client = new Client(new NewtonsoftSerializationAdapter(), createHttpClient: factory.CreateClient, baseUri: RestCountriesAllUri, logger: _logger.Object);

            await AssertThrowsAsync<HttpStatusException>(client.GetAsync<List<RestCountry>>(), Messages.GetErrorMessageNonSuccess((int)statusCode, RestCountriesAllUri));
        }

        [TestMethod]
        public void TestFactoryDisposeTwice()
        {
            using var httpClient = new HttpClient();
            var factory = new SingletonHttpClientFactory(httpClient);
            factory.Dispose();
            factory.Dispose();

            var factory2 = new DefaultHttpClientFactory();
            factory2.Dispose();
            factory2.Dispose();
        }

        [TestMethod]
        public async Task TestBadRequestCanDeserializeErrorMessage()
        {
            var adapter = new NewtonsoftSerializationAdapter();

            using var mockHttp = new MockHttpMessageHandler();

            const HttpStatusCode statusCode = HttpStatusCode.BadRequest;

            var expectedError = new Error { Message = "Test", ErrorCode = 100 };

            _ = mockHttp.When(RestCountriesAllUriString)
                    .Respond(statusCode, HeadersExtensions.JsonMediaType, JsonConvert.SerializeObject(expectedError));

            var httpClient = mockHttp.ToHttpClient();

            using var factory = new SingletonHttpClientFactory(httpClient);

            using var client = new Client(adapter, createHttpClient: factory.CreateClient, baseUri: RestCountriesAllUri, logger: _logger.Object, throwExceptionOnFailure: false);

            var response = await client.GetAsync<List<RestCountry>>();

            var error = client.SerializationAdapter.Deserialize<Error>(response.GetResponseData(), response.Headers);

            Assert.AreEqual(expectedError.Message, error.Message);
        }

        [TestMethod]
        public async Task TestGetRestCountries()
        {
            using var client = new Client(new NewtonsoftSerializationAdapter(),
                baseUri: RestCountriesAllUri,
                createHttpClient: _createHttpClient,
                logger: _logger.Object);

            var response = await client.GetAsync<List<RestCountry>>();
            Assert.IsNotNull(response);
            Assert.IsTrue(response?.Body?.Count > 0);

#if !NET472
            _logger.VerifyLog((state, t) => state.CheckValue("{OriginalFormat}", Messages.InfoSendReturnedNoException), LogLevel.Information, 1);

            _logger.VerifyLog((state, t) =>
            state.CheckValue<IRequest>("request", (a) => a.Uri == RestCountriesAllUri && a.HttpRequestMethod == HttpRequestMethod.Get) &&
            state.CheckValue("{OriginalFormat}", Messages.InfoAttemptingToSend)
            , LogLevel.Trace, 1);

            _logger.VerifyLog((state, t) =>
            state.CheckValue("{OriginalFormat}", Messages.TraceResponseProcessed) &&
            state.CheckValue<Response>("response", (a) => a.RequestUri == RestCountriesAllUri && a.StatusCode == 200)
            , LogLevel.Trace, 1);
#endif

            var httpResponseMessageResponse = response;

            Assert.AreEqual(StandardContentTypeToString, httpResponseMessageResponse?.Headers["Content-Type"].First());

            Assert.AreEqual(RestCountriesAllHeaders[TransferEncodingHeaderName], response?.Headers[TransferEncodingHeaderName].First());
        }

        /// <summary>
        /// TODO: fix this for the real scenario. It fails when actually contacting the server
        /// </summary>
        [TestMethod]
        public async Task TestDelete()
        {
            using var client = new Client(new NewtonsoftSerializationAdapter(),
                baseUri: JsonPlaceholderBaseUri,
                logger: _logger.Object,
                createHttpClient: _createHttpClient
                );
            var response = await client.DeleteAsync(JsonPlaceholderFirstPostSlug);

#if !NET472
            _logger.VerifyLog((state, t) =>
            state.CheckValue("{OriginalFormat}", Messages.TraceBeginSend) &&
            state.CheckValue<IRequest>("request", (r) => r.HttpRequestMethod == HttpRequestMethod.Delete)
            , LogLevel.Trace, 1);

            _logger.VerifyLog((state, t) =>
            state.CheckValue("{OriginalFormat}", Messages.TraceResponseProcessed) &&
            state.CheckValue<Response>("response", (r) => r.StatusCode == (int)HttpStatusCode.OK)
            , LogLevel.Trace, 1);
#endif

            //This doesn't work when actually contacting the server...
            Assert.AreEqual(JsonPlaceholderDeleteHeaders[SetCookieHeaderName], response.Headers[SetCookieHeaderName].First());
        }

        [TestMethod]
        public async Task TestGetRestCountriesAsJson()
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                RestCountriesAustraliaUri,
                createHttpClient: _createHttpClient,
                name: null);

            var json = await client.GetAsync<string>();
            var country = JsonConvert.DeserializeObject<List<RestCountry>>(json).First();
            Assert.AreEqual("Australia", country.name);
        }

        [TestMethod]
        public async Task TestGetRestCountriesNoBaseUri()
        {
            using var client = new Client(new NewtonsoftSerializationAdapter(), createHttpClient: _createHttpClient);
            List<RestCountry> countries = await client.GetAsync<List<RestCountry>>(RestCountriesAustraliaUri);
            var country = countries.First();
            Assert.AreEqual("Australia", country.name);
        }

        [TestMethod]
        public async Task TestPostUserWithCancellation()
        {
            try
            {
                using var client = new Client(new NewtonsoftSerializationAdapter(), baseUri: JsonPlaceholderBaseUri);

                using var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;

                var task = client.PostAsync<UserPost, UserPost>(new UserPost { title = "Moops" }, new Uri("/posts", UriKind.Relative), cancellationToken: token);

                tokenSource.Cancel();

                _ = await task;
            }
            catch (TaskCanceledException)
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

        /*
        [TestMethod]
        public async Task TestPostUserTimeout()
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: JsonPlaceholderBaseUri,
                timeout: new TimeSpan(0, 0, 0, 0, 1),
                logger: _logger.Object);

            var exception = await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => client.PostAsync<UserPost, UserPost>(new UserPost { title = "Moops" }, new Uri("/posts", UriKind.Relative)));

#if !NET472
            _logger.VerifyLog<Client, OperationCanceledException>((state, t)
                => state.CheckValue("{OriginalFormat}", Messages.ErrorTaskCancelled), LogLevel.Error, 1);
#endif
        }
        */

        [TestMethod]
#if NETCOREAPP3_1
        //TODO: seems like this can't be mocked on .NET Framework?
        [DataRow(HttpRequestMethod.Patch)]
#endif
        [DataRow(HttpRequestMethod.Post)]
        [DataRow(HttpRequestMethod.Put)]
        public async Task TestUpdate(HttpRequestMethod httpRequestMethod)
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: JsonPlaceholderBaseUri,
                createHttpClient: _createHttpClient,
                logger: _logger.Object,
                defaultRequestHeaders: HeadersExtensions.CreateHeadersCollectionWithJsonContentType());
            var responseUserPost = httpRequestMethod switch
            {
                HttpRequestMethod.Patch => await client.PatchAsync<UserPost, UserPost>(_userRequestBody, new Uri("/posts/1", UriKind.Relative)),
                //TODO: Shouldn't expectedStatusCode = HttpStatusCode.Created
                HttpRequestMethod.Post => await client.PostAsync<UserPost, UserPost>(_userRequestBody, "/posts"),
                HttpRequestMethod.Put => await client.PutAsync<UserPost, UserPost>(_userRequestBody, new Uri("/posts/1", UriKind.Relative)),
                HttpRequestMethod.Get => throw new NotImplementedException(),
                HttpRequestMethod.Delete => throw new NotImplementedException(),
                HttpRequestMethod.Custom => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
            Assert.AreEqual(_userRequestBody.userId, responseUserPost.Body?.userId);
            Assert.AreEqual(_userRequestBody.title, responseUserPost.Body?.title);

#if !NET472
            var expectedStatusCode = HttpStatusCode.OK;

            _logger.VerifyLog((state, t) =>
             state.CheckValue("{OriginalFormat}", Messages.TraceBeginSend) &&
             state.CheckValue<IRequest>("request", (r) => r.HttpRequestMethod == httpRequestMethod)
             , LogLevel.Trace, 1);

            _logger.VerifyLog((state, t) =>
            state.CheckValue("{OriginalFormat}", Messages.TraceResponseProcessed) &&
            state.CheckValue("response", (Func<Response, bool>)((r) => r.StatusCode == (int)expectedStatusCode))
            , LogLevel.Trace, 1);
#endif
        }

        [TestMethod]
        public async Task TestConsoleLogging()
        {
            //var logger = new ConsoleLogger();
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: JsonPlaceholderBaseUri,
                createHttpClient: _createHttpClient,
                logger: consoleLoggerFactory.CreateLogger<Client>());
            var response = await client.PostAsync<PostUserResponse, UserPost>(_userRequestBody, JsonPlaceholderPostsSlug);
            Assert.AreEqual(JsonPlaceholderPostHeaders[CacheControlHeaderName], response.Headers[CacheControlHeaderName].Single());
            Assert.AreEqual(JsonPlaceholderPostHeaders[CacheControlHeaderName], response.Headers[CacheControlHeaderName].Single());
            //JSON placeholder seems to return 101 no matter what Id is passed in...
            Assert.AreEqual(101, response.Body?.Id);
            Assert.AreEqual(201, response.StatusCode);
        }

        [TestMethod]
        public async Task TestGetWithXmlSerialization()
        {
            using var client = new Client(new XmlSerializationAdapter(), baseUri: new Uri("http://www.geoplugin.net/xml.gp"));
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

            using var client = new Client(new NewtonsoftSerializationAdapter(), createHttpClient: _testServerHttpClientFactory.CreateClient);
            var responsePerson = await client.PostAsync<Person, Person>(requestPerson, new Uri($"{LocalBaseUriString}/person"));
            Assert.AreEqual(requestPerson.BillingAddress.Street, responsePerson.Body?.BillingAddress.Street);
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

            const string personKey = "123";

            using var client = new Client(
                new ProtobufSerializationAdapter(),
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: "PersonKey".CreateHeadersCollection(personKey));

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
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: useDefault ?
                DefaultJsonContentHeaderCollection.WithHeaderValue("Test", "Test")
                : DefaultJsonContentHeaderCollection
                );

            Person responsePerson = await client.GetAsync<Person>(new Uri(
                "headers",
                UriKind.Relative),
                "Test".CreateHeadersCollection("Test")
                );

            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersResponseLocalGet()
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: "Test".CreateHeadersCollection("Test"));

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
        public async Task TestHeadersResponseLocalGet2()
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: "Test".CreateHeadersCollection("Test"));

            var response = await client.GetAsync<Person>("headers");

            Assert.IsTrue(response.Headers.Names.ToList().Contains("Test1"));

        }

        [TestMethod]
        public void TestCanEnumerateNullHeaders()
        {
            foreach (var kvp in NullHeadersCollection.Instance)
            {
                Assert.IsNotNull(kvp);
            }

            Assert.IsFalse(NullHeadersCollection.Instance.Contains("asdasd"));

            Assert.IsTrue(!NullHeadersCollection.Instance[""].Any());

            var value = (NullKvpEnumerator<string, IEnumerable<string>>)NullHeadersCollection.Instance.GetEnumerator();

            value.Reset();

            Assert.IsNotNull(value);

            var current = value.Current;

            Assert.IsNull(current.Key);
            Assert.IsNull(current.Value);

            //For coverage
            new NullHeadersCollection().Dispose();
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestHeadersTraceLocalGet(bool useDefault)
        {
            var headersCollections = HeadersExtensions.CreateHeadersCollectionWithJsonContentType();

            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                testServerBaseUri,
                logger: _logger.Object,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                name: null,
                defaultRequestHeaders: useDefault ?
                headersCollections.WithHeaderValue("Test", "Test")
                : headersCollections

                );

            _ = await client.GetAsync<Person>(new Uri(
                "headers",
                UriKind.Relative),
                requestHeaders: "Test"
                .CreateHeadersCollection("Test"));

#if !NET472
            _logger.VerifyLog((state, t) =>
            state.CheckValue<IRequest>("request", (request) => request.Headers != null && CheckRequestHeaders(request.Headers)) &&
            state.CheckValue("{OriginalFormat}", Messages.InfoAttemptingToSend)
            , LogLevel.Trace, 1);

            _logger.VerifyLog((state, t) =>
            state.CheckValue<Response>("response", (response) => response.Headers != null && CheckResponseHeaders(response.Headers)) &&
            state.CheckValue("{OriginalFormat}", Messages.TraceResponseProcessed)
            , LogLevel.Trace, 1);
#endif
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestHeadersLocalPost(bool useDefault)
        {
            var headersCollections = HeadersExtensions.CreateHeadersCollectionWithJsonContentType();

            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: useDefault ?
                headersCollections.WithHeaderValue("Test", "Test")
                : headersCollections
                );

            var responsePerson = await client.PostAsync<Person, Person>(
                new Person { FirstName = "Bob" },
                new Uri("headers", UriKind.Relative),
                requestHeaders: "Test".CreateHeadersCollection("Test")
                );

            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectGet()
        {
            var serializationAdapter = new NewtonsoftSerializationAdapter();

            try
            {
                using var client = new Client(
                    serializationAdapter,
                    baseUri: testServerBaseUri,
                    createHttpClient: _testServerHttpClientFactory.CreateClient,
                    //The server expects the value of "Test"
                    defaultRequestHeaders: "Test".CreateHeadersCollection("Tests"));

                _ = await client.GetAsync<Person>(new Uri("headers", UriKind.Relative));
                Assert.Fail();
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.Response.StatusCode);
                var apiResult = serializationAdapter.Deserialize<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
                Assert.AreEqual(ApiMessages.HeadersControllerExceptionMessage, apiResult.Errors[0]);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectPost()
        {
            var serializationAdapter = new NewtonsoftSerializationAdapter();

            try
            {
                using var client = new Client(
                    serializationAdapter,
                    baseUri: testServerBaseUri,
                    createHttpClient: _testServerHttpClientFactory.CreateClient,
                    //The server expects the value of "Test"
                    defaultRequestHeaders: HeadersExtensions.CreateHeadersCollectionWithJsonContentType().Append("Test", "Tests"));

                _ = await client.PostAsync<Person, Person>(new Person(), new Uri("headers", UriKind.Relative));
                Assert.Fail();
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.Response.StatusCode);
                var apiResult = serializationAdapter.Deserialize<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
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
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: useDefault ?
                DefaultJsonContentHeaderCollection.WithHeaderValue("Test", "Test")
                : DefaultJsonContentHeaderCollection
                );

            var responsePerson = await client.PutAsync<Person, Person>(
                new Person { FirstName = "Bob" },
                new Uri("headers", UriKind.Relative),
                requestHeaders: "Test".CreateHeadersCollection("Test")
                );

            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalPutStringOverload()
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions.CreateHeadersCollectionWithJsonContentType().Append("Test", "Test"));

            var responsePerson = await client.PutAsync<Person, Person>(new Person { FirstName = "Bob" }, "headers");
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public void TestHeadersToString()
        {
            var asdasd = "asd".CreateHeadersCollection("321");
            var afasds = asdasd.ToString();
            Assert.AreEqual("asd: 321\r\n", afasds);
        }



        [TestMethod]
        public void TestHeaders()
        {
            var count = 0;
            var enumerable = (IEnumerable)"asd".CreateHeadersCollection("321");
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Assert.IsNotNull(enumerator.Current);
                count++;
            }
            Assert.AreEqual(1, count);

            enumerator = ((IEnumerable)NullHeadersCollection.Instance).GetEnumerator();
            var current = enumerator.Current;
            Assert.IsNotNull(current);
            while (enumerator.MoveNext())
            {
            }
        }

        [TestMethod]
        public void TestAppendHeaders()
        {
            var headers = "asd".CreateHeadersCollection("123");
            const string expectedValue = "321";
            var headers2 = "asd".CreateHeadersCollection(expectedValue);
            var headers3 = headers.Append(headers2);
            Assert.AreEqual(expectedValue, headers3.First().Value.First());
        }

        [TestMethod]
        public void TestAppendHeaders2()
        {
            var headers = "asd".CreateHeadersCollection("123");
            var headers2 = headers.Append(null);
            Assert.AreEqual(1, headers2.Count());
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectPut()
        {
            var serializationAdapter = new NewtonsoftSerializationAdapter();

            try
            {
                using var client = new Client(
                    new NewtonsoftSerializationAdapter(),
                    baseUri: testServerBaseUri,
                    createHttpClient: _testServerHttpClientFactory.CreateClient,
                    //The server expects the value of "Test"
                    defaultRequestHeaders: HeadersExtensions.CreateHeadersCollectionWithJsonContentType().Append("Test", "Tests"));

                _ = await client.PutAsync<Person, Person>(new Person(), new Uri("headers", UriKind.Relative));
                Assert.Fail();
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.Response.StatusCode);
                var apiResult = serializationAdapter.Deserialize<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
                Assert.AreEqual(ApiMessages.HeadersControllerExceptionMessage, apiResult.Errors[0]);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public async Task TestGetWitStringResource()
        {

            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                //The server expects the value of "Test"
                defaultRequestHeaders: HeadersExtensions.CreateHeadersCollectionWithJsonContentType());

            _ = await client.GetAsync<List<Person>>("JsonPerson/People");

        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestHeadersLocalPatch(bool useDefault)
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: useDefault ?
                DefaultJsonContentHeaderCollection.WithHeaderValue("Test", "Test")
                : DefaultJsonContentHeaderCollection);

            var responsePerson = await client.PatchAsync<Person, Person>(
                new Person { FirstName = "Bob" },
                new Uri("headers", UriKind.Relative),
                requestHeaders: "Test".CreateHeadersCollection("Test")
                );

            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectPatch()
        {
            var serializationAdapter = new NewtonsoftSerializationAdapter();

            try
            {
                using var client = new Client(
                    new NewtonsoftSerializationAdapter(),
                    baseUri: testServerBaseUri,
                    createHttpClient: _testServerHttpClientFactory.CreateClient,
                    defaultRequestHeaders: HeadersExtensions.CreateHeadersCollectionWithJsonContentType().Append("Test", "Tests"));

                _ = await client.PatchAsync<Person, Person>(new Person(), new Uri("headers", UriKind.Relative));
                Assert.Fail();
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.Response.StatusCode);
                var apiResult = serializationAdapter.Deserialize<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
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
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: useDefault ?
                DefaultJsonContentHeaderCollection.WithHeaderValue("Test", "Test")
                : DefaultJsonContentHeaderCollection);

            _ = await client.DeleteAsync(new Uri("headers/1", UriKind.Relative), "Test".CreateHeadersCollection("Test"));
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectDelete()
        {
            var serializationAdapter = new NewtonsoftSerializationAdapter();

            try
            {
                using var client = new Client(
                    new NewtonsoftSerializationAdapter(),
                    baseUri: testServerBaseUri,
                    createHttpClient: _testServerHttpClientFactory.CreateClient);
                _ = await client.DeleteAsync(new Uri("headers/1", UriKind.Relative));
                Assert.Fail();
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.Response.StatusCode);
                var apiResult = serializationAdapter.Deserialize<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
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
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                createHttpClient: _testServerHttpClientFactory.CreateClient);
            var requestHeadersCollection = "Test".CreateHeadersCollection("Test");
            Person responsePerson = await client.SendAsync<Person, object>
                (
                new Request<object>(testServerBaseUri.Combine(new Uri("headers", UriKind.Relative)), null, requestHeadersCollection, HttpRequestMethod.Get, cancellationToken: default)
                ); ;
            Assert.IsNotNull(responsePerson);
        }
        #endregion

        #region Local Errors
        [TestMethod]
        public async Task TestErrorsLocalGet()
        {
            var serializationAdapter = new NewtonsoftSerializationAdapter();

            using var client = new Client(
                serializationAdapter,
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                throwExceptionOnFailure: false
                );

            var response = await client.GetAsync<Person>("error");
            Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
            var apiResult = serializationAdapter.Deserialize<ApiResult>(response.GetResponseData(), response.Headers);
            Assert.AreEqual(ApiMessages.ErrorControllerErrorMessage, apiResult.Errors.First());

            //Check that the response values are getting set correctly
            Assert.AreEqual(new Uri($"{LocalBaseUriString}/error"), response.RequestUri);
            Assert.AreEqual(HttpRequestMethod.Get, response.HttpRequestMethod);
        }

        [TestMethod]
        public async Task TestErrorsLocalGetThrowException()
        {
            var serializationAdapter = new NewtonsoftSerializationAdapter();

            using var restClient = new Client(
                serializationAdapter,
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient);

            try
            {
                var response = await restClient.GetAsync<Person>("error");
                Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
            }
            catch (HttpStatusException hex)
            {
                var apiResult = serializationAdapter.Deserialize<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
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
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions.CreateHeadersCollectionWithJsonContentType());

            var response = await client.PostAsync<AuthenticationResult, AuthenticationRequest>(
                new AuthenticationRequest { ClientId = "a", ClientSecret = "b" },
                new Uri("secure/authenticate", UriKind.Relative)
                );

            var bearerToken = response.Body?.BearerToken;

            if (bearerToken == null) throw new InvalidOperationException("No bearer token");

            using var client2 = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions
                .CreateHeadersCollectionWithJsonContentType()
                .WithBearerTokenAuthentication(bearerToken));

            Person person = await client2.GetAsync<Person>(new Uri("secure/bearer", UriKind.Relative));
            Assert.AreEqual("Bear", person.FirstName);
        }

        [TestMethod]
        public async Task TestBasicAuthenticationLocalWithError()
        {
            var serializationAdapter = new NewtonsoftSerializationAdapter();

            using var restClient = new Client(
                serializationAdapter,
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions.CreateHeadersCollectionWithBasicAuthentication("Bob", "WrongPassword"));

            try
            {
                _ = await restClient.GetAsync<Person>(new Uri("secure/basic", UriKind.Relative));
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.Unauthorized, hex.Response.StatusCode);
                var apiResult = serializationAdapter.Deserialize<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
                Assert.AreEqual(ApiMessages.SecureControllerNotAuthorizedMessage, apiResult.Errors.First());
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public async Task TestBasicAuthenticationLocal()
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions.CreateHeadersCollectionWithBasicAuthentication("Bob", "ANicePassword"));

            Person person = await client.GetAsync<Person>(new Uri("secure/basic", UriKind.Relative));
            Assert.AreEqual("Sam", person.FirstName);
        }

        [TestMethod]
        public async Task TestBearerTokenAuthenticationLocalWithError()
        {
            var serializationAdapter = new NewtonsoftSerializationAdapter();

            using var restClient = new Client(
                serializationAdapter,
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions.CreateHeadersCollectionWithBearerTokenAuthentication("321"));

            try
            {
                _ = await restClient.GetAsync<Person>(new Uri("secure/bearer", UriKind.Relative));
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.Unauthorized, hex.Response.StatusCode);
                var apiResult = serializationAdapter.Deserialize<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
                Assert.AreEqual(ApiMessages.SecureControllerNotAuthorizedMessage, apiResult.Errors.First());
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public async Task TestBasicAuthenticationPostLocal()
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions
                .CreateHeadersCollectionWithJsonContentType()
                .WithBasicAuthentication("Bob", "ANicePassword"));

            Person person = await client.PostAsync<Person, Person>(new Person { FirstName = "Sam" }, new Uri("secure/basic", UriKind.Relative));
            Assert.AreEqual("Sam", person.FirstName);
        }

        [TestMethod]
        public async Task TestBasicAuthenticationPostLocalWithError()
        {
            var serializationAdapter = new NewtonsoftSerializationAdapter();

            using var restClient = new Client(
                serializationAdapter,
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions
                .CreateHeadersCollectionWithJsonContentType()
                .WithBasicAuthentication("Bob", "WrongPassword"));

            try
            {
                _ = await restClient.PostAsync<Person, Person>(new Person { FirstName = "Sam" }, new Uri("secure/basic", UriKind.Relative));
            }
            catch (HttpStatusException ex)
            {
                Assert.AreEqual((int)HttpStatusCode.Unauthorized, ex.Response.StatusCode);
                var apiResult = serializationAdapter.Deserialize<ApiResult>(ex.Response.GetResponseData(), ex.Response.Headers);
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
        public async Task TestLocalPostBody2()
        {
            var client = GetJsonClient(new Uri($"{LocalBaseUriString}/jsonperson/save2"));
            jsonperson responsePerson = await client.PostAsync<jsonperson>();
            Assert.AreEqual("J", responsePerson.FirstName);
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
        public async Task TestLocalPutBody2()
        {
            var client = GetJsonClient(new Uri($"{LocalBaseUriString}/jsonperson/save2"));
            jsonperson responsePerson = await client.PutAsync<jsonperson>();
            Assert.AreEqual("J", responsePerson.FirstName);
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
        public async Task TestLocalPatchBody2()
        {
            var client = GetJsonClient(new Uri($"{LocalBaseUriString}/jsonperson/save2"));
            jsonperson responsePerson = await client.PatchAsync<jsonperson>();
            Assert.AreEqual("J", responsePerson.FirstName);
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
        [TestMethod]
        public void TestDefaultGetHttpRequestMessageCustomNoThing()
        {
            var defaultGetHttpRequestMessage = new DefaultGetHttpRequestMessage();
            var request = new Request<string>(
                new Uri("http://www.test.com"),
                default,
                NullHeadersCollection.Instance,
                HttpRequestMethod.Custom,
                cancellationToken: default);

            _ = Assert.ThrowsException<InvalidOperationException>(() => _ = defaultGetHttpRequestMessage.GetHttpRequestMessage(
                  request,
                  _logger.Object,
                  new Mock<ISerializationAdapter>().Object));
        }


        [TestMethod]
        public void TestInvalidHttpRequestMethod()
        {
            var defaultGetHttpRequestMessage = new DefaultGetHttpRequestMessage();
            var request = new Request<string>(
                new Uri("http://www.test.com"),
                default,
                NullHeadersCollection.Instance,
                (HttpRequestMethod)10,
                cancellationToken: default);

            _ = Assert.ThrowsException<InvalidOperationException>(() => _ = defaultGetHttpRequestMessage.GetHttpRequestMessage(
                  request,
                  _logger.Object,
                  new Mock<ISerializationAdapter>().Object));
        }

        [TestMethod]
        public async Task TestBadBaseUri()
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: (n) => new HttpClient { BaseAddress = new Uri("http://www.test.com") },
                defaultRequestHeaders: HeadersExtensions.CreateHeadersCollectionWithJsonContentType().Append("Test", "Test"));

            var exception = await Assert.ThrowsExceptionAsync<SendException>(() =>
               client.PutAsync<Person, Person>(new Person { FirstName = "Bob" }, "headers"));

            Assert.IsTrue(exception.InnerException is InvalidOperationException);
        }

        [TestMethod]
        public void TestDisposeTwice()
        {
            var client = new Client(new Mock<ISerializationAdapter>().Object);
            client.Dispose();
            client.Dispose();
        }

        [TestMethod]
        public async Task TestHttpResponseMessageIsNullThrowsException()
        {
            var sendHttpRequestMessage = new Mock<ISendHttpRequestMessage>();
            var serializationAdapter = new Mock<ISerializationAdapter>();

            _ = sendHttpRequestMessage.Setup(s => s.SendHttpRequestMessage(
                  It.IsAny<HttpClient>(),
                  new Mock<IGetHttpRequestMessage>().Object,
                  new Mock<IRequest<object>>().Object,
                  _logger.Object,
                  serializationAdapter.Object)).Returns<object>(null);

            using var client = new Client(
                serializationAdapter.Object,
                new Uri("http://www.test.com"),
                sendHttpRequest: sendHttpRequestMessage.Object,
                name: "asd");

            var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => client.GetAsync<string>());

            Assert.AreEqual("httpResponseMessage", exception.ParamName);

        }

        [TestMethod]
        public async Task TestDeserializationException()
        {
            using MockHttpMessageHandler mockHttpMessageHandler = new();

            const string Content = "Hi";



            _ = mockHttpMessageHandler.When(RestCountriesAllUriString)
            .Respond(
            RestCountriesAllHeaders,
            HeadersExtensions.JsonMediaType,
            Content);

            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                logger: _logger.Object,
                baseUri: new Uri(RestCountriesAllUriString),
                createHttpClient: (n) => new HttpClient(mockHttpMessageHandler));

            try
            {
                await client.GetAsync<Person>();
            }
            catch (DeserializationException dex)
            {
#if !NET472
                _logger.VerifyLog<Client, DeserializationException>((state, t)
                    => state.CheckValue("{OriginalFormat}", Messages.ErrorMessageDeserialization), LogLevel.Error, 1);
#endif
                Assert.IsTrue(dex.GetResponseData().SequenceEqual(Encoding.ASCII.GetBytes(Content)));

                return;

            }

            Assert.Fail();
        }

        /// <summary>
        /// This is just a stub to make sure it's easy to mock requests and responses
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestCanMockRequestAndResponse()
        {
            var clientMock = new Mock<IClient>();
            var headersMock = new Mock<IHeadersCollection>();
            using var httpResponseMessage = new HttpResponseMessage();
            using var httpClient = new HttpClient();
            var response = new Response<string>(
                headersMock.Object,
                10,
                HttpRequestMethod.Custom,
                //Shouldn't this be filled in?
                new byte[0],
                "test",
                new Uri("http://www.test.com"));

            _ = clientMock.Setup(c => c.SendAsync<string, object>(It.IsAny<IRequest<object>>())).Returns
                (
                Task.FromResult(response)
                );

            var returnedResponse = await clientMock.Object.SendAsync<string, object>(new Mock<IRequest<object>>().Object);

            Assert.IsTrue(ReferenceEquals(response, returnedResponse));
        }

        [TestMethod]
        public async Task TestErrorLogging()
        {
            try
            {
                using var client = new Client(
                    new NewtonsoftSerializationAdapter(),
                    baseUri: testServerBaseUri,
                    createHttpClient: (n) =>
                    {
                        var httpClient = new HttpClient();
                        httpClient.DefaultRequestHeaders.Add("asd", "asds");
                        return httpClient;
                    },
                    logger: _logger.Object);

                var requestPerson = new Person();
                _ = await client.PostAsync<Person, Person>(requestPerson);
            }
            catch (SendException sex)
            {
#if !NET472
                _logger.VerifyLog<Client, SendException>((state, t)
                    => state.CheckValue("{OriginalFormat}", Messages.ErrorSendException), LogLevel.Error, 1);
#endif

                Assert.AreEqual(testServerBaseUri, sex.Request.Uri);

                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public async Task TestInvalidUriInformation()
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                logger: _logger.Object);

            _ = await Assert.ThrowsExceptionAsync<InvalidOperationException>(()
                => client.PostAsync<Person, Person>(new Person()));
        }

        [TestMethod]
        public async Task TestFactoryCreationWithUri()
        {
            var clientFactory = new ClientFactory(_createHttpClient, new NewtonsoftSerializationAdapter());
            var client = ClientFactoryExtensions.CreateClient(clientFactory.CreateClient, "test", RestCountriesAllUri);
            var response = await client.GetAsync<List<RestCountry>>();
            Assert.IsTrue(response.Body?.Count > 0);
        }

#if !NET472

        private static ILoggerFactory GetLoggerFactory(Action<object?> callback)
        {
            var callbackLogger = new CallbackLogger<Client>(callback);

            var callbackLoggerFactory = new LoggerFactory();
#pragma warning disable CA2000 // Dispose objects before losing scope
            var provider = new LoggerProvider(callbackLogger);
#pragma warning restore CA2000 // Dispose objects before losing scope
            callbackLoggerFactory.AddProvider(provider);

            return callbackLoggerFactory;
        }

        [TestMethod]
        public async Task TestFactoryDoesntUseSameHttpClient()
        {
            var logCount = 0;
            HttpClient? firstClient = null;
            HttpClient? secondClient = null;

            using var callbackLoggerFactory = GetLoggerFactory((state) => { GetHttpClentsFromLogs(state, ref logCount, ref firstClient, ref secondClient); });

            var clientFactory = new ClientFactory(
                _createHttpClient,
                new NewtonsoftSerializationAdapter(),
                loggerFactory: callbackLoggerFactory);

            var client = ClientFactoryExtensions.CreateClient(clientFactory.CreateClient, "1", RestCountriesAllUri);
            var response = await client.GetAsync<List<RestCountry>>();

            client = ClientFactoryExtensions.CreateClient(clientFactory.CreateClient, "2", RestCountriesAllUri);
            response = await client.GetAsync<List<RestCountry>>();

            Assert.IsNotNull(firstClient);
#pragma warning disable CA1508 // Avoid dead conditional code
            var isEqual = ReferenceEquals(firstClient, secondClient);
#pragma warning restore CA1508 // Avoid dead conditional code
            Assert.IsFalse(isEqual);
        }

        [TestMethod]
        public async Task TestHttpClientFactoryDoesntUseSameHttpClient()
        {
            var logCount = 0;
            HttpClient? firstClient = null;
            HttpClient? secondClient = null;

            using var callbackLoggerFactory = GetLoggerFactory((state) => { GetHttpClentsFromLogs(state, ref logCount, ref firstClient, ref secondClient); });

            using var defaultHttpClientFactory = new DefaultHttpClientFactory(_createLazyHttpClientFunc);

            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                logger: callbackLoggerFactory.CreateLogger<Client>(),
                baseUri: RestCountriesAllUri,
                createHttpClient: defaultHttpClientFactory.CreateClient); ;

            var response = await client.GetAsync<List<RestCountry>>();

            using var client2 = new Client(
                new NewtonsoftSerializationAdapter(),
                logger: callbackLoggerFactory.CreateLogger<Client>(),
                baseUri: RestCountriesAllUri,
                createHttpClient: defaultHttpClientFactory.CreateClient);

            response = await client2.GetAsync<List<RestCountry>>();

            Assert.IsNotNull(firstClient);
#pragma warning disable CA1508 // Avoid dead conditional code
            Assert.IsFalse(ReferenceEquals(firstClient, secondClient));
#pragma warning restore CA1508 // Avoid dead conditional code

            Assert.AreEqual(2, logCount);
        }

        /// <summary>
        /// This test is controversial. Should non-named clients always be Singleton? This is the way the factory is designed, but could trip some users up.
        /// </summary>
        [TestMethod]
        public void TestClientFactoryReusesClient()
        {
            using var defaultHttpClientFactory = new DefaultHttpClientFactory(_createLazyHttpClientFunc);

            var clientFactory = new ClientFactory(defaultHttpClientFactory.CreateClient, new NewtonsoftSerializationAdapter());

            var firstClient = ClientFactoryExtensions.CreateClient(clientFactory.CreateClient, "RestClient", RestCountriesAllUri);

            var secondClient = ClientFactoryExtensions.CreateClient(clientFactory.CreateClient, "RestClient", RestCountriesAllUri);

            Assert.IsNotNull(firstClient);
            Assert.IsTrue(ReferenceEquals(firstClient, secondClient));
        }

        [TestMethod]
        public async Task TestHttpClientFactoryReusesHttpClient()
        {
            var logCount = 0;
            HttpClient? firstClient = null;
            HttpClient? secondClient = null;
            using var callbackLoggerFactory = GetLoggerFactory((state) => { GetHttpClentsFromLogs(state, ref logCount, ref firstClient, ref secondClient); });

            using var defaultHttpClientFactory = new DefaultHttpClientFactory(_createLazyHttpClientFunc);

            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                logger: callbackLoggerFactory.CreateLogger<Client>(),
                baseUri: RestCountriesAllUri,
                createHttpClient: defaultHttpClientFactory.CreateClient);

            var response = await client.GetAsync<List<RestCountry>>();

            response = await client.GetAsync<List<RestCountry>>();

#pragma warning disable CA1508 // Avoid dead conditional code
            Assert.IsNotNull(firstClient);
            Assert.IsTrue(ReferenceEquals(firstClient, secondClient));
            Assert.AreEqual(2, logCount);
#pragma warning restore CA1508 // Avoid dead conditional code
        }

        [TestMethod]
        public async Task TestHttpClientFactoryReusesHttpClientWithoutFunc()
        {
            var logCount = 0;
            HttpClient? firstClient = null;
            HttpClient? secondClient = null;
            using var callbackLoggerFactory = GetLoggerFactory((state) => { GetHttpClentsFromLogs(state, ref logCount, ref firstClient, ref secondClient); });

            using var defaultHttpClientFactory = new DefaultHttpClientFactory();

            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                logger: callbackLoggerFactory.CreateLogger<Client>(),
                baseUri: RestCountriesAllUri,
                createHttpClient: defaultHttpClientFactory.CreateClient);

            var response = await client.GetAsync<List<RestCountry>>();

            response = await client.GetAsync<List<RestCountry>>();

#pragma warning disable CA1508 // Avoid dead conditional code
            Assert.IsNotNull(firstClient);
            Assert.IsTrue(ReferenceEquals(firstClient, secondClient));
            Assert.AreEqual(2, logCount);
#pragma warning restore CA1508 // Avoid dead conditional code
        }

        private static void GetHttpClentsFromLogs(object? state, ref int logCount, ref HttpClient? firstClient, ref HttpClient? secondClient)
        {
            if (state == null) return;
            var exists = state.GetValue<HttpClient>("httpClient", out var httpClient);
            if (exists)
            {
                if (firstClient != null)
                {
                    secondClient = httpClient;
                }
                else
                {
                    firstClient = httpClient;
                }

                //This is a filthy unit test. We do this to make sure that the http client was only logged twice (one for each GET)
                logCount++;
            }
        }

        [TestMethod]
        public async Task TestHttpClientFactoryReusesHttpClientWhenSameName()
        {
            var logCount = 0;
            HttpClient? firstClient = null;
            HttpClient? secondClient = null;

            using var callbackLoggerFactory = GetLoggerFactory((state) => { GetHttpClentsFromLogs(state, ref logCount, ref firstClient, ref secondClient); });

            using var defaultHttpClientFactory = new DefaultHttpClientFactory(_createLazyHttpClientFunc);

            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: RestCountriesAllUri,
                createHttpClient: defaultHttpClientFactory.CreateClient,
                logger: callbackLoggerFactory.CreateLogger<Client>(),
                name: "Test");
            var response = await client.GetAsync<List<RestCountry>>();

            using var client2 = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: RestCountriesAllUri,
                createHttpClient: defaultHttpClientFactory.CreateClient,
                logger: callbackLoggerFactory.CreateLogger<Client>(),
                name: "Test");
            response = await client2.GetAsync<List<RestCountry>>();

#pragma warning disable CA1508 // Avoid dead conditional code
            Assert.IsTrue(ReferenceEquals(firstClient, secondClient));
            Assert.AreEqual(2, logCount);
#pragma warning restore CA1508 // Avoid dead conditional code
        }
#endif

        #endregion

        #region Uri Construction

#if !NET472

        //Test TODOs:
        // - Uri doesn't add a forward slash when only base ur is supplied. E.g. if the base uri is http://www.test.com with no resource, the request uri should be http://www.test.com with no forward slash at the end
        // - Base uri should absolute. Rejust non-absolute Uris
        // - Resource must be relative if base is supplied
        // - If base is not supplied resource must allow absolute
        // - Same tests with string resources

        /// <summary>
        /// Deals with this issue: https://stackoverflow.com/questions/64617310/httpclient-modifies-baseaddress-in-some-cases/64617792?noredirect=1#comment114255884_64617792
        /// </summary>
        [TestMethod]
        public async Task TestConcatenateUrisWithNoSlash()
        {
            //Arrange
            const string expectedUriString = "http://www.test.com/test/test";
            var expectedUri = new Uri(expectedUriString);

            using var mockHttpMessageHandler = new MockHttpMessageHandler();
            _ = mockHttpMessageHandler.When(expectedUriString)
            .Respond(HeadersExtensions.JsonMediaType, "Hi");

            var httpClient = mockHttpMessageHandler.ToHttpClient();

            var baseUri = new Uri("http://www.test.com/test", UriKind.Absolute);
            var resource = new Uri("test", UriKind.Relative);

            using var client = new Client(baseUri: baseUri, createHttpClient: (n) => httpClient);

            //Act
            var response = await client.GetAsync<string>(resource);

            var requestUri = response?.RequestUri;
            if (requestUri == null) throw new InvalidOperationException("No uri");

            //Assert
            Assert.AreEqual(expectedUri, requestUri);
        }
#endif

        #endregion

        #region With

        [TestMethod]
        public void TestWithThrowExceptionOnFailure()
        {
            using var clientBase = GetBaseClient();

            var clientClone = clientBase.With(false);

            Assert.IsFalse(clientClone.ThrowExceptionOnFailure);

            Assert.IsTrue(ReferenceEquals(clientBase.BaseUri, clientClone.BaseUri));

            Assert.IsTrue(ReferenceEquals(clientBase.SerializationAdapter, clientClone.SerializationAdapter));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<IGetHttpRequestMessage>(clientBase, "getHttpRequestMessage"),
                GetFieldValue<IGetHttpRequestMessage>(clientClone, "getHttpRequestMessage")));

            Assert.AreEqual(clientBase.Name, clientClone.Name);
            Assert.AreEqual(clientBase.Timeout, clientClone.Timeout);

            //Note the header reference is getting copied across. This might actually be problematic if the collection is not immutable
            Assert.IsTrue(ReferenceEquals(clientBase.DefaultRequestHeaders, clientClone.DefaultRequestHeaders));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<ILogger<Client>>(clientBase, "logger"),
                GetFieldValue<ILogger<Client>>(clientClone, "logger")
                ));

            Assert.IsTrue(ReferenceEquals(
            GetFieldValue<CreateHttpClient>(clientBase, "createHttpClient"),
            GetFieldValue<CreateHttpClient>(clientClone, "createHttpClient")
            ));

            Assert.IsTrue(ReferenceEquals(
            GetFieldValue<ISendHttpRequestMessage>(clientBase, "sendHttpRequestMessage"),
            GetFieldValue<ISendHttpRequestMessage>(clientClone, "sendHttpRequestMessage")));
        }

        [TestMethod]
        public void TestWithBaseUri()
        {
            using var clientBase = GetBaseClient();

            var baseUri = new Uri("http://www.one.com");

            var clientClone = clientBase.With(baseUri);

            Assert.AreEqual(baseUri, clientClone.BaseUri);

            Assert.IsTrue(ReferenceEquals(clientBase.SerializationAdapter, clientClone.SerializationAdapter));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<IGetHttpRequestMessage>(clientBase, "getHttpRequestMessage"),
                GetFieldValue<IGetHttpRequestMessage>(clientClone, "getHttpRequestMessage")));

            Assert.AreEqual(clientBase.Name, clientClone.Name);
            Assert.AreEqual(clientBase.Timeout, clientClone.Timeout);
            Assert.AreEqual(clientBase.ThrowExceptionOnFailure, clientClone.ThrowExceptionOnFailure);

            //Note the header reference is getting copied across. This might actually be problematic if the collection is not immutable
            Assert.IsTrue(ReferenceEquals(clientBase.DefaultRequestHeaders, clientClone.DefaultRequestHeaders));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<ILogger<Client>>(clientBase, "logger"),
                GetFieldValue<ILogger<Client>>(clientClone, "logger")
                ));

            Assert.IsTrue(ReferenceEquals(
            GetFieldValue<CreateHttpClient>(clientBase, "createHttpClient"),
            GetFieldValue<CreateHttpClient>(clientClone, "createHttpClient")
            ));

            Assert.IsTrue(ReferenceEquals(
            GetFieldValue<ISendHttpRequestMessage>(clientBase, "sendHttpRequestMessage"),
            GetFieldValue<ISendHttpRequestMessage>(clientClone, "sendHttpRequestMessage")));
        }

        [TestMethod]
        public void TestWithSerializationAdapter()
        {
            using var clientBase = GetBaseClient();

            var serializationAdapter = new NewtonsoftSerializationAdapter();

            var clientClone = clientBase.With(serializationAdapter);

            Assert.IsTrue(ReferenceEquals(serializationAdapter, clientClone.SerializationAdapter));

            Assert.AreEqual(clientBase.ThrowExceptionOnFailure, clientClone.ThrowExceptionOnFailure);

            Assert.IsTrue(ReferenceEquals(clientBase.BaseUri, clientClone.BaseUri));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<IGetHttpRequestMessage>(clientBase, "getHttpRequestMessage"),
                GetFieldValue<IGetHttpRequestMessage>(clientClone, "getHttpRequestMessage")));

            Assert.AreEqual(clientBase.Name, clientClone.Name);
            Assert.AreEqual(clientBase.Timeout, clientClone.Timeout);

            //Note the header reference is getting copied across. This might actually be problematic if the collection is not immutable
            Assert.IsTrue(ReferenceEquals(clientBase.DefaultRequestHeaders, clientClone.DefaultRequestHeaders));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<ILogger<Client>>(clientBase, "logger"),
                GetFieldValue<ILogger<Client>>(clientClone, "logger")
                ));

            Assert.IsTrue(ReferenceEquals(
            GetFieldValue<CreateHttpClient>(clientBase, "createHttpClient"),
            GetFieldValue<CreateHttpClient>(clientClone, "createHttpClient")
            ));

            Assert.IsTrue(ReferenceEquals(
            GetFieldValue<ISendHttpRequestMessage>(clientBase, "sendHttpRequestMessage"),
            GetFieldValue<ISendHttpRequestMessage>(clientClone, "sendHttpRequestMessage")));
        }

        [TestMethod]
        public void TestWithGetHttpRequestMessage()
        {
            using var clientBase = GetBaseClient();

            var getHttpRequestMessage = new DefaultGetHttpRequestMessage();

            var clientClone = clientBase.With(getHttpRequestMessage);

            Assert.IsTrue(ReferenceEquals(getHttpRequestMessage, GetFieldValue<IGetHttpRequestMessage>(clientClone, "getHttpRequestMessage")));

            Assert.AreEqual(clientBase.ThrowExceptionOnFailure, clientClone.ThrowExceptionOnFailure);

            Assert.IsTrue(ReferenceEquals(clientBase.BaseUri, clientClone.BaseUri));

            Assert.AreEqual(clientBase.Name, clientClone.Name);
            Assert.AreEqual(clientBase.Timeout, clientClone.Timeout);

            //Note the header reference is getting copied across. This might actually be problematic if the collection is not immutable
            Assert.IsTrue(ReferenceEquals(clientBase.DefaultRequestHeaders, clientClone.DefaultRequestHeaders));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<ILogger<Client>>(clientBase, "logger"),
                GetFieldValue<ILogger<Client>>(clientClone, "logger")
                ));

            Assert.IsTrue(ReferenceEquals(
            GetFieldValue<CreateHttpClient>(clientBase, "createHttpClient"),
            GetFieldValue<CreateHttpClient>(clientClone, "createHttpClient")
            ));

            Assert.IsTrue(ReferenceEquals(
            GetFieldValue<ISendHttpRequestMessage>(clientBase, "sendHttpRequestMessage"),
            GetFieldValue<ISendHttpRequestMessage>(clientClone, "sendHttpRequestMessage")));
        }

        [TestMethod]
        public void TestWithTimeout()
        {
            using var clientBase = GetBaseClient();

            var timeout = new TimeSpan(0, 2, 0);

            var clientClone = clientBase.With(timeout);

            Assert.AreEqual(timeout, clientClone.Timeout);

            Assert.IsTrue(ReferenceEquals(clientBase.BaseUri, clientClone.BaseUri));

            Assert.IsTrue(ReferenceEquals(clientBase.SerializationAdapter, clientClone.SerializationAdapter));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<IGetHttpRequestMessage>(clientBase, "getHttpRequestMessage"),
                GetFieldValue<IGetHttpRequestMessage>(clientClone, "getHttpRequestMessage")));

            Assert.AreEqual(clientBase.Name, clientClone.Name);
            Assert.AreEqual(clientBase.ThrowExceptionOnFailure, clientClone.ThrowExceptionOnFailure);

            //Note the header reference is getting copied across. This might actually be problematic if the collection is not immutable
            Assert.IsTrue(ReferenceEquals(clientBase.DefaultRequestHeaders, clientClone.DefaultRequestHeaders));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<ILogger<Client>>(clientBase, "logger"),
                GetFieldValue<ILogger<Client>>(clientClone, "logger")
                ));

            Assert.IsTrue(ReferenceEquals(
            GetFieldValue<CreateHttpClient>(clientBase, "createHttpClient"),
            GetFieldValue<CreateHttpClient>(clientClone, "createHttpClient")
            ));

            Assert.IsTrue(ReferenceEquals(
            GetFieldValue<ISendHttpRequestMessage>(clientBase, "sendHttpRequestMessage"),
            GetFieldValue<ISendHttpRequestMessage>(clientClone, "sendHttpRequestMessage")));
        }

        [TestMethod]
        public void TestWithHeaders()
        {
            using var clientBase = GetBaseClient();

            var headersCollection = new HeadersCollection(ImmutableDictionary.Create<string, IEnumerable<string>>());

            var clientClone = clientBase.With(headersCollection);

            Assert.AreEqual(headersCollection, clientClone.DefaultRequestHeaders);

            Assert.IsTrue(ReferenceEquals(clientBase.BaseUri, clientClone.BaseUri));

            Assert.IsTrue(ReferenceEquals(clientBase.SerializationAdapter, clientClone.SerializationAdapter));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<IGetHttpRequestMessage>(clientBase, "getHttpRequestMessage"),
                GetFieldValue<IGetHttpRequestMessage>(clientClone, "getHttpRequestMessage")));

            Assert.AreEqual(clientBase.Name, clientClone.Name);
            Assert.AreEqual(clientBase.Timeout, clientClone.Timeout);
            Assert.AreEqual(clientBase.ThrowExceptionOnFailure, clientClone.ThrowExceptionOnFailure);

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<ILogger<Client>>(clientBase, "logger"),
                GetFieldValue<ILogger<Client>>(clientClone, "logger")
                ));

            Assert.IsTrue(ReferenceEquals(
            GetFieldValue<CreateHttpClient>(clientBase, "createHttpClient"),
            GetFieldValue<CreateHttpClient>(clientClone, "createHttpClient")
            ));

            Assert.IsTrue(ReferenceEquals(
            GetFieldValue<ISendHttpRequestMessage>(clientBase, "sendHttpRequestMessage"),
            GetFieldValue<ISendHttpRequestMessage>(clientClone, "sendHttpRequestMessage")));
        }

        [TestMethod]
        public void TestWithHeaders2()
        {
            using var clientBase = GetBaseClient();

            const string Key = "test";
            const string Value = "test1";

            var clientClone = clientBase.WithDefaultRequestHeaders(Key, Value);

            Assert.AreEqual(Value, clientClone.DefaultRequestHeaders[Key].First());

            Assert.IsTrue(ReferenceEquals(clientBase.BaseUri, clientClone.BaseUri));

            Assert.IsTrue(ReferenceEquals(clientBase.SerializationAdapter, clientClone.SerializationAdapter));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<IGetHttpRequestMessage>(clientBase, "getHttpRequestMessage"),
                GetFieldValue<IGetHttpRequestMessage>(clientClone, "getHttpRequestMessage")));

            Assert.AreEqual(clientBase.Name, clientClone.Name);
            Assert.AreEqual(clientBase.Timeout, clientClone.Timeout);
            Assert.AreEqual(clientBase.ThrowExceptionOnFailure, clientClone.ThrowExceptionOnFailure);

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<ILogger<Client>>(clientBase, "logger"),
                GetFieldValue<ILogger<Client>>(clientClone, "logger")
                ));

            Assert.IsTrue(ReferenceEquals(
            GetFieldValue<CreateHttpClient>(clientBase, "createHttpClient"),
            GetFieldValue<CreateHttpClient>(clientClone, "createHttpClient")
            ));

            Assert.IsTrue(ReferenceEquals(
            GetFieldValue<ISendHttpRequestMessage>(clientBase, "sendHttpRequestMessage"),
            GetFieldValue<ISendHttpRequestMessage>(clientClone, "sendHttpRequestMessage")));
        }

        [TestMethod]
        public void TestWithLogger()
        {
            using var clientBase = GetBaseClient();

            var clientClone = clientBase.With(_logger.Object);

            Assert.IsTrue(ReferenceEquals(_logger.Object, GetFieldValue<ILogger<Client>>(clientClone, "logger")));

            Assert.IsTrue(ReferenceEquals(clientBase.BaseUri, clientClone.BaseUri));

            Assert.IsTrue(ReferenceEquals(clientBase.SerializationAdapter, clientClone.SerializationAdapter));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<IGetHttpRequestMessage>(clientBase, "getHttpRequestMessage"),
                GetFieldValue<IGetHttpRequestMessage>(clientClone, "getHttpRequestMessage")));

            Assert.AreEqual(clientBase.Name, clientClone.Name);
            Assert.AreEqual(clientBase.Timeout, clientClone.Timeout);
            Assert.AreEqual(clientBase.ThrowExceptionOnFailure, clientClone.ThrowExceptionOnFailure);

            //Note the header reference is getting copied across. This might actually be problematic if the collection is not immutable
            Assert.IsTrue(ReferenceEquals(clientBase.DefaultRequestHeaders, clientClone.DefaultRequestHeaders));

            Assert.IsTrue(ReferenceEquals(
            GetFieldValue<CreateHttpClient>(clientBase, "createHttpClient"),
            GetFieldValue<CreateHttpClient>(clientClone, "createHttpClient")
            ));

            Assert.IsTrue(ReferenceEquals(
            GetFieldValue<ISendHttpRequestMessage>(clientBase, "sendHttpRequestMessage"),
            GetFieldValue<ISendHttpRequestMessage>(clientClone, "sendHttpRequestMessage")));
        }

        [TestMethod]
        public void TestWithCreateHttpClient()
        {
            using var clientBase = GetBaseClient();

            CreateHttpClient createHttpClient = (n) => new HttpClient();

            var clientClone = clientBase.With(createHttpClient);


            Assert.IsTrue(ReferenceEquals(clientBase.BaseUri, clientClone.BaseUri));

            Assert.IsTrue(ReferenceEquals(clientBase.SerializationAdapter, clientClone.SerializationAdapter));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<IGetHttpRequestMessage>(clientBase, "getHttpRequestMessage"),
                GetFieldValue<IGetHttpRequestMessage>(clientClone, "getHttpRequestMessage")));

            Assert.AreEqual(clientBase.Name, clientClone.Name);
            Assert.AreEqual(clientBase.Timeout, clientClone.Timeout);
            Assert.AreEqual(clientBase.ThrowExceptionOnFailure, clientClone.ThrowExceptionOnFailure);

            //Note the header reference is getting copied across. This might actually be problematic if the collection is not immutable
            Assert.IsTrue(ReferenceEquals(clientBase.DefaultRequestHeaders, clientClone.DefaultRequestHeaders));

            Assert.IsTrue(ReferenceEquals(
            createHttpClient,
            GetFieldValue<CreateHttpClient>(clientClone, "createHttpClient")
            ));

            Assert.IsTrue(ReferenceEquals(
            GetFieldValue<ISendHttpRequestMessage>(clientBase, "sendHttpRequestMessage"),
            GetFieldValue<ISendHttpRequestMessage>(clientClone, "sendHttpRequestMessage")));
        }


        [TestMethod]
        public void TestWithSendHttpRequestMessage()
        {
            using var clientBase = GetBaseClient();

            var sendHttpRequestMessage = new Mock<ISendHttpRequestMessage>().Object;

            var clientClone = clientBase.With(sendHttpRequestMessage);

            Assert.IsTrue(ReferenceEquals(
            sendHttpRequestMessage,
            GetFieldValue<ISendHttpRequestMessage>(clientClone, "sendHttpRequestMessage")
            ));


            Assert.IsTrue(ReferenceEquals(clientBase.SerializationAdapter, clientClone.SerializationAdapter));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<IGetHttpRequestMessage>(clientBase, "getHttpRequestMessage"),
                GetFieldValue<IGetHttpRequestMessage>(clientClone, "getHttpRequestMessage")));

            Assert.IsTrue(ReferenceEquals(
            GetFieldValue<CreateHttpClient>(clientBase, "createHttpClient"),
            GetFieldValue<CreateHttpClient>(clientClone, "createHttpClient")
            ));

            Assert.AreEqual(clientBase.Name, clientClone.Name);
            Assert.AreEqual(clientBase.Timeout, clientClone.Timeout);
            Assert.AreEqual(clientBase.ThrowExceptionOnFailure, clientClone.ThrowExceptionOnFailure);
            Assert.AreEqual(clientBase.BaseUri, clientClone.BaseUri);

            //Note the header reference is getting copied across. This might actually be problematic if the collection is not immutable
            Assert.IsTrue(ReferenceEquals(clientBase.DefaultRequestHeaders, clientClone.DefaultRequestHeaders));
        }

        private static Client GetBaseClient()
        {
            var serializationAdapterMock = new Mock<ISerializationAdapter>();
            const string name = "test";
            var uri = new Uri("http://www.test.com");
            var headersCollectionMock = new Mock<IHeadersCollection>();
            var loggerMock = new Mock<ILogger<Client>>();
#pragma warning disable IDE0039 // Use local function
            CreateHttpClient? createHttpClient = (n) => new HttpClient();
#pragma warning restore IDE0039 // Use local function
            var sendHttpRequestMessageMock = new Mock<ISendHttpRequestMessage>();
            var getHttpRequestMessageMock = new Mock<IGetHttpRequestMessage>();
            var timeout = new TimeSpan(0, 1, 0);
            const bool throwExceptionOnFailure = true;
            var clientBase = new Client(
                            serializationAdapterMock.Object,
                            uri,
                            headersCollectionMock.Object,
                            loggerMock.Object,
                            createHttpClient,
                            sendHttpRequestMessageMock.Object,
                            getHttpRequestMessageMock.Object,
                            timeout,
                            throwExceptionOnFailure
,
                            name);

            return clientBase;
        }

        private static T? GetFieldValue<T>(Client clientClone, string fieldName)
        {
            var fieldInfo = typeof(Client).GetField(fieldName, BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);

            return fieldInfo == null ? throw new InvalidOperationException() : (T?)fieldInfo.GetValue(clientClone);
        }
        #endregion

        #endregion

        #region Helpers

#if !NET472
        //TODO: Point a test at these on .NET 4.5

        private static void GetHttpClientMoq(out Mock<HttpMessageHandler> handlerMock, out HttpClient httpClient, HttpResponseMessage value)
        {
            handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(value)
            .Verifiable();

            httpClient = new HttpClient(handlerMock.Object);
        }

        private static bool CheckRequestMessage(HttpRequestMessage httpRequestMessage, Uri requestUri, List<KeyValuePair<string, IEnumerable<string>>> expectedHeaders, bool hasDefaultJsonContentHeader)
        {
            if (hasDefaultJsonContentHeader)
            {
                KeyValuePair<string, IEnumerable<string>>? contentTypeHeader = httpRequestMessage.Content.Headers.FirstOrDefault(k => k.Key == HeadersExtensions.ContentTypeHeaderName);
                if (contentTypeHeader.Value.Value.FirstOrDefault() != HeadersExtensions.JsonMediaType) return false;
            }

            if (expectedHeaders != null)
            {
                foreach (var expectedHeader in expectedHeaders)
                {
                    KeyValuePair<string, IEnumerable<string>>? foundKeyValuePair = httpRequestMessage.Headers.FirstOrDefault(k => k.Key == expectedHeader.Key);

                    var foundHeaderStrings = foundKeyValuePair.Value.Value.ToList();

                    var i = 0;
                    foreach (var expectedHeaderString in expectedHeader.Value)
                    {
                        if (foundHeaderStrings[i] != expectedHeaderString)
                        {
                            return false;
                        }
                        i++;
                    }
                }
            }

            return
                httpRequestMessage.Method == HttpMethod.Post &&
                httpRequestMessage.RequestUri == requestUri;
        }
#endif

        public static async Task AssertThrowsAsync<T>(Task task, string expectedMessage) where T : Exception
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (ex is T)
                {
                    Assert.AreEqual(expectedMessage, ex.Message);
                    return;
                }

                Assert.Fail($"Expection exception type: {typeof(T)} Actual type: {ex.GetType()}");
            }

            Assert.Fail($"No exception thrown");
        }

        private static HttpClient MintClient()
        {
#if NETCOREAPP3_1

            var httpClient = _testServer.CreateClient();
            testServerBaseUri = httpClient.BaseAddress;
            httpClient.BaseAddress = null;
            return httpClient;
#else
            testServerBaseUri = new Uri(LocalBaseUriString);
            return new HttpClient();
#endif
        }

        private static IClient GetJsonClient(Uri? baseUri = null)
        {
            IClient restClient;

            var defaultHeaders = HeadersExtensions.CreateHeadersCollectionWithJsonContentType();

            if (baseUri != null)
            {
                var httpClient = MintClient();
                var testClientFactory = new TestClientFactory(httpClient);
                restClient = new Client(
                    new NewtonsoftSerializationAdapter(),
                    baseUri: baseUri,
                    createHttpClient: testClientFactory.CreateClient,
                    defaultRequestHeaders: defaultHeaders);
            }
            else
            {
                restClient = new Client(
                    new NewtonsoftSerializationAdapter(),
                    baseUri: testServerBaseUri,
                    createHttpClient: _testServerHttpClientFactory.CreateClient,
                    defaultRequestHeaders: defaultHeaders);
            }

            return restClient;
        }

#if !NET472
        private static bool CheckRequestHeaders(IHeadersCollection requestHeadersCollection) =>
            requestHeadersCollection.Contains("Test") && requestHeadersCollection["Test"].First() == "Test";

        private static bool CheckResponseHeaders(IHeadersCollection responseHeadersCollection) => responseHeadersCollection.Contains("Test1") && responseHeadersCollection["Test1"].First() == "a";
#endif
        #endregion
    }
}
