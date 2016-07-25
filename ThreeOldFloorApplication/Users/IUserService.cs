using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeOldFloorApplication.Users
{
    /// <summary>
    /// 用户相关接口
    /// </summary>
    public interface IUserService
    {

        public ResponeModel GetUsers(Users user);

        public ResponeModel AddUser(Users user);

        public ResponeModel EditUser(Users user);

        public ResponeModel DeleteUser(Users user);

        public ResponseModel GetUseInfoById(int id);

    }
}
