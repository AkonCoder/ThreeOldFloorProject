using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using ThreeOldFloor.Entity.Api;
using ThreeOldFloor.Entity.Enum;

namespace ThreeOldFloor.WebAPIFramework.WebAPI
{
    public class BadRequestAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null)
                return;

            // 对于本地返回 全局更改 http status code到400 
            var responseModel = actionExecutedContext.Response.Content.ReadAsAsync<ResponseModel>().Result;
            if (responseModel != null)
            {
                if (responseModel.Code != (int) ResponseErrorcode.C200 &&
                    responseModel.Code != (int) ErrorCodeEnum.Success)
                {
                    actionExecutedContext.Response.StatusCode = HttpStatusCode.BadRequest;
                }
                if (responseModel.Code == (int)ResponseErrorcode.C404)
                {
                    actionExecutedContext.Response.StatusCode = HttpStatusCode.NotFound;
                }
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}