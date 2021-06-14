﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ConfigureMS
{
    public class SHIM_StartConfigurationMS : IStartConfigurationMS
    {
        //TODO: use RSCG
        public SHIM_StartConfigurationMS(IStartConfigurationMS configure)
        {
            this.Name = configure.Name;
            this.ConfiguredAt = configure.ConfiguredAt;
            this.ChoosenMainProvider = configure.ChoosenMainProvider;
            this.MainProviders = configure.MainProviders;
        }
        public bool IsConfigured() => ConfiguredAt != null;

        public string Name { get; set; }

        public DateTime? ConfiguredAt { get; set; }

        public string ChoosenMainProvider { get; set; }

        public Task<bool> IsComplete { get; set; }

        public string[] MainProviders { get; set; }

        public void ChooseConfiguration(string name, string value)
        {
            throw new NotImplementedException();
        }

        public Task<int> ConfigureAgain()
        {
            throw new NotImplementedException();
        }

        public Task<int> LoadConfiguration()
        {
            throw new NotImplementedException();
        }

        public Task<int> LoadData(IRepoMS repo)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveData(IRepoMS repo)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<ValidationResult> StartFinding(string baseDir)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
    public interface IStartConfigurationMS:ISaveAndLoadData, IValidatableObject        
    {
        string Name { get;}
        bool IsConfigured()=> ConfiguredAt != null;

        DateTime? ConfiguredAt { get; set; }
        public string ChoosenMainProvider { get; }
        IAsyncEnumerable<ValidationResult> StartFinding(string baseDir);

        Task<bool> IsComplete { get; }

        void ChooseConfiguration(string name, string value);
        public Task<int> LoadConfiguration();
        Task<int> ConfigureAgain();

        public string[] MainProviders { get; }



    }
    public interface ISaveAndLoadData
    {
        Task<int> SaveData(IRepoMS repo);
        Task<int> LoadData(IRepoMS repo);
    }
     
    public interface IRepoMS
    {
        Task<T> GetItem<T>();
        Task<int> SaveData<T>(T t);
    }
    public interface IData {

        Task<string> SavedData();
        Task Restore(string data);
        public IDictionary<string, object> WriteProperties();
        public IDictionary<string, object> ReadProperties(); 
        public void SetProperties(IDictionary<string, object> values);

    }
    public interface IConfigurableMS {
        string Name { get; set; }
        string Type{ get;  }

        string Description { get; }
    }
}
