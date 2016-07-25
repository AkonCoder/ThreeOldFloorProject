using System.ComponentModel;
using System.Reflection;
using System.Web.Http;
using ThreeOldFloor.Entity.Api;
using ThreeOldFloor.Entity.Enum;

namespace ThreeOldFloor.Controllers
{
    public class BaseApiController : ApiController
    {
        private const string HttpContextKey = "MS_HttpContext";

        private const string RemoteEndpointMessage =
            "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

        private const string OwinContext = "MS_OwinContext";

        protected UserContext GetUserContext()
        {
            return new UserContext();
        }

        protected ResponseModel Success(object data)
        {
            if (data != null && data is ResponseModel)
            {
                var result = (ResponseModel) data;
                result.Code = result.Code;
                result.Message = result.Message;
                return result;
            }

            return new ResponseModel()
            {
                Code = (int) ErrorCodeEnum.Success,
                Message = string.Empty,
                Data = data
            };
        }

        protected ResponseModel Fail(ErrorCodeEnum errorCode)
        {
            string errorDescription = "未知错误";
            MemberInfo[] memberInfo = (typeof (ErrorCodeEnum)).GetMember(errorCode.ToString());
            if (memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof (DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    //返回枚举值得描述信息
                    errorDescription = ((DescriptionAttribute) attrs[0]).Description;
                }
            }

            return new ResponseModel()
            {
                Code = (int) errorCode,
                Message = errorDescription,
            };
        }
    }
}