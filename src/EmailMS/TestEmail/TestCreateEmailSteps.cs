using ConfigureMS;
using EmailSmtpClientGmail;
using LightBDD.Framework;
using SimpleSMTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestEmail
{
    public partial class TestCreateEmail
    {
        EmailSmtpClientMS ms;
        SmtpClient client;
        string pwd;
        private void Given_A_SimpleEmail_SMTP()
        {
            StepExecution.Current.Comment("requirement :dotnet tool restore + start dotnet smpt4dev ");
            ms = new EmailSmtpClientMS();            
        }
        private void Given_The_Password_IsSet_Via_Environment()
        {
            StepExecution.Current.Comment("set in powershell with $env:PWDGMAIL= \"MyPassword\"");
            pwd = Environment.GetEnvironmentVariable("PWDGMAIL");
            if (string.IsNullOrWhiteSpace(pwd))
            {
                throw new ArgumentException("please set the environment variable PWDGMAIL ;start  powershell and $env:PWDGMAIL= \"MyPassword\"");
            }

        }
        private void Given_A_GMAIL_SMTP_With_Hidden_Credentials()
        {
            StepExecution.Current.Comment("requirement :if network security, goto https://myaccount.google.com/security, find lesser");
            var ms1 = new EmailSmtpClientMS_Gmail();
            ms1.UserName = "ignat.andrei";
            ms1.Password = pwd;
            ms = ms1;                     
        }
        private void And_Setting_The_Host_To(string host)
        {
            ms.Host = host;
        }
        private void And_Setting_The_Host_Property_To(string host)
        {
            IData data = ms as IData;
            data.SetProperties(new Dictionary<string, object>()
            {
                {"Host",host }
            });
        }
        private void And_Transform_To_Smtp_Regular()
        {
            client = ms.Client();
        }
        private void Then_Send_Email()
        {
            client.Send("ignat.andrei@gmail.com", "ignatandrei@yahoo.com", "test", "testbody");
        }
        private void Then_Send_Email_Will_Have_Error()
        {
            try
            {
                client.Send("ignat.andrei@gmail.com", "ignatandrei@yahoo.com", "test", "testbody");

            }
            catch (Exception ex)
            {
                Assert.True(true);
                return;
            }
            Assert.True(false, "it should give an error");
        }
    }
}
