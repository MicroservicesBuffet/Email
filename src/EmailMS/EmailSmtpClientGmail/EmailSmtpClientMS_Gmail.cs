using AOPMethodsCommon;
using ConfigureMS;
using EmailConfigurator;
using SimpleSMTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;

namespace EmailSmtpClientGmail
{
    [AutoMethods(template = TemplateMethod.CustomTemplateFile, CustomTemplateFileName = "ClassToDictionary.txt")]

    public partial class EmailSmtpClientMS_Gmail : IConfigurableMS, IEmailSmtpClient
    {
        public EmailSmtpClientMS_Gmail()
        {
            this.Host = "smtp.gmail.com";
            this.Port = 587;

        }
        public string Host { get; set; }
        public int Port { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }


        public SmtpClient Client()
        {
            var c = new SmtpClient(Host, Port);
            c.Credentials = new NetworkCredential(UserName, Password);
            c.EnableSsl = true;
            return c;
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

        Task<int> ISaveAndLoadData.SaveData(IRepoMS repo)
        {
            return repo.SaveData(this);
        }
        public string Name { get; set; }
        public string From { get; set; }

        async Task<int> ISaveAndLoadData.LoadData(IRepoMS repo)
        {
            var ms = await repo.GetItem<EmailSmtpClientMS_Gmail>();
            this.Host = ms.Host;
            this.Port = ms.Port;
            this.Name = ms.Name;
            this.From = ms.From;
            this.UserName = ms.UserName;
            this.Password = ms.Password;
            return 0;
        }
        public string Type
        {
            get
            {
                //TODO: aopmethods to not use reflection
                return this.GetType().Name;
            }
        }
        public string Description
        {
            get
            {
                return $"{Type} {Host}:{Port} {UserName}";
            }
        }
    }



    //string IData.SavedData
    //{
    //    get
    //    {
    //        var data = JsonSerializer.Serialize(this);
    //        return data;
    //    }
    //    set
    //    {
    //        var me = JsonSerializer.Deserialize<EmailSmtpClientMS>(value);
    //        this.Host = me.Host;
    //        this.Port = me.Port;
    //    }
    //}


    //HashSet<string> IData.Properties()
    //{
    //    throw new NotImplementedException();
    //}

}