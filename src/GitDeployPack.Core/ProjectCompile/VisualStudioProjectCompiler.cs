using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDeployPack.Core.MSBuild;
using GitDeployPack.Infrastructure;
using GitDeployPack.Logger;
using GitDeployPack.Model;

namespace GitDeployPack.Core.ProjectCompile
{
    public class VisualStudioProjectCompiler : IVisualStudioProjectCompiler
    {
        private readonly IBuildServiceFactory buildFactory;

        public IBuildServiceFactory BuildFactory => buildFactory;
        public VisualStudioProjectCompiler(
            IBuildServiceFactory buildFactory)
        {
            this.buildFactory = buildFactory;
        }
        public bool Compile(ProjectDescription description)
        {
            if (description.IsNeedCompile)
            {
                var logger = ContainerManager.Resolve<ILogger>();
                var buildService = BuildFactory.Create(description.ProjectType);
                var result = buildService.Build(description,
                    (message) => { logger.Information(message); },
                    (message, bo, str) => { logger.Information(message + " - " + bo); },
                    (message, exception) =>
                    {
                        logger.Error(message + "\n" + exception.Message);
                    });

                return result.IsSuccess;
            }
            return true;           
        }
    }
}
