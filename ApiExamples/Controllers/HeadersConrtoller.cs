using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using RestClientApiSamples;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiExamples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HeadersController : ControllerBase
    {
        [HttpGet]
        public Person GetAsync()
        {
            if (Request.Headers.ContainsKey("Test") && Request.Headers["Test"]=="Test")
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

        //[HttpPost]
        //public async Task<IActionResult> PostAsync()
        //{
        //    var stream = Request.BodyReader.AsStream();
        //    return File(stream, "application/octet-stream");
        //}
    }
}
