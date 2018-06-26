using GitDeployPack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Core
{
    public interface IBuildService
    {
        BuildSolutionResult Build(ProjectDescription description, Action<string> projectBuildStarted,
            Action<string, bool, string> projectBuildComplete, Action<string, Exception> errorLogger);
    }
}
