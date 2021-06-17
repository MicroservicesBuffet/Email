using ConfigureMS;
using EmailConfigurator;
using SimpleSMTP;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;

namespace EmailSmtpClientGmail
{
    public class EmailSmtpClientMS_Gmail : EmailSmtpClientMS,IConfigurableMS , IEmailSmtpClient
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


        string IData.SavedData
        {
            get
            {
                var data = JsonSerializer.Serialize(this);
                return data;
            }
            set
            {
                var me = JsonSerializer.Deserialize<EmailSmtpClientMS>(value);
                this.Host = me.Host;
                this.Port = me.Port;
            }
        }

    
    //HashSet<string> IData.Properties()
    //{
    //    throw new NotImplementedException();
    //}

}
}
