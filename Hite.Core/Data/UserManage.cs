using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hite.Model;
using System.Data.SqlClient;
using System.Data;
using Hite.Common.Reflection;
using Hite.Common;

namespace Hite.Data
{
    internal static class UserManage
    {
        public static int Add(UserInfo model) {
            string strSQL = "INSERT INTO Users(UserName,UserPwd,Email,RealName,Company,Phone,Industry,SiteId,Avatar,LastLoginDateTime,ModifyDateTime,OnlineState) VALUES(@UserName,@Userpwd,@Email,@RealName,@Company,@Phone,@Industry,@SiteId,'',GETDATE(),GETDATE(),1);SELECT @@IDENTITY;";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parms));
        }
        public static void Update(UserInfo model) {
            string strSQL = "UPDATE Users SET Email = @Email,RealName = @RealName,Company = @Company,Phone = @Phone,Industry = @Industry ,ModifyDateTime = @ModifyDateTime,Avatar = @Avatar WHERE Id = @Id";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parms);
        }
        public static void ChangePwd(string userName,string newPwd) {
            string strSQL = "UPDATE Users SET UserPwd = @NewPwd WHERE UserName = @UserName";
            SqlParameter[] parms = { 
                                    new SqlParameter("@UserName",SqlDbType.NVarChar),
                                    new SqlParameter("@NewPwd",SqlDbType.NVarChar),
                                   };
            parms[0].Value = userName;
            parms[1].Value = newPwd;
            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parms);
        }
        public static IPageOfList<UserInfo> List(int pageIndex, int pageSize) {
            FastPaging fp = new FastPaging();
            fp.PageIndex = pageIndex;
            fp.PageSize = pageSize;
            fp.Ascending = false;
            fp.TableName = "Users";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";
            fp.OverOrderBy = "p.LastLoginDateTime DESC";


            IList<UserInfo> list = new List<UserInfo>();
            UserInfo model = null;
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005());
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
            int count = CountForList();
            return new PageOfList<UserInfo>(list, pageIndex, pageSize, count);
        }
        private static int CountForList()
        {
            string strSQL = "SELECT COUNT(*) FROM Users WITH(NOLOCK)";
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, strSQL));
        }
        public static UserInfo Get(string userName) {
            string strSQL = "SELECT * FROM Users WITH(NOLOCK) WHERE UserName = @UserName";
            SqlParameter parm = new SqlParameter("@UserName", userName);
            DataRow dr = Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text,strSQL,parm);
            return Get(dr);
        }
        public static UserInfo Get(int userId) {
            string strSQL = "SELECT * FROM Users WITH(NOLOCK) WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("@Id",userId);
            DataRow dr = Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text, strSQL, parm);
            return Get(dr);
        }
        public static UserInfo Get(DataRow dr)
        {
            UserInfo model = new UserInfo();
            ReflectionHelper.Fill(dr, model);
            return model;
        }
        public static void UpdateLastLoginDateTime(int userId) {
            string strSQL = "UPDATE Users SET LastLoginDateTime = GETDATE(),OnlineState = 1 WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("@Id", userId);
            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parm);
        }
        public static bool ValidateUser(string userName,string userPwd) {
            string strSQL = "SELECT COUNT(*) FROM Users WITH(NOLOCK) WHERE UserName = @UserName AND UserPwd = @UserPwd";
            SqlParameter[] parms = { 
                                    new SqlParameter("@UserName",SqlDbType.NVarChar),
                                    new SqlParameter("@UserPwd",SqlDbType.NVarChar),
                                   };
            parms[0].Value = userName;
            parms[1].Value = userPwd;
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parms)) > 0;
        }
        public static bool IsExistsUserName(string userName) {
            if (string.IsNullOrEmpty(userName)) { return true; }
            string strSQL = "SELECT COUNT(*) FROM Users WITH(NOLOCK) WHERE UserName = @UserName";
            SqlParameter parm = new SqlParameter("@UserName",userName);
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,strSQL,parm)) >0;
        }
        public static bool IsExistsEmail(string email,int id) {
            if (string.IsNullOrEmpty(email)) { return true; }
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT COUNT(*) FROM Users WITH(NOLOCK)");
            sbSQL.Append("  WHERE 1 = 1 ");
            sbSQL.Append("  AND Email = @Email  ");
            if(id > 0){
                sbSQL.Append("  AND ID <> @ID");
            }
            SqlParameter[] parms = { 
                                new SqlParameter("@Email",SqlDbType.VarChar),
                                new SqlParameter("@ID",SqlDbType.Int)
                                };
            parms[0].Value = email;
            parms[1].Value = id;
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, sbSQL.ToString(), parms)) > 0;
        }
    }
}
