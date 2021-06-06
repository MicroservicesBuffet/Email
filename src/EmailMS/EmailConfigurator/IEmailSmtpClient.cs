using System.Net.Mail;

namespace EmailConfigurator
{
    public interface IEmailSmtpClient
    {
        SmtpClient Client();
        void Test(string to);
    }
}