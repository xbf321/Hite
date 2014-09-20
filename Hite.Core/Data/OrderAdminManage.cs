using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hite.Model;
using System.Data;
using Hite.Common.Reflection;
using System.Data.SqlClient;
using Goodspeed.Library.Data;
using Hite.Common;

namespace Hite.Data
{
    internal class OrderAdminManage
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Add(OrderAdminInfo model) {
            string strSQL = "INSERT INTO OrderAdmins(UserName,UserPwd,RoleType,IsDeleted,CreateDateTime) VALUES(@UserName,@UserPwd,@RoleType,0,GETDATE());SELECT @@IDENTITY;";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parms));
        }
        public static bool ValidateForLogin(string userName,string userPwd) {
            string strSQL = "SELECT COUNT(*) FROM OrderAdmins WITH(NOLOCK) WHERE UserName = @UserName AND UserPwd = @UserPwd AND IsDeleted = 0";
            SqlParameter[] parms = { 
                                    new SqlParameter("@UserName",SqlDbType.NVarChar),
                                    new SqlParameter("UserPwd",SqlDbType.NVarChar)
                                   };
            parms[0].Value = userName;
            parms[1].Value = userPwd;
            return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parms))>0;
        }
        public static void SetPwd(int userId,string newPwd) {
            string strSQL = "UPDATE OrderAdmins SET UserPwd = @UserPwd WHERE Id = @ID";
            SqlParameter[] parms = { 
                                    new SqlParameter("@ID",SqlDbType.Int),
                                    new SqlParameter("@UserPwd",SqlDbType.NVarChar)
                                   };
            parms[0].Value = userId;
            parms[1].Value = newPwd;
            SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parms);
        }
        
        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="id"></param>
        public static void Delete(int id) {
            string strSQL = "UPDATE OrderAdmins SET IsDeleted = 1 WHERE ID = @ID";
            SqlParameter parm = new SqlParameter("ID", id);
            SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parm);
        }
        public static bool IsExistsUserName(string userName) {
            string strSQL = "SELECT COUNT(*) FROM OrderAdmins WITH(NOLOCK) WHERE UserName = @UserName";
            SqlParameter parm = new SqlParameter("UserName",userName);
            return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parm)) >0;
        }
        /// <summary>
        /// 还原管理员
        /// </summary>
        /// <param name="id"></param>
        public static void Restore(int id) {
            string strSQL = "UPDATE OrderAdmins SET IsDeleted = 0 WHERE ID = @ID";
            SqlParameter parm = new SqlParameter("ID", id);
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parm);
        }
        public static OrderAdminInfo Get(string userName)
        {
            string strSQL = "SELECT * FROM OrderAdmins WITH(NOLOCK) WHERE UserName = @UserName";
            SqlParameter parm = new SqlParameter("UserName",userName);
            DataRow dr = SQLPlus.ExecuteDataRow(CommandType.Text, strSQL, parm);
            return Get(dr);
        }
        public static OrderAdminInfo Get(int id) {
            string strSQL = "SELECT * FROM OrderAdmins WITH(NOLOCK) WHERE Id = @ID";
            SqlParameter parm = new SqlParameter("ID", id);
            DataRow dr = SQLPlus.ExecuteDataRow(CommandType.Text, strSQL, parm);
            return Get(dr);
        }
        private static OrderAdminInfo Get(DataRow dr) {
            OrderAdminInfo model = new OrderAdminInfo();
            ReflectionHelper.Fill(dr, model);
            return model;
        }
        public static IPageOfList<OrderAdminInfo> List(SearchSetting settings)
        {
            FastPaging fp = new FastPaging();
            fp.PageIndex = settings.PageIndex;
            fp.PageSize = settings.PageSize;
            fp.TableName = "OrderAdmins";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";
            fp.OverOrderBy = "  CreateDateTime DESC";
            IList<OrderAdminInfo> list = new List<OrderAdminInfo>();
            OrderAdminInfo model = null;
            DataTable dt = SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005());
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model = Get(dr);
                    if (model != null)
                    {
                        list.Add(model);
                    }
                }
            }
            int count = Count();
            return new PageOfList<OrderAdminInfo>(list, settings.PageIndex, settings.PageSize, count);
        }
        private static int Count()
        {
            StringBuilder sbCondition = new StringBuilder(" SELECT COUNT(*) FROM OrderAdmins WITH(NOLOCK)");
            return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text, sbCondition.ToString()));
        }

    }
}
