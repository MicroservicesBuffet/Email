using Microsoft.Extensions.Configuration;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using System.Text.Json;

[assembly: InternalsVisibleTo("TestEmail")]


namespace EmailConfigurator
{

    public class ConfigureEmail  : ConfigurePlugins<IEmailSmtpClient>
    {
        public ConfigureEmail() : base()
        {
        }
        public ConfigureEmail(IFileSystem fileSystem):base(fileSystem)
        {
            Name = "ConfigureEmail";
            pluginFolder = smtpProvidersFolder;
        }
        public const string smtpProvidersFolder = "smtpProviders";
    }
}
