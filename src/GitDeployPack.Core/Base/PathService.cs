using GitDeployPack.Infrastructure;
using GitDeployPack.Setting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Core
{
    public class PathService : IPathService
    {
        private readonly string TEMPORARYDIRECTORY = "tmp";
        private readonly string PUBLICDIRECTORY = "public";
        private readonly PackSetting packSetting;
        public PackSetting PackSetting => packSetting;
        private DirectoryInfo tempLocation;
        private DirectoryInfo packLocation;
        private DirectoryInfo scriptLocation;
        public DirectoryInfo StaticLocation
        {
            get
            {
                return TemporaryLocation.CreateSubdirectory(PackSetting.StaticDirectory);
            }
        }

        public DirectoryInfo AssemblyLocation
        {
            get
            {
                return TemporaryLocation.CreateSubdirectory(PackSetting.AssemblyDirectory);
            }
        }

        public DirectoryInfo ScriptLocation
        {
            get
            {
                return TemporaryLocation.CreateSubdirectory("Script");
            }
        }

        public DirectoryInfo TemporaryLocation
        {
            get
            {
                if (tempLocation == null)
                {
                    var tmpDirectory = $"{System.AppDomain.CurrentDomain.BaseDirectory}\\{TEMPORARYDIRECTORY}";
                    tempLocation = new DirectoryInfo(tmpDirectory);
                    if (!tempLocation.Exists)
                    {
                        tempLocation.Create();
                    }

                    return tempLocation;
                }
                return tempLocation;
            }
        }

        public DirectoryInfo PackLocation
        {
            get
            {
                if (packLocation == null)
                {
                    var packDirectory = $"{System.AppDomain.CurrentDomain.BaseDirectory}\\{PUBLICDIRECTORY}";
                    packLocation = new DirectoryInfo(packDirectory);
                    if (!packLocation.Exists)
                    {
                        packLocation.Create();
                    }

                    return packLocation;
                }
                return packLocation;
            }
        }

        public string GitRootDirectory { get; set; }
       
        public PathService(
           PackSetting packSetting
           )
        {
            this.packSetting = packSetting;
        }

        public bool ClearTemperary()
        {
            var tmpDirectory = $"{System.AppDomain.CurrentDomain.BaseDirectory}\\{TEMPORARYDIRECTORY}";
            var tmp = new DirectoryInfo(tmpDirectory);
            if (!tmp.Exists)
            {
                tmp.Create();
            }
            else
            {
                //clear all the  files of directory
                FileTookit.DeleteFile(tmp.FullName);
            }
            return true;
        }

        public string GetNugetPath()
        {
            return $"{System.AppDomain.CurrentDomain.BaseDirectory}\\nuget.exe";
        }
    }
}