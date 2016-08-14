A simple REST Client for all .NET related platforms

Warning: Alpha Release (no guarantees of quality)

* Open Source. (MIT License)
* Markup language agnostic. (Supports JSON, SOAP and other markup languages)
* Use strong types with REST.
* Supports Android, iOS, Windows 10, Windows 10 Phone, Silverlight, .NET, .NET Core.
* Incredibly simple.
* Async friendly (uses async, await keywords).

Install-Package RESTClientDotNet 

```
#!c#

            var countryCodeClient = new RESTClient(new NewtonsoftSerializationAdapter(), new Uri("http://services.groupkt.com/country/get/all"));
            var countryData = await countryCodeClient.GetAsync<groupktResult<CountriesResult>>();
```

The version 0.1.0 release works well. But I welcome testing and issue logs. I've been working on this release and testing it for several months. The main hurdles of getting it to work on all the different platforms with GET, POST, and PUT have been overcome. However, the code is not yet complete, and several permutations of HTTP verbs, and platforms have not been tested. I very much invite people add unit tests to this solution and to write more meat in to the samples. The aim is to beef out the Xamarin Forms sample to have some useful BitBucket functionality and get that sample working across all Xamarin Forms platforms.

The public interface of the classes is fairly stable but is likely to change slightly in future. The DELETE, and PATCH verbs need to be implemented and tested. The PUT and POST verbs have not been worked through completely. But, despite all this, should you choose to use this library in your solution, any changes to the public interface should not affect your code too much.