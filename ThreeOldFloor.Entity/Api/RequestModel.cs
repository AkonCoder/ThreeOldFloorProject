namespace ThreeOldFloor.Entity.Api
{
    /// <summary>
    /// 请求参数
    /// </summary>
    public class RequestModel
    {
        /// <summary>
        /// AppKey
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// 加密签名
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// 随机数
        /// </summary>
        public string Nonce { get; set; }

        /// <summary>
        /// 店铺ID
        /// </summary>
        public string AccId { get; set; }

        /// <summary>
        /// 登录用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 店铺令牌Token
        /// </summary>
        public string Token { get; set; }
    }
}