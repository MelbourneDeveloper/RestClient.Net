using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using RestClientApiSamples;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiExamples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
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

            var data = person.ToByteArray();

            return File(data, "application/octet-stream");
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var stream = Request.BodyReader.AsStream();
            return File(stream, "application/octet-stream");
        }
    }
}
