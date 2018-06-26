using GitDeployPack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Core.ProjectParser
{
    public class VisualStudioProjectFinder: IProjectFinder
    {
        public bool FindDeployProject(IList<ProjectDescription> list)
        {
            IEnumerable<ProjectDescription> dList; 
            if ((dList = list.Where(p => p.ProjectType.HasFlag(VsProjectType.Web) 
            || p.ProjectType.HasFlag(VsProjectType.Service) 
            || p.ProjectType.HasFlag(VsProjectType.ClassLibrary) 
            || p.ProjectType.HasFlag(VsProjectType.Console))) != null)
            {
                foreach(var item in dList)
                {
                    var referenceProjects =
                        from n in list
                        from m in item.ReferenceProjects
                        where m.ToLower().Contains(n.ProjectGuid.ToLower())
                        select n;
                    
                    if (referenceProjects.Count() != 0)
                    {
                        foreach(var inn in referenceProjects)
                        {
                            inn.RelevanceProject = item;
                        }
                    }
                }

                return true;
            }
            return false;
        }
    }
}
