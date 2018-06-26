using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Core
{
    public interface IPathService
    {
        string GetNugetPath();

        bool ClearTemperary();


        DirectoryInfo StaticLocation { get; }

        DirectoryInfo AssemblyLocation { get; }
        DirectoryInfo TemporaryLocation { get; }

        DirectoryInfo PackLocation { get; }


    }
}
