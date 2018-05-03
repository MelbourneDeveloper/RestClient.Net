using Atlassian;
using CF.RESTClientDotNet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using restclientdotnet = CF.RESTClientDotNet;

namespace RESTClient.NET.CoreSample
{
    class Program
    {
        static string username = "MelbourneDeveloper";
        static string password = "";
        static restclientdotnet.RESTClient _BitbucketClient;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Go();
            Console.ReadLine();
        }

        private async static Task Go()
        {
            GetBitBucketClient(true);
            var repos = (await _BitbucketClient.GetAsync<RepositoryList>());
            var backup = repos.values[3];
            backup.description = "testdesc";

            var httpClient = new HttpClient();
            var stringContent = new StringContent(JsonConvert.SerializeObject(backup), Encoding.UTF8, "application/json");
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", "Basic " + credentials);

            //var Headers = new Dictionary<string, string>();
            //Headers.Add("Authorization", "Basic " + credentials);

            //foreach (var key in Headers.Keys)
            //{
            //    stringContent.Headers.Add(key, Headers[key]);
            //}

            var requestUri = $"https://api.bitbucket.org/2.0/repositories/{username}/{backup.full_name.Split('/')[1]}";
            var response = await httpClient.PutAsync(requestUri, stringContent);
            //backup = await _BitbucketClient.PostAsync<Repository, Repository>(backup, backup.full_name.Split('/')[1]);
        }

        private static void GetBitBucketClient(bool isGet)
        {
            var url = "https://api.bitbucket.org/2.0/repositories/" + username;
            _BitbucketClient = new restclientdotnet.RESTClient(new NewtonsoftSerializationAdapter(), new Uri(url));

            if (!string.IsNullOrEmpty(password))
            {
                var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
                _BitbucketClient.Headers.Add("Authorization", "Basic " + credentials);
            }

            _BitbucketClient.ErrorType = typeof(ErrorModel);
        }
    }
}
