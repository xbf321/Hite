using Hite.Model;
using System.Data.SqlClient;
using Hite.Common.Reflection;
using Goodspeed.Library.Data;
using System.Data;

namespace Hite.Data
{
    internal static class WebLogVisitManage
    {
        public static void Add(WebLogVisitInfo model) {
            string strSQL = "INSERT INTO Visit(Url,Referrer,Querys,IP,UserAgent,VisitTime,SiteId,OS,Brower,UserName) VALUES(@Url,@Referrer,@Querys,@IP,@UserAgent,GETDATE(),@SiteId,@OS,@Brower,@UserName) ";
            SqlParameter[] parms = { 
                                    new SqlParameter("Url",SqlDbType.NVarChar),
                                    new SqlParameter("Referrer",SqlDbType.NVarChar),
                                    new SqlParameter("Querys",SqlDbType.NVarChar),
                                    new SqlParameter("UserAgent",SqlDbType.NVarChar),
                                    new SqlParameter("SiteId",SqlDbType.Int),
                                    new SqlParameter("OS",SqlDbType.NVarChar),
                                    new SqlParameter("Brower",SqlDbType.NVarChar),
                                    new SqlParameter("UserName",SqlDbType.NVarChar),
                                    new SqlParameter("IP",SqlDbType.NVarChar),
                                   };
            parms[0].Value = model.Url;
            parms[1].Value = model.Referrer;
            parms[2].Value = model.Querys;
            parms[3].Value = model.UserAgent;
            parms[4].Value = model.SiteId;
            parms[5].Value = model.OS;
            parms[6].Value = model.Brower;
            parms[7].Value = model.UserName;
            parms[8].Value = model.IP;
            SQLPlus.ExecuteNonQuery("weblog", CommandType.Text, strSQL, parms);
        }
    }
}
