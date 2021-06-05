using ConfigureMS;
using EmailConfigurator;
using System;
using System.Net;
using System.Net.Mail;

namespace EmailSmtpClientGmail
{
    public class EmailSmtpClientMS_Gmail: EmailSmtpClientMS
    {
        public EmailSmtpClientMS_Gmail()
        {
            this.Host = "smtp.gmail.com";
            this.Port = 587;
            
        }
        public string UserName { get; set; }
        public string Password { get; set; }

        public override SmtpClient Client()
        {
            var c = base.Client();
            c.Credentials = new NetworkCredential(UserName, Password);
            c.EnableSsl = true;
            return c;


        }
    }
}
