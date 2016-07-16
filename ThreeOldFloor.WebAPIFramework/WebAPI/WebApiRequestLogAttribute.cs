using System;
using System.Linq;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using ThreeOldFloor.Entity;
using ThreeOldFloor.Entity.Model.Logging;

namespace ThreeOldFloor.WebAPIFramework.WebAPI
{
    public class WebApiRequestLogAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var log =  CreateRequestLog (actionContext);
            var tracker = new RequestTracker(log);

            actionContext.Request.Properties.Add(Constants.RequestLogId, log.LogId);
            actionContext.Request.Properties.Add(this.GetType().FullName, tracker);
            tracker.ProcessActionStart();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            String key = this.GetType().FullName;
            if (!actionExecutedContext.Request.Properties.ContainsKey(key))
            {
                return;
            }

            var tracker = actionExecutedContext.Request.Properties[key] as RequestTracker;
            if (tracker != null)
            {
                bool exceptionThrown = (actionExecutedContext.Exception != null);
                var statusCode = actionExecutedContext.Response != null ? (int)actionExecutedContext.Response.StatusCode : 0;

                tracker.ProcessActionComplete(exceptionThrown, statusCode);
            }
        }

        private RequestLog CreateRequestLog(HttpActionContext actionContext)
        {
            // 排除一些常见的头信息 
            var exceptedHeaderKeys = new string[] {
                "Accept", 
                "Accept-Encoding", 
                "Accept-Language", 
                "Connection", 
                "Cookie", 
                "Host", 
                "Origin", 
                "X-Requested-With", 
                "Referer", 
                "Cache-Control", 
                "User-Agent" };

            var headers = actionContext.Request.Headers.Where(h => !exceptedHeaderKeys.Contains(h.Key));
            var sbHeader = new StringBuilder();

            foreach (var header in headers)
            {
                sbHeader.AppendFormat("{0}={1};", header.Key, string.Join(",", header.Value));
            }

            var log = new RequestLog()
            {
                LogId = Guid.NewGuid(),
                Method = actionContext.Request.Method.ToString(),
                Headers = sbHeader.ToString(),
                Request = BuildReuqestFromActionArguments(actionContext),
                IpAddress = actionContext.Request.GetClientIpAddress(),
                CreatedOnUtc = DateTime.Now,
                Url =  actionContext.Request.RequestUri.AbsolutePath
            };

            if (actionContext.Request.Headers.Referrer != null)
            {
                log.Refer = actionContext.Request.Headers.Referrer.AbsoluteUri;
            }

            return log;
        }


        private string BuildReuqestFromActionArguments(HttpActionContext actionContext)
        {
            if (actionContext.ActionArguments == null || !actionContext.ActionArguments.Any())
                return string.Empty;

            var sbResult = new StringBuilder();
            foreach (var argument in actionContext.ActionArguments)
            {
                sbResult.AppendFormat("{0}={1};", argument.Key, JsonConvert.SerializeObject(argument.Value));
            }

            return sbResult.ToString();
        }
    }
}