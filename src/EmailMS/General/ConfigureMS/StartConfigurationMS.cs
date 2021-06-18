using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ConfigureMS
{
    public class SHIM_StartConfigurationMS : IStartConfigurationMS
    {
        //TODO: use RSCG
        public SHIM_StartConfigurationMS()
        {

        }
        public SHIM_StartConfigurationMS(IStartConfigurationMS configure)
        {
            this.Name = configure.Name;
            this.ConfiguredAt = configure.ConfiguredAt;
            this.ChoosenMainProvider = configure.ChoosenMainProvider;
            this.MainProviders = configure.MainProviders;
            this.BaseFolder = configure.BaseFolder;
            this.ChoosenProviderData = configure.ChoosenProviderData;
        }
        public Task<bool> IsComplete()
        {
            foreach (var item in Validate(null))
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);

        }
        public bool IsConfigured() => ConfiguredAt != null;
         
        public string Name { get; set; }

        public DateTime? ConfiguredAt { get; set; }

        public string BaseFolder
        {
            get;
        }
        public string ChoosenMainProvider { get; set; }


        public string[] MainProviders { get; set; }

        public IData ChoosenProviderData { get; set; }

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
            yield break;
        }
    }
    public interface IStartConfigurationMS:ISaveAndLoadData, IValidatableObject        
    {
        string Name { get;}
        bool IsConfigured()=> ConfiguredAt != null;
        public string BaseFolder
        {
            get;
        }
        DateTime? ConfiguredAt { get; set; }
        public string ChoosenMainProvider { get; set; }
        IAsyncEnumerable<ValidationResult> StartFinding(string baseDir);

        Task<bool> IsComplete();

        void ChooseConfiguration(string name, string value);
        public Task<int> LoadConfiguration();
        Task<int> ConfigureAgain();

        public string[] MainProviders { get; }

        public IData ChoosenProviderData { get; }

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
    public interface IData : ISaveAndLoadData
    {

        //string SavedData { get; set; }
        
        public IDictionary<string, object> WriteProperties();
        public IDictionary<string, object> ReadProperties(); 
        public void SetProperties(IDictionary<string, object> values);
        public Task Test();
    }
    public interface IConfigurableMS {
        string Name { get; set; }
        string Type{ get;  }

        string Description { get; }
    }
}
