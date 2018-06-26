using GitDeployPack.Infrastructure;
using GitDeployPack.Logger;
using GitDeployPack.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Core
{
    public class CompileFileAnalysis : IFileAnalysis
    {
        private readonly GitFilePackContext packContext;
        public GitFilePackContext PackContext => packContext;
        private IProjectFilter projectFiler;
        public IProjectFilter ProjectFiler => projectFiler;
        public CompileFileAnalysis(
            IProjectFilter projectFiler,
            GitFilePackContext pactContext
            )
        {
            this.projectFiler = projectFiler;
            this.packContext = pactContext;
        }

        public bool Do(string filePath)
        {
            if (!projectFiler.IsValidFile(filePath))
                return false;
            //查找其所在项目文件
            var logger = ContainerManager.Resolve<ILogger>();
            logger.AppendLog(PackPeriod.Analysis, Path.GetFileName(filePath));

            if (PackContext.ProjectsDescription.Where(p => p.CompileFiles.Any(x=>x.Equals(filePath))).ToList().Count > 0)
            {
                var description = PackContext.ProjectsDescription.Where(p => p.CompileFiles.Any(x => filePath.EndsWith(x))).First();
               logger.DebugTrace(filePath + "已存在项目中");
                //更改其可编绎状态
                description.IsNeedCompile = true;
                return true;
            }

            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                var directory = file.Directory;
                int csprojCount = 0; do
                {
                    try
                    {
                        var projlist = directory.EnumerateFiles().ToList()
                            .Where(p => p.Extension.EndsWith(Constants.PROJECTEXTENSION)).ToList();
                        csprojCount = projlist.Count;
                        foreach (var item in projlist)
                        {
                            //查找其包含文件
                            var compileFiles = MsToolkit.GetProjectCompileFiles(item.FullName)
                                .Select(p => $"{directory.FullName}\\{p}");
                            if (compileFiles.FirstOrDefault(p => p == filePath) != null)
                            {
                                //找到其项目文件,添加项目
                                ProjectDescription projDescription = null;
                                if ((projDescription = PackContext.ProjectsDescription
                                    .FirstOrDefault(p => p.Name == item.Name
                                    && p.Location.FullName == item.Directory.FullName)) != null)
                                {
                                    //已经存在
                                    //更新其它数据
                                    if (projDescription.CompileFiles.Count == 0)
                                        projDescription.CompileFiles = compileFiles.ToList();

                                    projDescription.IsNeedCompile = true;

                                    return true;
                                }
                                else
                                {
                                    if (!ProjectFiler.IsValid(item.FullName))
                                    {
                                        return false;
                                    }
                                    var ProjectParserFactory = ContainerManager.Resolve<IProjectParserServiceFactory>();
                                    var projectParser = ProjectParserFactory.Create(AnalysisFileType.CSPROJ);

                                    var description = projectParser.Parser(item.FullName);

                                    description.Name = item.Name;
                                    description.Location = item.Directory;
                                    description.IsChanged = false;
                                    description.FullName = item.FullName;
                                    description.IsNeedCompile = true;

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
