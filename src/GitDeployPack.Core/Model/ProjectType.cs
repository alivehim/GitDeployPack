using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Model
{
    [Flags]
    public enum ProjectType : int
    {
        Undefined = 0,
        Web = 1,
        Console = 2,
        Service = 4,
        ClassLibrary = 8,
        Deployment = 16,
        Database = 32,
        Test = 64,
        WindowsApplication = 128,
        ZipArchive = 256,
        GulpFile = 512,
        NetCore = 1024
    }
}
