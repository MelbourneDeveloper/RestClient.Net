![diagram](https://github.com/MelbourneDeveloper/Restclient.Net/blob/main/src/Images/Rendered/Logo.jpg) 

# .NET REST Client Framework for all platforms #

## [Follow Me on Twitter for Updates](https://twitter.com/intent/follow?screen_name=cfdevelop&tw_p=followbutton) ##

The best .NET REST Client with task-based async, strong types and dependency injection on all platforms. Consume your ASP .NET Core Web APIs or consume RESTful APIs over the internet in C# or Visual Basic.

![.NET Core](https://github.com/MelbourneDeveloper/RestClient.Net/workflows/.NET%20Core/badge.svg)

**[Documentation Here](https://github.com/MelbourneDeveloper/RestClient.Net/wiki)**

### Announcement ###

**Version 4.0.0 has been released!**

Check out the [features and fixes](https://github.com/MelbourneDeveloper/RestClient.Net/projects/3). There are some breaking changes. `IHttpClientFactory` and `IClientFactory` have been converted to delegates to save on boilerplate code.

Check out blog posts here https://christianfindlay.com/

### Why You Should Use It ###

* It's fast! [Initial tests](https://codereview.stackexchange.com/questions/235804/c-rest-client-benchmarking) show that it is faster than RestSharp and is one of the faster .NET Rest Clients available.
* Designed for Dependency Injection, Unit Testing and use with IoC Containers
* Async friendly. All operations use async, await keywords.
* Integrates with [Polly](https://github.com/MelbourneDeveloper/RestClient.Net/wiki/Integration-With-Polly) resilience and transient-fault-handling
* Automatic serialization with any method (JSON, Binary, SOAP, [Google Protocol Buffers](https://developers.google.com/protocol-buffers))
* Installation from NuGet is easy on any platform
* Uses strong types with content body
* Supports [WebAssembly](https://github.com/MelbourneDeveloper/RestClient.Net/wiki/Web-Assembly-Support), Android, iOS, Windows 10, .NET Framework 4.5+, .NET Core (.NET Standard 2.0)
* Supports GET, POST, PUT, PATCH, DELETE with ability to use less common HTTP methods

These features together make this the best C# REST client and the best alternative to RestSharp. Consuming REST APIs is simple and encourages best practice.

## [Quick Start & Samples](https://github.com/MelbourneDeveloper/RestClient.Net/wiki/Quick-Start-&-Samples)

See [documentation](https://github.com/MelbourneDeveloper/RestClient.Net/wiki/Quick-Start-&-Samples) for more examples.

NuGet: Install-Package RestClient.NET

Please read [this article](https://github.com/MelbourneDeveloper/RestClient.Net/wiki/Serialization-and-Deserialization-(ISerializationAdapter)#newtonsoft) about Serialization and Deserialization.


### [Get](https://github.com/MelbourneDeveloper/RestClient.Net/blob/13c95c615400d39523c02e803b46a564ff4c91db/RestClient.Net.UnitTests/UnitTests.cs#L81)

With [Newtonsoft serialization](https://github.com/MelbourneDeveloper/RestClient.Net/wiki/Serialization-and-Deserialization-With-ISerializationAdapter#newtonsoft)

<!-- snippet: GetNewtonsoft -->
<a id='snippet-getnewtonsoft'></a>
```cs
using var client = new Client(new NewtonsoftSerializationAdapter(), new Uri("https://restcountries.eu/rest/v2/"));
var response = await client.GetAsync<List<RestCountry>>();
```
<sup><a href='/src/RestClient.Net.UnitTests/Snippets.cs#L25-L30' title='Snippet source file'>snippet source</a> | <a href='#snippet-getnewtonsoft' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

With [default serialization on .NET Core](https://github.com/MelbourneDeveloper/RestClient.Net/wiki/Serialization-and-Deserialization-With-ISerializationAdapter#default-json-serialization-adapter)

<!-- snippet: GetDefault -->
<a id='snippet-getdefault'></a>
```cs
using var client = new Client(baseUri: new Uri("https://restcountries.eu/rest/v2/"));
var response = await client.GetAsync<List<RestCountry>>();
```
<sup><a href='/src/RestClient.Net.UnitTests/Snippets.cs#L37-L42' title='Snippet source file'>snippet source</a> | <a href='#snippet-getdefault' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Post / Put / Patch


#### Protocol Buffers (Binary)

<!-- snippet: PostBinary -->
<a id='snippet-postbinary'></a>
```cs
var person = new Person {FirstName = "Bob", Surname = "Smith"};
using var client = new Client(new ProtobufSerializationAdapter(), new Uri("http://localhost:42908/person"));
person = await client.PostAsync<Person, Person>(person);
```
<sup><a href='/src/RestClient.Net.UnitTests/Snippets.cs#L49-L55' title='Snippet source file'>snippet source</a> | <a href='#snippet-postbinary' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### JSON

<!-- snippet: PostNewtonsoft -->
<a id='snippet-postnewtonsoft'></a>
```cs
using var client = new Client(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"));
var body = new UserPost
{
    title = "Title"
};
UserPost userPost = await client.PostAsync<UserPost, UserPost>(body, "/posts");
```
<sup><a href='/src/RestClient.Net.UnitTests/Snippets.cs#L60-L69' title='Snippet source file'>snippet source</a> | <a href='#snippet-postnewtonsoft' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Delete

<!-- snippet: DeleteDefault -->
<a id='snippet-deletedefault'></a>
```cs
using var client = new Client(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"));
await client.DeleteAsync("posts/1");
```
<sup><a href='/src/RestClient.Net.UnitTests/Snippets.cs#L15-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-deletedefault' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Donate

| Coin           | Address |
| -------------  |:-------------:|
| Bitcoin        | [33LrG1p81kdzNUHoCnsYGj6EHRprTKWu3U](https://www.blockchain.com/btc/address/33LrG1p81kdzNUHoCnsYGj6EHRprTKWu3U) |
| Ethereum       | [0x7ba0ea9975ac0efb5319886a287dcf5eecd3038e](https://etherdonation.com/d?to=0x7ba0ea9975ac0efb5319886a287dcf5eecd3038e) |


## [Contribution](https://github.com/MelbourneDeveloper/RestClient.Net/blob/master/CONTRIBUTING.md)

Please log any issues or feedback in the issues section. For pull requests, please see the contribution guide.
