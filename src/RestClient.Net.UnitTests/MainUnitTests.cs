
using ApiExamples.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
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

#if NETCOREAPP3_1
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using ApiExamples;
#endif

#if NET45
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

        private static readonly ILoggerFactory consoleLoggerFactory =
#if NET45
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
        private readonly Uri RestCountriesAllUri = new Uri(RestCountriesAllUriString);
        private readonly Uri RestCountriesAustraliaUri = new Uri(RestCountriesAustraliaUriString);
        private readonly Uri JsonPlaceholderBaseUri = new Uri(JsonPlaceholderBaseUriString);
        //private readonly Uri JsonPlaceholderFirstPostUri = new Uri(JsonPlaceholderBaseUriString + JsonPlaceholderFirstPostSlug);
        private const string TransferEncodingHeaderName = "Transfer-Encoding";
        private const string SetCookieHeaderName = "Set-Cookie";
        private const string CacheControlHeaderName = "Cache-Control";
        private const string XRatelimitLimitHeaderName = "X-Ratelimit-Limit";

        private static readonly UserPost _userRequestBody = new UserPost { title = "foo", userId = 10, body = "testbody" };

        private static readonly string _userRequestBodyJson = "{\r\n" +
                $"  \"userId\": {_userRequestBody.userId},\r\n" +
                "  \"id\": 0,\r\n" +
                "  \"title\": \"foo\",\r\n" +
                "  \"body\": \"testbody\"\r\n" +
                "}";

        private readonly Dictionary<string, string> RestCountriesAllHeaders = new Dictionary<string, string>
        {
            {"Date", "Wed, 17 Jun 2020 22:51:03 GMT" },
            {TransferEncodingHeaderName, "chunked" },
            {"Connection", "keep-alive" },
            {"Set-Cookie", "__cfduid=dde664b010195275c339e4b049626e6261592434261; expires=Fri, 17-Jul-20 22:51:03 GMT; path=/; domain=.restcountries.eu; HttpOnly; SameSite=Lax" },
            {"Access-Control-Allow-Origin", "*" },
            {"Access-Control-Allow-Methods", "GET" },
            {"Access-Control-Allow-Headers", "Accept, X-Requested-With" },
            {CacheControlHeaderName, "public, max-age=86400" },
            {"CF-Cache-Status", "DYNAMIC" },
            {"cf-request-id", "0366139e2100001258170ec200000001" },
            {"Expect-CT", "max-age=604800, report-uri=\"https://report-uri.cloudflare.com/cdn-cgi/beacon/expect-ct\"" },
            {"Server", "cloudflare" },
            {"CF-RAY", "5a50554368bf1258-HKG" },
        };

        private readonly Dictionary<string, string> JsonPlaceholderDeleteHeaders = new Dictionary<string, string>
        {
            {"Date", "Thu, 18 Jun 2020 09:17:40 GMT" },
            {"Connection", "keep-alive" },
            {SetCookieHeaderName, "__cfduid=d4048d349d1b9a8c70f8eb26dbf91e9a91592471851; expires=Sat, 18-Jul-20 09:17:36 GMT; path=/; domain=.typicode.com; HttpOnly; SameSite=Lax" },
            {"X-Powered-By", "Express" },
            {"Vary", "Origin, Accept-Encoding" },
            {"Access-Control-Allow-Credentials", "true" },
            {CacheControlHeaderName, "no-cache" },
            {"Pragma", "no-cache" },
            {"Expires", "1" },
            {"X-Content-Type-Options", "nosniff" },
            {"Etag", "W/\"2-vyGp6PvFo4RvsFtPoIWeCReyIC1\"" },
            {"Via", "1.1 vegur" },
            {"CF-Cache-Status", "DYNAMIC" },
            {"cf-request-id", "0368513dc10000ed3f0020a200000001" },
            {"Expect-CT", "max-age=604800, report-uri=\"https://report-uri.cloudflare.com/cdn-cgi/beacon/expect-ct\"" },
            {"Server", "cloudflare" },
            {"CF-RAY", "5a52eb0f9d0bed3f-SJC" },
         };

        private readonly Dictionary<string, string> JsonPlaceholderPostHeaders = new Dictionary<string, string>
        {
            {"Date", "Thu, 18 Jun 2020 09:17:40 GMT" },
            {"Connection", "keep-alive" },
            {SetCookieHeaderName, "__cfduid=d4048d349d1b9a8c70f8eb26dbf91e9a91592471851; expires=Sat, 18-Jul-20 09:17:36 GMT; path=/; domain=.typicode.com; HttpOnly; SameSite=Lax" },
            {"X-Powered-By", "Express" },
            {XRatelimitLimitHeaderName, "10000" },
            {"X-Ratelimit-Remaining", "9990" },
            {"X-Ratelimit-Reset", "1592699847" },
            {"Vary", "Origin, Accept-Encoding" },
            {"Access-Control-Allow-Credentials", "true" },
            {CacheControlHeaderName, "no-cache" },
            {"Pragma", "no-cache" },
            {"Expires", "1" },
            {"Location","http://jsonplaceholder.typicode.com/posts/101" },
            {"X-Content-Type-Options", "nosniff" },
            {"Etag", "W/\"2-vyGp6PvFo4RvsFtPoIWeCReyIC1\"" },
            {"Via", "1.1 vegur" },
            {"CF-Cache-Status", "DYNAMIC" },
            {"cf-request-id", "0368513dc10000ed3f0020a200000002" },
            {"Expect-CT", "max-age=604800, report-uri=\"https://report-uri.cloudflare.com/cdn-cgi/beacon/expect-ct\"" },
            {"Server", "cloudflare" },
            {"CF-RAY", "5a52eb0f9d0bed3f-SJC" },
         };

        private readonly Dictionary<string, string> GoogleHeadHeaders = new Dictionary<string, string>
        {
            {"P3P", "CP=\"This is not a P3P policy! See g.co/p3phelp for more info.\"" },
            {"Date", "Sun, 21 Jun 2020 02:38:45 GMT" },
            {SetCookieHeaderName, "1P_JAR=2020-06-21-02; expires=Tue, 21-Jul-2020 02:38:45 GMT; path=/; domain=.google.com; Secure" },
            //TODO: there should be two lines of cookie here but mock http doesn't seem to allow for this...
            {"Server", "gws" },
            {"X-XSS-Protection", "0" },
            {"X-Frame-Options", "SAMEORIGIN" },
            {"Transfer-Encoding", "SAMEORIGIN" },
            {"Expires", "Sun, 21 Jun 2020 02:38:45 GMT" },
            {CacheControlHeaderName, "private" },
         };


        //For realises - with factory
        //private static readonly CreateHttpClient _createHttpClient = (n) => new HttpClient();
        //For realsies - no factory
        //private CreateHttpClient _createHttpClient = null;

        //Mock the httpclient
        private static readonly MockHttpMessageHandler _mockHttpMessageHandler = new MockHttpMessageHandler();
        private static readonly CreateHttpClient _createHttpClient = (n) => _mockHttpMessageHandler.ToHttpClient();
        private static readonly TestClientFactory _testServerHttpClientFactory;
        private static Mock<ILogger<Client>> _logger = new Mock<ILogger<Client>>();

#if NETCOREAPP3_1
        public const string LocalBaseUriString = "http://localhost";
        private static readonly TestServer _testServer;
#else
        public const string LocalBaseUriString = "https://localhost:44337";
#endif

        private readonly Func<string, Lazy<HttpClient>> _createLazyHttpClientFunc = (n) =>
        {
            var client = _createHttpClient(n);
            return new Lazy<HttpClient>(() => client);
        };
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


        static MainUnitTests()
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
                baseUri: baseUri,
                createHttpClient: _createHttpClient
                );

            var response = await client.SendAsync<string, object>(new Request(
                null,
                null,
                null,
                HttpRequestMethod.Custom,
                client,
                default,
                "HEAD")).ConfigureAwait(false);

            Assert.AreEqual(GoogleHeadHeaders[CacheControlHeaderName], response.Headers[CacheControlHeaderName].Single());
        }

#if !NET45


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

            _ = await client.PostAsync<List<RestCountry>, object>(parameters, null, headers).ConfigureAwait(false);
            _ = await client.PostAsync<List<RestCountry>, object>(parameters, null, headers).ConfigureAwait(false);
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
            _ = await client.PostAsync<List<RestCountry>, object>(new object(), null, testKvp.CreateHeadersCollection()).ConfigureAwait(false);

            //Make sure we can call it twice
            _ = await client.PostAsync<List<RestCountry>, object>(new object(), null, testKvp.CreateHeadersCollection()).ConfigureAwait(false);

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
            List<RestCountry> countries = await client.GetAsync<List<RestCountry>>().ConfigureAwait(false);
            Assert.IsNotNull(countries);
            Assert.IsTrue(countries.Count > 0);
        }

        [TestMethod]
        public async Task TestGetDefaultSerializationRestCountriesAsJson()
        {
            using var client = new Client(
                baseUri: RestCountriesAustraliaUri,
                createHttpClient: _createHttpClient);
            var json = await client.GetAsync<string>().ConfigureAwait(false);

            var country = JsonConvert.DeserializeObject<List<RestCountry>>(json).FirstOrDefault();
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

            await AssertThrowsAsync<HttpStatusException>(client.GetAsync<List<RestCountry>>(), Messages.GetErrorMessageNonSuccess((int)statusCode, RestCountriesAllUri)).ConfigureAwait(false);
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

            var response = await client.GetAsync<List<RestCountry>>().ConfigureAwait(false);

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

            var response = await client.GetAsync<List<RestCountry>>().ConfigureAwait(false);
            Assert.IsNotNull(response);
            Assert.IsTrue(response?.Body?.Count > 0);

#if !NET45
            _logger.VerifyLog((state, t) => state.CheckValue("{OriginalFormat}", Messages.InfoSendReturnedNoException), LogLevel.Information, 1);

            _logger.VerifyLog((state, t) =>
            state.CheckValue<IRequest>("request", (a) => a.Resource == null && a.HttpRequestMethod == HttpRequestMethod.Get) &&
            state.CheckValue("{OriginalFormat}", Messages.InfoAttemptingToSend)
            , LogLevel.Trace, 1);

            _logger.VerifyLog((state, t) =>
            state.CheckValue("{OriginalFormat}", Messages.TraceResponseProcessed) &&
            state.CheckValue<Response>("response", (a) => a.RequestUri == RestCountriesAllUri && a.StatusCode == 200)
            , LogLevel.Trace, 1);
#endif

            var httpResponseMessageResponse = response as HttpResponseMessageResponse<List<RestCountry>>;

            Assert.AreEqual(StandardContentTypeToString, httpResponseMessageResponse?.HttpResponseMessage?.Content?.Headers?.ContentType?.ToString());

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
            var response = await client.DeleteAsync(JsonPlaceholderFirstPostSlug).ConfigureAwait(false);

#if !NET45
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
                null,
                RestCountriesAustraliaUri,
                createHttpClient: _createHttpClient);

            var json = await client.GetAsync<string>().ConfigureAwait(false);
            var country = JsonConvert.DeserializeObject<List<RestCountry>>(json).FirstOrDefault();
            Assert.AreEqual("Australia", country.name);
        }

        [TestMethod]
        public async Task TestGetRestCountriesNoBaseUri()
        {
            using var client = new Client(new NewtonsoftSerializationAdapter(), createHttpClient: _createHttpClient);
            List<RestCountry> countries = await client.GetAsync<List<RestCountry>>(RestCountriesAustraliaUri).ConfigureAwait(false);
            var country = countries.FirstOrDefault();
            Assert.AreEqual("Australia", country.name);
        }

        [TestMethod]
        public async Task TestAbsoluteUriAsStringThrowsException()
        {
            try
            {
                using var client = new Client(new NewtonsoftSerializationAdapter(), createHttpClient: _createHttpClient);
                List<RestCountry> countries = await client.GetAsync<List<RestCountry>>(RestCountriesAustraliaUriString).ConfigureAwait(false);
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
                using var client = new Client(new NewtonsoftSerializationAdapter(), baseUri: JsonPlaceholderBaseUri);

                using var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;

                var task = client.PostAsync<UserPost, UserPost>(new UserPost { title = "Moops" }, new Uri("/posts", UriKind.Relative), cancellationToken: token);

                tokenSource.Cancel();

                _ = await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
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
                using var client = new Client(
                    new NewtonsoftSerializationAdapter(),
                    baseUri: JsonPlaceholderBaseUri,
                    timeout: new TimeSpan(0, 0, 0, 0, 1),
                    logger: _logger.Object);

                _ = await client.PostAsync<UserPost, UserPost>(new UserPost { title = "Moops" }, new Uri("/posts", UriKind.Relative)).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
#if !NET45
                _logger.VerifyLog<Client, TaskCanceledException>((state, t)
                    => state.CheckValue("{OriginalFormat}", Messages.ErrorOnSend), LogLevel.Error, 1);
#endif

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
                defaultRequestHeaders: HeadersExtensions.SetJsonContentTypeHeader());
            var responseUserPost = httpRequestMethod switch
            {
                HttpRequestMethod.Patch => await client.PatchAsync<UserPost, UserPost>(_userRequestBody, new Uri("/posts/1", UriKind.Relative)).ConfigureAwait(false),
                //TODO: Shouldn't expectedStatusCode = HttpStatusCode.Created
                HttpRequestMethod.Post => await client.PostAsync<UserPost, UserPost>(_userRequestBody, "/posts").ConfigureAwait(false),
                HttpRequestMethod.Put => await client.PutAsync<UserPost, UserPost>(_userRequestBody, new Uri("/posts/1", UriKind.Relative)).ConfigureAwait(false),
                HttpRequestMethod.Get => throw new NotImplementedException(),
                HttpRequestMethod.Delete => throw new NotImplementedException(),
                HttpRequestMethod.Custom => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
            Assert.AreEqual(_userRequestBody.userId, responseUserPost.Body?.userId);
            Assert.AreEqual(_userRequestBody.title, responseUserPost.Body?.title);

#if !NET45
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
            var response = await client.PostAsync<PostUserResponse, UserPost>(_userRequestBody, JsonPlaceholderPostsSlug).ConfigureAwait(false);
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
            var geoPlugin = await client.GetAsync<GeoPlugin>().ConfigureAwait(false);
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
            var responsePerson = await client.PostAsync<Person, Person>(requestPerson, new Uri($"{LocalBaseUriString}/person")).ConfigureAwait(false);
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

            Person responsePerson = await client.PutAsync<Person, Person>(requestPerson, new Uri($"{LocalBaseUriString}/person")).ConfigureAwait(false);
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
            using var client = new Client(new NewtonsoftSerializationAdapter(), createHttpClient: _testServerHttpClientFactory.CreateClient);
            var headers = GetHeaders(useDefault, client);
            Person responsePerson = await client.GetAsync<Person>(new Uri("headers", UriKind.Relative), headers).ConfigureAwait(false);
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersResponseLocalGet()
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: "Test".CreateHeadersCollection("Test"));

            var response = await client.GetAsync<Person>("headers").ConfigureAwait(false);

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
            using var client = new Client(new NewtonsoftSerializationAdapter(), null, null, logger: _logger.Object, createHttpClient: _testServerHttpClientFactory.CreateClient);
            var headers = GetHeaders(useDefault, client);
            var response = await client.GetAsync<Person>(new Uri("headers", UriKind.Relative), requestHeaders: headers).ConfigureAwait(false);

#if !NET45
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
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions.SetJsonContentTypeHeader()
                );
            var headers = GetHeaders(useDefault, client);
            var responsePerson = await client.PostAsync<Person, Person>(
                new Person { FirstName = "Bob" },
                new Uri("headers", UriKind.Relative),
                requestHeaders: headers
                ).ConfigureAwait(false);
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectGet()
        {
            try
            {
                using var client = new Client(
                    new NewtonsoftSerializationAdapter(),
                    createHttpClient: _testServerHttpClientFactory.CreateClient,
                    //The server expects the value of "Test"
                    defaultRequestHeaders: "Test".CreateHeadersCollection("Tests"));

                var responsePerson = await client.GetAsync<Person>(new Uri("headers", UriKind.Relative)).ConfigureAwait(false);
                Assert.Fail();
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.Response.StatusCode);
                var apiResult = hex.Client.DeserializeResponseBody<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
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
                using var client = new Client(
                    new NewtonsoftSerializationAdapter(),
                    createHttpClient: _testServerHttpClientFactory.CreateClient,
                    //The server expects the value of "Test"
                    defaultRequestHeaders: HeadersExtensions.SetJsonContentTypeHeader().Append("Test", "Tests"));

                var responsePerson = await client.PostAsync<Person, Person>(new Person(), new Uri("headers", UriKind.Relative)).ConfigureAwait(false);
                Assert.Fail();
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.Response.StatusCode);
                var apiResult = hex.Client.DeserializeResponseBody<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
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
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions.SetJsonContentTypeHeader());

            var headers = GetHeaders(useDefault, client);
            var responsePerson = await client.PutAsync<Person, Person>(
                new Person { FirstName = "Bob" },
                new Uri("headers", UriKind.Relative),
                requestHeaders: headers
                ).ConfigureAwait(false);
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalPutStringOverload()
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions.SetJsonContentTypeHeader().Append("Test", "Test"));

            var responsePerson = await client.PutAsync<Person, Person>(new Person { FirstName = "Bob" }, "headers").ConfigureAwait(false);
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectPut()
        {
            try
            {
                using var client = new Client(
                    new NewtonsoftSerializationAdapter(),
                    createHttpClient: _testServerHttpClientFactory.CreateClient,
                    //The server expects the value of "Test"
                    defaultRequestHeaders: HeadersExtensions.SetJsonContentTypeHeader().Append("Test", "Tests"));

                var responsePerson = await client.PutAsync<Person, Person>(new Person(), new Uri("headers", UriKind.Relative)).ConfigureAwait(false);
                Assert.Fail();
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.Response.StatusCode);
                var apiResult = hex.Client.DeserializeResponseBody<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
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
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions.SetJsonContentTypeHeader());

            var headers = GetHeaders(useDefault, client);
            var responsePerson = await client.PatchAsync<Person, Person>(
                new Person { FirstName = "Bob" },
                new Uri("headers", UriKind.Relative),
                requestHeaders: headers
                ).ConfigureAwait(false);
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectPatch()
        {
            try
            {
                using var client = new Client(
                    new NewtonsoftSerializationAdapter(),
                    createHttpClient: _testServerHttpClientFactory.CreateClient,
                    defaultRequestHeaders: HeadersExtensions.SetJsonContentTypeHeader().Append("Test", "Tests"));

                var responsePerson = await client.PatchAsync<Person, Person>(new Person(), new Uri("headers", UriKind.Relative)).ConfigureAwait(false);
                Assert.Fail();
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.Response.StatusCode);
                var apiResult = hex.Client.DeserializeResponseBody<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
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
            using var client = new Client(new NewtonsoftSerializationAdapter(), createHttpClient: _testServerHttpClientFactory.CreateClient);
            var headers = GetHeaders(useDefault, client);
            _ = await client.DeleteAsync(new Uri("headers/1", UriKind.Relative), headers).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task TestHeadersLocalIncorrectDelete()
        {
            try
            {
                using var client = new Client(new NewtonsoftSerializationAdapter(), createHttpClient: _testServerHttpClientFactory.CreateClient);
                _ = await client.DeleteAsync(new Uri("headers/1", UriKind.Relative)).ConfigureAwait(false);
                Assert.Fail();
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.Response.StatusCode);
                var apiResult = hex.Client.DeserializeResponseBody<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
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
            using var client = new Client(new NewtonsoftSerializationAdapter(), createHttpClient: _testServerHttpClientFactory.CreateClient);
            var requestHeadersCollection = "Test".CreateHeadersCollection("Test");
            Person responsePerson = await client.SendAsync<Person, object>
                (
                new Request(new Uri("headers", UriKind.Relative), null, requestHeadersCollection, HttpRequestMethod.Get, client, default)
                ).ConfigureAwait(false); ;
            Assert.IsNotNull(responsePerson);
        }
        #endregion

        #region Local Errors
        [TestMethod]
        public async Task TestErrorsLocalGet()
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                throwExceptionOnFailure: false
                );

            var response = await client.GetAsync<Person>("error").ConfigureAwait(false);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
            var apiResult = client.DeserializeResponseBody<ApiResult>(response.GetResponseData(), response.Headers);
            Assert.AreEqual(ApiMessages.ErrorControllerErrorMessage, apiResult.Errors.First());

            //Check that the response values are getting set correctly
            Assert.AreEqual(new Uri($"{LocalBaseUriString}/error"), response.RequestUri);
            Assert.AreEqual(HttpRequestMethod.Get, response.HttpRequestMethod);
        }

        [TestMethod]
        public async Task TestErrorsLocalGetThrowException()
        {
            using var restClient = new Client(new NewtonsoftSerializationAdapter(), createHttpClient: _testServerHttpClientFactory.CreateClient);

            try
            {
                var response = await restClient.GetAsync<Person>("error").ConfigureAwait(false);
                Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
            }
            catch (HttpStatusException hex)
            {
                var apiResult = restClient.DeserializeResponseBody<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
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
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions.SetJsonContentTypeHeader());

            var response = await client.PostAsync<AuthenticationResult, AuthenticationRequest>(
                new AuthenticationRequest { ClientId = "a", ClientSecret = "b" },
                new Uri("secure/authenticate", UriKind.Relative)
                ).ConfigureAwait(false);

            var bearerToken = response.Body?.BearerToken;

            if (bearerToken == null) throw new InvalidOperationException("No bearer token");

            using var client2 = new Client(
                new NewtonsoftSerializationAdapter(),
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions
                .SetJsonContentTypeHeader()
                .SetBearerTokenAuthenticationHeader(bearerToken));

            Person person = await client.GetAsync<Person>(new Uri("secure/bearer", UriKind.Relative)).ConfigureAwait(false);
            Assert.AreEqual("Bear", person.FirstName);
        }

        [TestMethod]
        public async Task TestBasicAuthenticationLocalWithError()
        {
            using var restClient = new Client(
                new NewtonsoftSerializationAdapter(),
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions.SetBasicAuthenticationHeader("Bob", "WrongPassword"));

            try
            {
                Person person = await restClient.GetAsync<Person>(new Uri("secure/basic", UriKind.Relative)).ConfigureAwait(false);
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.Unauthorized, hex.Response.StatusCode);
                var apiResult = restClient.DeserializeResponseBody<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
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
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions.SetBasicAuthenticationHeader("Bob", "ANicePassword"));

            Person person = await client.GetAsync<Person>(new Uri("secure/basic", UriKind.Relative)).ConfigureAwait(false);
            Assert.AreEqual("Sam", person.FirstName);
        }

        [TestMethod]
        public async Task TestBearerTokenAuthenticationLocalWithError()
        {
            using var restClient = new Client(new NewtonsoftSerializationAdapter(), createHttpClient: _testServerHttpClientFactory.CreateClient);
            try
            {
                restClient.SetBearerTokenAuthenticationHeader("321");
                Person person = await restClient.GetAsync<Person>(new Uri("secure/bearer", UriKind.Relative)).ConfigureAwait(false);
            }
            catch (HttpStatusException hex)
            {
                Assert.AreEqual((int)HttpStatusCode.Unauthorized, hex.Response.StatusCode);
                var apiResult = restClient.DeserializeResponseBody<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
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
                createHttpClient: _testServerHttpClientFactory.CreateClient);
            client.SetBasicAuthenticationHeader("Bob", "ANicePassword");
            client.SetJsonContentTypeHeader();
            Person person = await client.PostAsync<Person, Person>(new Person { FirstName = "Sam" }, new Uri("secure/basic", UriKind.Relative)).ConfigureAwait(false);
            Assert.AreEqual("Sam", person.FirstName);
        }

        [TestMethod]
        public async Task TestBasicAuthenticationPostLocalWithError()
        {
            using var restClient = new Client(
                new NewtonsoftSerializationAdapter(),
                createHttpClient: _testServerHttpClientFactory.CreateClient);

            try
            {
                restClient.SetBasicAuthenticationHeader("Bob", "WrongPassword");
                restClient.SetJsonContentTypeHeader();
                Person person = await restClient.PostAsync<Person, Person>(new Person { FirstName = "Sam" }, new Uri("secure/basic", UriKind.Relative)).ConfigureAwait(false);
            }
            catch (HttpStatusException ex)
            {
                Assert.AreEqual((int)HttpStatusCode.Unauthorized, ex.Response.StatusCode);
                var apiResult = restClient.DeserializeResponseBody<ApiResult>(ex.Response.GetResponseData(), ex.Response.Headers);
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
            jsonperson responsePerson = await client.GetAsync<jsonperson>().ConfigureAwait(false);
            Assert.IsNotNull(responsePerson);
            Assert.IsNotNull("Sam", responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalGetStringUri()
        {
            var client = GetJsonClient();
            jsonperson responsePerson = await client.GetAsync<jsonperson>("JsonPerson").ConfigureAwait(false);
            Assert.IsNotNull(responsePerson);
            Assert.IsNotNull("Sam", responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalGetUri()
        {
            var client = GetJsonClient();
            jsonperson responsePerson = await client.GetAsync<jsonperson>(new Uri("JsonPerson", UriKind.Relative)).ConfigureAwait(false);
            Assert.IsNotNull(responsePerson);
            Assert.IsNotNull("Sam", responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalGetUriCancellationToken()
        {
            var client = GetJsonClient();
            jsonperson responsePerson = await client.GetAsync<jsonperson>(new Uri("JsonPerson", UriKind.Relative), cancellationToken: new CancellationToken()).ConfigureAwait(false);
            Assert.IsNotNull(responsePerson);
            Assert.IsNotNull("Sam", responsePerson.FirstName);
        }
        #endregion

        #region Delete
        [TestMethod]
        public async Task TestLocalDeleteStringUri()
        {
            var client = GetJsonClient(new Uri($"{LocalBaseUriString}/JsonPerson"));
            var response = await client.DeleteAsync("?personKey=abc").ConfigureAwait(false);
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public async Task TestLocalDeleteUri()
        {
            var client = GetJsonClient(new Uri($"{LocalBaseUriString}/JsonPerson"));
            var response = await client.DeleteAsync(new Uri("?personKey=abc", UriKind.Relative)).ConfigureAwait(false);
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public async Task TestLocalDeleteUriCancellationToken()
        {
            var client = GetJsonClient(new Uri($"{LocalBaseUriString}/JsonPerson"));
            var response = await client.DeleteAsync(new Uri("?personKey=abc", UriKind.Relative), cancellationToken: new CancellationToken()).ConfigureAwait(false);
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
            jsonperson responsePerson = await client.PostAsync<jsonperson, jsonperson>(requestPerson).ConfigureAwait(false);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPostBodyStringUri()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PostAsync<jsonperson, jsonperson>(requestPerson, "jsonperson/save").ConfigureAwait(false);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPostBodyUri()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PostAsync<jsonperson, jsonperson>(requestPerson, new Uri("jsonperson/save", UriKind.Relative)).ConfigureAwait(false);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPostBodyUriCancellationToken()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PostAsync<jsonperson, jsonperson>(requestPerson, new Uri("jsonperson/save", UriKind.Relative), null, new CancellationToken()).ConfigureAwait(false);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }
        #endregion

        #region Put
        [TestMethod]
        public async Task TestLocalPutBody()
        {
            var client = GetJsonClient(new Uri($"{LocalBaseUriString}/jsonperson/save"));
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PutAsync<jsonperson, jsonperson>(requestBody: requestPerson).ConfigureAwait(false);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPutBodyStringUri()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PutAsync<jsonperson, jsonperson>(requestPerson, "jsonperson/save").ConfigureAwait(false);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPutBodyUri()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PutAsync<jsonperson, jsonperson>(requestPerson, new Uri("jsonperson/save", UriKind.Relative)).ConfigureAwait(false);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPutBodyUriCancellationToken()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PutAsync<jsonperson, jsonperson>(requestPerson, new Uri("jsonperson/save", UriKind.Relative), cancellationToken: new CancellationToken()).ConfigureAwait(false);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }
        #endregion

        #region Patch
        [TestMethod]
        public async Task TestLocalPatchBody()
        {
            var client = GetJsonClient(new Uri($"{LocalBaseUriString}/jsonperson/save"));
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PatchAsync<jsonperson, jsonperson>(requestPerson).ConfigureAwait(false);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyStringUri()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PatchAsync<jsonperson, jsonperson>(requestPerson, "jsonperson/save").ConfigureAwait(false);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyUri()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PatchAsync<jsonperson, jsonperson>(requestPerson, new Uri("jsonperson/save", UriKind.Relative)).ConfigureAwait(false);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyUriCancellationToken()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PatchAsync<jsonperson, jsonperson>(requestPerson, new Uri("jsonperson/save", UriKind.Relative), cancellationToken: new CancellationToken()).ConfigureAwait(false);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyUriCancellationTokenContentType()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson responsePerson = await client.PatchAsync<jsonperson, jsonperson>(requestPerson, new Uri("jsonperson/save", UriKind.Relative), cancellationToken: new CancellationToken()).ConfigureAwait(false);
            Assert.AreEqual(requestPerson.FirstName, responsePerson.FirstName);
        }
        #endregion

        #endregion

        #region Misc

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
            var response = new HttpResponseMessageResponse<string>(
                headersMock.Object,
                10,
                HttpRequestMethod.Custom,
                //Shouldn't this be filled in?
                new byte[0],
                "test",
                httpResponseMessage,
                httpClient);

            _ = clientMock.Setup(c => c.SendAsync<string>(It.IsAny<IRequest>())).Returns
                (
                Task.FromResult<Response<string>>(response)
                );

            var returnedResponse = await clientMock.Object.SendAsync<string>(new Mock<IRequest>().Object).ConfigureAwait(false);

            Assert.IsTrue(ReferenceEquals(response, returnedResponse));
        }

        [TestMethod]
        public async Task TestErrorLogging()
        {
            try
            {
                using var client = new Client(new NewtonsoftSerializationAdapter(), logger: _logger.Object);
                var requestPerson = new Person();
                Person responsePerson = await client.PostAsync<Person, Person>(requestPerson).ConfigureAwait(false);
            }
            catch (SendException)
            {
#if !NET45
                _logger.VerifyLog<Client, SendException>((state, t)
                    => state.CheckValue("{OriginalFormat}", Messages.ErrorSendException), LogLevel.Error, 1);
#endif

                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public async Task TestFactoryCreationWithUri()
        {
            var clientFactory = new ClientFactory(_createHttpClient, new NewtonsoftSerializationAdapter());
            var client = ClientFactoryExtensions.CreateClient(clientFactory.CreateClient, "test", RestCountriesAllUri);
            var response = await client.GetAsync<List<RestCountry>>().ConfigureAwait(false);
            Assert.IsTrue(response.Body?.Count > 0);
        }

        [TestMethod]
        public async Task TestFactoryDoesntUseSameHttpClient()
        {
            var clientFactory = new ClientFactory(_createHttpClient, new NewtonsoftSerializationAdapter());

            var client = ClientFactoryExtensions.CreateClient(clientFactory.CreateClient, "1", RestCountriesAllUri);
            var response = (HttpResponseMessageResponse<List<RestCountry>>)await client.GetAsync<List<RestCountry>>().ConfigureAwait(false);
            var firstClient = response.HttpClient;

            client = ClientFactoryExtensions.CreateClient(clientFactory.CreateClient, "2", RestCountriesAllUri);
            response = (HttpResponseMessageResponse<List<RestCountry>>)await client.GetAsync<List<RestCountry>>().ConfigureAwait(false);
            var secondClient = response.HttpClient;

            Assert.IsFalse(ReferenceEquals(firstClient, secondClient));
        }

        [TestMethod]
        public async Task TestHttpClientFactoryDoesntUseSameHttpClient()
        {
            using var defaultHttpClientFactory = new DefaultHttpClientFactory(_createLazyHttpClientFunc);

            using var client = new Client(new NewtonsoftSerializationAdapter(), baseUri: RestCountriesAllUri, createHttpClient: defaultHttpClientFactory.CreateClient);
            var response = (HttpResponseMessageResponse<List<RestCountry>>)await client.GetAsync<List<RestCountry>>().ConfigureAwait(false);
            var firstClient = response.HttpClient;

            using var client2 = new Client(new NewtonsoftSerializationAdapter(), baseUri: RestCountriesAllUri, createHttpClient: defaultHttpClientFactory.CreateClient);
            response = (HttpResponseMessageResponse<List<RestCountry>>)await client2.GetAsync<List<RestCountry>>().ConfigureAwait(false);
            var secondClient = response.HttpClient;

            Assert.IsFalse(ReferenceEquals(firstClient, secondClient));
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

            Assert.IsTrue(ReferenceEquals(firstClient, secondClient));
        }

        [TestMethod]
        public async Task TestHttpClientFactoryReusesHttpClient()
        {
            using var defaultHttpClientFactory = new DefaultHttpClientFactory(_createLazyHttpClientFunc);

            using var client = new Client(new NewtonsoftSerializationAdapter(), baseUri: RestCountriesAllUri, createHttpClient: defaultHttpClientFactory.CreateClient);
            var response = (HttpResponseMessageResponse<List<RestCountry>>)await client.GetAsync<List<RestCountry>>().ConfigureAwait(false);
            var firstClient = response.HttpClient;

            response = (HttpResponseMessageResponse<List<RestCountry>>)await client.GetAsync<List<RestCountry>>().ConfigureAwait(false);
            var secondClient = response.HttpClient;

            Assert.IsTrue(ReferenceEquals(firstClient, secondClient));
        }

        [TestMethod]
        public async Task TestHttpClientFactoryReusesHttpClientWhenSameName()
        {
            using var defaultHttpClientFactory = new DefaultHttpClientFactory(_createLazyHttpClientFunc);

            using var client = new Client(new NewtonsoftSerializationAdapter(), baseUri: RestCountriesAllUri, createHttpClient: defaultHttpClientFactory.CreateClient, name: "Test");
            var response = (HttpResponseMessageResponse<List<RestCountry>>)await client.GetAsync<List<RestCountry>>().ConfigureAwait(false);
            var firstClient = response.HttpClient;

            using var client2 = new Client(new NewtonsoftSerializationAdapter(), baseUri: RestCountriesAllUri, createHttpClient: defaultHttpClientFactory.CreateClient, name: "Test");
            response = (HttpResponseMessageResponse<List<RestCountry>>)await client2.GetAsync<List<RestCountry>>().ConfigureAwait(false);
            var secondClient = response.HttpClient;

            Assert.IsTrue(ReferenceEquals(firstClient, secondClient));
        }

        #endregion

        #region Uri Construction

#if !NET45

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
            var response = await client.GetAsync<string>(resource).ConfigureAwait(false);

            var requestUri = response?.RequestUri;
            if (requestUri == null) throw new InvalidOperationException("No uri");

            //Assert
            Assert.AreEqual(expectedUri, requestUri);
        }
#endif

        #endregion

        #endregion

        #region Helpers
#if !NET45
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
                await task.ConfigureAwait(false);
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

        private static IHeadersCollection GetHeaders(bool useDefault, Client client)
        {
            IHeadersCollection headers = NullHeadersCollection.Instance;
            if (useDefault)
            {
                client.DefaultRequestHeaders.Add("Test", "Test");
            }
            else
            {
                headers = new HeadersCollection { new KeyValuePair<string, IEnumerable<string>>("Test", new List<string> { "Test" }) };
            }

            return headers;
        }



        private static HttpClient MintClient()
        {
#pragma warning disable IDE0022 // Use expression body for methods
#if NETCOREAPP3_1
            return _testServer.CreateClient();
#else
            return new HttpClient { BaseAddress = new Uri(LocalBaseUriString) };
#endif
#pragma warning restore IDE0022 // Use expression body for methods
        }

        private static IClient GetJsonClient(Uri? baseUri = null)
        {
            IClient restClient;

            var defaultHeaders = HeadersExtensions.SetJsonContentTypeHeader();

            if (baseUri != null)
            {
                var httpClient = MintClient();
                httpClient.BaseAddress = baseUri;
                var testClientFactory = new TestClientFactory(httpClient);
                restClient = new Client(
                    new NewtonsoftSerializationAdapter(),
                    createHttpClient: testClientFactory.CreateClient,
                    defaultRequestHeaders: defaultHeaders);
            }
            else
            {
                restClient = new Client(
                    new NewtonsoftSerializationAdapter(),
                    createHttpClient: _testServerHttpClientFactory.CreateClient,
                    defaultRequestHeaders: defaultHeaders);
            }

            return restClient;
        }

#if !NET45
        private static bool CheckRequestHeaders(IHeadersCollection requestHeadersCollection) =>
            requestHeadersCollection.Contains("Test") && requestHeadersCollection["Test"].First() == "Test";

        private static bool CheckResponseHeaders(IHeadersCollection responseHeadersCollection) => responseHeadersCollection.Contains("Test1") && responseHeadersCollection["Test1"].First() == "a";
#endif
        #endregion
    }
}
