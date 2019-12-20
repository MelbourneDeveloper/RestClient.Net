using Atlassian;
using RestClientDotNet;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTClient.NET.CoreSample
{
    internal class Program
    {
        #region Main Method
        private static void Main(string[] args)
        {
            Go();
            Console.ReadLine();
        }
        #endregion

        #region Methods
        private static async Task Go()
        {
            var restClient = new RestClient(new ProtobufSerializationAdapter(), new Uri("http://localhost:42908/person"));
            //restClient.GetAsync < Person>

            Console.WriteLine($"Saved repo and Bitbucket didn't complain. The description of the repo that came back from Bitbucket was '{repo.description}'.\r\n{(repo.description == RepoDescription ? "Changing the description succeeded." : "Changing the description failed.")}");
        }

        #endregion
    }
}
