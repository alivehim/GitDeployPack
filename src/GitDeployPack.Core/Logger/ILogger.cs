using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GitDeployPack.Model;

namespace GitDeployPack.Logger
{
    public interface ILogger
    {
        bool AppendLog(LogLevel level, string shortMessage, string fullMessage);

        bool AppendLog(PackPeriod level, string message);

        void DebugTrace(string shortMessage);
    }
}
