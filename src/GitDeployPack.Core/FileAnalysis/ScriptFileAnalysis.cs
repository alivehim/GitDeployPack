using GitDeployPack.Infrastructure;
using GitDeployPack.Logger;
using GitDeployPack.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Core.FileAnalysis
{
    public class ScriptFileAnalysis : IFileAnalysis
    {
        private readonly GitFilePackContext packContext;
        public GitFilePackContext PackContext => packContext;

        public ScriptFileAnalysis(GitFilePackContext pactContext)
        {
            this.packContext = pactContext;
        }

        public bool Do(string filePath)
        {
            var logger = ContainerManager.Resolve<ILogger>();
            logger.AppendLog(PackPeriod.Analysis, Path.GetFileName(filePath));
            packContext.ScriptFiles.Add(filePath);
            return true;
        }
    }
}
