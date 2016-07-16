namespace ThreeOldFloor.Core.Config
{
    public interface ISetting
    {
        /// <summary>
        /// Redis缓存服务器地址
        /// </summary>
        string RedisHost { get; set; }

        /// <summary>
        /// 老三栋数据库记录
        /// </summary>
        string ThreeOldFloorDbConnectionString { get; set; }

        /// <summary>
        /// API 日志记录数据库
        /// </summary>
        string WebApiLogDbConnection { get; set; }
    }
}
