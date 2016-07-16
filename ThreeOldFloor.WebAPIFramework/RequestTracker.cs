using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ThreeOldFloor.Entity.Model.Logging;
using ThreeOldFloorApplication.Logging;

namespace ThreeOldFloor.WebAPIFramework
{
    public class RequestTracker
    {
        private readonly RequestLog _requestLog;
        private Stopwatch _stopwatch;
        private readonly ILogger _iLogger = new NLogger();

        public RequestTracker(RequestLog info)
        {
            this._requestLog = info;
        }


        public void ProcessActionStart()
        {
            this._stopwatch = Stopwatch.StartNew();
        }


        public void ProcessActionComplete(bool exceptionThrown, int statusCode)
        {
            try
            {
                this._stopwatch.Stop();
                _requestLog.ElapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
                _requestLog.StatusCode = statusCode;
                _requestLog.Exception = exceptionThrown;

                Task.Run(() => { _iLogger.InsertRequestLog(_requestLog); });
            }
            catch (Exception ex)
            {
                String message =
                    String.Format(
                        "Exception {0} occurred PerformanceTracker.ProcessActionComplete().  Message {1}\nStackTrace {0}",
                        ex.GetType().FullName, ex.Message, ex.StackTrace);
                Trace.WriteLine(message);
            }
        }
    }
}