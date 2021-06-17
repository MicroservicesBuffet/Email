using ConfigureMS;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Text.Json;
using McMaster.NETCore.Plugins;

[assembly:InternalsVisibleTo("TestEmail")]


namespace EmailConfigurator
{

    
    public class ConfigureEmail : IStartConfigurationMS, ISaveAndLoadData
    {
        public ConfigureEmail()
        {
            this.fileSystem = new FileSystem();
            this.Name = "constructed";
        }
        public ConfigureEmail(IFileSystem fileSystem )
        {
            this.fileSystem = fileSystem; 
            this.Name = "ConfigureEmail";
        }
        public DateTime? ConfiguredAt { get; set; }
        public Task<bool> IsComplete()
        {
            foreach (var item in Validate(null))
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);

        }
        public Task<int> ConfigureAgain()
        {
            throw new NotImplementedException();
        }
        public const string smtpProvidersFolder = "smtpProviders";
        private IFileSystem fileSystem;

        public string Name { get; }

        public async IAsyncEnumerable<ValidationResult> StartFinding(string baseDir)
        {
            await Task.Delay(1000);
            //TODO: make this configurable  - load the path from a database instead of folders
            var emailProviderPath = fileSystem.Path.Combine(baseDir, smtpProvidersFolder);
            if (!fileSystem.Directory.Exists(emailProviderPath))
            {
                yield return new ValidationResult($"folder {emailProviderPath} for smtp providers does not exists",new[]{
                "path"                    
                });
                yield break;
            }
            MainProviders = fileSystem.Directory
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
            if (ChoosenMainProvider == null)
            {
                yield return new ValidationResult($"please call {nameof(ChooseConfiguration)}", new[] { nameof(ConfiguredAt) });
            }
        }
        
        public void ChooseConfiguration(string name, string value)
        {
            if (!string.Equals(name, smtpProvidersFolder, StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException($"you can configure just {smtpProvidersFolder}");
            switch(MainProviders?.Length)
            {
                case null:
                    throw new ArgumentException($"please call {nameof(StartFinding)}");
                case 0:
                    throw new ArgumentException($"did you have plugins in the folder called in {nameof(StartFinding)}");
                default:
                    value = value?.Trim();
                    ChoosenMainProvider = MainProviders.FirstOrDefault(it => string.Equals(it, value,StringComparison.CurrentCultureIgnoreCase));
                    if (ChoosenMainProvider == null) 
                        throw new ArgumentException($"value {value} must be one of {string.Join(",",MainProviders)}",nameof(value));
                    
                    ConfiguredAt = DateTime.UtcNow;
                    break;
            }
            return ;
        }

        public  async Task<int> SaveData(IRepoMS repo)
        {
            var c = await IsComplete();
            if (!c)
                throw new ValidationException($" should be valid , please use {nameof(ChooseConfiguration)}");

            return await repo.SaveData<ConfigureEmail>((dynamic)this);
            //var data = JsonSerializer.Serialize(this);
            //var name = this.GetType().Name;
            //var fullName = fileSystem.Path.Combine(BaseFolder, name);
            //await fileSystem.File.WriteAllTextAsync(fullName, data);
            
            //return data.Length;
        
        }
        public Task<int> LoadConfiguration()
        {
            var folder = fileSystem.Path.Combine(BaseFolder, smtpProvidersFolder, ChoosenMainProvider);
            var nameDll = fileSystem.Path.Combine(folder, $"{ChoosenMainProvider}.dll");
            if (!fileSystem.File.Exists(nameDll))
                throw new ArgumentException($"dll {nameDll} does not exists");
            
             var loader = PluginLoader.CreateFromAssemblyFile(
    assemblyFile: nameDll,
    sharedTypes: new[] { typeof(IEmailSmtpClient) },
    isUnloadable: true);
            var typeLoaded = loader
                .LoadDefaultAssembly()
                .GetTypes()
                 .Where(t => typeof(IEmailSmtpClient).IsAssignableFrom(t) && !t.IsAbstract)
                 .FirstOrDefault();
            if (typeLoaded == null)
                throw new ArgumentException($"cannot find {nameof(IEmailSmtpClient)} in {nameDll}");

            ChoosenProviderData = (IEmailSmtpClient)Activator.CreateInstance(typeLoaded);
            return Task.FromResult(1);
        }

        public async Task<int> LoadData(IRepoMS repo)
        {
            var me = await repo.GetItem<ConfigureEmail>();
            me.fileSystem = this.fileSystem;
            //todo: do not use reflection
            //var name = this.GetType().Name;
            //var fullName = fileSystem.Path.Combine(BaseFolder, name);
            //var data= await fileSystem.File.ReadAllTextAsync(fullName);
            //var me = JsonSerializer.Deserialize<ConfigureEmail>(data);
            this.BaseFolder = me.BaseFolder;
            await foreach (var item in this.StartFinding(me.BaseFolder))
            {
                throw new ArgumentException(item.ErrorMessage, item.MemberNames?.FirstOrDefault());
            }
            this.ChooseConfiguration(smtpProvidersFolder, ChoosenMainProvider);
            return 1;

        }

        public string BaseFolder { get; set; }
        public string ChoosenMainProvider { get; set; }
        public string[] MainProviders { get; set; }
        
        public IData ChoosenProviderData { get; set; }

        //public IEmailSmtpClient ChoosenSMTPClient;
    }
}
