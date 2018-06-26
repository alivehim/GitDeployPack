using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDeployPack.Exceptionx;
using Microsoft.Build.Framework;

namespace GitDeployPack.Logger
{
    public class MSBuildLogger : Microsoft.Build.Framework.ILogger
    {
        private readonly Action<string> projectBuildStarted;
        private readonly Action<string, bool, string> projectBuildComplete;
        private readonly Action<string, Exception> errorLogger;
        public LoggerVerbosity Verbosity { get; set; }
        public string Parameters { get; set; }

        public MSBuildLogger(Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete, Action<string, Exception> errorLogger)
        {
            this.projectBuildStarted = projectBuildStarted;
            this.projectBuildComplete = projectBuildComplete;
            this.errorLogger = errorLogger;
        }

        public void Initialize(IEventSource eventSource)
        {
            this.Verbosity = LoggerVerbosity.Diagnostic;
            eventSource.ProjectStarted += EventSourceOnProjectStarted;
            eventSource.ProjectFinished += EventSourceOnProjectFinished;
            eventSource.ErrorRaised += this.EventSourceOnErrorRaised;
        }

        void EventSourceOnErrorRaised(object sender, BuildErrorEventArgs e)
        {
            if (this.errorLogger == null)
            {
                return;
            }

            if (e.ProjectFile.EndsWith(".sln", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            MsBuildException buildException = new MsBuildException("Build error "+e.Message);
            buildException.Data.Add("File", e.File);
            buildException.Data.Add("Code", e.Code);
            buildException.Data.Add("LineNumber", e.LineNumber);
            buildException.Data.Add("ColumnNumber", e.ColumnNumber);
            buildException.Data.Add("Message", e.Message);


            this.errorLogger(e.ProjectFile, buildException);
        }

        private void EventSourceOnProjectStarted(object sender, ProjectStartedEventArgs projectStartedEventArgs)
        {
            if (projectStartedEventArgs.ProjectFile.EndsWith(".sln", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (this.projectBuildStarted != null)
            {
                this.projectBuildStarted(projectStartedEventArgs.ProjectFile);
            }
        }

        private void EventSourceOnProjectFinished(object sender, ProjectFinishedEventArgs projectFinishedEventArgs)
        {
            if (projectFinishedEventArgs.ProjectFile.EndsWith(".sln", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (this.projectBuildComplete != null)
            {
                this.projectBuildComplete(projectFinishedEventArgs.ProjectFile, projectFinishedEventArgs.Succeeded, projectFinishedEventArgs.Message);
            }
        }

        public void Shutdown()
        {

        }
    }
}
