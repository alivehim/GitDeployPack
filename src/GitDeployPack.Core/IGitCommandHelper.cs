using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Core
{
    public interface IGitCommandHelper
    {
        IList<string> CompareBranch(string originBranch, string newBranch, string workspace);

        IList<string> CompareFile(string orginBranch, string newBranch, string workspace, string file);

        string GetCurrentBranch(string workspace);

        string GetGitWorkSpace(string codeDirectory);
    }
}
