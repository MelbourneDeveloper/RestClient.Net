# Cross Platform REST Client for all C# platforms #

_NEW_ **.NET Framework 4.5+ Support added in Version 1.1**

**Please send [feedback](https://github.com/MelbourneDeveloper/RestClient.Net/issues/new)! I'd really like to know what challenges you are having and which features you would like this library to have.**

### Comparison ###

| Library | .NET Framework Get 15x | [.NET Core Get 15x](https://github.com/MelbourneDeveloper/RestClient.Net/blob/21eaff49ba8af1ddbaeff5f3d17b73144df97557/RestClient.Net.UnitTests/PerformanceTests.cs#L14) 
| ------------- |:-------------:|:-------------:|
| RestClient.Net | 5831.1539ms |5724.9672ms |
| RestSharp | 7010.3713ms | 12391.7717ms|

Note: benchmarks are biased! Please submit a [pull request](https://github.com/MelbourneDeveloper/RestClient.Net/compare) to fix these [benchmarks](https://github.com/MelbourneDeveloper/RestClient.Net/blob/21eaff49ba8af1ddbaeff5f3d17b73144df97557/RestClient.Net.UnitTests/PerformanceTests.cs#L8)

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

NuGet: Install-Package RestClient.NET

Blog: https://christianfindlay.wordpress.com/

### [Get](https://github.com/MelbourneDeveloper/RestClient.Net/blob/d39df96bc7534bb92981047f60861a812bcaafa3/RestClient.Net.Samples/RestClient.Net.Samples/MainPage.xaml.cs#L126)

```cs
var countryCodeClient = new RestClientDotNet.RestClient(new NewtonsoftSerializationAdapter(), new Uri("http://services.groupkt.com/country/get/all"));
var countryData = await countryCodeClient.GetAsync<groupktResult<CountriesResult>>();
```

### [Put](https://github.com/MelbourneDeveloper/RestClient.Net/blob/d39df96bc7534bb92981047f60861a812bcaafa3/RestClient.Net.Samples/RestClient.Net.Samples/MainPage.xaml.cs#L108)

Post is basically the same

```cs
private void GetBitBucketClient(string password, bool isGet)
{
    var url = "https://api.bitbucket.org/2.0/repositories/" + UsernameBox.Text;
    _BitbucketClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri(url));

    if (!string.IsNullOrEmpty(password))
    {
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(UsernameBox.Text + ":" + password));
        _BitbucketClient.Headers.Add("Authorization", "Basic " + credentials);
    }
}
        
private async Task OnSavedClicked()
{
    ToggleBusy(true);

    try
    {
        var selectedRepo = ReposBox.SelectedItem as Repository;
        if (selectedRepo == null)
        {
            return;
        }

        //Ensure the client is ready to go
        GetBitBucketClient(GetPassword(), false);

        //var repoSlug = selectedRepo.full_name.Split('/')[1];
        var requestUri = $"https://api.bitbucket.org/2.0/repositories/{UsernameBox.Text}/{selectedRepo.full_name.Split('/')[1]}";

        //Post the change
        var retVal = await _BitbucketClient.PutAsync<Repository, Repository>(selectedRepo, requestUri);

        await DisplayAlert("Saved", "Your repo was updated.");
    }
    catch (Exception ex)
    {
        await HandleException(ex);
    }

    ToggleBusy(false);
}            
```

### [Patch](https://github.com/MelbourneDeveloper/RestClient.Net/blob/d39df96bc7534bb92981047f60861a812bcaafa3/RestClient.Net.Samples/RestClient.Net.Samples/MainPage.xaml.cs#L222)


```cs
var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"));
var userPost = await restClient.PatchAsync<UserPost, UserPost>(new UserPost { title = "Moops" }, "/posts/1");
```

### [Delete](https://github.com/MelbourneDeveloper/RestClient.Net/blob/d39df96bc7534bb92981047f60861a812bcaafa3/RestClient.Net.Samples/RestClient.Net.Samples/MainPage.xaml.cs#L215)


```cs
var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"));
await restClient.DeleteAsync("/posts/1");

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

## [Contribution](https://github.com/MelbourneDeveloper/RestClient.Net/blob/master/CONTRIBUTING.md)


