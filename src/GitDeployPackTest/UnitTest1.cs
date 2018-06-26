using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GitDeployPack.Core;
using System.IO;

namespace GitDeployPackTest
{
    [TestClass]
    public class BuildTest 
    {
        [TestMethod]
        public void TestBuild()
        {
            MSBuildBuildService builder = new MSBuildBuildService();
            builder.Build(new GitDeployPack.Model.ProjectDescription {
                FullName = @"D:\WorkSpace\New\GHB2C\Website\Presentation\GH.Web\Administration\GH.Admin.csproj"
            },
                (message) => { Console.WriteLine(message); },
                    (message,bo,str) => { Console.WriteLine(message + " - " + bo); },
                    (message, exception) =>
                    {
                        Console.WriteLine(message + "\n" + exception);
                        Console.WriteLine(exception.Message + "\n" + exception);
                    });
        }

        [TestMethod]
        public void FileCopyTest()
        {
            File.Copy(@"C:\Users\alive\Desktop\新建文本文档 (2).txt", @"D:\123\123.txt");
        }
    }
}
