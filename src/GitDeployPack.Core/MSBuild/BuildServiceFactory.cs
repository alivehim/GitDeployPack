using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDeployPack.Model;

namespace GitDeployPack.Core.MSBuild
{
    public class BuildServiceFactory : IBuildServiceFactory
    {
        public IBuildService Create(VsProjectType type)
        {
            //return new MSBuildBuildService();MSCommandlineBuildService
            return new MSCommandlineBuildService();
        }
    }
}
