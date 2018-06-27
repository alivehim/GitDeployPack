using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDeployPack.Model;
using System.IO;
using GitDeployPack.Extensions;
using Microsoft.Build.Evaluation;
using GitDeployPack.Setting;

namespace GitDeployPack.Core
{
    public class ProjectFilePreparer : IChangedFilePreparer
    {
        private readonly Options options;
        private readonly IFileAnalysisFactory fileAnalysisFactory;
        private readonly IProjectParserServiceFactory projectParserFactory;
        private GitFilePackContext packContext;
        private IProjectFilter projectFiler;
        private IProjectDiffer projectDiff;
        private ISolutionFinder solutionFinder;
        private INugetPackageManager nugetPackageManager;
        private IPathService pathService;

        public Options Options => options;
        public GitFilePackContext PackContext => packContext;
        public IProjectFilter ProjectFiler => projectFiler;

        public IProjectParserServiceFactory ProjectParserFactory=> projectParserFactory;
        public IFileAnalysisFactory FileAnalysisFactory => fileAnalysisFactory;
        public IProjectDiffer ProjectDiffer => projectDiff;
        public ISolutionFinder SolutionFinder => solutionFinder;
        public IPathService PathService => pathService;
        public INugetPackageManager NugetPackageManager => nugetPackageManager;

        public ProjectFilePreparer(
            Options options, 
            IFileAnalysisFactory fileAnalysisFactory,
            GitFilePackContext pactContext,
            IProjectFilter projectFiler,
            IProjectDiffer projectDiff,
            ISolutionFinder solutionFinder,
            INugetPackageManager nugetPackageManager,
            IPathService pathService,
            IProjectParserServiceFactory projectParserFactory
            )
        {
            this.options = options;
            this.fileAnalysisFactory = fileAnalysisFactory;
            this.packContext = pactContext;
            this.projectFiler = projectFiler;
            this.projectDiff = projectDiff;
            this.projectParserFactory = projectParserFactory;
            this.nugetPackageManager = nugetPackageManager;
            this.solutionFinder = solutionFinder;
            this.pathService = pathService;
        }

        public IList<ProjectDescription> Analysis(ChangedFileList list)
        {
            //分析项目文件
            var projectFile = list.Where(p => p.EndsWith(".csproj"));
            var projectParser= ProjectParserFactory.Create(AnalysisFileType.CSPROJ);

            foreach (var item in projectFile)
            {
                if (!projectFiler.IsValid(item))
                    continue;

                var filePath = $"{PathService.GitRootDirectory}\\{item.Replace("/","\\")}";
                FileInfo file = new FileInfo(filePath);

                if (!file.FullName.StartsWith(options.GitWorkPath))
                    continue;

                var description=projectParser.Parser(filePath);

                description.Name = file.Name;
                description.Location = file.Directory;
                description.IsChanged = true;
                description.FullName = file.FullName;

                //check file 
                ProjectDiffer.Diff(description);

                PackContext.ProjectsDescription.Add(description);
            }


            list.Except(projectFile).ToList().ForEach(p => Analysis(p));
            //restore nuget files
            PrepareNugetFiles();

            return PackContext.ProjectsDescription;
        }

        private void Analysis(string filePath)
        {
            try
            {
                FileInfo file = new FileInfo($"{PathService.GitRootDirectory}\\{filePath}");
                if (file.FullName.StartsWith(Options.GitWorkPath) &&  file.Exists)
                {
                    var fileAnalysis = FileAnalysisFactory.GetFileAnalysis(file.Extension.Replace(".", "")
                       .GetEnumName<AnalysisFileType>());
                    fileAnalysis.Do(file.FullName);
                }
               
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        
        }


        private void PrepareNugetFiles()
        {
            var prjlist = solutionFinder.FindSolution(PackContext.ProjectsDescription);
            if (prjlist.Count != 0)
            {
                foreach(var item in prjlist)
                {
                    NugetPackageManager.RestoreSolutionPackages(item.FullName);
                }
            }

        }
    }
}
