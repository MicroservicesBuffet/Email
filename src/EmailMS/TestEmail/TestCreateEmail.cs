using EmailConfigurator;
using EmailSmtpClientGmail;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using System;
using System.Net;
using System.Net.Mail;
using Xunit;

namespace TestEmail
{
    [FeatureDescription(@"Test create email")]
    [Label(nameof(TestCreateEmail))]
    public partial class TestCreateEmail: FeatureFixture
    {
        [Scenario]
        [ScenarioCategory("Gmail")]
        [Trait("RealTest", "1")]
        public async void TestSendEmailGmail()
        {

            await Runner.AddSteps(
Given_The_Password_IsSet_Via_Environment,    
Given_A_GMAIL_SMTP_With_Hidden_Credentials,
    And_Transform_To_Smtp_Regular,
    Then_Send_Email
    )
    .RunAsync();
                
        }




        [Scenario]
        [ScenarioCategory("Smtp4Dev")]
        [Trait("RealTest", "0")]
        public async void TestSendEmailSmtp4Dev()
        {

            
            await Runner.AddSteps(
                Given_A_SimpleEmail_SMTP,
                And_Transform_To_Smtp_Regular, 
                Then_Send_Email
                )
                .RunAsync();
            
        }
    }
}