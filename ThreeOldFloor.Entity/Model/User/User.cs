using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeOldFloor.Entity.Api
{
    [Table("User")]
    public class User
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [Key]
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public int SectionID { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// 密码
        /// </summary>

        public string Password { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int DicStatus { get; set; }

        /// <summary>
        /// 照片路径
        /// </summary>
        public string PhonePath { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int DiCSex { get; set; }

        /// <summary>
        /// 地址Id
        /// </summary>
        public int DicAddress { get; set; }

    }
}