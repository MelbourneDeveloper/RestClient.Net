using ApiExamples.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestClientApiSamples;

namespace ApiExamples.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class SecureController : ControllerBase
    {
        public const string NotAuthorizedMessage = "Not authorized";

        [HttpGet]
        [Route("basic")]
        public IActionResult Get()
        {
            if (Validate())
            {
                var person = CreatePerson();
                return Ok(person);
            }

            var json = JsonConvert.SerializeObject(new ApiResult { Errors = { NotAuthorizedMessage } });
            return Unauthorized(json);
        }

        [HttpPost]
        [Route("basic")]
        public IActionResult Post([FromBody] Person person)
        {
            if (Validate())
            {
                return Ok(person);
            }

            var json = JsonConvert.SerializeObject(new ApiResult { Errors = { NotAuthorizedMessage } });
            return Unauthorized(json);
        }

        private bool Validate()
        {
            return Request.Headers["Authorization"] == "Basic Qm9iOkFOaWNlUGFzc3dvcmQ=";
        }

        private static Person CreatePerson()
        {
            return new Person
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
        }
    }
}
