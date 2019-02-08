# Cross Platform REST Client for all C# platforms #

**Please send [feedback](https://github.com/MelbourneDeveloper/RestClient.Net/issues/new)! I'd really like to know what challenges you are having and which features you would like this library to have.**

### Features ###

* Open Source. (MIT License)
* Markup language agnostic. (Supports JSON, Binary, SOAP and other markup languages with dependency injection)
* Use strong types with REST.
* Supports Android, iOS, Windows 10, .NET Framework, .NET Core (.NET Standard 2.0) .
* Incredibly simple (All source code less than 100 lines)
* Async friendly (uses async, await keywords).
* Only one .NET Standard library for all platforms

## Quick Start & Samples ##
Samples for all platforms in this Git repo:

https://github.com/MelbourneDeveloper/RestClient.Net.git

NuGet: Install-Package RESTClient.NET

Blog: https://christianfindlay.wordpress.com/

```cs
var countryCodeClient = new RESTClient(new NewtonsoftSerializationAdapter(), new Uri("http://services.groupkt.com/country/get/all"));
var countryData = await countryCodeClient.GetAsync<groupktResult<CountriesResult>>();
```

Hardfolio runs on RestClient.Net

Windows Store
https://www.microsoft.com/en-au/p/hardfolio/9p8xx70n5d2j

Google Play
https://play.google.com/store/apps/details?id=com.Hardfolio

## Donate

Bitcoin: 33LrG1p81kdzNUHoCnsYGj6EHRprTKWu3U

Ethereum: 0x7ba0ea9975ac0efb5319886a287dcf5eecd3038e

Litecoin: MVAbLaNPq7meGXvZMU4TwypUsDEuU6stpY

## Contribution

Contribution is welcome. Please fork, tighten up the code (real tight), test, and submit a pull request. Please log issues in the issues section.


