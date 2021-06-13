using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ConfigureMS
{
    public interface StartConfigurationMS:ISaveAndLoadData, IValidatableObject        
    {
        string Name { get; }
        bool IsConfigured()=> ConfiguredAt != null;

        DateTime? ConfiguredAt { get; set; }

        IAsyncEnumerable<ValidationResult> StartFinding(string baseDir);

        Task<bool> IsComplete { get; }

        void ChooseConfiguration(string name, string value);
        public Task<int> LoadConfiguration();
        Task<int> ConfigureAgain();

        
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
