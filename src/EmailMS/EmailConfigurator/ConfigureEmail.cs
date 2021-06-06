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
using System.Text.Json;

[assembly:InternalsVisibleTo("TestEmail")]


namespace EmailConfigurator
{

    
    public class ConfigureEmail : StartConfigurationMS, SaveAndLoadData
    {
        public ConfigureEmail(IFileSystem fileSystem )
        {
            this.fileSystem = fileSystem;
        }
        public DateTime? ConfiguredAt { get; set; }
        public Task<bool> IsComplete
        {
            get
            {                
                foreach (var item in Validate(null))
                {
                    return Task.FromResult(false);
                }
                return Task.FromResult(true);
            }
        }
        public Task<int> ConfigureAgain()
        {
            throw new NotImplementedException();
        }
        public const string smtpProvidersFolder = "smtpProviders";
        private readonly IFileSystem fileSystem;

        public async IAsyncEnumerable<ValidationResult> StartFinding(string baseDir)
        {
            await Task.Delay(1000);
            //TODO: make this configurable  - load the path from a database instead of folders
            var emailProviderPath = fileSystem.Path.Combine(baseDir, smtpProvidersFolder);
            if (!fileSystem.Directory.Exists(emailProviderPath))
            {
                yield return new ValidationResult($"folder {emailProviderPath} for smtp providers does not exists");
                yield break;
            }
            EmailSmtp = fileSystem.Directory
                .GetDirectories(emailProviderPath)
                .ToArray()
                .Select(it => it.Substring(emailProviderPath.Length+1))
                .Select(it=>it?.Trim())
                .ToArray();

            BaseFolder = baseDir;
            yield break;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(ConfiguredAt is null)
            {
                yield return new ValidationResult("not configured", new[] { nameof(ConfiguredAt) });
            }
            if (ChoosenSmtp == null)
            {
                yield return new ValidationResult($"please call {nameof(ChooseConfiguration)}", new[] { nameof(ConfiguredAt) });
            }
        }
        
        public void ChooseConfiguration(string name, string value)
        {
            if (!string.Equals(name, smtpProvidersFolder, StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException($"you can configure just {smtpProvidersFolder}");
            switch(EmailSmtp?.Length)
            {
                case null:
                    throw new ArgumentException($"please call {nameof(StartFinding)}");
                case 0:
                    throw new ArgumentException($"did you have plugins in the folder called in {nameof(StartFinding)}");
                default:
                    value = value?.Trim();
                    ChoosenSmtp = EmailSmtp.FirstOrDefault(it => string.Equals(it, value,StringComparison.CurrentCultureIgnoreCase));
                    if (ChoosenSmtp == null) 
                        throw new ArgumentException($"value {value} must be one of {string.Join(",",EmailSmtp)}",nameof(value));
                    
                    ConfiguredAt = DateTime.UtcNow;
                    break;
            }
            return ;
        }

        public async Task<int> SaveData(RepoMS repo)
        {
            var c = await IsComplete;
            if (!c)
                throw new ValidationException($" should be valid , please use {nameof(ChooseConfiguration)}");
            
            var data = JsonSerializer.Serialize(this);
            //todo: do not use reflection
            var name = this.GetType().Name;
            var fullName = fileSystem.Path.Combine(BaseFolder, name);
            await fileSystem.File.WriteAllTextAsync(fullName, data);
            
            return data.Length;
        
        }

        public async Task<int> LoadData(RepoMS repo)
        {
            //todo: do not use reflection
            var name = this.GetType().Name;
            var fullName = fileSystem.Path.Combine(BaseFolder, name);
            var data= await fileSystem.File.ReadAllTextAsync(fullName);
            var me = JsonSerializer.Deserialize<ConfigureEmail>(data);
            await foreach(var item in this.StartFinding(me.BaseFolder))
            {
                throw new ArgumentException(item.ErrorMessage, item.MemberNames?.FirstOrDefault());
            }
            this.ChooseConfiguration(smtpProvidersFolder, me.ChoosenSmtp);
            return fullName.Length;

        }

        public string BaseFolder { get; private set; }
        public string ChoosenSmtp { get; private set; }
        public string[] EmailSmtp { get; set; } 
    }
}
