![diagram](https://github.com/MelbourneDeveloper/Restclient.Net/blob/master/Images/Rendered/Logo.jpg) 

# REST Client Framework for all .NET Platforms #

The best .NET REST Client with task-based async, strong types and dependency injection on all platforms. 

**[Documentation Here](https://github.com/MelbourneDeveloper/RestClient.Net/wiki)**

### Announcement ###

**Version 3 has been released!**

[Follow me on Twitter](https://twitter.com/cfdevelop) for updates.

A series of Blog posts will introduce the new functionality in the coming weeks. https://christianfindlay.com/

### Features ###

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

### [Get](https://github.com/MelbourneDeveloper/RestClient.Net/blob/13c95c615400d39523c02e803b46a564ff4c91db/RestClient.Net.UnitTests/UnitTests.cs#L81)

```cs
var client = new Client(new NewtonsoftSerializationAdapter(), new Uri("https://restcountries.eu/rest/v2/"));
var response = await client.GetAsync<List<RestCountry>>();
```

### Post / Put / Patch

[**Protocol Buffers**](https://github.com/MelbourneDeveloper/RestClient.Net/blob/80d19ebc599027e2c68acb06a4e1f853683c3517/RestClient.Net.Samples/RestClient.Net.CoreSample/Program.cs#L25) (Binary)
```cs
var person = new Person { FirstName = "Bob", Surname = "Smith" };
var client = new Client(new ProtobufSerializationAdapter(), new Uri("http://localhost:42908/person"));
person = await client.PostAsync<Person, Person>(person);
```

[**JSON**](https://github.com/MelbourneDeveloper/RestClient.Net/blob/236a454232455aa3dc0cea230e991329288c153d/RestClient.Net.Samples/RestClient.NET.Samples/MainPage.xaml.cs#L233)
```cs
var client = new Client(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"));
client.SetJsonContentTypeHeader();
UserPost userPost = await client.PostAsync<UserPost, UserPost>(new UserPost { title = "Title" }, "/posts");
```

### [Delete](https://github.com/MelbourneDeveloper/RestClient.Net/blob/f7f4f88b90c6b0014530891d094d958193776a52/RestClient.Net.UnitTests/UnitTests.cs#L94)
```cs
var client = new Client(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"));
await client.DeleteAsync("posts/1");
```

## Donate

| Coin           | Address |
| -------------  |:-------------:|
| Bitcoin        | [33LrG1p81kdzNUHoCnsYGj6EHRprTKWu3U](https://www.blockchain.com/btc/address/33LrG1p81kdzNUHoCnsYGj6EHRprTKWu3U) |
| Ethereum       | [0x7ba0ea9975ac0efb5319886a287dcf5eecd3038e](https://etherdonation.com/d?to=0x7ba0ea9975ac0efb5319886a287dcf5eecd3038e) |

## [Contribution](https://github.com/MelbourneDeveloper/RestClient.Net/blob/master/CONTRIBUTING.md)

Please log any issues or feedback in the issues section. For pull requests, please see the contribution guide.
