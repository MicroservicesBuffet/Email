using EmailConfigurator;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestEmail
{
    [FeatureDescription(@"Test configuration")]
    [Label(nameof(TestCreateEmail))]
    public partial class TestConfigurable: FeatureFixture
    {
        [Scenario]
        [ScenarioCategory("ConfigureReadSMTP")]
        [Trait("RealTest", "0")]
        public async void TestReadSMTPPlugins()
        {

            await Runner.AddSteps(
                _=> Given_Create_FileSystem_WithPlugins(),
                _ => When_Create_Configurable_EmailSettings(),
                _ => Then_Can_Found_SMTPProviders(),
                _ => And_The_Number_of_SMTPProviders_is(2)
            )
            .RunAsync();


        }
        [Scenario]
        [ScenarioCategory("ConfigureSimpleEmail")]
        [Trait("RealTest", "0")]

        public async void TestConfigureSimpleEmail()
        {

            await Runner.AddSteps(
                _ => Given_Create_FileSystem_WithPlugins(),
                _ => When_Create_Configurable_EmailSettings(),
                _ => Then_Can_Found_SMTPProviders(),
                _ => And_Choose_The_SmtpProvider(ConfigureEmail.smtpProvidersFolder,"simple"),
                _ => Then_Configuration_Is_Complete(true)
            )
            .RunAsync();


        }
        [Scenario]
        [ScenarioCategory("TestSaveAndRestore")]
        [Trait("RealTest", "0")]

        public async void TestSaveAndRestore()
        {

            await Runner.AddSteps(
                _ => Given_Create_FileSystem_WithPlugins(),
                _ => When_Create_Configurable_EmailSettings(),
                _ => Then_Can_Found_SMTPProviders(),
                _ => And_Choose_The_SmtpProvider(ConfigureEmail.smtpProvidersFolder, "simple"),
                _ => Then_Configuration_Is_Complete(true),
                _ => And_Save_Configuration(),
                _ => And_Restore_Configuration(),
                _ => Then_Configuration_Is_Complete(true)
            )
            .RunAsync();


        }


        [Scenario]
        [ScenarioCategory("ConfigureReadSMTP")]
        [Trait("RealTest", "0")]
        public async void TestConfigCompleteAtStart()
        {
            await Runner.AddSteps(
                _ => Given_Create_FileSystem_WithPlugins(),
                _ => When_Create_Configurable_EmailSettings(),
                _ => Then_Configuration_Is_Complete(false)
            )
            .RunAsync();


        }
    }
}