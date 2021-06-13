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
        private readonly StartConfigurationMS config;

        public MiddlewareConfigure(StartConfigurationMS config)
        {
            this.config = config;
        }
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (config.IsConfigured())
                return next(context);
            //if it is not configured , go to Start Configure
            context.Request.Path = "/StartConfigure";
            return next(context);
        }
    }
}
