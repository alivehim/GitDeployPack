using GitDeployPack.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitDeployPack.Core
{
    public class GitCommandHelper: IGitCommandHelper
    {
        private IPathService _pathService;
        public GitCommandHelper(IPathService pathService)
        {
            this._pathService = pathService;
        }

        /// <summary>
        /// 比较两个分支
        /// </summary>
        /// <param name="originBranch"></param>
        /// <param name="newBranch"></param>
        /// <returns>返回有改变的文件列表</returns>
        public IList<string> CompareBranch(string originBranch,string newBranch,string workspace)
        {
            string tmpcomparefile = $"{_pathService.TemporaryLocation.FullName}\\o.txt";

            
            DosCommandOutput.Execute($"git diff {originBranch} {newBranch} --name-only > {tmpcomparefile}", workspace);

            //read the file
            StreamReader objReader = new StreamReader(tmpcomparefile);
            string sLine = "";
            List<string> LineList = new List<string>(); 
            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null && !sLine.Equals(""))
                    LineList.Add(sLine.Replace("/","\\"));
            }
            objReader.Close();
            return LineList;
        }


        public IList<string> CompareFile(string originBranch, string newBranch, string workspace,string file)
        {
            string tmpcomparefile = $"{_pathService.TemporaryLocation.FullName}\\o.txt";
            DosCommandOutput.Execute($"git diff {originBranch} {file}> {tmpcomparefile}", workspace);
            StreamReader objReader = new StreamReader(tmpcomparefile);
            string sLine = "";
            List<string> LineList = new List<string>();
            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null && !sLine.Equals(""))
                    LineList.Add(sLine);
            }
            objReader.Close();
            return LineList;
        }

        public string GetCurrentBranch(string workspace)
        {
            var content=DosCommandOutput.Execute($"git status --branch", workspace);

            var lines=Regex.Split(content, @"\n");
            foreach(var item in lines)
            {
                Match mc;
                if ((mc=Regex.Match(item, @"On branch (?<key>.*?)$")).Success)
                {
                    return mc.Groups["key"].Value;
                }
            }

            throw new Exception("can't found current branch");
        }

        public string GetGitWorkSpace(string codeDirectory)
        {
            var content = DosCommandOutput.Execute($"git worktree list ", codeDirectory);
            var mc = Regex.Match(content,@"^(?<key>.*?)\s");
            if(mc.Success)
            {
                return mc.Groups["key"].Value.Replace("/","\\");
            }
            throw new Exception("Can not found work space");
        }

    }
}
