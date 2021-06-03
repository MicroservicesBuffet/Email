﻿using System;
using System.Threading.Tasks;

namespace ConfigureMS
{
    public interface StartConfigurationMS        
    {
        DateTime? ConfiguredAt { get; set; }

        Task<int> StartFinding(string baseDir, RepoMS repoMS);

        Task<bool> IsComplete { get; set; }

        Task<int> SaveData(RepoMS repo);

    }

    public interface RepoMS
    {
        Task<int> SaveData<T>(T t);
        Task<int> SaveData<T>(T[] t);
    }

    public interface IConfigurableMS {
        string Name { get; set; }
        string Type{ get; set; }

    }
}