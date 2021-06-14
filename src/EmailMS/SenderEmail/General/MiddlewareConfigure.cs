using ConfigureMS;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SenderEmail.General
{
    public class MiddlewareConfigure : IMiddleware
    {
        private readonly IStartConfigurationMS config;

        public MiddlewareConfigure(IStartConfigurationMS config)
        {
            this.config = config;
        }
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (config.IsConfigured())
                return next(context);
            
            if (context.Request.Path.Value?.Contains("StartConfigure")??false)
                return next(context);
            if (context.Request.Path.Value?.EndsWith("css") ?? false)
                return next(context);
            if (context.Request.Path.Value?.EndsWith("js") ?? false)
                return next(context);

            //if it is not configured , go to Start Configure
            context.Response.Redirect("/StartConfigure/Index");
            context.Response.StatusCode = StatusCodes.Status302Found;
            return Task.CompletedTask;
            //return next(context);
        }
    }
}
