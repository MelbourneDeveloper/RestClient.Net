using Microsoft.AspNetCore.Mvc;
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

                return person;
            }

            throw new Exception("Incorrect header");
        }

        [HttpPost]
        public async Task<Person> PostAsync([FromBody] Person person)
        {
            if (Request.Headers.ContainsKey("Test") && Request.Headers["Test"] == "Test")
            {
                return person;
            }

            throw new Exception("Incorrect header");
        }

        [HttpPut]
        public async Task<Person> PutAsync([FromBody] Person person)
        {
            if (Request.Headers.ContainsKey("Test") && Request.Headers["Test"] == "Test")
            {
                return person;
            }

            throw new Exception("Incorrect header");
        }

    }
}
