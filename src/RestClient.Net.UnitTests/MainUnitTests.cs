
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
using Urls;

#if NET45
using Microsoft.Extensions.Logging.Abstractions;
#else
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using ApiExamples;
using Moq.Protected;
#endif

#pragma warning disable CA1506 // Initialize reference type fields inline
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1810
#pragma warning disable IDE0061 
#pragma warning disable CA1825 // Avoid zero-length array allocations

namespace RestClient.Net.UnitTests
{
    [TestClass]
    public class MainUnitTests : IDisposable
    {
        #region Fields
        private readonly AbsoluteUrl testServerBaseUri = new(LocalBaseUriString);
        private readonly IHeadersCollection DefaultJsonContentHeaderCollection = HeadersExtensions.FromJsonContentType();
        private const string StandardContentTypeToString = "application/json; charset=utf-8";
        private const string GoogleUrlString = "https://www.google.com";
        private const string RestCountriesAllUriString = "https://restcountries.eu/rest/v2";
        private const string RestCountriesAustraliaUriString = "https://restcountries.eu/rest/v2/name/australia";
        private const string JsonPlaceholderBaseUriString = "https://jsonplaceholder.typicode.com";
        private const string JsonPlaceholderFirstPostSlug = "/posts/1";
        private const string JsonPlaceholderPostsSlug = "/posts";
        private readonly AbsoluteUrl RestCountriesAllUri = new(RestCountriesAllUriString);
        private readonly AbsoluteUrl RestCountriesAustraliaUri = new(RestCountriesAustraliaUriString);
        private readonly AbsoluteUrl JsonPlaceholderBaseUri = new(JsonPlaceholderBaseUriString);
        private readonly AbsoluteUrl GeoPluginUrl = new("http://www.geoplugin.net/xml.gp");
        private const string TransferEncodingHeaderName = "Transfer-Encoding";
        private const string SetCookieHeaderName = "Set-Cookie";
        private const string CacheControlHeaderName = "Cache-Control";
        private const string XRatelimitLimitHeaderName = "X-Ratelimit-Limit";
        private readonly UserPost _userRequestBody;
        private readonly string _userRequestBodyJson;

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

        //Mock the httpclient
        private readonly MockHttpMessageHandler _mockHttpMessageHandler = new();
        private readonly TestClientFactory _testServerHttpClientFactory;
        private Mock<ILogger<Client>> _logger = new();
        private readonly ILoggerFactory consoleLoggerFactory =
#if NET45
        NullLoggerFactory.Instance;

        private const string LocalBaseUriString = "https://localhost:44337";
#else
        LoggerFactory.Create(builder => _ = builder.AddConsole().SetMinimumLevel(LogLevel.Trace));
        private static readonly TestServer _testServer;

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
                With(request => request?.Content?.Headers?.ContentType == null).
                Respond(
                HttpStatusCode.Created,
                JsonPlaceholderPostHeaders,
                null,
                //This is the JSON that gets returned when the content type is empty
                "{\r\n" +
                "  \"id\": 101\r\n" +
                "}"
                );

#if !NET45
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
                With(request => request?.Content?.Headers?.ContentType?.MediaType == HeadersExtensions.JsonMediaType).
                Respond(
                HttpStatusCode.OK,
                JsonPlaceholderPostHeaders,
                HeadersExtensions.JsonMediaType,
                _userRequestBodyJson
                );

            _mockHttpMessageHandler.
            When(HttpMethod.Put, JsonPlaceholderBaseUriString + JsonPlaceholderFirstPostSlug).
            With(request => request?.Content?.Headers?.ContentType?.MediaType == HeadersExtensions.JsonMediaType).
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

            //Return all rest countries with a status code of 200
            _mockHttpMessageHandler.When(GeoPluginUrl.ToString())
                    .Respond(
                RestCountriesAllHeaders,
                HeadersExtensions.JsonMediaType,
                File.ReadAllText("JSON/GeoPlugin.xml"));

        }
        #endregion

        #region Public Methods
        public static TestClientFactory GetTestClientFactory()
        {
            var testClient = MintClient();
            return new TestClientFactory(testClient);
        }

        static MainUnitTests()
        {
#if !NET45

            var hostBuilder = new WebHostBuilder();
            hostBuilder.UseStartup<Startup>();
            _testServer = new TestServer(hostBuilder);
#endif
        }

        public MainUnitTests()
        {
            _userRequestBody = new() { title = "foo", userId = 10, body = "testbody" };

            _userRequestBodyJson = "{\r\n" +
                   $"  \"userId\": {_userRequestBody.userId},\r\n" +
                   "  \"id\": 0,\r\n" +
                   "  \"title\": \"foo\",\r\n" +
                   "  \"body\": \"testbody\"\r\n" +
                   "}";

            _testServerHttpClientFactory = GetTestClientFactory();
        }
        #endregion

        #region Tests

        #region External Api Tests
        [TestMethod]
        public async Task TestHead()
        {
            var baseUri = new AbsoluteUrl(GoogleUrlString);
            using var client = new Client(
                serializationAdapter: new NewtonsoftSerializationAdapter(),
                createHttpClient: GetCreateHttpClient()
                );

            Assert.AreEqual(true, client.ThrowExceptionOnFailure);

            var response = await client.SendAsync<string, object>(new Request<object>(
                baseUri,
                null,
                NullHeadersCollection.Instance,
                HttpRequestMethod.Custom,
                "HEAD",
                default));

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

            using var client = new Client(RestCountriesAllUri, createHttpClient: GetCreateHttpClient());

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


            using var client = new Client(RestCountriesAllUri, createHttpClient: createHttpClient, defaultRequestHeaders: testDefaultKvp.ToHeadersCollection());


            //Act
            _ = await client.PostAsync<List<RestCountry>, object>(new object(), null, testKvp.ToHeadersCollection());

            //Make sure we can call it twice
            _ = await client.PostAsync<List<RestCountry>, object>(new object(), null, testKvp.ToHeadersCollection());

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
            using var client = new Client(RestCountriesAllUri, createHttpClient: GetCreateHttpClient());
            List<RestCountry>? countries = await client.GetAsync<List<RestCountry>>();
            Assert.IsNotNull(countries);
            Assert.IsTrue(countries.Count > 0);
        }

        [TestMethod]
        public async Task TestGetDefaultSerializationRestCountriesAsJson()
        {
            using var client = new Client(
                RestCountriesAustraliaUri,
                createHttpClient: GetCreateHttpClient());
            var json = await client.GetAsync<string>();

            Assert.IsNotNull(json);

#pragma warning disable CS8604 // Possible null reference argument.
            var country = JsonConvert.DeserializeObject<List<RestCountry>>(json)?.First();
#pragma warning restore CS8604 // Possible null reference argument.
            Assert.AreEqual("Australia", country?.name);
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

            _ = await Assert.ThrowsExceptionAsync<HttpStatusException>(() => client.GetAsync<List<RestCountry>>(), Messages.GetErrorMessageNonSuccess((int)statusCode, RestCountriesAllUri));
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

            Assert.AreEqual(expectedError.Message, error?.Message);
        }

        [TestMethod]
        public async Task TestGetRestCountries()
        {
            using var client = new Client(new NewtonsoftSerializationAdapter(),
                baseUri: RestCountriesAllUri,
                createHttpClient: GetCreateHttpClient(),
                logger: _logger.Object);

            var response = await client.GetAsync<List<RestCountry>>();
            Assert.IsNotNull(response);
            Assert.IsTrue(response?.Body?.Count > 0);

#if !NET45
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
                createHttpClient: GetCreateHttpClient()
                );
            var response = await client.DeleteAsync(JsonPlaceholderFirstPostSlug);

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
                RestCountriesAustraliaUri,
                createHttpClient: GetCreateHttpClient(),
                name: null);

            var json = await client.GetAsync<string>();

            Assert.IsNotNull(json);

#pragma warning disable CS8604 // Possible null reference argument.
            var country = JsonConvert.DeserializeObject<List<RestCountry>>(json)?.First();
#pragma warning restore CS8604 // Possible null reference argument.
            Assert.AreEqual("Australia", country?.name);
        }

        [TestMethod]
        public async Task TestGetRestCountriesNoBaseUri()
        {
            using var client = new Client(new NewtonsoftSerializationAdapter(), createHttpClient: GetCreateHttpClient());
            List<RestCountry>? countries = await client.GetAsync<List<RestCountry>>(RestCountriesAustraliaUri);
            var country = countries?.First();
            Assert.IsNotNull(country);
            Assert.AreEqual("Australia", country?.name);
        }

        [TestMethod]
        public Task TestPostUserWithCancellation()
        => Assert.ThrowsExceptionAsync<TaskCanceledException>(async () =>
           {

               using var client = new Client(new NewtonsoftSerializationAdapter(), baseUri: JsonPlaceholderBaseUri);

               using var tokenSource = new CancellationTokenSource();
               var token = tokenSource.Token;

               var task = client.PostAsync<UserPost, UserPost>(new UserPost { title = "Moops" }, new RelativeUrl("/posts"), cancellationToken: token);

               tokenSource.Cancel();

               _ = await task;
           });


        [TestMethod]
#if !NET45
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
                createHttpClient: GetCreateHttpClient(),
                logger: _logger.Object,
                defaultRequestHeaders: HeadersExtensions.FromJsonContentType());
            var responseUserPost = httpRequestMethod switch
            {
                HttpRequestMethod.Patch => await client.PatchAsync<UserPost, UserPost>(_userRequestBody, new RelativeUrl("/posts/1")),
                //TODO: Shouldn't expectedStatusCode = HttpStatusCode.Created
                HttpRequestMethod.Post => await client.PostAsync<UserPost, UserPost>(_userRequestBody, "/posts"),
                HttpRequestMethod.Put => await client.PutAsync<UserPost, UserPost>(_userRequestBody, new RelativeUrl("/posts/1")),
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
                createHttpClient: GetCreateHttpClient(),
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
            using var client = new Client(
                new XmlSerializationAdapter(),
                createHttpClient: GetCreateHttpClient(),
                baseUri: GeoPluginUrl);
            var geoPlugin = await client.GetAsync<GeoPlugin>();

            Assert.IsNotNull(geoPlugin);
            Assert.AreEqual("-37.7858", geoPlugin?.Body?.Geoplugin_latitude);
            Assert.AreEqual("Oceania", geoPlugin?.Body?.Geoplugin_continentName);
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

            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                new AbsoluteUrl(LocalBaseUriString),
                createHttpClient: _testServerHttpClientFactory.CreateClient);
            var responsePerson = await client.PostAsync<Person, Person>(requestPerson, new RelativeUrl("person"));
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
                new(LocalBaseUriString),
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: "PersonKey".ToHeadersCollection(personKey));

            Person? responsePerson = await client.PutAsync<Person, Person>(requestPerson, new RelativeUrl("person"));
            Assert.AreEqual(requestPerson.BillingAddress.Street, responsePerson?.BillingAddress?.Street);
            Assert.AreEqual(personKey, responsePerson?.PersonKey);
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

            Person? responsePerson = await client.GetAsync<Person>(new RelativeUrl(
                "headers"),
                "Test".ToHeadersCollection("Test")
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
                defaultRequestHeaders: "Test".ToHeadersCollection("Test"));

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
                defaultRequestHeaders: "Test".ToHeadersCollection("Test"));

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
            var headersCollections = HeadersExtensions.FromJsonContentType();

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

            _ = await client.GetAsync<Person>(new RelativeUrl(
                "headers"),
                requestHeaders: "Test"
                .ToHeadersCollection("Test"));

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
            var headersCollections = HeadersExtensions.FromJsonContentType();

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
                new RelativeUrl("headers"),
                requestHeaders: "Test".ToHeadersCollection("Test")
                );

            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        [DataRow(HttpRequestMethod.Get)]
        [DataRow(HttpRequestMethod.Post)]
        [DataRow(HttpRequestMethod.Put)]
        [DataRow(HttpRequestMethod.Patch)]
        [DataRow(HttpRequestMethod.Delete)]
        public async Task TestHeadersIncorrectLocal(HttpRequestMethod httpRequestMethod)
        {
            var serializationAdapter = new NewtonsoftSerializationAdapter();

            var hex = await Assert.ThrowsExceptionAsync<HttpStatusException>(async () =>
            {
                using var client = new Client(
                serializationAdapter,
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                //The server expects the value of "Test"
                defaultRequestHeaders: HeadersExtensions.FromJsonContentType().Append("Test", "Tests"));

#pragma warning disable IDE0072 // Add missing cases
                var response = httpRequestMethod switch
#pragma warning restore IDE0072 // Add missing cases
                {
                    HttpRequestMethod.Get => await client.GetAsync<Person>(new RelativeUrl("headers")),
                    HttpRequestMethod.Post => await client.PostAsync<Person, Person>(new Person(), new RelativeUrl("headers")),
                    HttpRequestMethod.Put => await client.PutAsync<Person, Person>(new Person(), new RelativeUrl("headers")),
                    HttpRequestMethod.Patch => await client.PatchAsync<Person, Person>(new Person(), new RelativeUrl("headers")),
                    HttpRequestMethod.Delete => await client.DeleteAsync(new RelativeUrl("headers/1")),
                    _ => throw new NotImplementedException()
                };
            });

            Assert.AreEqual((int)HttpStatusCode.BadRequest, hex.Response.StatusCode);
            var apiResult = serializationAdapter.Deserialize<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
            Assert.AreEqual(ApiMessages.HeadersControllerExceptionMessage, apiResult?.Errors[0]);
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
                new RelativeUrl("headers"),
                requestHeaders: "Test".ToHeadersCollection("Test")
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
                defaultRequestHeaders: HeadersExtensions.FromJsonContentType().Append("Test", "Test"));

            var responsePerson = await client.PutAsync<Person, Person>(new Person { FirstName = "Bob" }, "headers");
            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public void TestHeadersToString()
        {
            var headers = "a".ToHeadersCollection("1").Append("b", "2");
            var actualResult = headers.ToString();
            Assert.AreEqual("a: 1\r\nb: 2", actualResult);
        }



        [TestMethod]
        public void TestHeaders()
        {
            var count = 0;
            var enumerable = (IEnumerable)"asd".ToHeadersCollection("321");
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
            var headers = "asd".ToHeadersCollection("123");
            const string expectedValue = "321";
            var headers2 = "asd".ToHeadersCollection(expectedValue);
            var headers3 = headers.Append(headers2);
            Assert.AreEqual(expectedValue, headers3.First().Value.First());
        }

        [TestMethod]
        public void TestAppendHeaders2()
        {
            var headers = "asd".ToHeadersCollection("123");
            var headers2 = headers.Append(null);
            Assert.AreEqual(1, headers2.Count());
        }

        [TestMethod]
        public async Task TestGetWitStringResource()
        {

            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                //The server expects the value of "Test"
                defaultRequestHeaders: HeadersExtensions.FromJsonContentType());

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
                new RelativeUrl("headers"),
                requestHeaders: "Test".ToHeadersCollection("Test")
                );

            Assert.IsNotNull(responsePerson);
        }

        [TestMethod]
        public async Task TestTimeoutPatch()
        {
            //Note this works by specifying a really quick timespan. The assumption is that the server won't respond quickly enough.

            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                logger: _logger.Object,
                createHttpClient: _testServerHttpClientFactory.CreateClient);

            var exception = await Assert.ThrowsExceptionAsync<SendException>(() => client.PatchAsync<Person, Person>(
                new Person { FirstName = "Bob" },
                TimeSpan.FromMilliseconds(.01),
                new("headers"),
                requestHeaders: "Test".ToHeadersCollection("Test")
                ));

#if !NET45
            _logger.VerifyLog<Client, OperationCanceledException>((state, t)
                => state.CheckValue("{OriginalFormat}", Messages.ErrorMessageOperationCancelled), LogLevel.Error, 1);
#endif
        }

        [TestMethod]
        public async Task TestTimeoutPatch2()
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: DefaultJsonContentHeaderCollection.WithHeaderValue("Test", "Test"));

            var responsePerson = await client.PatchAsync<Person, Person>(
                new Person { FirstName = "Bob" },
                TimeSpan.FromMilliseconds(10000),
                new("headers"),
                requestHeaders: "Test".ToHeadersCollection("Test")
                );
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

            _ = await client.DeleteAsync(new("headers/1"), "Test".ToHeadersCollection("Test"));
        }
        #endregion

        #region Local Headers In Request
        [TestMethod]
        public async Task TestHeadersLocalInRequest()
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                createHttpClient: _testServerHttpClientFactory.CreateClient);
            var requestHeadersCollection = "Test".ToHeadersCollection("Test");
            Person? responsePerson = await client.SendAsync<Person, object>(
                new Request<object>(testServerBaseUri.WithRelativeUrl(new("headers")), null, requestHeadersCollection, HttpRequestMethod.Get, cancellationToken: default)
                );
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
            Assert.AreEqual(ApiMessages.ErrorControllerErrorMessage, apiResult?.Errors.First());

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


            var hex = await Assert.ThrowsExceptionAsync<HttpStatusException>(async ()
                => await restClient.GetAsync<Person>("error"));

            var apiResult = serializationAdapter.Deserialize<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
            Assert.AreEqual(ApiMessages.ErrorControllerErrorMessage, apiResult?.Errors.First());
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
                defaultRequestHeaders: HeadersExtensions.FromJsonContentType());

            var response = await client.PostAsync<AuthenticationResult, AuthenticationRequest>(
                new AuthenticationRequest { ClientId = "a", ClientSecret = "b" },
                new RelativeUrl("secure/authenticate")
                );

            var bearerToken = response.Body?.BearerToken;

            if (bearerToken == null) throw new InvalidOperationException("No bearer token");

            using var client2 = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions
                .FromJsonContentType()
                .WithBearerTokenAuthentication(bearerToken));

            Person? person = await client2.GetAsync<Person>(new RelativeUrl("secure/bearer"));
            Assert.AreEqual("Bear", person?.FirstName);
        }

        [TestMethod]
        public async Task TestBasicAuthenticationLocalWithError()
        {
            var serializationAdapter = new NewtonsoftSerializationAdapter();

            using var restClient = new Client(
                serializationAdapter,
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions.FromBasicCredentials("Bob", "WrongPassword"));

            var hex = await Assert.ThrowsExceptionAsync<HttpStatusException>(() => restClient.GetAsync<Person>(new RelativeUrl("secure/basic")));

            Assert.AreEqual((int)HttpStatusCode.Unauthorized, hex.Response.StatusCode);
            var apiResult = serializationAdapter.Deserialize<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
            Assert.AreEqual(ApiMessages.SecureControllerNotAuthorizedMessage, apiResult?.Errors.First());

        }

        [TestMethod]
        public async Task TestBasicAuthenticationLocal()
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions.FromBasicCredentials("Bob", "ANicePassword"));

            Person? person = await client.GetAsync<Person>(new RelativeUrl("secure/basic"));
            Assert.AreEqual("Sam", person?.FirstName);
        }

        [TestMethod]
        public async Task TestBearerTokenAuthenticationLocalWithError()
        {
            var serializationAdapter = new NewtonsoftSerializationAdapter();

            using var restClient = new Client(
                serializationAdapter,
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions.FromBearerToken("321"));


            var hex = await Assert.ThrowsExceptionAsync<HttpStatusException>(() => restClient.GetAsync<Person>(new RelativeUrl("secure/bearer")));

            Assert.AreEqual((int)HttpStatusCode.Unauthorized, hex.Response.StatusCode);
            var apiResult = serializationAdapter.Deserialize<ApiResult>(hex.Response.GetResponseData(), hex.Response.Headers);
            Assert.AreEqual(ApiMessages.SecureControllerNotAuthorizedMessage, apiResult?.Errors.First());
        }

        [TestMethod]
        public async Task TestBasicAuthenticationPostLocal()
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: testServerBaseUri,
                createHttpClient: _testServerHttpClientFactory.CreateClient,
                defaultRequestHeaders: HeadersExtensions
                .FromJsonContentType()
                .WithBasicAuthentication("Bob", "ANicePassword"));

            Person? person = await client.PostAsync<Person, Person>(new Person { FirstName = "Sam" }, new RelativeUrl("secure/basic"));
            Assert.AreEqual("Sam", person?.FirstName);
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
                .FromJsonContentType()
                .WithBasicAuthentication("Bob", "WrongPassword"));

            var ex = await Assert.ThrowsExceptionAsync<HttpStatusException>(() => restClient.PostAsync<Person, Person>(new Person { FirstName = "Sam" }, new RelativeUrl("secure/basic")));

            Assert.AreEqual((int)HttpStatusCode.Unauthorized, ex.Response.StatusCode);
            var apiResult = serializationAdapter.Deserialize<ApiResult>(ex.Response.GetResponseData(), ex.Response.Headers);
            Assert.AreEqual(ApiMessages.SecureControllerNotAuthorizedMessage, apiResult?.Errors.First());
        }
        #endregion

        #region All Extension Overloads

        #region Get
        [TestMethod]
        public async Task TestLocalGetNoArgs()
        {
            var client = GetJsonClient(new($"{LocalBaseUriString}/JsonPerson"));
            jsonperson? responsePerson = await client.GetAsync<jsonperson>();
            Assert.IsNotNull(responsePerson);
            Assert.IsNotNull("Sam", responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalGetStringUri()
        {
            var client = GetJsonClient();
            jsonperson? responsePerson = await client.GetAsync<jsonperson>("JsonPerson");
            Assert.IsNotNull(responsePerson);
            Assert.IsNotNull("Sam", responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalGetUri()
        {
            var client = GetJsonClient();
            jsonperson? responsePerson = await client.GetAsync<jsonperson>(new RelativeUrl("JsonPerson"));
            Assert.IsNotNull(responsePerson);
            Assert.IsNotNull("Sam", responsePerson.FirstName);
        }

        [TestMethod]
        public async Task TestLocalGetUriCancellationToken()
        {
            var client = GetJsonClient();
            jsonperson? responsePerson = await client.GetAsync<jsonperson>(new RelativeUrl("JsonPerson"), cancellationToken: new CancellationToken());
            Assert.IsNotNull(responsePerson);
            Assert.IsNotNull("Sam", responsePerson.FirstName);
        }
        #endregion

        #region Delete
        [TestMethod]
        public async Task TestLocalDeleteStringUri()
        {
            var client = GetJsonClient(new($"{LocalBaseUriString}/JsonPerson"));
            var response = await client.DeleteAsync(client.BaseUrl.RelativeUrl.AddQueryString("personKey", "abc"));
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public async Task TestLocalDeleteUri()
        {
            var baseUri = new AbsoluteUrl($"{LocalBaseUriString}/JsonPerson");
            var client = GetJsonClient(baseUri);
            var response = await client.DeleteAsync(baseUri.RelativeUrl.AddQueryString("personKey", "abc"));
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public async Task TestLocalDeleteUriCancellationToken()
        {
            var client = GetJsonClient(new($"{LocalBaseUriString}/JsonPerson"));
            var response = await client.DeleteAsync(client.BaseUrl.RelativeUrl.AddQueryString("personKey", "abc"), cancellationToken: new CancellationToken());
            Assert.AreEqual(200, response.StatusCode);

            //TODO: Verify the log
        }
        #endregion

        #region Post
        [TestMethod]
        public async Task TestLocalPostBody()
        {
            var client = GetJsonClient(new($"{LocalBaseUriString}/JsonPerson/save"));
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson? responsePerson = await client.PostAsync<jsonperson, jsonperson>(requestPerson);
            Assert.AreEqual(requestPerson.FirstName, responsePerson?.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPostBody2()
        {
            var client = GetJsonClient(new($"{LocalBaseUriString}/jsonperson/save2"));
            jsonperson? responsePerson = await client.PostAsync<jsonperson>();
            Assert.AreEqual("J", responsePerson?.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPostBodyStringUri()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson? responsePerson = await client.PostAsync<jsonperson, jsonperson>(requestPerson, "jsonperson/save");
            Assert.AreEqual(requestPerson.FirstName, responsePerson?.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPostBodyUri()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson? responsePerson = await client.PostAsync<jsonperson, jsonperson>(requestPerson, new RelativeUrl("jsonperson/save"));
            Assert.AreEqual(requestPerson.FirstName, responsePerson?.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPostBodyUriCancellationToken()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson? responsePerson = await client.PostAsync<jsonperson, jsonperson>(requestPerson, new("jsonperson/save"), null, new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson?.FirstName);
        }
        #endregion

        #region Put
        [TestMethod]
        public async Task TestLocalPutBody()
        {
            var client = GetJsonClient(new($"{LocalBaseUriString}/jsonperson/save"));
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson? responsePerson = await client.PutAsync<jsonperson, jsonperson>(requestBody: requestPerson);
            Assert.AreEqual(requestPerson.FirstName, responsePerson?.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPutBody2()
        {
            var client = GetJsonClient(new($"{LocalBaseUriString}/jsonperson/save2"));
            jsonperson? responsePerson = await client.PutAsync<jsonperson>();
            Assert.AreEqual("J", responsePerson?.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPutBodyStringUri()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson? responsePerson = await client.PutAsync<jsonperson, jsonperson>(requestPerson, "jsonperson/save");
            Assert.AreEqual(requestPerson.FirstName, responsePerson?.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPutBodyUri()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson? responsePerson = await client.PutAsync<jsonperson, jsonperson>(requestPerson, new RelativeUrl("jsonperson/save"));
            Assert.AreEqual(requestPerson.FirstName, responsePerson?.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPutBodyUriCancellationToken()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson? responsePerson = await client.PutAsync<jsonperson, jsonperson>(requestPerson, new RelativeUrl("jsonperson/save"), cancellationToken: new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson?.FirstName);
        }
        #endregion

        #region Patch
        [TestMethod]
        public async Task TestLocalPatchBody()
        {
            var client = GetJsonClient(new($"{LocalBaseUriString}/jsonperson/save"));
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson? responsePerson = await client.PatchAsync<jsonperson, jsonperson>(requestPerson);
            Assert.AreEqual(requestPerson.FirstName, responsePerson?.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBody2()
        {
            var client = GetJsonClient(new($"{LocalBaseUriString}/jsonperson/save2"));
            jsonperson? responsePerson = await client.PatchAsync<jsonperson>();
            Assert.AreEqual("J", responsePerson?.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyStringUri()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson? responsePerson = await client.PatchAsync<jsonperson, jsonperson>(requestPerson, "jsonperson/save");
            Assert.AreEqual(requestPerson.FirstName, responsePerson?.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyUri()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson? responsePerson = await client.PatchAsync<jsonperson, jsonperson>(requestPerson, new RelativeUrl("jsonperson/save"));
            Assert.AreEqual(requestPerson.FirstName, responsePerson?.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyUriCancellationToken()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson? responsePerson = await client.PatchAsync<jsonperson, jsonperson>(requestPerson, new RelativeUrl("jsonperson/save"), cancellationToken: new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson?.FirstName);
        }

        [TestMethod]
        public async Task TestLocalPatchBodyUriCancellationTokenContentType()
        {
            var client = GetJsonClient();
            var requestPerson = new jsonperson { FirstName = "Bob" };
            jsonperson? responsePerson = await client.PatchAsync<jsonperson, jsonperson>(requestPerson, new("jsonperson/save"), cancellationToken: new CancellationToken());
            Assert.AreEqual(requestPerson.FirstName, responsePerson?.FirstName);
        }
        #endregion

        #endregion

        #region Misc
        [TestMethod]
        public void TestJsonHeaders()
        {
            var keyValuePair = HeadersExtensions.JsonContentTypeHeaders.First();
            Assert.AreEqual(HeadersExtensions.ContentTypeHeaderName, keyValuePair.Key);
            Assert.AreEqual(HeadersExtensions.JsonMediaType, keyValuePair.Value.First());
        }

        [TestMethod]
        public void TestDefaultGetHttpRequestMessageCustomNoThing()
        {
            var defaultGetHttpRequestMessage = new DefaultGetHttpRequestMessage();
            var request = new Request<string>(
                new("http://www.test.com"),
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
                new("http://www.test.com"),
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
                defaultRequestHeaders: HeadersExtensions.FromJsonContentType().Append("Test", "Test"));

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
                new("http://www.test.com"),
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
                baseUri: new(RestCountriesAllUriString),
                createHttpClient: (n) => new HttpClient(mockHttpMessageHandler));

            var dex = await Assert.ThrowsExceptionAsync<DeserializationException>(() => client.GetAsync<Person>());
#if !NET45
            _logger.VerifyLog<Client, DeserializationException>((state, t)
                => state.CheckValue("{OriginalFormat}", Messages.ErrorMessageDeserialization), LogLevel.Error, 1);
#endif
            Assert.IsTrue(dex.GetResponseData().SequenceEqual(Encoding.ASCII.GetBytes(Content)));
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
                new("http://www.test.com"));

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
            var sex = await Assert.ThrowsExceptionAsync<SendException>(() => client.PostAsync<Person, Person>(requestPerson));

#if !NET45
            _logger.VerifyLog<Client, SendException>((state, t)
                => state.CheckValue("{OriginalFormat}", Messages.ErrorSendException), LogLevel.Error, 1);
#endif

            Assert.AreEqual(testServerBaseUri, sex.Request.Uri);
        }

        [TestMethod]
        public async Task TestInvalidUriInformation()
        {
            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                logger: _logger.Object);

            _ = await Assert.ThrowsExceptionAsync<SendException>(()
                => client.PostAsync<Person, Person>(new Person()));
        }

        [TestMethod]
        public async Task TestFactoryCreationWithUri()
        {
            var clientFactory = new ClientFactory(GetCreateHttpClient());
            var client = clientFactory.CreateClient("test", (o) =>
            {
                o.BaseUrl = RestCountriesAllUri;
                o.SerializationAdapter = new NewtonsoftSerializationAdapter();
            });
            var response = await client.GetAsync<List<RestCountry>>();
            Assert.IsTrue(response.Body?.Count > 0);
        }

#if !NET45

        [TestMethod]
        public async Task TestFactoryDoesntUseSameHttpClient()
        {
            var clientFactory = new ClientFactory(
                GetCreateHttpClient());

            var client = (Client)clientFactory.CreateClient("1", (o) => o.BaseUrl = RestCountriesAllUri);
            var response = await client.GetAsync<List<RestCountry>>();

            var client2 = (Client)clientFactory.CreateClient("2", (o) => o.BaseUrl = RestCountriesAllUri);
            response = await client2.GetAsync<List<RestCountry>>();

            Assert.IsNotNull(client.lazyHttpClient.Value);
            var isEqual = ReferenceEquals(client.lazyHttpClient.Value, client2.lazyHttpClient.Value);
            Assert.IsFalse(isEqual);
        }

        [TestMethod]
        public async Task TestHttpClientFactoryDoesntUseSameHttpClient()
        {
            using var defaultHttpClientFactory = new DefaultHttpClientFactory(GetLazyCreate());

            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: RestCountriesAllUri,
                createHttpClient: defaultHttpClientFactory.CreateClient); ;

            var response = await client.GetAsync<List<RestCountry>>();

            using var client2 = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: RestCountriesAllUri,
                createHttpClient: defaultHttpClientFactory.CreateClient);

            response = await client2.GetAsync<List<RestCountry>>();

            Assert.IsNotNull(client.lazyHttpClient.Value);
            Assert.IsFalse(ReferenceEquals(client.lazyHttpClient.Value, client2.lazyHttpClient.Value));
        }

        [TestMethod]
        public void TestHttpClientFactoryDoesntUseSameHttpClientByName()
        {
            using var defaultHttpClientFactory = new DefaultHttpClientFactory();
            var client1 = defaultHttpClientFactory.CreateClient("Test1");
            var client2 = defaultHttpClientFactory.CreateClient("Test2");
            Assert.IsNotNull(client1);
            Assert.IsFalse(ReferenceEquals(client1, client2));
        }

        /// <summary>
        /// This test is controversial. Should non-named clients always be Singleton? This is the way the factory is designed, but could trip some users up.
        /// </summary>
        [TestMethod]
        public void TestClientFactoryReusesClient()
        {
            using var defaultHttpClientFactory = new DefaultHttpClientFactory(GetLazyCreate());

            var clientFactory = new ClientFactory(defaultHttpClientFactory.CreateClient);

            var firstClient = clientFactory.CreateClient("RestClient", (o) => o.BaseUrl = RestCountriesAllUri);

            var secondClient = clientFactory.CreateClient("RestClient", (o) => o.BaseUrl = RestCountriesAllUri);

            Assert.IsNotNull(firstClient);
            Assert.IsTrue(ReferenceEquals(firstClient, secondClient));
        }

        [TestMethod]
        public void TestClientFactoryValues()
        {
            using var defaultHttpClientFactory = new DefaultHttpClientFactory(GetLazyCreate());

            var clientFactory = new ClientFactory(defaultHttpClientFactory.CreateClient);

            static HttpClient createHttpClient(string n) => new();
            var defaultGetHttpRequestMessage = new DefaultGetHttpRequestMessage();
            var newtonsoftSerializationAdapter = new NewtonsoftSerializationAdapter();

            const string Name = "123";

            var client = (Client)clientFactory.CreateClient(Name, (o) =>
            {
                o.BaseUrl = RestCountriesAllUri;
                o.CreateHttpClient = createHttpClient;
                o.GetHttpRequestMessage = DefaultGetHttpRequestMessage.Instance;
                o.DefaultRequestHeaders = HeadersCollection.Empty;
                o.SendHttpRequestMessage = DefaultSendHttpRequestMessage.Instance;
                o.SerializationAdapter = newtonsoftSerializationAdapter;
                o.ThrowExceptionOnFailure = true;
            });

            Assert.AreEqual(RestCountriesAllUri, client.BaseUrl);
            Assert.AreEqual(createHttpClient, client.createHttpClient);
            Assert.AreEqual(DefaultGetHttpRequestMessage.Instance, client.getHttpRequestMessage);
            Assert.AreEqual(HeadersCollection.Empty, client.DefaultRequestHeaders);
            Assert.AreEqual(DefaultSendHttpRequestMessage.Instance, client.sendHttpRequestMessage);
            Assert.AreEqual(true, client.ThrowExceptionOnFailure);
            Assert.AreEqual(Name, client.Name);
        }

        [TestMethod]
        public async Task TestHttpClientFactoryReusesHttpClientWhenSameName()
        {
            using var defaultHttpClientFactory = new DefaultHttpClientFactory(GetLazyCreate());

            using var client = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: RestCountriesAllUri,
                createHttpClient: defaultHttpClientFactory.CreateClient,
                name: "Test");
            var response = await client.GetAsync<List<RestCountry>>();

            using var client2 = new Client(
                new NewtonsoftSerializationAdapter(),
                baseUri: RestCountriesAllUri,
                createHttpClient: defaultHttpClientFactory.CreateClient,
                name: "Test");
            response = await client2.GetAsync<List<RestCountry>>();

            Assert.IsNotNull(client.lazyHttpClient.Value);
            Assert.IsTrue(ReferenceEquals(client.lazyHttpClient.Value, client2.lazyHttpClient.Value));
        }

#else
        [TestMethod]
        public void TestFactoryCreationWithoutAdapterThrowsException()
        {
            _ = Assert.ThrowsException<InvalidOperationException>(() =>
            {
                var clientFactory = new ClientFactory(GetCreateHttpClient());
                var client = clientFactory.CreateClient("test");
            });
        }

#endif

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

            using var client = new Client(new AbsoluteUrl("http://www.test.com/test"), createHttpClient: (n) => httpClient);

            //Act
            var response = await client.GetAsync<string>(client.BaseUrl.AppendPath("test"));

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

            Assert.IsTrue(ReferenceEquals(clientBase.BaseUrl, clientClone.BaseUrl));

            Assert.IsTrue(ReferenceEquals(clientBase.SerializationAdapter, clientClone.SerializationAdapter));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<IGetHttpRequestMessage>(clientBase, "getHttpRequestMessage"),
                GetFieldValue<IGetHttpRequestMessage>(clientClone, "getHttpRequestMessage")));

            Assert.AreEqual(clientBase.Name, clientClone.Name);

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

            var baseUri = new AbsoluteUrl("http://www.one.com");

            var clientClone = clientBase.With(baseUri);

            Assert.AreEqual(baseUri, clientClone.BaseUrl);

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
        public void TestWithSerializationAdapter()
        {
            using var clientBase = GetBaseClient();

            var serializationAdapter = new NewtonsoftSerializationAdapter();

            var clientClone = clientBase.With(serializationAdapter);

            Assert.IsTrue(ReferenceEquals(serializationAdapter, clientClone.SerializationAdapter));

            Assert.AreEqual(clientBase.ThrowExceptionOnFailure, clientClone.ThrowExceptionOnFailure);

            Assert.IsTrue(ReferenceEquals(clientBase.BaseUrl, clientClone.BaseUrl));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<IGetHttpRequestMessage>(clientBase, "getHttpRequestMessage"),
                GetFieldValue<IGetHttpRequestMessage>(clientClone, "getHttpRequestMessage")));

            Assert.AreEqual(clientBase.Name, clientClone.Name);

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

            Assert.IsTrue(ReferenceEquals(clientBase.BaseUrl, clientClone.BaseUrl));

            Assert.AreEqual(clientBase.Name, clientClone.Name);

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

            Assert.IsTrue(ReferenceEquals(clientBase.BaseUrl, clientClone.BaseUrl));

            Assert.IsTrue(ReferenceEquals(clientBase.SerializationAdapter, clientClone.SerializationAdapter));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<IGetHttpRequestMessage>(clientBase, "getHttpRequestMessage"),
                GetFieldValue<IGetHttpRequestMessage>(clientClone, "getHttpRequestMessage")));

            Assert.AreEqual(clientBase.Name, clientClone.Name);
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

            Assert.IsTrue(ReferenceEquals(clientBase.BaseUrl, clientClone.BaseUrl));

            Assert.IsTrue(ReferenceEquals(clientBase.SerializationAdapter, clientClone.SerializationAdapter));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<IGetHttpRequestMessage>(clientBase, "getHttpRequestMessage"),
                GetFieldValue<IGetHttpRequestMessage>(clientClone, "getHttpRequestMessage")));

            Assert.AreEqual(clientBase.Name, clientClone.Name);
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

            Assert.IsTrue(ReferenceEquals(clientBase.BaseUrl, clientClone.BaseUrl));

            Assert.IsTrue(ReferenceEquals(clientBase.SerializationAdapter, clientClone.SerializationAdapter));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<IGetHttpRequestMessage>(clientBase, "getHttpRequestMessage"),
                GetFieldValue<IGetHttpRequestMessage>(clientClone, "getHttpRequestMessage")));

            Assert.AreEqual(clientBase.Name, clientClone.Name);
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


            Assert.IsTrue(ReferenceEquals(clientBase.BaseUrl, clientClone.BaseUrl));

            Assert.IsTrue(ReferenceEquals(clientBase.SerializationAdapter, clientClone.SerializationAdapter));

            Assert.IsTrue(ReferenceEquals(
                GetFieldValue<IGetHttpRequestMessage>(clientBase, "getHttpRequestMessage"),
                GetFieldValue<IGetHttpRequestMessage>(clientClone, "getHttpRequestMessage")));

            Assert.AreEqual(clientBase.Name, clientClone.Name);
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
            Assert.AreEqual(clientBase.ThrowExceptionOnFailure, clientClone.ThrowExceptionOnFailure);
            Assert.AreEqual(clientBase.BaseUrl, clientClone.BaseUrl);

            //Note the header reference is getting copied across. This might actually be problematic if the collection is not immutable
            Assert.IsTrue(ReferenceEquals(clientBase.DefaultRequestHeaders, clientClone.DefaultRequestHeaders));
        }

        private static Client GetBaseClient()
        {
            var serializationAdapterMock = new Mock<ISerializationAdapter>();
            const string name = "test";
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
                            new("http://www.test.com"),
                            headersCollectionMock.Object,
                            loggerMock.Object,
                            createHttpClient,
                            sendHttpRequestMessageMock.Object,
                            getHttpRequestMessageMock.Object,
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

        #region Response
        [TestMethod]
        [DataRow(null)]
        [DataRow("test")]
        public void TestResponseCanReturnValue(string Body)
        {
            var emptyResponse = new Response<string>(HeadersCollection.Empty, 1, HttpRequestMethod.Custom, new byte[0], Body, AbsoluteUrl.Empty);
            string? responseString = emptyResponse;
            Assert.AreEqual(Body, responseString);
        }

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        [TestMethod]
        public void TestResponseCanBeNull()
        {
            Response<string> emptyResponse = null;
            string? responseString = emptyResponse;
            Assert.AreEqual(null, responseString);
        }
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        #endregion

        #region Request
#if !NET45
        [TestMethod]
        public void TestRequestToStringHasSomething()
        {
            var url = "http://www.test.com".ToAbsoluteUrl();
            var request = new Request<string>(url, null, HeadersCollection.Empty, HttpRequestMethod.Post, null, default);
            Assert.IsTrue(request.ToString().Contains(url.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestJsonSerializationAdapterDefaultsAreCorrect()
        {
            var jsonSerializationAdapter = new JsonSerializationAdapter();
            Assert.AreEqual(true, jsonSerializationAdapter.JsonSerializationOptions.PropertyNameCaseInsensitive);
        }
#endif
        #endregion

        #region Headers Collection

#if !NET45
#pragma warning disable CA1502
        [TestMethod]
        [DataRow(HeadersExtensions.ContentTypeHeaderName, HeadersExtensions.JsonMediaType, true)]
        [DataRow(HeadersExtensions.ContentEncodingHeaderName, HeadersExtensions.ContentEncodingGzip, true)]
        [DataRow(HeadersExtensions.ContentLanguageHeaderName, "de-DE", true)]
        [DataRow(HeadersExtensions.ContentLengthHeaderName, "256", true)]
        [DataRow(HeadersExtensions.ContentLocationHeaderName, "Sandwiches", true)]
        [DataRow(HeadersExtensions.ContentMD5HeaderName, "Q2h1Y2sgSW51ZwDIAXR5IQ==", true)]
        [DataRow(HeadersExtensions.ContentRangeHeaderName, "bytes 200-1000/67589", true)]
        [DataRow("Content-Stuff", "123", false)]
        public void TestGetHttpRequestMessage(string headerName, string headerValue, bool isContentHeader)
        {
            var loggerMock = new Mock<ILogger>();

            var request = new Request<string>(new("http://www.test.com"), "a",
                headerName.ToHeadersCollection(headerValue),
                HttpRequestMethod.Get);

            var defaultGetHttpRequestMessage = new DefaultGetHttpRequestMessage();

            var httpRequestMessage = defaultGetHttpRequestMessage.GetHttpRequestMessage(
                request,
                loggerMock.Object,
                new JsonSerializationAdapter());

            if (isContentHeader)
            {
                switch (headerName)
                {
                    case HeadersExtensions.ContentTypeHeaderName:
                        Assert.AreEqual(headerValue, httpRequestMessage?.Content?.Headers?.ContentType?.MediaType);
                        break;

                    case HeadersExtensions.ContentEncodingHeaderName:
                        Assert.AreEqual(headerValue, httpRequestMessage?.Content?.Headers?.ContentEncoding?.First());
                        break;

                    case HeadersExtensions.ContentLanguageHeaderName:
                        Assert.AreEqual(headerValue, httpRequestMessage?.Content?.Headers?.ContentLanguage?.First());
                        break;

                    case HeadersExtensions.ContentLengthHeaderName:
#pragma warning disable CA1305 // Specify IFormatProvider
                        Assert.AreEqual(long.Parse(headerValue), httpRequestMessage?.Content?.Headers?.ContentLength);
#pragma warning restore CA1305 // Specify IFormatProvider
                        break;

                    case HeadersExtensions.ContentLocationHeaderName:
                        Assert.AreEqual(new Uri(headerValue, UriKind.Relative), httpRequestMessage?.Content?.Headers?.ContentLocation);
                        break;

                    case HeadersExtensions.ContentMD5HeaderName:
                        var expectedBytes = Convert.FromBase64String(headerValue);
                        var actualBytes = httpRequestMessage?.Content?.Headers?.ContentMD5 ?? new byte[0];
                        Assert.IsTrue(expectedBytes.SequenceEqual(actualBytes));
                        break;

                    case HeadersExtensions.ContentRangeHeaderName:
                        Assert.AreEqual(headerValue, httpRequestMessage?.Content?.Headers?.ContentRange?.ToString());

                        break;

                    default:
                        throw new NotImplementedException();
                }

                Assert.AreEqual(0, httpRequestMessage?.Headers?.ToList().Count);
            }
            else
            {
                Assert.AreEqual(headerValue, httpRequestMessage.Headers.First().Value.First());
            }
        }
#pragma warning restore CA1502 // Specify IFormatProvider
#endif

        [TestMethod]
        public void TestHeadersCollectionConstructor()
        {
            const string Key = "a";
            const string Value = "b";
            var headersCollection = new HeadersCollection(Key, Value);
            Assert.AreEqual(Key, headersCollection.First().Key);
            Assert.AreEqual(Value, headersCollection.First().Value.First());
        }
        #endregion

        #endregion

        #region Helpers

        private CreateHttpClient GetCreateHttpClient() => (n) => _mockHttpMessageHandler.ToHttpClient();


#if !NET45
        private Func<string, Lazy<HttpClient>> GetLazyCreate()
            => new((n) => new Lazy<HttpClient>(() => _mockHttpMessageHandler.ToHttpClient()));

        //TODO: Point a test at these on .NET 4.5

        private static void GetHttpClientMoq(out Mock<HttpMessageHandler> handlerMock, out HttpClient httpClient, HttpResponseMessage value)
        {
            handlerMock = new Mock<HttpMessageHandler>();

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
                var contentTypeHeader = httpRequestMessage?.Content?.Headers.FirstOrDefault(k => k.Key == HeadersExtensions.ContentTypeHeaderName);
                if (contentTypeHeader?.Value.FirstOrDefault() != HeadersExtensions.JsonMediaType) return false;
            }

            if (expectedHeaders != null)
            {
                foreach (var expectedHeader in expectedHeaders)
                {
                    var foundKeyValuePair = httpRequestMessage?.Headers.FirstOrDefault(k => k.Key == expectedHeader.Key);

                    if (foundKeyValuePair?.Value == null) throw new InvalidOperationException("Didn't find header");

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
                httpRequestMessage?.Method == HttpMethod.Post &&
                httpRequestMessage.RequestUri == requestUri;
        }
#endif
        private static HttpClient MintClient()
#if !NET45
        {
            var httpClient = _testServer.CreateClient();
            httpClient.BaseAddress = null;
            return httpClient;
        }
#else
        => new();
#endif

        private IClient GetJsonClient(AbsoluteUrl? baseUri = null)
        {
            IClient restClient;

            var defaultHeaders = HeadersExtensions.FromJsonContentType();

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

#if !NET45
        private static bool CheckRequestHeaders(IHeadersCollection requestHeadersCollection) =>
            requestHeadersCollection.Contains("Test") && requestHeadersCollection["Test"].First() == "Test";

        private static bool CheckResponseHeaders(IHeadersCollection responseHeadersCollection) => responseHeadersCollection.Contains("Test1") && responseHeadersCollection["Test1"].First() == "a";
#endif

        public void Dispose()
        {
            _mockHttpMessageHandler.Dispose();
            consoleLoggerFactory.Dispose();
        }

        #endregion
    }
}
