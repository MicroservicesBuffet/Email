using ConfigureMS;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EmailConfigurator
{
    public interface IEmailSmtpClient: IData
    {
        SmtpClient Client();
        Task Test(string from);
    }
}