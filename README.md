# Cross Platform REST Client for all C# platforms #

### Features ###

* Open Source. (MIT License)
* Markup language agnostic. (Supports JSON, Binary, SOAP and other markup languages with dependency injection)
* Use strong types with REST.
* Supports Android, iOS, Windows 10, .NET Framework, .NET Core (.NET Standard 2.0) .
* Incredibly simple (All source code less than 100 lines)
* Async friendly (uses async, await keywords).
* Only one .NET Standard library for all platforms

## Quick Start ##
Samples for all platforms in this Git repo:
https://MelbourneDeveloper@bitbucket.org/MelbourneDeveloper/restclient-.net.git

NuGet: Install-Package RESTClient.NET

Blog: https://christianfindlay.wordpress.com/

```
#!c#

            var countryCodeClient = new RESTClient(new NewtonsoftSerializationAdapter(), new Uri("http://services.groupkt.com/country/get/all"));
            var countryData = await countryCodeClient.GetAsync<groupktResult<CountriesResult>>();
```
