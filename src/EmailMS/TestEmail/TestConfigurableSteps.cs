using EmailConfigurator;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestEmail
{
    public partial class TestConfigurable
    {
        IFileSystem fileSystem;
        ConfigureEmail configure;
        private void Given_Create_FileSystem_WithPlugins()
        {
            
            fileSystem = new MockFileSystem();
            fileSystem.Directory.CreateDirectory(@"C:\plugins");
            fileSystem.Directory.CreateDirectory(@$"C:\plugins\{ConfigureEmail.smptProvidersFolder}\gmail");
            fileSystem.Directory.CreateDirectory(@$"C:\plugins\{ConfigureEmail.smptProvidersFolder}\simple");


        }
        private void When_Create_Configurable_EmailSettings()
        {
            configure = new ConfigureEmail(fileSystem);
        }
        private async void Then_Can_Found_SMTPProviders()
        {
            var nr = 0;
            await foreach(var item in configure.StartFinding(@"C:\plugins", null))
            {
                nr++;

            }
            var expected = 0;
            Assert.Equal(expected, nr);//"no errors expected");
        }

        private void And_The_Number_of_SMTPProviders_is(int nr)
        {
            Assert.Equal(nr, configure.EmailSmtp?.Length);//"no errors expected");
        }
    }
}
