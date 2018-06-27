using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Model
{
    public class ProjectDescription
    {
        public string ProjectGuid { get; set; }

        public string AssemblyName { get; set; }
        public VsProjectType ProjectType { get; set; } = VsProjectType.Undefined;

        /// <summary>
        /// location of binary files
        /// </summary>
        public string BinLocation { get; set; }
        /// <summary>
        /// Project Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Full Path of Project
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Indicating whether Project File was changed 
        /// </summary>
        public bool IsChanged { get; set; }

        /// <summary>
        /// Indicating whether to complile the project  
        /// </summary>
        public bool IsNeedCompile { get; set; } = false;

        /// <summary>
        /// the location of project file
        /// </summary>
        public DirectoryInfo Location { get; set; }

        /// <summary>
        /// The Path of output 
        /// </summary>
        public string OutputPath { get; set; }

        public string OutputType { get; set; }

        public string OutputName
        {
          

            get
            {
                Func<string> getext = () => {
                    switch (OutputType)
                    {
                        case "Library":
                            return "dll";
                        case "Exe":
                            return "exe";
                    }
                    return "dll";
                };

                return $"{AssemblyName}.{getext()}";
            }
        }
        

        /// <summary>
        /// the collection of compile files
        /// </summary>
        public IList<string> CompileFiles { get; set; } = new List<string>();

        public IList<string> ContentFiles { get; set; } = new List<string>();

        public IList<string> ReferenceProjects { get; set; } = new List<string>();

        public IList<string> ReferenceAssembly { get; set; } = new List<string>();


        public ProjectDescription RelevanceProject { get; set; }

        /// <summary>
        /// the collection of static files
        /// </summary>
        public IList<string> StaticFiles { get; set; } = new List<string>();

        public IList<string> HtmlFiles { get; set; } = new List<string>();


        public override string ToString()
        {
            return $"{AssemblyName}:{ProjectType}";
        }
    }
}
