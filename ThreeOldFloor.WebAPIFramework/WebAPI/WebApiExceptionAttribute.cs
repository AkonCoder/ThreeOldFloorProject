using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Filters;
using Newtonsoft.Json.Serialization;
using ThreeOldFloor.Core;
using ThreeOldFloor.Entity;
using ThreeOldFloor.Entity.Api;
using ThreeOldFloorApplication.Logging;

namespace ThreeOldFloor.WebAPIFramework.WebAPI
{

    /// <summary>
    /// 异常拦截器
    /// </summary>
    public class WebApiExceptionAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger _logger = new NLogger();

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var responseModel = new ResponseModel();

            var jsonFormatter = new JsonMediaTypeFormatter();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var exception = actionExecutedContext.Exception as HttpResponseException;
            if (exception != null && exception.Response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }

            var httpRequestException = actionExecutedContext.Exception as ThreeOldFloorHttpRequestException;
            if (httpRequestException != null)
            {
                responseModel.Code = httpRequestException.Code;
                responseModel.Message = httpRequestException.Message;

                actionExecutedContext.Response = new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new ObjectContent<ResponseModel>(responseModel, jsonFormatter, "application/json"),
                };
            }

            var message = actionExecutedContext.Exception.Message;
            if (actionExecutedContext.Response == null && !(actionExecutedContext.Exception is ThreeOldFloorException))
            {
                var resultModel = new ResponseModel
                {
                    Code = (int) HttpStatusCode.InternalServerError,
                    Message = message
                };

                actionExecutedContext.Response = new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content =  new ObjectContent<ResponseModel>(resultModel, jsonFormatter, "application/json"),
                };
            }

            if (actionExecutedContext.Exception != null)
            {
                var requestLogId = actionExecutedContext.Request.Properties.ContainsKey(Constants.RequestLogId)
                    ? actionExecutedContext.Request.Properties[Constants.RequestLogId].ToString()
                    : string.Empty;

                _logger.Error(actionExecutedContext.Exception.Message, requestLogId, actionExecutedContext.Exception);
                
            }
        }
    }

}
