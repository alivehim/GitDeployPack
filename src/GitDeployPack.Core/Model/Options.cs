using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Model
{
    public class Options
    {
        //[Option('r', "read", Required = true, HelpText = "Input files to be processed.")]
        //public IEnumerable<string> InputFiles { get; set; }

        //// Omitting long name, defaults to name of property, ie "--verbose"
        //[Option(Default = false, HelpText = "Prints all messages to standard output.")]
        //public bool Verbose { get; set; }

        //[Option("stdin", Default = false, HelpText = "Read from stdin")]
        //public bool stdin { get; set; }

        [Option('o', "orginrepo", Required = false, Default = "master", HelpText = "被比对分支名称")]
        public string OriginRepository { get; set; }

        [Option('n', "newrepo", Required = false, Default = "uat", HelpText = "比对提交分支称")]
        public string NewRepository { get; set; }

        [Option('w', "WorkPath", Required = true, HelpText = "Git目录")]
        public string GitWorkPath { get; set; }

        [Option('p', "PackNamePattern", Required = false, HelpText = "文件名模板")]
        public string PackNamePattern { get; set; }

        /// <summary>
        /// 默认为当前程序目录
        /// </summary>
        [Option('l', "PackLocation", Required = false, HelpText = "打包的根目录")]
        public string PackageLocation { get; set; }


        [Option('t', "FileType", Required = false, Default = "zip", HelpText = "文件类型")]
        public string PackageFileType { get; set; }
    }
}
