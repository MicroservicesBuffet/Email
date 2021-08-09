﻿using ConfigureMS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using McMaster.NETCore.Plugins;

[assembly: InternalsVisibleTo("TestEmail")]


namespace EmailConfigurator
{
    public class ConfigurePlugins<T> : IStartConfigurationMS, ISaveAndLoadData, IDisposable
        where T: IData
    {
        protected Type GenericType;
        public ConfigurePlugins():this(new FileSystem())
        {
        }
        public ConfigurePlugins(IFileSystem fileSystem )
        {
            this.fileSystem = fileSystem; 
            GenericType = typeof(T);
        }
        public DateTime? ConfiguredAt { get; set; }
        //public Task<bool> IsComplete()
        //{
        //    foreach (var item in Validate(null))
        //    {
        //        return Task.FromResult(false);
        //    }
        //    return Task.FromResult(true);

        //}
        public Task<int> ConfigureAgain()
        {
            throw new NotImplementedException();
        }
        public string pluginFolder { get; init; }
        private IFileSystem fileSystem;

        public string Name { get; protected set; }

        public async IAsyncEnumerable<ValidationResult> StartFinding(string baseDir)
        {
            await Task.Delay(1000);
            //TODO: make this configurable  - load the path from a database instead of folders
            var emailProviderPath = fileSystem.Path.Combine(baseDir, pluginFolder);
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
                yield return new ValidationResult($"please call {nameof(ChooseConfiguration)}", new[] { nameof(ChoosenMainProvider) });
            }
        }
        
        public async Task<int> ChooseConfiguration(string name, string value)
        {
            if (!string.Equals(name, pluginFolder, StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException($"you can configure just {pluginFolder}");
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
            return await LoadConfiguration();
        }

        public  async Task<int> SaveData(IRepoMS repo)
        {
            var c = Validate(null).ToArray();
            if (c.Length >0)
                throw new ValidationException(c[0].ErrorMessage);

            await repo.SaveData<ConfigurePlugins<T>>(this);
            await this.ChoosenProviderData.SaveData(repo);
            //await repo.SaveData<IEmailSmtpClient>(this.ChoosenProviderData as IEmailSmtpClient);
            //var data = JsonSerializer.Serialize(this);
            //var name = this.GetType().Name;
            //var fullName = fileSystem.Path.Combine(BaseFolder, name);
            //await fileSystem.File.WriteAllTextAsync(fullName, data);
            return 0;
            //return data.Length;
        
        }
        private PluginLoader loader;
        private Task<int> LoadConfiguration()
        {
            var folder = fileSystem.Path.Combine(BaseFolder, pluginFolder, ChoosenMainProvider);
            var nameDll = fileSystem.Path.Combine(folder, $"{ChoosenMainProvider}.dll");
            if (!fileSystem.File.Exists(nameDll))
                throw new ArgumentException($"dll {nameDll} does not exists");
             
            loader?.Dispose();
            loader = PluginLoader.CreateFromAssemblyFile(
    assemblyFile: nameDll,
    sharedTypes: new[] { GenericType },
    isUnloadable: true);
            var typeLoaded = loader
                .LoadDefaultAssembly()
                .GetTypes()
                 .Where(t => GenericType.IsAssignableFrom(t) && !t.IsAbstract)
                 .FirstOrDefault();
            if (typeLoaded == null)
                throw new ArgumentException($"cannot find {GenericType.Name} in {nameDll}");

            ChoosenProviderData = (IData) Activator.CreateInstance(typeLoaded) ;
            return Task.FromResult(1);
        }

        public async Task<int> LoadData(IRepoMS repo)
        {
            var me = await repo.GetItem<ConfigurePlugins<T>>();
            me.fileSystem = this.fileSystem;
            //todo: do not use reflection
            //var name = this.GetType().Name;
            //var fullName = fileSystem.Path.Combine(BaseFolder, name);
            //var data= await fileSystem.File.ReadAllTextAsync(fullName);
            //var me = JsonSerializer.Deserialize<ConfigureEmail>(data);
            this.BaseFolder = me.BaseFolder;
            this.ChoosenMainProvider = me.ChoosenMainProvider;
            await foreach (var item in this.StartFinding(me.BaseFolder))
            {
                throw new ArgumentException(item.ErrorMessage, item.MemberNames?.FirstOrDefault());
            }
            await this.ChooseConfiguration(pluginFolder, ChoosenMainProvider);
            await this.ChoosenProviderData.LoadData(repo);
            return 1;

        }

        public void Dispose()
        {
            loader?.Dispose();
        }

        public string BaseFolder { get; set; }
        public string ChoosenMainProvider { get; set; }
        public string[] MainProviders { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public IData ChoosenProviderData { get; set; }

        
        //public IEmailSmtpClient ChoosenSMTPClient;
    }
}
