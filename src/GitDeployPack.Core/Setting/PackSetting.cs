using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Setting
{
    public class PackSetting
    {
        public string StaticDirectory { get; set; } = "OSS";

        public string AssemblyDirectory { get; set; } = "Assembly";

        public string MsbuildPath { get; set; } = @"D:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin";

        public string IgnoreProjects { get; set; }

        public string IgnorePath { get;set; }

        public string ExtentPath { get; set; }
        
        public string PackNamePattern { get; set; }
    }
}
