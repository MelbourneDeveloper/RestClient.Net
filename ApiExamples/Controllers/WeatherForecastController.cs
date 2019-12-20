using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiExamples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        [HttpGet]
        public byte[] Get()
        {
            var person = new DBTogRPC.Person();

            using (var s = new MemoryStream())
            {

                using (var stream = new CodedOutputStream(s))
                {
                    // Save the person to a stream
                    person.WriteTo(stream);
                    return s.ToArray();
                }
            }
        }
    }
}
