using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDeployPack.Core.Model;
using GitDeployPack.Model;

namespace GitDeployPack.Core.ProjectParser
{
    public class VisualStudioSolutionFinder : ISolutionFinder
    {
        private ISolutionParser _solutionParser;
        public VisualStudioSolutionFinder(ISolutionParser solutionParser)
        {
            _solutionParser = solutionParser;
        }
        public IList<SolutionDescription> FindSolution(IList<ProjectDescription> prjlist)
        {
            IList<SolutionDescription> solutions = new List<SolutionDescription>();
            foreach (var item in prjlist)
            {
                if(FindSolution(item,out SolutionDescription description))
                {
                    if(!solutions.Any(p=> p.FullName== description.FullName))
                        solutions.Add(description);
                }
            }
            return solutions;
        }

        private bool FindSolution(ProjectDescription description,out SolutionDescription solutionDescription)
        {
            var parentDirecotry = new FileInfo(description.FullName).Directory.Parent;
            while (parentDirecotry != null)
            {
                var solutions = parentDirecotry.GetFiles().Where(p => p.FullName.EndsWith(".sln"));
                if (solutions.Any())
                {
                    foreach (var item in solutions)
                    {
                        var prjlist = _solutionParser.Parser(item.FullName);
                        if (prjlist.Any(p => $"{{{p.Guid.ToString().ToLower()}}}"== description.ProjectGuid.ToLower()))
                        {
                            solutionDescription = new SolutionDescription { FullName = item.FullName, Location = item.Directory, Name = item.Name };
                            return true;
                        }
                    }
                }
                parentDirecotry = parentDirecotry.Parent;
            }
            solutionDescription = null;
            return false;
        }
    }
}
