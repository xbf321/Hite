using System.ComponentModel;

namespace Hite.Model
{
    public enum OrderAdminRoleType
    {
        /// <summary>
        /// 超级管理员
        /// </summary>
        [Description("超级管理员")]
        SuperAdmin = 1,
        /// <summary>
        /// 订单操作员
        /// </summary>
        [Description("订单操作员")]
        OrderOperator = 2
    }
}
