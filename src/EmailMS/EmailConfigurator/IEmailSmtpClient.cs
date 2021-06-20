using ConfigureMS;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EmailConfigurator
{
    public interface IEmailSmtpClient: IData
    {
        public string Host { get; set; }
        SmtpClient Client();   
    }
}