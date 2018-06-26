using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GitDeployPack.Model;
using Microsoft.Build.Evaluation;

namespace GitDeployPack.Core.ProjectParser
{
    public class VisualStudioProjectParser : IProjectParser
    {
        readonly Regex guidParser = new Regex(@"{(?<guid>[^}]+)}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        public ProjectDescription Parser(string projectFilePath)
        {
            ProjectDescription description = new ProjectDescription();
            try
            {
                ProjectCollection pro = new ProjectCollection();
                //pro.DefaultToolsVersion = "14.0";

                Dictionary<string, string> globalProperty = new Dictionary<string, string>
                {
                    {"Configuration", "Release"},
                    //{"VisualStudioVersion", "15.4"},
                    //{"Platform", "Any CPU"},
                    //{"OutputPath", @"d:\"},
                    { "BclBuildImported","Ignore"}
                };

                var items = pro.LoadProject(projectFilePath, globalProperty,null);

                description.ReferenceProjects = items.AllEvaluatedItems.Where(e => e.ItemType == "ProjectReference").Select(p => p.GetMetadataValue("Project")).ToList();

                description.ProjectGuid= items.GetPropertyValue("ProjectGuid");

                description.ContentFiles  = items.AllEvaluatedItems
                    .Where(e => e.ItemType == "Content").Select(p => p.EvaluatedInclude).ToList();

                description.CompileFiles = items.AllEvaluatedItems
                    .Where(e => e.ItemType == "Compile").Select(p => p.EvaluatedInclude).ToList(); 

                description.OutputPath = items.GetPropertyValue("OutputPath");

                description.OutputType=items.GetPropertyValue("OutputType");

                description.AssemblyName = items.GetPropertyValue("AssemblyName");
                var projectTypeGuids = string.IsNullOrEmpty(items.GetPropertyValue("ProjectTypeGuids"))?
                    items.GetPropertyValue("ProjectGuid"): items.GetPropertyValue("ProjectTypeGuids");
                
                Match match = guidParser.Match(projectTypeGuids);
                while (match.Success)
                {
                    description.ProjectType |= this.GetTypeByGuid(match.Groups["guid"].Value);
                    match = match.NextMatch();
                }

                return description;

            }
            catch (Exception ex)
            {
                //Console.WriteLine("---- " + projectFilePath);
                throw ex;
            }
        }


        private VsProjectType GetTypeByGuid(string typeGuid)
        {
            switch (typeGuid.ToUpper())
            {
                case "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC":
                    return VsProjectType.ClassLibrary;

                case "A9ACE9BB-CECE-4E62-9AA4-C7E7C5BD2124":
                    return VsProjectType.Database;

                case "3AC096D0-A1C2-E12C-1390-A8335801FDAB":
                    return VsProjectType.Test;

                case "349C5851-65DF-11DA-9384-00065B846F21":
                    return VsProjectType.Web;

                case "00D1A9C2-B5F0-4AF3-8072-F6C62B433612":
                    return VsProjectType.Database;

                default:
                    return VsProjectType.Undefined;
            }
        }
    }
}
