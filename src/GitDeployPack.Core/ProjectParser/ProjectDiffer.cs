using GitDeployPack.Extensions;
using GitDeployPack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitDeployPack.Core.ProjectParser
{
    public class ProjectDiffer : IProjectDiffer
    {
        private string referencePattern = @"<HintPath>(?<key>.*?).dll</HintPath>";
        private readonly Options options;
        private IGitCommandHelper _gitCommandHelper;
   
        public IGitCommandHelper GitCommandHelper => _gitCommandHelper;
        public Options Options => options;
        public ProjectDiffer(IGitCommandHelper gitCommandHelper,
            Options options)
        {
            this._gitCommandHelper = gitCommandHelper;
            this.options = options;
        }
        public void Diff(ProjectDescription project)
        {
            var projectChangedContent = GitCommandHelper.CompareFile(Options.OriginRepository, options.NewRepository, options.GitWorkPath, project.FullName);

            foreach (var item in projectChangedContent)
            {
                if (item.StartsWith("+"))
                {
                    var mc = Regex.Match(item, referencePattern);
                    if (mc.Success)
                    {
                        var referassembly = mc.Groups["key"].Value;
                        project.ReferenceAssembly.Add($"{referassembly}.dll");
                    }
                }
            }
        }
    }
}
