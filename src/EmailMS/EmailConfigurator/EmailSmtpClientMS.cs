using ConfigureMS;
using System;

namespace EmailConfigurator
{
    public abstract class EmailSmtpClientMS: IConfigurableMS
    {
        public string Name { get; set ; }


        public string Type { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

        

    }
}
