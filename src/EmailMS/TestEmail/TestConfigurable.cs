using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
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
        public async void TestReadSMTP()
        {

            await Runner.AddSteps(
                Given_Create_FileSystem_WithPlugins,
                When_Create_Configurable_EmailSettings,
                Then_Can_Found_SMTPProviders
            )
            .AddStep(nameof(And_The_Number_of_SMTPProviders_is), _ => And_The_Number_of_SMTPProviders_is(2))
            .RunAsync();


        }
    }
}