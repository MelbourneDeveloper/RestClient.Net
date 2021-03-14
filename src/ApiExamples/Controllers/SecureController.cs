using ApiExamples.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestClientApiSamples;

#pragma warning disable CA1062

namespace ApiExamples.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class SecureController : ControllerBase
    {

        [HttpGet]
        [Route("basic")]
        public IActionResult Get()
        {
            if (ValidateBasic())
            {
                var person = CreatePerson();
                return Ok(person);
            }

            var json = JsonConvert.SerializeObject(new ApiResult { Errors = { ApiMessages.SecureControllerNotAuthorizedMessage } });
            return Unauthorized(json);
        }

        [HttpPost]
        [Route("basic")]
        public IActionResult Post([FromBody] Person person)
        {
            if (ValidateBasic())
            {
                return Ok(person);
            }

            var json = JsonConvert.SerializeObject(new ApiResult { Errors = { ApiMessages.SecureControllerNotAuthorizedMessage } });
            return Unauthorized(json);
        }

        private bool ValidateBasic() => Request.Headers["Authorization"] == "Basic Qm9iOkFOaWNlUGFzc3dvcmQ=";

        private bool ValidateBearer() => Request.Headers["Authorization"] == "Bearer 123";

        private static Person CreatePerson() => new Person
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

        [HttpPost]
        [Route("authenticate")]
        public IActionResult Post([FromBody] AuthenticationRequest request)
        {
            if (request.ClientId == "a" && request.ClientSecret == "b")
            {
                return Ok(new AuthenticationResult { BearerToken = "123" });
            }

            var json = JsonConvert.SerializeObject(new ApiResult { Errors = { ApiMessages.SecureControllerNotAuthorizedMessage } });
            return Unauthorized(json);
        }

        [HttpGet]
        [Route("bearer")]
        public IActionResult GetBearer()
        {
            if (ValidateBearer())
            {
                return Ok(new Person { FirstName = "Bear" });
            }

            var json = JsonConvert.SerializeObject(new ApiResult { Errors = { ApiMessages.SecureControllerNotAuthorizedMessage } });
            return Unauthorized(json);
        }
    }
}
