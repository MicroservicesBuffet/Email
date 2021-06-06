using System.Net.Mail;

namespace SimpleSMTP
{
    public interface IEmailSmtpClient
    {
        string Description { get; }

        SmtpClient Client();
    }
}