using ApiExamples.Model;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiExamples.Controllers
{
    [ApiController]
    public class ExceptionController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            var json = JsonConvert.SerializeObject(new ApiResult { Errors = { context.Error.Message } });
            return BadRequest(json);


            //return Problem(
            //    detail: context.Error.Message,
            //    title: context.Error.Message);
        }
    }
}
