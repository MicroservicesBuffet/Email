using AOPMethodsCommon;
using ConfigureMS;
using EmailConfigurator;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleSMTP
{
    /// <summary>
    /// TODO:with AOP find the names of the properties to configure
    /// </summary>
    [AutoMethods(template = TemplateMethod.CustomTemplateFile, CustomTemplateFileName = "ClassToDictionary.txt")]
    public partial class EmailSmtpClientMS : IConfigurableMS,  IEmailSmtpClient
    {
        public EmailSmtpClientMS()
        {

            Port = 25;
            //TODO: aopmethods to not use reflection
            Type = this.GetType().Name;

        }
        public string Name { get; set; }


        public string Type { get; private set; }
        public string Host { get; set; }
        public int Port { get; set; }

        public string Description
        {
            get;
            set;
        }


        public virtual SmtpClient Client()
        {
            return new SmtpClient(Host, Port);
        }

        Task IData.Restore(string data)
        {
            var me = JsonSerializer.Deserialize<EmailSmtpClientMS>(data);
            this.Host = me.Host;
            this.Port = me.Port;
            return Task.CompletedTask;
        }

        Task<string> IData.SavedData()
        {
            var data = JsonSerializer.Serialize(this);
            return Task.FromResult(data);
        }

        IDictionary<string, object>  IData.WriteProperties()
        { 
            return MyProperties(); 
        }
        public Task Test(string from)
        {
            return Client().SendMailAsync(from, from, "TestEmail", "Welcome configurable email!");
        }

        void IData.SetProperties(IDictionary<string, object> values)
        {
            this.WriteMyProperties(values);
        }
    }
}
