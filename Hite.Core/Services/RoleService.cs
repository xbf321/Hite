using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hite.Model;
using Hite.Data;

namespace Hite.Services
{
    public static class RoleService
    {
        public static List<RoleInfo> List() {
            return RoleManage.List();
        }
    }
}
