using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Core.FileAnalysis
{
    public class UnkownFileAnalysis : IFileAnalysis
    {
        public bool Do(string filePath)
        {
            return true;
        }
    }
}
