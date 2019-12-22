using ApiExamples.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestClientApiSamples;
using System.Threading.Tasks;

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
            if (Request.Headers["Authorization"] != "Basic Qm9iOkFOaWNlUGFzc3dvcmQ=")
            {
                var json = JsonConvert.SerializeObject(new ApiResult { Errors = { NotAuthorizedMessage } });
                return Unauthorized(json);
            }

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

            return Ok(person);
        }

    }
}
