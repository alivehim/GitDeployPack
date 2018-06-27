using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDeployPack.Model;
using GitDeployPack.Core.FileAnalysis;
using GitDeployPack.Infrastructure;

namespace GitDeployPack.Core
{
    public class FileAnalysisFactory : IFileAnalysisFactory
    {
        public GitFilePackContext packContext;
        public GitFilePackContext PackContext => packContext;

        public FileAnalysisFactory(GitFilePackContext pactContext)
        {
            this.packContext = pactContext;
        }

        public IFileAnalysis GetFileAnalysis(AnalysisFileType analsisFileType)
        {
            IFileAnalysis fileAnalysis;
            switch (analsisFileType)
            {
                case AnalysisFileType.CS:
                    fileAnalysis = new CompileFileAnalysis( ContainerManager.Resolve<IProjectFilter>(), PackContext);
                    break;
                case AnalysisFileType.CSHTML:
                case AnalysisFileType.CSS:
                case AnalysisFileType.HTML:
                case AnalysisFileType.JS:
                case AnalysisFileType.JSON:
                case AnalysisFileType.XML:
                    fileAnalysis = new StaticFileAnalysis(ContainerManager.Resolve<IProjectFilter>(), PackContext);
                    break;
                case AnalysisFileType.SQL:
                    fileAnalysis = new ScriptFileAnalysis(PackContext);
                    break; 
                default:
                    //throw new NotImplementedException();                    fileAnalysis = new UnkownFileAnalysis();
                    break;
            }
            return fileAnalysis;
        }
    }
}
