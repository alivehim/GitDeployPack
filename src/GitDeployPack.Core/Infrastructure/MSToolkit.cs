using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GitDeployPack.Infrastructure
{
    public class MsToolkit
    {
        public static bool IsWebProject(string projectFilePath)
        {
            try
            {
                ProjectCollection pro = new ProjectCollection();
                pro.DefaultToolsVersion = "14.0";
                var items = pro.LoadProject(projectFilePath, "14.0");
                if (items.GetPropertyValue("OutputType") == "Library" && items.GetProperty("MvcBuildViews") != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IList<string> GetProjectCompileFiles(string projectFilePath)
        {
            try
            {
                XDocument xDocument = XDocument.Load(projectFilePath);
                XNamespace fileNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

                return xDocument.Descendants(fileNamespace + "Compile").Select(n => n.Attribute("Include").Value).ToList();

                //ProjectCollection pro = new ProjectCollection();
                //pro.DefaultToolsVersion = "14.0";
                //Dictionary<string, string> globalProperty = new Dictionary<string, string>
                //{
                //    {"Configuration", "Release"},
                //    { "BclBuildImported","Ignore"}
                //};
                //var items = pro.LoadProject(projectFilePath, globalProperty,"14.0");
                //var files=items.AllEvaluatedItems
                //    .Where(e => e.ItemType == "Compile").Select(p=>p.EvaluatedInclude).ToList();


                //var out1 = items.AllEvaluatedProperties.Where(p=>p.Name== "OutputPath");
                //return files;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("---- " + projectFilePath);
                throw ex;
            }
        }

        /// <summary>
        /// returns the collection of Content files from project 
        /// </summary>
        /// <param name="projectFilePath"></param>
        /// <returns></returns>
        public static IList<string> GetProjectContentFiles(string projectFilePath)
        {
            try
            {
                XDocument xDocument = XDocument.Load(projectFilePath);
                XNamespace fileNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

                return xDocument.Descendants(fileNamespace + "Content").Select(n => n.Attribute("Include").Value).ToList();
                //ProjectCollection pro = new ProjectCollection();
                //pro.DefaultToolsVersion = "14.0";
                //var items = pro.LoadProject(projectFilePath, "14.0");
                //var files = items.AllEvaluatedItems
                //    .Where(e => e.ItemType == "Content").Select(p => p.EvaluatedInclude).ToList();
                //return files;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("---- " + projectFilePath);
                throw ex;
            }
        }
    }
}
