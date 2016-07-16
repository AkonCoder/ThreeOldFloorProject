using System;
using ThreeOldFloor.Entity.Enum;
using ThreeOldFloor.Entity.Model.Logging;

namespace ThreeOldFloorApplication.Logging
{
    public partial interface ILogger
    {
        void InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "" ,string requestLogId="");

        void InsertLog(LogLevel logLevel, Exception exception, string message, params  object[] args);

        void InsertLog(LogLevel logLevel, Exception exception);

        void InsertRequestLog(RequestLog log);
    }
}
