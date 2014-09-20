using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Hite.Model
{
    public enum OrderStatus
    {
        [Description("设计")]
        A = 1,
        [Description("生产中")]
        B = 2,
        [Description("部分完工")]
        C = 3,
        [Description("完工待发货")]
        D = 4,
        [Description("已发货")]
        E = 5,
        [Description("已开票")]
        F = 6
    }
}
