using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GitDeployPack.Extensions;
using GitDeployPack.Model;
using GitDeployPack.Core;
using GitDeployPack.Infrastructure;
using GitDeployPack.Core.ProjectParser;

namespace GitDeployPackTest
{
    [TestClass]
    public class FunctionTest
    {
        [TestMethod]
        public void EnumTest()
        {
            string cs = "CS";
            var enumValue = cs.ParseAsEnum<AnalysisFileType>();

            Assert.AreEqual(enumValue, AnalysisFileType.CS);
        }

        [TestMethod]
        public void TestReadProject()
        {
            //MSToolkit.IsWebProject(@"D:\WorkSpace\XNew\GHB2C\Website\Presentation\GH.Web\GH.Web.csproj");
        }

        [TestMethod]
        public void TestGetProjectIncludeFiles()
        {
            var list = MsToolkit.GetProjectCompileFiles(@"D:\WorkSpace\New\GHB2C\Website\Presentation\GH.Web\GH.Web.csproj");
            Console.WriteLine("ss");
        }


        [TestMethod]
        public void TestGetProjectIncludeFiles2()
        {
            XmlVisualStudioProjectParser parser = new XmlVisualStudioProjectParser();

            var list = parser.Parser(@"D:\WorkSpace\New\GHB2C\Website\Presentation\GH.Web\GH.Web.csproj");
            Console.WriteLine("ss");
        }


        [TestMethod]
        public void EnumTest0()
        {
            var v = "gif".GetEnumName<AnalysisFileType>();
            Console.WriteLine(v);
        }

        [TestMethod]
        public void DecodeTest()
        {
            GitToolkit.PercentDecode("s");
        }

        [TestMethod]
        public void ZipTest()
        {
            ZipHelper.ZipDirectory(@"E:\Project\RxConsole\RxConsole\bin\Debug\tmp\", @"E:\Project\RxConsole\RxConsole\bin\Debug\public\1.zip");

           
        }
    }
}
