using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using GitDeployPack.Extensions;
using GitDeployPack.Model;

namespace GitDeployPack.Core.ProjectParser
{
    public class XmlVisualStudioProjectParser : IProjectParser
    {
        readonly Regex guidParser = new Regex(@"{(?<guid>[^}]+)}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        public ProjectDescription Parser(string projectFile)
        {
            ProjectDescription description = new ProjectDescription();
            XDocument xDocument = XDocument.Load(projectFile);
            XNamespace fileNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

            description.ContentFiles = xDocument.Descendants(fileNamespace + "Content").Select(n => n.Attribute("Include").Value).ToList();
            description.CompileFiles = xDocument.Descendants(fileNamespace + "Compile").Select(n => n.Attribute("Include").Value).ToList();
            description.ReferenceProjects = xDocument.Descendants(fileNamespace + "ProjectReference")
                .Select(  n => n.Descendants(fileNamespace + "Project").Select(p => p.Value).FirstOrDefault()).ToList();

            description.ProjectGuid = xDocument.Descendants(fileNamespace + "ProjectGuid").Select(n => n.Value).FirstOrDefault();
            description.OutputType = xDocument.Descendants(fileNamespace + "OutputType").Select(n => n.Value).FirstOrDefault();
            description.AssemblyName = xDocument.Descendants(fileNamespace + "AssemblyName").Select(n => n.Value).FirstOrDefault();

            var  outputParent = xDocument.Descendants(fileNamespace + "PropertyGroup").Where(n => n.Attribute("Condition")!=null && n.Attribute("Condition").Value.Contains("Release")).FirstOrDefault();
            if (outputParent != null)
            {
                description.OutputPath= outputParent.Descendants(fileNamespace + "OutputPath").Select(n => n.Value).FirstOrDefault();
            }

            var projectTypeGuids=xDocument.Descendants(fileNamespace + "ProjectTypeGuids").Select(n => n.Value).FirstOrDefault();
            if (projectTypeGuids.IsNotEmpty())
            {
                Match match = guidParser.Match(projectTypeGuids);
                while (match.Success)
                {
                    description.ProjectType |= this.GetTypeByGuid(match.Groups["guid"].Value);
                    match = match.NextMatch();
                }
            }
            else
            {
                if(!description.OutputType.IsNotEmpty() && description.ProjectType==VsProjectType.Undefined)
                {
                    switch(description.OutputType)
                    {
                        case "Exe":
                            description.ProjectType = VsProjectType.Console;
                            break;
                        case "Library":
                            description.ProjectType = VsProjectType.ClassLibrary;
                            break;
                        default:
                            break;
                    }
                }
            }
          
            return description;
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
