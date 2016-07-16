namespace ThreeOldFloor.Entity.Api
{
    /// <summary>
    /// 请求响应(Api)
    /// </summary>
    public class ResponseModel
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 返回的数据实体
        /// </summary>
        public object Data { get; set; }
    } 
}