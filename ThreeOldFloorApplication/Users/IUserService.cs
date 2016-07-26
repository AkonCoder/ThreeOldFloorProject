using ThreeOldFloor.Entity.Api;

namespace ThreeOldFloorApplication.Users
{
    /// <summary>
    /// 用户相关接口
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        ///获取用户列表
        /// </summary>
        /// <returns></returns>
        ResponseModel GetUsers();

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        ResponseModel AddUser(User user);

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        ResponseModel EditUser(User user);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        ResponseModel DeleteUser(User user);

        /// <summary>
        /// 根据id获取用户详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel GetUseInfoById(int id);
    }
}