using Common.Logging;
using GitDeployPack.Extensions;
using GitDeployPack.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Logger
{
    public class ScreenLogger : ILogger
    {
        private static readonly ILog logger = LogManager.GetLogger("");
        
        public ScreenLogger()
        {
            
        }

        public bool AppendLog(LogLevel level, string shortMessage, string fullMessage)
        {
            if (level == LogLevel.Error)
            {
                Console.WriteLine("\n"+shortMessage + " " + fullMessage);
            }

            else if (level == LogLevel.Info)
            {
                Console.WriteLine("\n" + shortMessage + " " + fullMessage);

            }
            return true;
        }


        public bool AppendLog(PackPeriod level, string message)
        {
            Console.Write($"\r{level}: {message}                       ");
            return true;
        }

        
        public void DebugTrace(string shortMessage )
        {
            DebugTracex(shortMessage);
        }

        [Conditional("DEBUG")]
        private void DebugTracex(string shortMessage)
        {
            AppendLog(LogLevel.Info, shortMessage, "");
        }
    }
}
