A simple REST Client for all .NET related platforms

Beta Release (Looking for testers)

* Open Source. (MIT License)
* Markup language agnostic. (Supports JSON, SOAP and other markup languages)
* Use strong types with REST.
* Supports Android, iOS, Windows 10, Windows 10 Phone, Silverlight, .NET, .NET Core, .NET Standard 1.4.
* Incredibly simple (All source code just over 100 lines)
* Async friendly (uses async, await keywords).

To get started clone (direct access to binaries):
https://MelbourneDeveloper@bitbucket.org/MelbourneDeveloper/restclient-.net.git

NuGet: Install-Package RESTClient.NET

Blog: https://christianfindlay.wordpress.com/

```
#!c#

            var countryCodeClient = new RESTClient(new NewtonsoftSerializationAdapter(), new Uri("http://services.groupkt.com/country/get/all"));
            var countryData = await countryCodeClient.GetAsync<groupktResult<CountriesResult>>();
```

The public interface of the classes is fairly stable but is likely to change slightly in future. The DELETE, and PATCH verbs need to be implemented and tested. The PUT and POST verbs have not been worked through completely. But, despite all this, should you choose to use this library in your solution, any changes to the public interface should not affect your code too much.