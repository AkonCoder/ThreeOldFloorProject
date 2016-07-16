using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThreeOldFloor.Entity.Model.Logging
{
    [Table("RequestLogs")]
    public class RequestLog
    {
        /// <summary>
        /// 请求日志Id
        /// </summary>
        [Key]
        public Guid LogId { get; set; }

        /// <summary>
        /// 父级请求Id
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 请求Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// http方法
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 请求处理耗时 
        /// </summary>
        public long ElapsedMilliseconds { get; set; }

        /// <summary>
        /// 请求来源
        /// </summary>
        public string Refer { get; set; }

        /// <summary>
        /// 请求
        /// </summary>
        public string Request { get; set; }

        /// <summary>
        /// 请求响应
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// IP 地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 头信息
        /// </summary>
        public string Headers { get; set; }


        /// <summary>
        /// 返回状态 
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// 是否有异常
        /// </summary>
        public bool Exception { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        public string ToFormatString()
        {
            return string.Format(@"RequestId:{0} Url:{1} Headers:{2} Request:{3} Response:{4} Status:{5}", LogId, Url , Headers,
                Request, Response, StatusCode);
        }

    }
}
