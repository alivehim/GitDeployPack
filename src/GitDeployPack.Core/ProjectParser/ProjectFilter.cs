using GitDeployPack.Model;
using GitDeployPack.Setting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Core.ProjectParser
{
    public class ProjectFilter: IProjectFilter
    {
        private PackSetting packSetting;
        private Options options;
        private IPathService pathService;
        public PackSetting PackSetting => packSetting;
        public Options Options => options;
        public IPathService PathService => pathService;

        private IList<DirectoryInfo> ignorePathlist;
        private IList<string> ignoreProjectlist;
        public ProjectFilter(
            PackSetting packSetting,
            IPathService pathService,
            Options options)
        {
            this.packSetting = packSetting;
            this.pathService = pathService;
            this.options = options;
            Init();
        }


        private void Init()
        {
            ignorePathlist = PackSetting.IgnorePath.Split(',').Select(p => new DirectoryInfo(p)).ToList();
            ignoreProjectlist = PackSetting.IgnoreProjects.Split(',');
        }
        public bool IsValid(string projectName)
        {

            if (projectName.ToLower().Contains("test"))
                return false;

            if (ignoreProjectlist.Any(p =>  projectName.Trim().ToLower().Contains(p.ToLower())))
                return false;

            return true;
        }

        public bool IsValidFile(string fileName)
        {
            var ignorePathlist = PackSetting.IgnorePath.Split(',').Select(p => 
            {
                Directory.SetCurrentDirectory(Options.GitWorkPath);
                return new DirectoryInfo(Path.GetFullPath(p));
            }).Where(p=>p.Exists).ToList();

            if (ignorePathlist.Count == 0)
                return true;
            var fileInfo =  new FileInfo(fileName);
            if(fileInfo!=null)
            {
                if (ignorePathlist.Where(p=>p.Name!="bin" || p.Name!="obj").Any(p => IsHasFold(p,fileInfo.Directory)))
                    return false;
            }
            return true;
        }

        private bool IsHasFold(DirectoryInfo p, DirectoryInfo c)
        {
            if (p.FullName == c.FullName)
                return true;
            var childs = p.GetDirectories();
            if (childs == null || childs.Count() == 0)
                return false;

            foreach (var item in childs.Where(m => m.Name != "bin" || m.Name != "obj"))
            {
                if (IsHasFold(item, c))
                    return true;
            }
            return false;
        }
    }
}
