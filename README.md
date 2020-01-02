![diagram](https://github.com/MelbourneDeveloper/Restclient.Net/blob/master/Images/Rendered/Logo.jpg) 

# REST Client Framework for all .NET Platforms #

The most simple task-based async, strongly typed, cross-platform .NET REST Client. 

### Announcement ###

Version 3 is on it's way to the NuGet gallery. Documentation on this page relates to version 3. Grab the latest from the develop branch to start using Version 3 before the official release. [Follow me on Twitter](https://twitter.com/cfdevelop) for updates.

A series of Blog posts will introduce the new functionality in the coming weeks. https://christianfindlay.com/

### Features ###

* Designed for Dependency Injection, Unit Testing and use with IoC Containers
* Async friendly. All operations use async, await keywords.
* Integrates with [Polly](https://github.com/App-vNext/Polly) resilience and transient-fault-handling
* Automatic serialization with any method (JSON, Binary, SOAP, [Google Protocol Buffers](https://developers.google.com/protocol-buffers))
* Installation from NuGet is easy on any platform
* Uses strong types with content body
* Supports [WebAssembly](https://webassembly.org/), Android, iOS, Windows 10, .NET Framework 4.5+, .NET Core (.NET Standard 2.0)

## Quick Start & Samples ##

Clone this repo and open the samples solution. There are samples for WebAssembly (in browser, UWP, Android, iOS, .NET Core, and many examples of common tasks like Polly Integration in the Unit Tests). The solution is called RestClient.Net.Samples.sln.  

or

NuGet: Install-Package RestClient.NET

### [Get](https://github.com/MelbourneDeveloper/RestClient.Net/blob/13c95c615400d39523c02e803b46a564ff4c91db/RestClient.Net.UnitTests/UnitTests.cs#L81)

```cs
var client = new Client(new NewtonsoftSerializationAdapter(), new Uri("https://restcountries.eu/rest/v2/"));
var response = await client.GetAsync<List<RestCountry>>();
```

### [Post / Put / Patch](https://github.com/MelbourneDeveloper/RestClient.Net/blob/80d19ebc599027e2c68acb06a4e1f853683c3517/RestClient.Net.Samples/RestClient.Net.CoreSample/Program.cs#L25)

```cs
var person = new Person { FirstName = "Bob", Surname = "Smith" };
var client = new Client(new ProtobufSerializationAdapter(), new Uri("http://localhost:42908/person"));
person = await client.PostAsync<Person, Person>(person);
```

### [Delete](https://github.com/MelbourneDeveloper/RestClient.Net/blob/f7f4f88b90c6b0014530891d094d958193776a52/RestClient.Net.UnitTests/UnitTests.cs#L94)

```cs
var client = new Client(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"));
await client.DeleteAsync("posts/1");
```

### In Browser (Wasm) Support with [Uno Platform](https://platform.uno/) - New! ###

RestClient.Net can now run inside the browser via WebAssembly. To see a sample:

 - Clone the repo
 - Make sure you've got .NET Core 3.0 installed
 - Open the solution RestClient.Net.Samples.sln in Visual Studio 2017
 - Switch to debug
 - Unload or remove any projects whose frameworks are not installed (e.g. Android/iOS)
 - Wait for NuGet packages to be restored 
 - Ensure the project RestClient.Net.Samples.Uno.Wasm is built
 - Run the project RestClient.Net.Samples.Uno.Wasm
 - Click "Get My Repos" to get my public repos, or enter your own username/password for your private repos
 - [This](https://github.com/MelbourneDeveloper/RestClient.Net/blob/master/RestClient.Net.Samples.Uno/RestClient.Net.Samples.Uno.Shared/MainPage.xaml.cs) is the code. It's shared across Wasm and UWP
 - If you run the RestClient.Net.Samples.Uno.UWP project, you will see that the app is almost identicle to the one in the browser
 - Log any issues in the issues section please!

## Donate

| Coin           | Address |
| -------------  |:-------------:|
| Bitcoin        | [33LrG1p81kdzNUHoCnsYGj6EHRprTKWu3U](https://www.blockchain.com/btc/address/33LrG1p81kdzNUHoCnsYGj6EHRprTKWu3U) |
| Ethereum       | [0x7ba0ea9975ac0efb5319886a287dcf5eecd3038e](https://etherdonation.com/d?to=0x7ba0ea9975ac0efb5319886a287dcf5eecd3038e) |

## [Contribution](https://github.com/MelbourneDeveloper/RestClient.Net/blob/master/CONTRIBUTING.md)


