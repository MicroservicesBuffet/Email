using ConfigureMS;
using EmailConfigurator;
using System;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleSMTP
{
    /// <summary>
    /// TODO:with AOP find the names of the properties to configure
    /// </summary>
    public class EmailSmtpClientMS : IConfigurableMS,  IEmailSmtpClient
    {
        public EmailSmtpClientMS()
        {

            Port = 25;
            Host = "localhost";
            //TODO: aopmethods to not use reflection
            Type = this.GetType().Name;

        }
        public string Name { get; set; }


        public string Type { get; init; }
        public string Host { get; set; }
        public int Port { get; set; }

        public string Description
        {
            get
            {
                return $"simple email sender with {Host}:{Port}";
            }
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


        public Task Test(string from)
        {
            return Client().SendMailAsync(from, from, "TestEmail", "Welcome configurable email!");
        }
    }
}
