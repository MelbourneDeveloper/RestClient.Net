using ApiExamples.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using RestClientApiSamples;
using System;
using System.Threading.Tasks;

namespace ApiExamples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HeadersController : ControllerBase
    {
        [HttpGet]
        public async Task<Person> GetAsync()
        {
            if (Request.Headers.ContainsKey("Test") && Request.Headers["Test"] == "Test")
            {

                var person = new Person
                {
                    FirstName = "Sam",
                    BillingAddress = new Address
                    {
                        StreeNumber = "100",
                        Street = "Somewhere",
                        Suburb = "Sometown"
                    },
                    Surname = "Smith"
                };

                Response.Headers.Add("Test1", "a");
                Response.Headers.Add("Test2", new StringValues(new string[] { "a", "b" }));

                return person;
            }

            throw new Exception(ApiMessages.HeadersControllerExceptionMessage);
        }

        [HttpPost]
        public async Task<Person> PostAsync([FromBody] Person person)
        {
            if (Request.Headers.ContainsKey("Test") && Request.Headers["Test"] == "Test")
            {
                return person;
            }

            throw new Exception(ApiMessages.HeadersControllerExceptionMessage);
        }

        [HttpPut]
        public async Task<Person> PutAsync([FromBody] Person person)
        {
            if (Request.Headers.ContainsKey("Test") && Request.Headers["Test"] == "Test")
            {
                return person;
            }

            throw new Exception(ApiMessages.HeadersControllerExceptionMessage);
        }

        [HttpPatch]
        public async Task<Person> PatchAsync([FromBody] Person person)
        {
            if (Request.Headers.ContainsKey("Test") && Request.Headers["Test"] == "Test")
            {
                return person;
            }

            throw new Exception(ApiMessages.HeadersControllerExceptionMessage);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new Exception("No id");

            if (Request.Headers.ContainsKey("Test") && Request.Headers["Test"] == "Test")
            {
                return;
            }

            throw new StatusException(ApiMessages.HeadersControllerExceptionMessage, System.Net.HttpStatusCode.BadRequest);
        }
    }
}
