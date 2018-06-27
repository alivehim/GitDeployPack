using GitDeployPack.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Model
{
    public class GitFilePackContext
    {
        public IList<ProjectDescription> ProjectsDescription { get; set; } = new List<ProjectDescription>();

        public IList<string> ScriptFiles { get; set; } = new List<string>();
    }
}
