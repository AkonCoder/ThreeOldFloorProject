using System.ComponentModel;

namespace ThreeOldFloor.Entity.Enum
{
    public enum ErrorCodeEnum
    {
        [Description("成功")] Success = 0,

        [Description("失败")] Failed = 9000,

        [Description("参数为空")] NullArguments = 9001,

        [Description("无权限操作")] Forbidden = 403
    }
}