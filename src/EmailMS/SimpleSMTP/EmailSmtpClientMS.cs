﻿using ConfigureMS;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SimpleSMTP
{
    public class EmailSmtpClientMS : IConfigurableMS, SaveAndLoadData
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

        public virtual async Task<int> LoadData(RepoMS repo)
        {

            var data = await repo.GetItem<EmailSmtpClientMS>();

            this.Host = data.Host;
            this.Port = data.Port;
            return 1;
        }

        public virtual Task<int> SaveData(RepoMS repo)
        {
            return repo.SaveData<EmailSmtpClientMS>(this);
        }

        Task<int> SaveAndLoadData.LoadData(RepoMS repo)
        {
            throw new NotImplementedException();
        }

        
    }
}
