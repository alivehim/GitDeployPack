using GitDeployPack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Core
{
    public interface IVisualStudioProjectCompiler
    {
        bool Compile(ProjectDescription description);
    }
}
