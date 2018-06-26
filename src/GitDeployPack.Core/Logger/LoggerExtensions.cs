using Common.Logging;
using System;

namespace GitDeployPack.Logger
{
    /// <summary>
    /// 日志扩展
    /// </summary>
    public static class LoggerExtensions
    {
        public static bool Debug(this ILogger logger, string shortMessage, Exception exception = null)
        {
            return Log(logger, LogLevel.Debug, shortMessage, exception);
        }

        public static bool Information(this ILogger logger, string shortMessage, Exception exception = null)
        {
            return Log(logger, LogLevel.Info, shortMessage, exception);
        }

        public static bool Warning(this ILogger logger, string shortMessage, Exception exception = null)
        {
            return Log(logger, LogLevel.Warn, shortMessage, exception);
        }

        public static bool Error(this ILogger logger, string shortMessage, Exception exception = null)
        {
            return Log(logger, LogLevel.Error, shortMessage, exception);
        }

        public static bool Fatal(this ILogger logger, string shortMessage, Exception exception = null)
        {
            return Log(logger, LogLevel.Fatal, shortMessage, exception);
        }


        private static bool Log(ILogger logger, LogLevel level, string shortMessage, Exception exception = null)
        {
            if (exception is System.Threading.ThreadAbortException)
                return true;

            var fullMessage = null == exception ? string.Empty : exception.ToString();
            return logger.AppendLog(level, shortMessage, fullMessage);
        }
     
    }
}
