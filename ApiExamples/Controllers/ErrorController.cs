using ApiExamples.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiExamples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ErrorController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetAsync()
        {
            var json = JsonConvert.SerializeObject(new ApiResult { Errors = { ApiMessages.ErrorControllerErrorMessage } });
            return BadRequest(json);
        }
    }
}
