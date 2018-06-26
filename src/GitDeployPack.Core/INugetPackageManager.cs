using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Core
{
    public interface INugetPackageManager
    {
        void RestoreSolutionPackages(string solutionFile);
        void RestoreProjectPackages(string projectDirectory, string solutionDirectory);
    }
}
