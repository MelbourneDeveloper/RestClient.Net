using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestClient.Net.Samples.Model;
using RestClient.Net.UnitTests.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet.UnitTests
{
    [TestClass]
    public class UnitTests
    {
        #region Tests
        [TestMethod]
        public async Task TestGetRestCountries()
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("https://restcountries.eu/rest/v2/"));
            var countries = await restClient.GetAsync<List<RestCountry>>();
            Assert.IsNotNull(countries);
            Assert.IsTrue(countries.Count > 0);
        }

        [TestMethod]
        public async Task TestPostUserWithCancellation()
        {
            try
            {
                var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"));

                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;

                var task = restClient.PostAsync<UserPost, UserPost>(new UserPost { title = "Moops" }, new Uri("/posts", UriKind.Relative), token);

                tokenSource.Cancel();

                await task;
            }
            catch (OperationCanceledException ex)
            {
                //Success
                return;
            }
            catch (Exception)
            {
                Assert.Fail("The operation threw an exception that was not an OperationCanceledException");
            }

            Assert.Fail("The operation completed successfully");
        }
        #endregion

        [TestMethod]
        [DataRow(HttpVerb.Patch)]
        [DataRow(HttpVerb.Post)]
        [DataRow(HttpVerb.Put)]
        public async Task TestUpdate(HttpVerb verb)
        {
            var restClient = new RestClient(new NewtonsoftSerializationAdapter(), new Uri("https://jsonplaceholder.typicode.com"));
            var requestUserPost = new UserPost { title = "foo", userId = 10, body = "testbody" };
            UserPost responseUserPost = null;

            switch (verb)
            {
                case HttpVerb.Patch:
                    responseUserPost = await restClient.PatchAsync<UserPost, UserPost>(requestUserPost, new Uri("/posts/1", UriKind.Relative));
                    break;
                case HttpVerb.Post:
                    responseUserPost = await restClient.PostAsync<UserPost, UserPost>(requestUserPost, "/posts");
                    break;
                case HttpVerb.Put:
                    responseUserPost = await restClient.PutAsync<UserPost, UserPost>(requestUserPost, new Uri("/posts/1", UriKind.Relative));
                    break;
            }

            Assert.AreEqual(requestUserPost.userId, responseUserPost.userId);
            Assert.AreEqual(requestUserPost.title, responseUserPost.title);
        }
    }
}
