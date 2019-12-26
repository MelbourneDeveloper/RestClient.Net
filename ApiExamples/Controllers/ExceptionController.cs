using ApiExamples.Model;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace ApiExamples.Controllers
{
    [ApiController]
    public class ExceptionController : ControllerBase
    {
        [Route("/exception")]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            var json = JsonConvert.SerializeObject(new ApiResult { Errors = { context.Error.Message } });

            if (context.Error is StatusException statusException)
            {
                //Returning BadRequest is different to Problem. Problem returns its own model
                if(statusException.HttpStatusCode == HttpStatusCode.BadRequest)
                {
                    return BadRequest(json);
                }

                return Problem(json, null, (int)statusException.HttpStatusCode);
            }
            else
            {
                return Problem(json, null, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
