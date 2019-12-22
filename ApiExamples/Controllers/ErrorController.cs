using ApiExamples.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiExamples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ErrorController : ControllerBase
    {
        public const string ErrorMessage = "Not this time buddy";

        [HttpGet]
        public IActionResult GetAsync()
        {
            var json = JsonConvert.SerializeObject(new ApiResult { Errors = { ErrorMessage } });
            return BadRequest(json);
        }
    }
}
