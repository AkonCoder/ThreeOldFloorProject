using System.Web.Http;
using ThreeOldFloor.Entity.Api;

namespace ThreeOldFloor.Controllers
{
    /// <summary>
    /// 角色相关接口
    /// </summary>
    [RoutePrefix("v0")]
    public class RoleController : ApiController
    {
        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        [HttpGet,HttpOptions]
        [Route("roles")]
        public ResponseModel GetRoleList()
        {
            return new ResponseModel();
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <returns></returns>
        [Route("roles")]
        [HttpPost,HttpOptions]
        public  ResponseModel   AddRole()
        {
            return new ResponseModel();
        }

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <returns></returns>
        [Route("roles")]
        [HttpPut,HttpOptions]
        public ResponseModel UpdateRole()
        {
            return new ResponseModel();
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        [Route("roles/{id}")]
        [HttpDelete,HttpOptions]
        public ResponseModel DeleteRole(int id)
        {
            return new ResponseModel();
        }

    }
}