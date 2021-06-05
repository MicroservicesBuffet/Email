using ConfigureMS;
using System;
using System.Net.Mail;

namespace EmailConfigurator
{
    public class EmailSmtpClientMS: IConfigurableMS
    {
        public EmailSmtpClientMS()
        {
            Port = 25;
            Host = "localhost";
            Type=this.GetType().Name; 
        }
        public string Name { get; set ; }


        public string Type { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

        public virtual SmtpClient Client()
        {
            return new SmtpClient(Host, Port);
        }

    }
}
