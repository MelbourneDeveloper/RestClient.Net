A simple REST Client for all .NET related platforms

Warning: Alpha Release (no guarantees of quality)

* Markup language agnostic (supports JSON, SOAP and other markup languages)
* Use strong types with REST
* Supports Android, iOS, Windows 10, Windows 10 Phone, Silverlight, .NET, .NET Core
* Incredibly simple
* Async friendly (uses async, await keywords)


```
#!c#

            var countryCodeClient = new RESTClient(new NewtonsoftSerializationAdapter(), new Uri("http://services.groupkt.com/country/get/all"));
            var countryData = await countryCodeClient.GetAsync<groupktResult<CountriesResult>>();
```