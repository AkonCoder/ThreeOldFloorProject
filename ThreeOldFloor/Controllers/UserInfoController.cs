using System.Web.Http;
using ThreeOldFloor.Entity.Api;

namespace ThreeOldFloor.Controllers
{
    /// <summary>
    /// 用户信息相关接口
    /// </summary>
    [RoutePrefix("v0")]
    public class UserInfoController
    {
        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        /// <returns></returns>
        [Route("users")]
        [HttpGet, HttpOptions]
        public ResponseModel GetUsers()
        {
            return new ResponseModel();
        }

        /// <summary>
        /// 根据Id获取用户详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("users/{id}")]
        [HttpGet, HttpOptions]
        public ResponseModel GetUserById(int id)
        {
            return new ResponseModel();
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <returns></returns>
        [Route("users")]
        [HttpPost, HttpOptions]
        public ResponseModel AddUser(User user)
        {
            if (user == null)
            {
                return new ResponseModel
                {
                    Code = 0,
                    Message = "参数错误",
                    Data = null
                };

            }
            return new ResponseModel();
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <returns></returns>
        [Route("users")]
        [HttpPut, HttpOptions]
        public ResponseModel EditUser()
        {
            return new ResponseModel();
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("users/{id}")]
        [HttpDelete, HttpOptions]
        public ResponseModel DeleteUser(int id)
        {
            return new ResponseModel();
        }
    }
}