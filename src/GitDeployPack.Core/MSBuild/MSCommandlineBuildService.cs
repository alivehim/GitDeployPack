using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDeployPack.Infrastructure;
using GitDeployPack.Logger;
using GitDeployPack.Model;
using GitDeployPack.Setting;

namespace GitDeployPack.Core.MSBuild
{
    public class MSCommandlineBuildService : IBuildService
    {

        public MSCommandlineBuildService()
        {

        }

        public BuildSolutionResult Build(ProjectDescription description, Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete, Action<string, Exception> errorLogger)
        {
            var logger = ContainerManager.Resolve<ILogger>();
            logger.AppendLog(PackPeriod.Compilie,$"{description}");
            var packSetting= ContainerManager.Resolve<PackSetting>();
            var result=DosCommandOutput.Execute($"msbuild.exe \"{description.FullName}\" /nologo /verbosity:minimal /consoleloggerparameters:ErrorsOnly /t:Build,Restore /p:configuration=Release ", packSetting.MsbuildPath);
          
            return new BuildSolutionResult() { IsSuccess = !result.Contains("error") };
        }
    }
}
