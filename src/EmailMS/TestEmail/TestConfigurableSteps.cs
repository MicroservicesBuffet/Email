using ConfigureMS;
using EmailConfigurator;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestEmail
{
    public partial class TestConfigurable
    {
        IFileSystem fileSystem;
        ConfigureEmail configure;
        string pathPlugins = @"C:\plugins";
        private void DirectoryCopy(string sourceDirName, string destDirName,
                                     bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            var dir =fileSystem.DirectoryInfo.FromDirectoryName(sourceDirName);
            
            if (!dir.Exists)
            {
                throw new System.IO.DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            var dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!fileSystem.Directory.Exists(destDirName))
            {
                fileSystem.Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                string temppath = fileSystem.Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (var subdir in dirs)
                {
                    string temppath = fileSystem.Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private void Given_Create_MockFileSystem_WithPlugins()
        {
            
            fileSystem = new MockFileSystem();
            fileSystem.Directory.CreateDirectory(pathPlugins);
            fileSystem.Directory.CreateDirectory(@$"{pathPlugins}\{ConfigureEmail.smtpProvidersFolder}\gmail");
            fileSystem.Directory.CreateDirectory(@$"{pathPlugins}\{ConfigureEmail.smtpProvidersFolder}\SimpleSMTP");
            
        }
        private async Task Given_Create_RealFileSystem_WithPlugins()
        {
            await Task.Delay(10);

            fileSystem = new FileSystem();
            bool IsDeleted = false;
            do
            {
                
                try
                {
                    GC.Collect();
                    if (fileSystem.Directory.Exists(pathPlugins))
                        fileSystem.Directory.Delete(pathPlugins, true);

                    IsDeleted = true;
                }
                catch
                {
                    GC.Collect();                    
                    await Task.Delay(2000);
                    IsDeleted = false;
                }
            } while (!IsDeleted);
            fileSystem.Directory.CreateDirectory(pathPlugins);
            fileSystem.Directory.CreateDirectory(@$"{pathPlugins}\{ConfigureEmail.smtpProvidersFolder}");
            fileSystem.Directory.CreateDirectory(@$"{pathPlugins}\{ConfigureEmail.smtpProvidersFolder}\gmail");
            fileSystem.Directory.CreateDirectory(@$"{pathPlugins}\{ConfigureEmail.smtpProvidersFolder}\SimpleSMTP");
            string pathFrom = @"..\..\..\..\SimpleSMTP\bin\Debug\net5.0";
            string pathWhere = @$"{pathPlugins}\{ConfigureEmail.smtpProvidersFolder}\SimpleSMTP";
            DirectoryCopy(pathFrom, pathWhere, true);
        }
        private async void And_Restore_Configuration()
        {
            await configure.LoadData(new RepoMSFile("andrei.txt", fileSystem));
        }

        private async void And_Save_Configuration()
        {
            await  configure.SaveData(new RepoMSFile("andrei.txt", fileSystem));
        }

        private void When_Create_Configurable_EmailSettings()
        {
            configure = new ConfigureEmail(fileSystem);
        }
        private async void Then_Can_Found_SMTPProviders()
        {
            var nr = 0;
            await foreach(var item in configure.StartFinding( pathPlugins))
            {
                nr++;

            }
            var expected = 0;
            Assert.Equal(expected, nr);//"no errors expected");
        }

        private void And_The_Number_of_SMTPProviders_is(int nr)
        {
            Assert.Equal(nr, configure.MainProviders?.Length);//"no errors expected");
        }

        private async void Then_Configuration_Is_Complete(bool value)
        {
            var complete = await configure.IsComplete();
            Assert.Equal(value, complete);
        }
        private async void And_Choose_The_SmtpProvider(string name,string value)
        {
            await configure.ChooseConfiguration(name, value);           
        }
    }
}
