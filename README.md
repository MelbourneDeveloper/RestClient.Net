![diagram](https://github.com/MelbourneDeveloper/Restclient.Net/blob/main/src/Images/Rendered/Logo.jpg) 

# .NET REST Client Framework for all platforms #

[![buildandtest](https://github.com/MelbourneDeveloper/RestClient.Net/actions/workflows/buildandtest.yml/badge.svg)](https://github.com/MelbourneDeveloper/RestClient.Net/actions/workflows/buildandtest.yml)

RestClient.Net is a powerful .NET REST API client that features task-based async, strong types, and dependency injection support for all platforms. Use it to consume ASP.NET Core Web APIs or interact with RESTful APIs over the internet in C#, F#, or Visual Basic. It's designed with functional-style programming and F# in mind.

NuGet: [RestClient.Net](https://www.nuget.org/packages/RestClient.Net)

### [Follow Me on Twitter for Updates](https://twitter.com/intent/follow?screen_name=cfdevelop&tw_p=followbutton) ##

[![.NET](https://github.com/MelbourneDeveloper/RestClient.Net/actions/workflows/dotnet.yml/badge.svg?branch=5%2Fdevelop)](https://github.com/MelbourneDeveloper/RestClient.Net/actions/workflows/dotnet.yml)

## 6.0 Release

This release updates all dependencies and targets major versions of .NET: 4.5, 5, 6, and 7.

### [Follow Me on Twitter for Updates](https://twitter.com/intent/follow?screen_name=cfdevelop&tw_p=followbutton) ##


### Key Features

* **First-class URLs**: Utilizes [Urls](https://github.com/MelbourneDeveloper/Urls) to treat URLs as immutable [records](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/records) with a fluent API for construction.
* **Dependency Injection Support**: Easily mock REST calls and add RestClient.Net to your IoC container with a single line of code.
* **Async-Friendly**: All operations use async and await keywords.
* **Automatic Serialization**: Automatically serializes request/response bodies to/from strong types (JSON, Binary, SOAP, [Google Protocol Buffers](https://developers.google.com/protocol-buffers)). The library is decoupled from Newtonsoft, allowing you to use any serialization method or version of Newtonsoft. This means compatibility with any version of Azure Functions.
* **Cross-Platform Compatibility**: Install from NuGet on any platform from .NET Framework 4.5 up to .NET 5. Supports Xamarin (Mono, iOS, Android), UWP, [WebAssembly](https://github.com/MelbourneDeveloper/RestClient.Net/wiki/Web-Assembly-Support), and Unity with .NET Standard 2.0.
* **HTTP Methods**: Supports GET, POST, PUT, PATCH, DELETE, and custom methods.
* **Fluent API**: Provides a fluent API for construction, non-destructive mutation, and URL construction.
* **Logging**: Uses .NET Core Logging - [`ILogger`](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-5.0) for logging all aspects of HTTP calls.
* **Thread Safety**: Immutable client for thread safety.
* **High-Quality Code**: Tight code and complete test coverage allow you to make changes if needed.

![diagram](https://github.com/MelbourneDeveloper/Restclient.Net/blob/main/src/Images/Rendered/Stats.png) 
## Examples

For a complete set of examples, see these [unit tests](https://github.com/MelbourneDeveloper/RestClient.Net/blob/3574038f02a83a299f9536b71c7f839ae72e0e08/src/RestClient.Net.UnitTests/MainUnitTests.cs#L279).

#### POST an Object and get Response

```cs
using var client =
    //Build the Url from the host name
    new Client("jsonplaceholder.typicode.com".ToHttpsUriFromHost());

UserPost userPost = await client.PostAsync<UserPost, UserPost>(
    //POST the UserPost to the server
    new UserPost { title = "Title" }, "posts"
    );
```

### Dependency Injection ([RestClient.Net.DependencyInjection](https://www.nuget.org/packages/RestClient.Net.DependencyInjection) NuGet Package)

#### Wiring it up
```cs
var serviceCollection = new ServiceCollection()
    //Add a service which has an IClient dependency
    .AddSingleton<IGetString, GetString1>()
    //Add RestClient.Net with a default Base Url of http://www.test.com
    .AddRestClient((o) => o.BaseUrl = "http://www.test.com".ToAbsoluteUrl());

//Use HttpClient dependency injection
_ = serviceCollection.AddHttpClient();
```

#### Getting a Global IClient in a Service

```cs
public class GetString1 : IGetString
{
    public IClient Client { get; }

    public GetString1(IClient client) => Client = client;

    public async Task<string> GetStringAsync() => await Client.GetAsync<string>();
}
```

#### Getting a IClient Using a Factory

```cs
public class GetString2 : IGetString
{
    public IClient Client { get; }

    public GetString2(CreateClient createClient)
    {
        //Use the options to set the BaseUrl or other properties on the Client
        Client = createClient("test", (o) => { o.BaseUrl = o.BaseUrl with { Host = "www.test.com" }; });
    }

    public async Task<string> GetStringAsync() => await Client.GetAsync<string>();
}
```

#### Make Call and Construct Client

```cs
//This constructs an AbsoluteUrl from the string, makes the GET call, and deserializes the JSON to a strongly typed list
//The response also contains a Client with the base of the Url that you can reuse
//Note: not available on .NET 4.5

var response = await "https://restcountries.eu/rest/v2"
    .ToAbsoluteUrl()
    .GetAsync<List<RestCountry>>();
```

#### Query Github Issues with GraphQL (You must authorize GraphQL Github App)

```cs
using RestClient.Net.Abstractions.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Urls;

namespace RestClient.Net
{
    public static class GitHubGraphQLMethods
    {
        public static async Task<T> GetIssues<T>(string repo, string accessToken)
        => (await "https://api.github.com/graphql"
            .ToAbsoluteUrl()
            .PostAsync<QueryResponse<T>, QueryRequest>(
                new QueryRequest("{ search(query: \"repo:" + repo + "\", type: ISSUE, first: 100) {nodes {... on Issue { number title body } } }}")
                , HeadersExtensions.FromBearerToken(accessToken)
            .Append("User-Agent", "RestClient.Net"))).Response.Body.data.search;
    }

    public record QueryRequest(string query);
    public record Issue(int? number, string title, string body);
    public record Issues(List<Issue> nodes);
    public record Data<T>(T search);
    public record QueryResponse<T>(Data<T> data);

}
```

#### Url Construction with F#

```fs
[<TestMethod>]
member this.TestComposition () =

    let uri =
        "host.com".ToHttpUrlFromHost(5000)
        .AddQueryParameter("fieldname1", "field<>Value1")
        .WithCredentials("username", "password")
        .AddQueryParameter("FieldName2", "field<>Value2")
        .WithFragment("frag")
        .WithPath("pathpart1", "pathpart2")

    Assert.AreEqual("http://username:password@host.com:5000/pathpart1/pathpart2?fieldname1=field%3C%3EValue1&FieldName2=field%3C%3EValue2#frag",uri.ToString());
```

## [Contribution](https://github.com/MelbourneDeveloper/RestClient.Net/blob/master/CONTRIBUTING.md)
