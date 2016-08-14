using Atlassian;
using CF.RESTClientDotNet;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var authenticationHeader = Request.Headers["Authentication"];
            var credentials = authenticationHeader.FirstOrDefault();

            GetBitBucketClient(credentials, id);

            var reposResult = (await _BitbucketClient.GetAsync());

            return reposResult.Data;

            //_BitbucketClient.GetAsync
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

        private void GetBitBucketClient(string credentials, string username)
        {
            if (_BitbucketClient != null)
            {
                return;
            }

            string url = "https://api.bitbucket.org/2.0/repositories/" + username;
            _BitbucketClient.Headers.Add("Authorization", credentials);
            _BitbucketClient.ErrorType = typeof(ErrorModel);
        }
    }
}
