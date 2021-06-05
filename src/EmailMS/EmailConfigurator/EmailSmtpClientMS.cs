using ConfigureMS;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EmailConfigurator
{
    public class EmailSmtpClientMS: IConfigurableMS, SaveAndLoadData
    {
        private readonly RepoMS repoMS;

        public EmailSmtpClientMS()
        {
            
            Port = 25;
            Host = "localhost";
            //TODO: aopmethods to not use reflection
            Type=this.GetType().Name;
            
        }
        public string Name { get; set ; }


        public string Type { get; init; }
        public string Host { get; set; }
        public int Port { get; set; }

        public virtual SmtpClient Client()
        {
            return new SmtpClient(Host, Port);
        }

        public async Task<int> LoadData(RepoMS repo)
        {

            var data = await repoMS.GetItem<EmailSmtpClientMS>(this.Type);

            this.Host = data.Host;
            this.Port = data.Port;
            return 1;
        }

        public Task<int> SaveData(RepoMS repo)
        {
            return repoMS.SaveData<EmailSmtpClientMS>(this);
        }
    }
}
