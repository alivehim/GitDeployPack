using GitDeployPack.Model;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Core
{
    public class MSBuildBuildService: IBuildService
    {

        private readonly INugetPackageManager nugetPackageManager;
        
        public MSBuildBuildService()
        {

        }

        public BuildSolutionResult Build(ProjectDescription description, Action<string> projectBuildStarted,
            Action<string, bool, string> projectBuildComplete, Action<string, Exception> errorLogger)
        {
            //还原Nuget包
            //this.nugetPackageManager.RestoreSolutionPackages(Path.Combine(sourcesFolder, projectVersion.SolutionFile));

            return BuildInternal(description.FullName, projectBuildStarted, projectBuildComplete, errorLogger);
        }


        private BuildSolutionResult BuildInternal(string targetFile, Action<string> projectBuildStarted, 
            Action<string, bool, string> projectBuildComplete, Action<string, Exception> errorLogger)
        {



            ProjectCollection projectCollection = new ProjectCollection();

            //projectCollection.DefaultToolsVersion = "14.0";
            Dictionary<string, string> globalProperty = new Dictionary<string, string>
            {
                {"Configuration", "Release"},
                //{"VisualStudioVersion", "15.4"},
                //{"Platform", "Any CPU"},
                //{"OutputPath", @"d:\"},
                { "BclBuildImported","Ignore"}
            };

            BuildRequestData buildRequestData = new BuildRequestData(
                targetFile,
                globalProperty,
                null,
                new[]
                {
                    "Rebuild"
                },
                null);

            BuildParameters buildParameters = new BuildParameters(projectCollection);
            buildParameters.MaxNodeCount = 4;

            buildParameters.Loggers = new List<ILogger>
            {
                new GitDeployPack.Logger.MSBuildLogger(projectBuildStarted, projectBuildComplete, errorLogger)
            };

            BuildResult buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequestData);

            return new BuildSolutionResult
            {
                IsSuccess = buildResult.OverallResult == BuildResultCode.Success
            };
        }

        private string LatestToolsVersion()
        {
            if (Directory.Exists(@"C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v14.0"))
            {
                return "14.0";
            }

            if (Directory.Exists(@"C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v12.0"))
            {
                return "12.0";
            }

            return null;
        }
    }
}
