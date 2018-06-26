﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDeployPack.Infrastructure;
using GitDeployPack.Logger;
using GitDeployPack.Model;
using GitDeployPack.Setting;

namespace GitDeployPack.Core.FilePack
{
    public class FilePackService :IFilePackService 
    {
        private readonly Options options;
        private readonly PackSetting packSetting;
        private readonly IPathService pathService;
        public Options PackOptions => options;
        public PackSetting PackSetting => packSetting;

        public IPathService PathService => pathService;

        private ILogger logger;

        public FilePackService(
            Options options,
            PackSetting packSetting,
            IPathService pathServie
            )
        {
            this.options = options;
            this.packSetting = packSetting;
            this.logger=ContainerManager.Resolve<ILogger>();
            this.pathService = pathServie;
        }

        public bool Pack(ProjectDescription description)
        {
            //if(description.ProjectType.HasFlag(VsProjectType.Web))
            {

                //Copy static files
                PackStaticFiles(description, pathService.StaticLocation);
                //Copy dynamic files
                PackAssemblyFiles(description, pathService.AssemblyLocation);

                PackReferenceAssembly(description, pathService.AssemblyLocation);


            }

            return false;
        }

        private bool PackStaticFiles(ProjectDescription description, DirectoryInfo root)
        {
          
            description.StaticFiles.ToList().ForEach(file =>
            {
                logger.AppendLog(PackPeriod.Pack, $"{Path.GetFileName(file)}");

                var relativePath = file.Replace(options.GitWorkPath, "");
                if (relativePath[0] == '\\')
                {
                    relativePath = relativePath.Remove(0, 1);
                }

                if (EnsureParentDirectory(root, relativePath))
                {
                    var copyFilePath = Path.Combine(root.FullName, relativePath);
                    if (File.Exists(file))
                        File.Copy(file, copyFilePath);
                }
            });
            return true;
        }

        private bool PackAssemblyFiles(ProjectDescription description, DirectoryInfo root)
        {
            if (description.IsNeedCompile)
            {
                logger.AppendLog(PackPeriod.Pack, $"{description.AssemblyName}");
                if (description.RelevanceProject != null)
                {
                    Directory.SetCurrentDirectory(description.RelevanceProject.Location.FullName);
                    var assemblySourceFile = Path.Combine(Path.GetFullPath(description.RelevanceProject.OutputPath), description.OutputName);
                    //reflative of path
                    var relativePath = assemblySourceFile.Replace(options.GitWorkPath, "");
                    //create sub folds

                    if (relativePath[0] == '\\')
                    {
                        relativePath = relativePath.Remove(0, 1);
                    }

                    if (EnsureParentDirectory(root, relativePath))
                    {
                        var copyFilePath = Path.Combine(root.FullName, relativePath);
                        if(!File.Exists(copyFilePath) && File.Exists(assemblySourceFile))
                            File.Copy(assemblySourceFile, copyFilePath);
                    }
                }
                else
                {
                    Directory.SetCurrentDirectory(description.Location.FullName);
                    var assemblySourceFile = Path.Combine(Path.GetFullPath(description.OutputPath), description.OutputName);
                    //reflative of path
                    var relativePath = assemblySourceFile.Replace(options.GitWorkPath, "");
                    //create sub folds

                    if (relativePath[0] == '\\')
                    {
                        relativePath = relativePath.Remove(0, 1);
                    }

                    if (EnsureParentDirectory(root, relativePath))
                    {
                        var copyFilePath = Path.Combine(root.FullName, relativePath);
                        if (!File.Exists(copyFilePath) && File.Exists(assemblySourceFile))
                            File.Copy(assemblySourceFile, copyFilePath);
                    }
                }
            }
            return true;
        }

        private bool PackReferenceAssembly(ProjectDescription description, DirectoryInfo root)
        {
            foreach (var item in description.ReferenceAssembly)
            {
                if (description.RelevanceProject != null)
                {
                    Directory.SetCurrentDirectory(description.Location.FullName);
                    var assemblySourceFile = Path.GetFullPath(item);
                    //reflative of path
                    Directory.SetCurrentDirectory(description.RelevanceProject.Location.FullName);
                    var targetSourceFile = Path.Combine(Path.GetFullPath(description.RelevanceProject.OutputPath), Path.GetFileName(assemblySourceFile));
                    var relativePath = targetSourceFile.Replace(options.GitWorkPath, "");
                    //create sub folds

                    if (relativePath[0] == '\\')
                    {
                        relativePath = relativePath.Remove(0, 1);
                    }

                    if (EnsureParentDirectory(root, relativePath))
                    {
                        var copyFilePath = Path.Combine(root.FullName, relativePath);
                        if (!File.Exists(copyFilePath) && File.Exists(assemblySourceFile))
                            File.Copy(assemblySourceFile, copyFilePath);
                    }
                }
                else
                {
                    Directory.SetCurrentDirectory(description.Location.FullName);
                    var assemblySourceFile = Path.GetFullPath(item);
                    //reflative of path
                    var targetSourceFile = Path.Combine(Path.GetFullPath(description.OutputPath), Path.GetFileName(assemblySourceFile));
                    var relativePath = targetSourceFile.Replace(options.GitWorkPath, "");

                    //create sub folds
                    if (relativePath[0] == '\\')
                    {
                        relativePath = relativePath.Remove(0, 1);
                    }

                    if (EnsureParentDirectory(root, relativePath))
                    {
                        var copyFilePath = Path.Combine(root.FullName, relativePath);
                        if (!File.Exists(copyFilePath) && File.Exists(assemblySourceFile))
                            File.Copy(assemblySourceFile, copyFilePath);
                    }
                }
            }
            return true;
        }

        private bool PackScriptFiles(ProjectDescription description, DirectoryInfo root)
        {
            return false;
        }

        private bool EnsureParentDirectory(DirectoryInfo root,string filePath)
        {
            int pos = filePath.LastIndexOf('\\');
            if (pos > 0)
            {
                var rel = filePath.Remove(pos, filePath.Length - pos);
                var parentDirectory = Path.Combine(root.FullName, rel);

                if (!Directory.Exists(parentDirectory))
                {
                    root.CreateSubdirectory(rel);
                    return true;
                }
                return true;
            }
            return false;
        }
    }
}