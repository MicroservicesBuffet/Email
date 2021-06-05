using ConfigureMS;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

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
        public ConfigureEmail()
        {

        }
        public DateTime? ConfiguredAt { get; set; }
        public Task<bool> IsComplete { get; set; }

        public Task<int> ConfigureAgain()
        {
            throw new NotImplementedException();
        }

        public Task<int> StartFinding(string baseDir, RepoMS repoMS)
        {
            throw new NotImplementedException();
        }
    }
}
