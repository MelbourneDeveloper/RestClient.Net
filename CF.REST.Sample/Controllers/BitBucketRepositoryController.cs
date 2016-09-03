using Atlassian;
using CF.RESTClientDotNet;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CF.REST.Sample.Controllers
{
    [Route("api/[controller]")]
    public class BitBucketRepositoryController : Controller
    {
        #region Fields
        private RESTClient _BitbucketClient;
        #endregion

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<string> Get(string id)
        {
            //string credentials = string.Empty;
            try
            {
                var credentialTokens = id.Split('-');
                GetBitBucketClient(credentialTokens[0], credentialTokens[1]);
                var reposResult = (await _BitbucketClient.GetAsync());
                return Encoding.ASCII.GetString(reposResult.Data);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private void GetBitBucketClient(string username, string password)
        {
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
            string url = "https://api.bitbucket.org/2.0/repositories/" + username;
            _BitbucketClient = new RESTClient(new NewtonsoftSerializationAdapter(), new Uri(url));
            _BitbucketClient.Headers.Add("Authorization", "Basic " + credentials);
            _BitbucketClient.ErrorType = typeof(ErrorModel);
        }
    }
}
