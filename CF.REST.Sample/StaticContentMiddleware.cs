using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CF.REST.Sample
{
    public class StaticContentMiddleware
    {
        #region Fields
        private readonly IHostingEnvironment _hostingEnvironment;
        #endregion

        #region Constructor
        public StaticContentMiddleware(RequestDelegate next, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion

        #region Public Methods
        public async Task Invoke(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Request == null) throw new ArgumentNullException(nameof(context.Request));

            // we skip the first slash and we reverse the slashes
            var baseUrl = context.Request.Path.Value.Substring(1).Replace("/", "\\");

            var destinationFile = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", baseUrl);

            var text = File.ReadAllText(destinationFile);

            await context.Response.WriteAsync(text);
        }
        #endregion
    }
}
