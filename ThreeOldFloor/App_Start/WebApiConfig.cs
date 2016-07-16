using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using ThreeOldFloor.WebAPIFramework.WebAPI;

namespace ThreeOldFloor
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // 移除XML格式输出
            var json = config.Formatters.JsonFormatter;

            // 解决json序列化时的循环引用问题
            json.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            // 禁用XML序列化器
            config.Formatters.XmlFormatter.UseXmlSerializer = false;

            // 使用attribute路由规则 
            config.MapHttpAttributeRoutes();

            // 为支持CORS，默认不处理任何options的请求
            config.MessageHandlers.Add(new IgnoreOptionsRequestHandler());

            //增加过滤器
            config.Filters.Add(new WebApiAuthAttribute());

            // 添加性能记录器（日志记录Api请求时间，测试阶段关闭）
            // config.Filters.Add(new WebApiRequestLogAttribute());

            config.Filters.Add(new WebApiExceptionAttribute());
            config.Filters.Add(new BadRequestAttribute());

            //日期格式
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new MyDateTimeConvertor());

            ////设置json对象的首字符小写
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // 目前Jil使用不是很完备，后期会加进去，Jil提升序列化速度
            // config.Formatters.Insert(0,new JilMediaFormat());
        }

        /// <summary>
        /// 自定义时间格式
        /// </summary>
        public class MyDateTimeConvertor : IsoDateTimeConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(((DateTime) value).ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
    }
}