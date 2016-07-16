namespace ThreeOldFloor.Entity.Api
{
    public class UserContext
    {
        public int AccId { get; set; }
        
        public int UserId { get; set; }

        public string OperaterName { get; set; }

        public int Operater { get; set; }

        public string IpAddress { get; set; }

        public string Token { get; set; }

        public string AppKey { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public int Role { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public int Powers { get; set; }

        /// <summary>
        /// 请求跟踪Id
        /// </summary>
        public string TrackingId { get; set; }
    }
}