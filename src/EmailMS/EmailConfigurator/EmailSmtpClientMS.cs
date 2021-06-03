using ConfigureMS;
using System;

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

        

    }
}
