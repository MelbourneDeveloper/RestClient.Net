using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using CF.RESTClientDotNet;
using Atlassian;

namespace CF.REST.Sample.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
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
        public async Task<string> Post([FromBody]string value)
        {
            BinaryDataContractSerializationAdapter.KnownDataContracts.Add(typeof(Repository));
            var serializer = new BinaryDataContractSerializationAdapter();
            var bytes = Encoding.ASCII.GetBytes(value);
            var repo = await serializer.DeserializeAsync<Repository>(bytes);
            bytes = await serializer.SerializeAsync<Repository>(repo);
            var returnValue = Encoding.ASCII.GetString(bytes);
            return returnValue;
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
    }
}
