using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDeployPack.Model;

namespace GitDeployPack.Core.ProjectParser
{
    public class ProjectParserServiceFactory : IProjectParserServiceFactory
    {
        public IProjectParser Create(AnalysisFileType fileType)
        {
            return new XmlVisualStudioProjectParser();
        }
    }
}
