using System.Collections.Generic;
using Hite.Model;
using System.Data;
using Goodspeed.Library.Data;
using Hite.Common.Reflection;

namespace Hite.Data
{
    internal static class RoleManage
    {
        public static List<RoleInfo> List() {
            string strSQL = "SELECT * FROM Roles WITH(NOLOCK) ORDER BY SiteId ASC";
            DataTable dt = SQLPlus.ExecuteDataTable(CommandType.Text,strSQL);
            List<RoleInfo> list = new List<RoleInfo>();
            if(dt != null && dt.Rows.Count>0){
                foreach(DataRow dr in dt.Rows){
                    list.Add(Get(dr));
                }
            }
            return list;
        }
        public static RoleInfo Get(int id)
        {
            string strSQL = string.Format("SELECT * FROM Roles WITH(NOLOCK) WHERE Id = {0}", id);
            return Get(SQLPlus.ExecuteDataRow(CommandType.Text, strSQL));
        }
        public static RoleInfo Get(DataRow dr)
        {
            RoleInfo model = new RoleInfo();
            ReflectionHelper.Fill(dr, model);
            return model;
        }
    }
}
