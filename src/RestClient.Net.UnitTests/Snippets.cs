using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiExamples.Model.JsonModel;
using RestClient.Net.Samples.Model;
using RestClient.Net.UnitTests.Model;

namespace RestClient.Net.UnitTests
{
    [TestClass]
    public class Snippets
    {
        [TestMethod]
        public async Task Delete()
        {
            #region DeleteDefault

            using var client = new Client(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"));
            await client.DeleteAsync("posts/1");

            #endregion
        }

        [TestMethod]
        public async Task GetNewtonsoft()
        {
            #region GetNewtonsoft

            using var client = new Client(new NewtonsoftSerializationAdapter(), new Uri("https://restcountries.eu/rest/v2/"));
            var response = await client.GetAsync<List<RestCountry>>();

            #endregion
        }

#if NETCOREAPP

        [TestMethod]
        public async Task GetDefault()
        {
            #region GetDefault

            using var client = new Client(baseUri: new Uri("https://restcountries.eu/rest/v2/"));
            var response = await client.GetAsync<List<RestCountry>>();

            #endregion
        }

#endif

        public async Task PostBinary()
        {
            #region PostBinary

            var person = new Person {FirstName = "Bob", Surname = "Smith"};
            using var client = new Client(new ProtobufSerializationAdapter(), new Uri("http://localhost:42908/person"));
            person = await client.PostAsync<Person, Person>(person);

            #endregion
        }

        [TestMethod]
        public async Task PostNewtonsoft()
        {
            #region PostNewtonsoft

            using var client = new Client(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"));
            var body = new UserPost
            {
                title = "Title"
            };
            UserPost userPost = await client.PostAsync<UserPost, UserPost>(body, "/posts");

            #endregion
        }
    }
}