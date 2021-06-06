using ConfigureMS;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("TestEmail")]


namespace EmailConfigurator
{

    
    public class RepoFromAppSettings: RepoMS
    {
        private readonly IConfiguration c;
        private readonly string name;

        public RepoFromAppSettings(IConfiguration c,string nameRoot)
        {
            this.c = c;
            this.name = nameRoot;
        }

        public Task<T[]> GetAllData<T>()
        {
            return null;
        }

        public Task<T> GetItem<T>(string id)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveData<T>(T t)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveData<T>(T[] t)
        {
            throw new NotImplementedException();
        }
    }
    public class ConfigureEmail : StartConfigurationMS
    {
        public ConfigureEmail(IFileSystem fileSystem )
        {
            this.fileSystem = fileSystem;
        }
        public DateTime? ConfiguredAt { get; set; }
        public Task<bool> IsComplete { get; set; }

        public Task<int> ConfigureAgain()
        {
            throw new NotImplementedException();
        }
        internal const string smptProvidersFolder = "smtpProviders";
        private readonly IFileSystem fileSystem;

        public async IAsyncEnumerable<ValidationResult> StartFinding(string baseDir, RepoMS repoMS)
        {
            await Task.Delay(1000);
            //TODO: make this configurable  - load the path from a database instead of folders
            var emailProviderPath = fileSystem.Path.Combine(baseDir, smptProvidersFolder);
            if (!fileSystem.Directory.Exists(emailProviderPath))
            {
                yield return new ValidationResult($"folder {emailProviderPath} for smtp providers does not exists");
                yield break;
            }
            EmailSmtp = fileSystem.Directory.GetDirectories(emailProviderPath).ToArray();
            yield break;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(ConfiguredAt is null)
            {
                yield return new ValidationResult("not configured", new[] { nameof(ConfiguredAt) });
            }
        }

        public string[] EmailSmtp { get; set; } 
    }
}
