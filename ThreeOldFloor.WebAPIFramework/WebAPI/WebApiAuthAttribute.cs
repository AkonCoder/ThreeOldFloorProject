using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using ThreeOldFloor.CommonLib;
using ThreeOldFloor.Entity.Api;

namespace ThreeOldFloor.WebAPIFramework.WebAPI
{
    public class WebApiAuthAttribute : AuthorizationFilterAttribute
    {
        private readonly Dictionary<string, string> _appKeys = new Dictionary<string, string>()
        {
            {"iPadMaO8VUvVH0eBss", "HOMe70yWgHjwevu8BwXoRHBPTQCk5z8p"},
            {"iPhoneHT5I0O4HDN65", "XKCE9P34TSRITS0W18RX6PK07MALZJ7Y"},
            {"AndroidYnHWyROQosO", "LLIhqKis7Leim1PLe9JOhFbFFHJlz5FR"}
        };

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //签名校验
            var request = actionContext.Request.Headers;
            var responseModel = new ResponseModel();
            var oSignature = request.SingleOrDefault(x => x.Key.ToLower() == "signature");
            var oTimestamp = request.SingleOrDefault(x => x.Key.ToLower() == "timestamp");
            var oNonce = request.SingleOrDefault(x => x.Key.ToLower() == "nonce");
            var oAppKey = request.SingleOrDefault(x => x.Key.ToLower() == "appkey");
            var oToken = request.SingleOrDefault(x => x.Key.ToLower() == "token");

            var strSignature = (oSignature.Key == null ? "" : oSignature.Value.FirstOrDefault());
            var strTimestamp = (oTimestamp.Key == null ? "0" : oTimestamp.Value.FirstOrDefault());
            var strNonce = (oNonce.Key == null ? "" : oNonce.Value.FirstOrDefault());
            var strAppKey = (oAppKey.Key == null ? "" : oAppKey.Value.FirstOrDefault());
            var strToken = (oToken.Key == null ? "" : oToken.Value.FirstOrDefault());


            //缺少Token值
            if (string.IsNullOrWhiteSpace(strToken))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, responseModel);
                actionContext.Response.Headers.Add("Unauthorized", "缺少Token值");
                actionContext.Response.StatusCode = HttpStatusCode.BadRequest;
                responseModel.Code = 401;
                responseModel.Message = "缺少Token值";
                responseModel.Data = null;
                return;
            }

            if (!string.IsNullOrWhiteSpace(strAppKey) && _appKeys.ContainsKey(strAppKey))
            {
                var strAppValue = _appKeys[strAppKey];
                var strSign = new StringBuilder();
                strSign.Append(strAppKey);
                strSign.Append(strTimestamp);
                strSign.Append(strNonce);
                strSign.Append(strAppValue);
                long timeSpan = Convert.ToInt64(Helper.GetTimeStamp()) - Convert.ToInt64(strTimestamp);
                var strAuthCode = Helper.Md5Hash(strSign.ToString());
                if (strSignature != null && (strAuthCode.ToUpper() != strSignature.ToUpper() || timeSpan > 3*60*1000))
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden,
                        responseModel);
                    //签名未通过
                    actionContext.Response.Headers.Add("Forbidden", "请求验证未通过");
                    actionContext.Response.StatusCode = HttpStatusCode.Forbidden;
                    responseModel.Code = 403;
                    responseModel.Message = "请求验证未通过";
                    responseModel.Data = null;
                    return;
                }
            }
        }
    }
}