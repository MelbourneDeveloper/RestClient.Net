using Atlassian;
using CF.RESTClientDotNet;
using System;
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
            backup = await _BitbucketClient.PostAsync<Repository, Repository>(backup, backup.full_name.Split('/')[1]);
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
