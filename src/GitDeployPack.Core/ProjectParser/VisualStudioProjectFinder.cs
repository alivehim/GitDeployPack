using GitDeployPack.Infrastructure;
using GitDeployPack.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Core.ProjectParser
{
    public class VisualStudioProjectFinder : IProjectFinder
    {
        private readonly ISolutionParser _solutionParser;
        private readonly IProjectFilter _projectFilter;
        public VisualStudioProjectFinder(ISolutionParser solutionParser,
            IProjectFilter projectFilter)
        {
            _solutionParser = solutionParser;
            _projectFilter = projectFilter;
        }

        public bool FindDeployProject(IList<ProjectDescription> list)
        {
            bool result = false;
            IEnumerable<ProjectDescription> dList;
            if ((dList = list.Where(p => p.ProjectType.HasFlag(VsProjectType.Web)
            || p.ProjectType.HasFlag(VsProjectType.Service)
            //|| p.ProjectType.HasFlag(VsProjectType.ClassLibrary) 
            || p.ProjectType.HasFlag(VsProjectType.Console))).ToList().Count != 0)
            {
                foreach (var item in dList)
                {
                    var referenceProjects =
                        from n in list
                        from m in item.ReferenceProjects
                        where m.ToLower().Contains(n.ProjectGuid.ToLower())
                        select n;

                    if (referenceProjects.Count() != 0)
                    {
                        foreach (var inn in referenceProjects)
                        {
                            inn.RelevanceProjects.Add(item);
                        }
                        result = true;
                    }
                }


            }

            var dllList = list.Where(p => (p.ProjectType==VsProjectType.ClassLibrary
            || p.ProjectType==VsProjectType.Undefined)&& p.RelevanceProjects.Count==0).ToList();

            if (dllList.Count != 0)
            {
                foreach (var item in dllList)
                {
                    //find project which includes the dll file from  parent fold
                    DirectoryInfo parent = new DirectoryInfo(item.FullName).Parent;

                    bool isMatch = false;
                    while (parent != null)
                    {
                        var slnfiles = parent.GetFiles().ToList().Where(p => p.FullName.EndsWith(".sln")).ToList();
                        if (slnfiles.Count != 0)
                        {
                            foreach (var sln in slnfiles)
                            {
                                if (ProjectFinder(sln.FullName, item, out IList<ProjectDescription> projectDescriptions))
                                {
                                    item.RelevanceProjects = projectDescriptions;
                                    isMatch = true;
                                    result = true;
                                    break;
                                }
                            }

                            if (isMatch)
                            {
                                break;
                            }

                        }
                        parent = parent.Parent;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// find project
        /// </summary>
        /// <param name="solutionFilePath"></param>
        /// <param name="project"></param>
        /// <param name="ownerProject"></param>
        /// <returns></returns>
        private bool ProjectFinder(string solutionFilePath, ProjectDescription project, out IList<ProjectDescription> ownerProjects)
        {
            var projectList = _solutionParser.Parser(solutionFilePath);
            ownerProjects = new List<ProjectDescription>();
            if (projectList != null && projectList.Any())
            {
                foreach (var proj in projectList)
                {
                    var referProjects = MsToolkit.GetReferAssembly(proj.ProjectFile);
                    if (referProjects.Where(p => p.ToLower().Contains(project.ProjectGuid.ToLower())).ToList().Count() != 0)
                    {
                        var ProjectParserFactory = ContainerManager.Resolve<IProjectParserServiceFactory>();
                        var projectParser = ProjectParserFactory.Create(AnalysisFileType.CSPROJ);

                        var projectx = projectParser.Parser(proj.ProjectFile);
                        if (!_projectFilter.IsValid(Path.GetFileName(proj.ProjectFile)))
                            continue;
                        if (projectx.ProjectType==VsProjectType.ClassLibrary || projectx.ProjectType == VsProjectType.Undefined)
                            continue;
                        projectx.Name = proj.Name;
                        projectx.Location = new FileInfo(proj.ProjectFile).Directory;
                        projectx.FullName = proj.ProjectFile;
                        projectx.IsNeedCompile = false;
                        ownerProjects.Add(projectx);
                    }

                }

                return ownerProjects.Count > 0;
            }
            return false;
        }
    }
}
