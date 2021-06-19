using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
//using Newtonsoft.Json;

namespace ConfigureMS
{
    public class RepoMSFile : IRepoMS
    {
        private readonly string fileName;
        private readonly IFileSystem fileSystem;
        private readonly int lastDot;

        public RepoMSFile(string fileName, IFileSystem fileSystem)
        {
            this.fileName = fileName;
            this.fileSystem = fileSystem;
            this.lastDot = fileName.LastIndexOf("."); 
        }
        private string AppendType(Type t)
        {
            string fileForType = fileName + "." + t.Name;
            if (lastDot > 0)
                fileForType = fileName.Substring(0, lastDot) + t.Name + "." + fileName.Substring(lastDot);
            return fileForType;
        }
        public async Task<T> GetItem<T>()
        {

            var data = await fileSystem.File.ReadAllTextAsync(AppendType(typeof(T)));
            
            //          var obj = JsonConvert.DeserializeObject(data,
            //typeof(T),
            //new JsonSerializerSettings()
            //{
            //    TypeNameHandling = TypeNameHandling.Auto,
            //    Formatting = Formatting.Indented
            //}) ;
            //return (T)obj ;
            //System.Text.Json cannot serialize properly interfaces
            return JsonSerializer.Deserialize<T>(data);


        }

        public async Task<int> SaveData<T>(T t)
        {
            ////System.Text.Json cannot serialize properly interfaces
            var data = JsonSerializer.Serialize(t,
                
                new JsonSerializerOptions()
                {
                    WriteIndented =true
                });
  //          var data= JsonConvert.SerializeObject(
  //t,
  //typeof(T),
  //new JsonSerializerSettings()
  //{
  //    TypeNameHandling = TypeNameHandling.Auto,
  //    Formatting = Formatting.Indented,
  //});
            await fileSystem.File.WriteAllTextAsync(AppendType(typeof(T)),data);
            return data.Length;
        }


    }
}
