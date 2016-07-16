using System.Configuration;

namespace ThreeOldFloor.Core.Config
{
    public sealed class WebConfigSetting
    {
        private static WebConfigSetting _instance = null;
        private static readonly object Padlock = new object();

        public static WebConfigSetting Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Padlock)
                    {
                        if (_instance == null)
                        {
                            _instance = InitConfiguration();
                        }
                    }
                }
                return _instance;
            }
        }


        private static WebConfigSetting InitConfiguration()
        {
            return new WebConfigSetting()
            {
                RedisHost = ConfigurationManager.AppSettings["RedisHost"],
                ThreeOldFloorDbConnectionString = ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString,
                WebApiLogDbConnection =
                    ConfigurationManager.ConnectionStrings["WebApiPerformance.ConnectionString"].ConnectionString,
            };
        }

        /// <summary>
        /// Redis缓存服务器地址
        /// </summary>
        public string RedisHost { get; set; }

        /// <summary>
        /// 老三栋数据库记录
        /// </summary>
        public string ThreeOldFloorDbConnectionString { get; set; }

        /// <summary>
        /// API 日志记录数据库
        /// </summary>
        public string WebApiLogDbConnection { get; set; }
    }
}