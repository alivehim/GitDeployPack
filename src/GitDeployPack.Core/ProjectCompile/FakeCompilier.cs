using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDeployPack.Infrastructure;
using GitDeployPack.Logger;
using GitDeployPack.Model;

namespace GitDeployPack.Core.ProjectCompile
{
    public class FakeCompilier : IVisualStudioProjectCompiler
    {
        public bool Compile(ProjectDescription description)
        {
            var logger = ContainerManager.Resolve<ILogger>();
            logger.Information($"Compiliing Project {description}");
            return true;
        }
    }
}
