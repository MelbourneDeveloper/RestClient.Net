using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using RestClientApiSamples;

namespace ApiExamples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        [HttpGet]
        public byte[] Get()
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

            Person.Parser.

            return person.ToByteArray();
        }
    }
}
