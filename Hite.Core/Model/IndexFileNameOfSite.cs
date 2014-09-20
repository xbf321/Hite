using System.ComponentModel;

namespace Hite.Model
{
    /// <summary>
    /// 各个站点的首页文件名
    /// 样式也是按照这个名称
    /// </summary>
    public enum IndexFileNameOfSite
    {
        [Description("==请选择==")]
        None = 0,
        /// <summary>
        /// WWW站点
        /// </summary>
        [Description("WWW站点")]
        WWW = 1,
        /// <summary>
        /// 成套站点
        /// </summary>
        [Description("成套站点")]
        Distribution = 2,
        /// <summary>
        /// 电气站点
        /// </summary>
        [Description("电气站点")]
        Electric = 3,
        /// <summary>
        /// 新能源站点
        /// </summary>
        [Description("新能源站点")]
        Energy = 4,
        /// <summary>
        /// 起重站点
        /// </summary>
        [Description("起重站点")]
        Hoisting = 5,
        /// <summary>
        /// 机电一体化站点
        /// </summary>
        [Description("机电一体化站点")]
        Mechatronics = 6,
        /// <summary>
        /// 系统业务站点
        /// </summary>
        [Description("系统业务站点")]
        Systematic = 7,
        /// <summary>
        /// 海思科站点
        /// </summary>
        [Description("海思科站点")]
        Hiscom = 8,
        /// <summary>
        /// 成都海得站点
        /// </summary>
        [Description("成都海得站点")]
        Chengdu = 9,
        /// <summary>
        /// 成都海得站点
        /// </summary>
        [Description("上海嘉仪")]
        Jiayi = 10
    }
}
