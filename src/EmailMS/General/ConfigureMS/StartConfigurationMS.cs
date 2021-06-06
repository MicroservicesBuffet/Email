using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ConfigureMS
{
    public interface StartConfigurationMS: IValidatableObject        
    {
        DateTime? ConfiguredAt { get; set; }

        IAsyncEnumerable<ValidationResult> StartFinding(string baseDir);

        Task<bool> IsComplete { get; }

        void ChooseConfiguration(string name, string value);

        Task<int> ConfigureAgain();
    }
    public interface SaveAndLoadData
    {
        Task<int> SaveData(RepoMS repo);
        Task<int> LoadData(RepoMS repo);
    }
     
    public interface RepoMS
    {
        Task<T> GetItem<T>();
        Task<int> SaveData<T>(T t);
    }

    public interface IConfigurableMS {
        string Name { get; set; }
        string Type{ get;  }

        string Description { get; }
    }
}
