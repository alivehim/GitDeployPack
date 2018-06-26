using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Core.Model
{
    public class SolutionDescription
    {
        public string FullName { get; set; }
        public DirectoryInfo Location { get; set; }

        public string Name { get; set; }
    }
}
