using System;
using NLog;
using ThreeOldFloor.Entity.Model.Logging;
using LogLevel = ThreeOldFloor.Entity.Enum.LogLevel;

namespace ThreeOldFloorApplication.Logging
{
    public class NLogger : ILogger
    {
        private readonly Logger _logger;

        public NLogger()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }


        private NLog.LogLevel GetNLoggerLever(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Information:
                    return NLog.LogLevel.Info;
                case LogLevel.Error:
                    return NLog.LogLevel.Error;
                case LogLevel.Debug:
                    return NLog.LogLevel.Debug;
                case LogLevel.Fatal:
                    return NLog.LogLevel.Fatal;
                case LogLevel.Off:
                    return NLog.LogLevel.Off;
                case LogLevel.Warning:
                    return NLog.LogLevel.Warn;
                default:
                    return NLog.LogLevel.Trace;
            }
        }


        public void InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", string requestLogId = "")
        {
            try
            {
                _logger.Log(GetNLoggerLever(logLevel), shortMessage);
            }
            catch (Exception ex)
            {
                // 记日志出错不能再抛出
            }
        }

        public void InsertLog(LogLevel logLevel, Exception exception, string message, params object[] args)
        {
            try
            {
                _logger.Log(GetNLoggerLever(logLevel), exception, message, args);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void InsertLog(LogLevel logLevel, Exception exception)
        {
            try
            {
                _logger.Log(GetNLoggerLever(logLevel), exception);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void InsertRequestLog(RequestLog log)
        {
            try
            {
                _logger.Log(NLog.LogLevel.Info, log.ToFormatString());
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}