using System;
using ThreeOldFloor.Entity.Enum;
using ThreeOldFloor.Entity.Model.Logging;

namespace ThreeOldFloorApplication.Logging
{
    public static class LoggingExtensions
    {
        public static void Debug(this ILogger logger, string message, Exception exception = null)
        {
            FilteredLog(logger, LogLevel.Debug, message, exception);
        }
        public static void Information(this ILogger logger, string message, Exception exception = null )
        {
            FilteredLog(logger, LogLevel.Information, message, exception);
        }
        public static void Warning(this ILogger logger, string message, Exception exception = null)
        {
            FilteredLog(logger, LogLevel.Warning, message, exception);
        }
        public static void Error(this ILogger logger, string message, Exception exception = null)
        {
            FilteredLog(logger, LogLevel.Error, message, exception);
        }

        public static void Error(this ILogger logger, string message, string requestLogId, Exception exception = null)
        {
            FilteredLog(logger, LogLevel.Error, message, requestLogId, exception);
        }

        public static void Fatal(this ILogger logger, string message, Exception exception = null )
        {
            FilteredLog(logger, LogLevel.Fatal, message, exception);
        }

        public static void LogRequest(this ILogger logger, RequestLog requestLog)
        {
            logger.InsertRequestLog(requestLog);
        }

        private static void FilteredLog(ILogger logger, LogLevel level, string message, Exception exception = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            string fullMessage = exception == null ? string.Empty : exception.ToString();
            logger.InsertLog(level, message, fullMessage);
            
        }

        private static void FilteredLog(ILogger logger, LogLevel level, string message, string requestLogId, Exception exception = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            string fullMessage = exception == null ? string.Empty : exception.ToString();
            logger.InsertLog(level, message, fullMessage, requestLogId);
            
        }
    }
}
