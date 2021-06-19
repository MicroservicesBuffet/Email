using ConfigureMS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SenderEmail.General
{
    public class MiddlewareConfigure : IMiddleware
    {
        private readonly IStartConfigurationMS config;
        private readonly IRepoMS data;
        private readonly IWebHostEnvironment webHostEnvironment;

        public MiddlewareConfigure(IStartConfigurationMS config, IRepoMS data, IWebHostEnvironment webHostEnvironment)
        {
            this.config = config;
            this.data = data;
            this.webHostEnvironment = webHostEnvironment;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!config.IsConfigured())
            try
            {
                var pluginsFolder = Path.Combine(webHostEnvironment.WebRootPath, "plugins");
                await foreach (var item in config.StartFinding(pluginsFolder))
                {
                    //ModelState.AddModelError(item.MemberNames.FirstOrDefault() ?? "error", item.ErrorMessage);
                }
                await config.LoadData(data);
            }
            catch
            {
                //do nothing
            }
            if (config.IsConfigured())
            {
                await next(context);
                return;
            }

            if (context.Request.Path.Value?.Contains("StartConfigure") ?? false)
            {
                await next(context);
                return;
            }
            if (context.Request.Path.Value?.EndsWith("css") ?? false)
            {
                await next(context);
                return;
            }
            if (context.Request.Path.Value?.EndsWith("js") ?? false)
            {
                await next(context);
                return;
            }

            //if it is not configured , go to Start Configure
            context.Response.Redirect("/StartConfigure/Index");
            context.Response.StatusCode = StatusCodes.Status302Found;
            //return Task.CompletedTask;
            //return next(context);
        }
    }
}
