using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using GitDeployPack.Model;

namespace GitDeployPack.Core.ProjectParser
{
    public class VisualStudioSolutionParser : ISolutionParser
    {
        readonly Regex projectParser = new Regex(@"Project\(""\{(?<type>[A-F0-9-]+)\}""\) = ""(?<name>[^""]+)"", ""(?<file>[^""]+)"", ""\{(?<guid>[A-F0-9-]+)\}""", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        readonly Regex guidParser = new Regex(@"{(?<guid>[^}]+)}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        public IList<VisualStudioSolutionProject> Parser(string solutionFilePath)
        {
            string text = File.ReadAllText(solutionFilePath);
            string folder = Path.GetDirectoryName(solutionFilePath);

            IList<VisualStudioSolutionProject> result = new List<VisualStudioSolutionProject>();

            Match match = projectParser.Match(text);

            while (match.Success)
            {
                if (match.Groups["type"].Value == "2150E333-8FDC-42A3-9474-1A3956D46DE8") // project group
                {
                    match = match.NextMatch();
                    continue;
                }

                VisualStudioSolutionProject visualStudioProject = new VisualStudioSolutionProject()
                {
                    Guid = Guid.Parse(match.Groups["guid"].Value),
                    Name = match.Groups["name"].Value,
                    ProjectFile = match.Groups["file"].Value,
                    TypeGuid = Guid.Parse(match.Groups["type"].Value),
                };

                string projectFile = Path.Combine(folder, visualStudioProject.ProjectFile);
                visualStudioProject.ProjectFile = projectFile;
                result.Add(visualStudioProject);

                match = match.NextMatch();
            }

            return result;
        }
    }
}
