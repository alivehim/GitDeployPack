using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxConsole
{
    public class GitUtility
    {
        private static string EnvironmentVariable
        {
            get
            {
                string sPath = System.Environment.GetEnvironmentVariable("Path");
                var result = sPath.Split(';');
                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i].Contains(@"Git\bin"))
                    {
                        sPath = result[i];
                    }
                }
                return sPath;
            }
        }


        public static void GetCommitID()
        {
            string gitPath = @"C:\Program Files\Git\bin\" + "git.exe";
            Process p = new Process();
            p.StartInfo.FileName = gitPath;
            p.StartInfo.Arguments = "rev-parse HEAD";
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.OutputDataReceived += OnOutputDataReceived;
            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();

        }

        private static void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e != null && !string.IsNullOrEmpty(e.Data))
            {
               Console.WriteLine(e.Data);
            }
        }
    }
}
