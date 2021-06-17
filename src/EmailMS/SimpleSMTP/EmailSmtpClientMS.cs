using AOPMethodsCommon;
using ConfigureMS;
using EmailConfigurator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleSMTP
{
    /// <summary>
    /// TODO:with AOP find the names of the properties to configure
    /// </summary>
    [AutoMethods(template = TemplateMethod.CustomTemplateFile, CustomTemplateFileName = "ClassToDictionary.txt")]
    public partial class EmailSmtpClientMS : IConfigurableMS, IEmailSmtpClient
    {
        public EmailSmtpClientMS()
        {

            Port = 25;

        }
        public string Name { get; set; }


        public string Type
        {
            get
            {
                //TODO: aopmethods to not use reflection
                return this.GetType().Name;
            }
        }
        public string Host { get; set; }
        public int Port { get; set; }

        public string Description
        {
            get
            {
                return $"{Type} {Host}:{Port}";
            }
        }
        public string From { get; set; }

        public virtual SmtpClient Client()
        {
            return new SmtpClient(Host, Port);
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

        IDictionary<string, object> IData.WriteProperties()
        {
            var arr = MyProperties()
                .Where(it => it.Value.CanWrite)
                .Select(it => new KeyValuePair<string, object>(it.Key, it.Value.Value))
                .ToArray();
            var dict = new Dictionary<string, object>(arr);
            return dict;
        }

        void IData.SetProperties(IDictionary<string, object> values)
        {
            this.WriteMyProperties(values);
        }

        IDictionary<string, object> IData.ReadProperties()
        {
            var arr = MyProperties()
                .Where(it => it.Value.CanRead)
                .Select(it => new KeyValuePair<string, object>(it.Key, it.Value.Value))
                .ToArray();
            var dict = new Dictionary<string, object>(arr);
            return dict;
        }

        Task IData.Test()
        {
            return Client().SendMailAsync(From, From, "TestEmail", "Welcome configurable email!");
        }
    }

}
