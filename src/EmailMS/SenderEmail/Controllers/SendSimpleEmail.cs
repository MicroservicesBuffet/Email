using ConfigureMS;
using EmailConfigurator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SenderEmail.Controllers
{
    public class SimpleSendEmail
    {
        public string from { get; set; }
        public string to { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
    }
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class SendSimpleEmail : ControllerBase
    {
        [HttpPost]
        public Task SendEmail([FromServices]IStartConfigurationMS config, [FromBody]SimpleSendEmail send)
        {
            var client = config.ChoosenProviderData as IEmailSmtpClient;
            return client.Client().SendMailAsync(send.from, send.to, send.subject, send.body);
        }
    }
}
