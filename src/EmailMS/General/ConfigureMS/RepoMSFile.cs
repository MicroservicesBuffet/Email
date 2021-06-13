using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConfigureMS
{
    public class RepoMSFile : IRepoMS
    {
        private readonly string fileName;
        private readonly IFileSystem fileSystem;

        public RepoMSFile(string fileName, IFileSystem fileSystem)
        {
            this.fileName = fileName;
            this.fileSystem = fileSystem;
        }

        public async Task<T> GetItem<T>()
        {
            var data = await fileSystem.File.ReadAllTextAsync(fileName);
            return JsonSerializer.Deserialize<T>(data);
        }

        public async Task<int> SaveData<T>(T t)
        {
            var data = JsonSerializer.Serialize(t);
            await fileSystem.File.WriteAllTextAsync(fileName,data);
            return data.Length;
        }


    }
}
