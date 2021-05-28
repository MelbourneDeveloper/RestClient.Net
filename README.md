![diagram](https://github.com/MelbourneDeveloper/Restclient.Net/blob/main/src/Images/Rendered/Logo.jpg) 

# .NET REST Client Framework for all platforms #

The best .NET REST Client with task-based async, strong types, and dependency injection on all platforms. Consume your ASP .NET Core Web APIs or consume RESTful APIs over the internet in C#, F# or Visual Basic. Designed for functional style programming with F# in mind.

NuGet: [RestClient.Net](https://www.nuget.org/packages/RestClient.Net)

### [Follow Me on Twitter for Updates](https://twitter.com/intent/follow?screen_name=cfdevelop&tw_p=followbutton) ##

[![.NET](https://github.com/MelbourneDeveloper/RestClient.Net/actions/workflows/dotnet.yml/badge.svg?branch=5%2Fdevelop)](https://github.com/MelbourneDeveloper/RestClient.Net/actions/workflows/dotnet.yml)

## 5.0 Release

5.0 brings a Fluent API, immutable types, lots of useful extension methods, and thorough unit testing. Check out [the tests](https://github.com/MelbourneDeveloper/RestClient.Net/blob/98560ee7f7470218f80e7416f2e8e9c60bfab759/src/RestClient.Net.UnitTests/MainUnitTests.cs#L290) for code examples.

### [Follow Me on Twitter for Updates](https://twitter.com/intent/follow?screen_name=cfdevelop&tw_p=followbutton) ##

### Why You Should Use It ###

* Treats Urls as first-class citizens with [Urls](https://github.com/MelbourneDeveloper/Urls). URLs are immutable [records](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/records) and have a fluent API for construction.
* Designed for Dependency Injection. Mock your REST calls and add RestClient.Net to your IoC container with one line of code
* Async friendly. All operations use async, await keywords
* Automatic request/response body serialization to/from strong types (JSON, Binary, SOAP, [Google Protocol Buffers](https://developers.google.com/protocol-buffers))
  * The library is decoupled from Newtsonsoft so you can use any serialization method or version of Newtonsoft
* Install from NuGet on any platform from .NET Framework 4.5 up to .NET 5. Supports Xamarin (Mono, iOS, Android), UWP, [WebAssembly](https://github.com/MelbourneDeveloper/RestClient.Net/wiki/Web-Assembly-Support) and Unity with .NET Standard 2.0
* Supports GET, POST, PUT, PATCH, DELETE with ability and custom methods
* Tight code (around 350 lines) means you can make a change if you need to

## Examples

For a full set of examples see these [unit tests](https://github.com/MelbourneDeveloper/RestClient.Net/blob/3574038f02a83a299f9536b71c7f839ae72e0e08/src/RestClient.Net.UnitTests/MainUnitTests.cs#L279).

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
//This constructs an AbsoluteUrl from the string, makes the GET call and deserializes the JSON to a strongly typed list
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


## Donate

| Coin           | Address |
| -------------  |:-------------:|
| Bitcoin        | [33LrG1p81kdzNUHoCnsYGj6EHRprTKWu3U](https://www.blockchain.com/btc/address/33LrG1p81kdzNUHoCnsYGj6EHRprTKWu3U) |
| Ethereum       | [0x7ba0ea9975ac0efb5319886a287dcf5eecd3038e](https://etherdonation.com/d?to=0x7ba0ea9975ac0efb5319886a287dcf5eecd3038e) |

## [Contribution](https://github.com/MelbourneDeveloper/RestClient.Net/blob/master/CONTRIBUTING.md)

 

