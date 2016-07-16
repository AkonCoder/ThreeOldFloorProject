using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ThreeOldFloor.CommonLib;
using ThreeOldFloor.Core;
using ThreeOldFloor.Entity.Api;
using ThreeOldFloor.Entity.Enum;
using ThreeOldFloorApplication.Logging;

namespace ThreeOldFloorApplication.Proxy
{
    public class BaseProxyService
    {
        protected readonly string BaseUrl = ConfigurationManager.AppSettings["DataBase"];

        protected readonly ILogger Logger = new NLogger();


        private ResponseModel ExecuteWithTryCatch(Func<HttpClient, HttpResponseMessage> func, string httpMethod,
            string url, object requestBody, UserContext userContext, Dictionary<string, string> headers)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //var requestLog= new RequestLog();
                //var stopwatch = new Stopwatch();

                //requestLog.LogId = Guid.NewGuid();
                //requestLog.CreatedOnUtc = DateTime.Now;

                HttpResponseMessage response = null;
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                response = func(client);

                //日志捕捉先注释（add by liupeng20160715）
                // try
                // {
                //     requestLog.Headers = BuildHeaderString(headers);
                //     requestLog.ParentId = userContext.TrackingId;
                //     requestLog.Request = requestBody == null ? string.Empty : Helper.JsonSerializeObject(requestBody);
                //     requestLog.Method = httpMethod.ToUpper();

                //     stopwatch.Start();


                //     requestLog.Url = response.RequestMessage.RequestUri.AbsoluteUri;
                //     requestLog.StatusCode = (int)response.StatusCode;
                //     requestLog.Response = response.Content.ReadAsStringAsync().Result;                        
                // }
                //catch (Exception ex)
                // {
                //     requestLog.Exception = true;
                //     throw ex;
                // }
                // finally
                // {
                //     stopwatch.Stop();
                //     requestLog.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                //     Task.Run(() =>
                //     {
                //         _Logger.InsertRequestLog(requestLog);
                //     });
                // }


                ResponseModel responseModel = null;
                if (response.IsSuccessStatusCode)
                {
                    var strResult = response.Content.ReadAsStringAsync().Result;
                    responseModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseModel>(strResult);
                }
                else
                {
                    // 远程请求返回失败
                    var requestException = new ThreeOldFloorException((int) response.StatusCode,
                        "远程请求失败" + response.Content.ReadAsStringAsync().Result);
                    throw requestException;
                }


                if (responseModel != null && responseModel.Code != (int) ResponseErrorcode.C200)
                {
                    var requestException = new ThreeOldFloorException((int) responseModel.Code,
                        "远程处理失败" + responseModel.Message);
                    throw requestException;
                }

                return responseModel;
            }
        }

        protected ResponseModel RestPost<T>(string url, UserContext userContext, T request,
            Dictionary<string, string> headers)
        {
            return ExecuteWithTryCatch(client => client.PostAsJsonAsync(url, request).Result, "post", url, request,
                userContext, headers);
        }

        protected ResponseModel RestPut<T>(string url, UserContext userContext, T request,
            Dictionary<string, string> headers)
        {
            return ExecuteWithTryCatch(client => client.PutAsJsonAsync(url, request).Result, "put", url, request,
                userContext, headers);
        }

        protected ResponseModel RestGet<T>(string url, UserContext userContext, Dictionary<string, string> headers)
        {
            return ExecuteWithTryCatch(client => client.GetAsync(url).Result, "get", url, null, userContext, headers);
        }

        protected ResponseModel RestDelete<T>(string url, UserContext userContext, Dictionary<string, string> headers)
        {
            return ExecuteWithTryCatch(client => client.DeleteAsync(url).Result, "delete", url, null, userContext,
                headers);
        }

        /// <summary>
        /// 填充header
        /// </summary>
        /// <param name="userContext"></param>
        /// <returns></returns>
        protected Dictionary<string, string> RestHead(UserContext userContext)
        {
            var requestMd = CreateAuthCode();
            var dic = new Dictionary<string, string>
            {
                {"BusinessKey", requestMd.AppKey},
                {"Signature", requestMd.Signature},
                {"Timestamp", requestMd.Timestamp},
                {"Nonce", requestMd.Nonce},
                {"AccId", userContext.AccId.ToString()},
                {"Operater", userContext.UserId.ToString()},
                {"OperaterName", userContext.OperaterName},
                {"Ip", userContext.IpAddress}
            };
            return dic;
        }

        /// <summary>
        /// 获取http头信息
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        protected string BuildHeaderString(Dictionary<string, string> headers)
        {
            if (headers == null || !headers.Any())
                return string.Empty;

            var result = new StringBuilder();
            foreach (var header in headers)
            {
                result.AppendFormat("{0}={1};", header.Key, header.Value);
            }

            return result.ToString();
        }

        /// <summary>
        /// 生成验证信息
        /// </summary>
        /// <returns></returns>
        private RequestModel CreateAuthCode()
        {
            string stroBusinessKey = "";
            string strAppValue = "";
            var requestMd = new RequestModel
            {
                Timestamp = Helper.GetTimeStamp(),
                Nonce = Helper.GetRandomNum(),
                AppKey = stroBusinessKey
            };
            var strSign = new StringBuilder();
            strSign.Append(stroBusinessKey);
            strSign.Append(requestMd.Timestamp);
            strSign.Append(requestMd.Nonce);
            strSign.Append(strAppValue);
            requestMd.Signature = Helper.Md5Hash(strSign.ToString());
            return requestMd;
        }
    }
}