using Microsoft.AspNetCore.Mvc;

namespace ApiExamples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ErrorController : ControllerBase
    {
        public const string ExceptionMessage = "Incorrect or missing header";

        [HttpGet]
        public IActionResult GetAsync()
        {
            return BadRequest("Not this time buddy");
        }
    }
}
