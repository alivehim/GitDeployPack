using GitDeployPack.Infrastructure;
using GitDeployPack.Logger;
using GitDeployPack.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Core.FileAnalysis
{
    public class StaticFileAnalysis : IFileAnalysis
    {
        private IProjectFilter projectFiler;
        public GitFilePackContext packContext;
        public GitFilePackContext PackContext => packContext;
        public IProjectFilter ProjectFiler => projectFiler;
        public StaticFileAnalysis(IProjectFilter projectFiler,
            GitFilePackContext packContext)
        {
            this.packContext = packContext;
            this.projectFiler = projectFiler;
        }

        public bool Do(string filePath)
        {
            if (!projectFiler.IsValidFile(filePath))
                return false;

            var logger = ContainerManager.Resolve<ILogger>();
            logger.AppendLog(PackPeriod.Analysis, Path.GetFileName(filePath));
            if (PackContext.ProjectsDescription.Where(p => p.StaticFiles.Any(x => x.Equals(filePath))).ToList().Count > 0)
            {
                var description = PackContext.ProjectsDescription.Where(p => p.ContentFiles.Any(x => filePath.EndsWith(x))).First();
                logger.DebugTrace(filePath + "已存在项目中");
                description.StaticFiles.Add(filePath);
                return true;
            }

            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                var directory = file.Directory;
                int csprojCount = 0;
                do
                {
                    try
                    {
                        var projlist = directory.EnumerateFiles().ToList()
                            .Where(p => p.Extension.EndsWith(Constants.PROJECTEXTENSION)).ToList();
                        csprojCount = projlist.Count;
                        foreach (var item in projlist)
                        {
                            //get collection of content files from project file
                            var contentFiles = MsToolkit.GetProjectContentFiles(item.FullName)
                                .Select(p => $"{directory.FullName}\\{p}");
                            if (contentFiles.FirstOrDefault(p => p == filePath) != null)
                            {
                                //find project which owned it from the context
                                ProjectDescription projDescription = null;
                                if ((projDescription = PackContext.ProjectsDescription
                                    .FirstOrDefault(p => p.Name == item.Name
                                     && p.Location.FullName == item.Directory.FullName)) != null)
                                {
                                    //add file to the collection of statics files
                                    projDescription.StaticFiles.Add(filePath);
                                    return true;
                                }
                                else
                                {
                                    //解析项目文件
                                    var ProjectParserFactory = ContainerManager.Resolve<IProjectParserServiceFactory>();
                                    var projectParser = ProjectParserFactory.Create(AnalysisFileType.CSPROJ);

                                    var description = projectParser.Parser(item.FullName);

                                    description.Name = item.Name;
                                    description.Location = item.Directory;
                                    description.IsChanged = false;
                                    description.FullName = item.FullName;
                                    description.IsNeedCompile = true;

                                    //add file to the collection of statics files
                                    description.StaticFiles.Add(filePath);

                                    PackContext.ProjectsDescription.Add(description);
                                    return true;
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        logger.Error(filePath + "=>" + ex.Message);
                        return false;
                    }

                    directory = directory.Parent;
                }
                while (csprojCount == 0 && directory != null);
            }

            return false;
        }
    }
}
