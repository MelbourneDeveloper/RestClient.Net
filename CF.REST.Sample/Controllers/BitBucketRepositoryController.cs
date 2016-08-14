using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CF.RESTClientDotNet;
using System.Text;

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
        public string Get(int id)
        {
            return "value";
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
            if (_BitbucketClient != null)
            {
                return;
            }

            string url = "https://api.bitbucket.org/2.0/repositories/" + username;
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
            _BitbucketClient = new RESTClient(new NewtonsoftSerializationAdapter(), new Uri(url));
            _BitbucketClient.Headers.Add("Authorization", "Basic " + credentials);
            _BitbucketClient.ErrorType = typeof(ErrorModel);
        }
    }
}
