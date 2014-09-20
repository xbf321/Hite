using System.ComponentModel;

namespace Hite.Model
{
    public enum ForumApplyStatus
    {
        [Description("申请中")]
        Applying = 0,
        [Description("已通过")]
        Passed = 1,
        [Description("申请被驳回")]
        NoPass = -1
    }
}
