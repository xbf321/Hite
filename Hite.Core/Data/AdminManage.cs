using System;
using System.Data;
using System.Linq;
using System.Data.SqlClient;


using Hite.Model;
using Hite.Common.Reflection;
using Goodspeed.Library.Data;
using System.Collections.Generic;
using Hite.Common;
using System.Text;

namespace Hite.Data
{
    internal static class AdminManage
    {
        /// <summary>
        /// Add
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Add(AdminInfo model) {
            string strSQL = "INSERT INTO Admins(UserName,UserPwd,IsEnabled,IsDeleted,CreateDateTime) VALUES(@UserName,@UserPwd,@IsEnabled,@IsDeleted,GETDATE());SELECT @@IDENTITY;";
            SqlParameter[] parms = { 
                                    new SqlParameter("UserName",SqlDbType.NVarChar),
                                    new SqlParameter("UserPwd",SqlDbType.NVarChar),
                                    new SqlParameter("IsDeleted",SqlDbType.Bit),
                                    new SqlParameter("IsEnabled",SqlDbType.Bit)
                                   };
            parms[0].Value = model.UserName;
            parms[1].Value = model.UserPwd;
            parms[2].Value = model.IsDeleted;
            parms[3].Value = model.IsEnabled;
            int id = Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parms));
            
            //Delete AdminInRoles By AdminId
            DeleteAdminInRolesByAdminId(id);
            //Insert AdminInRoles 表
            model.Id = id;
            InsertAdminInRoles(model);
            return id;
        }
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="model"></param>
        public static void Update(AdminInfo model) {
            string strSQL = "UPDATE Admins SET IsEnabled = @IsEnabled WHERE ID = @ID";
            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.NVarChar),
                                    new SqlParameter("UserPwd",SqlDbType.NVarChar),
                                    new SqlParameter("IsEnabled",SqlDbType.Bit)
                                   };
            parms[0].Value = model.Id;
            parms[1].Value = model.UserPwd;
            parms[2].Value = model.IsEnabled;
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);

            //Delete AdminInRoles By AdminId
            DeleteAdminInRolesByAdminId(model.Id);
            //Insert AdminInRoles 表
            InsertAdminInRoles(model);

        }
        /// <summary>
        /// 是否存在用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool IsExistsUser(string userName) {
            string strSQL = "SELECT COUNT(*) FROM Admins WITH(NOLOCK) WHERE UserName = @UserName";
            SqlParameter parm = new SqlParameter("UserName",userName);
            return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text,strSQL,parm)) > 0;
        }
        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="pwd"></param>
        public static void SetPwd(int adminId,string pwd) {
            string strSQL = "UPDATE Admins SET UserPwd = @UserPwd WHERE Id = @ID";
            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.NVarChar),
                                    new SqlParameter("UserPwd",SqlDbType.NVarChar)
                                   };
            parms[0].Value = adminId;
            parms[1].Value = pwd;
            SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parms);
        }
        public static IPageOfList<AdminInfo> List(SearchSetting settings)
        {
            FastPaging fp = new FastPaging();
            fp.PageIndex = settings.PageIndex;
            fp.PageSize = settings.PageSize;
            fp.TableName = "Admins";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";

            StringBuilder sbCondition = new StringBuilder();
            if (!settings.ShowDeleted)
            {
                sbCondition.Append("  IsDeleted = 0 ");
            }
            fp.Condition = sbCondition.ToString();
            fp.OverOrderBy = " CreateDateTime DESC";
            IList<AdminInfo> list = new List<AdminInfo>();
            AdminInfo model = null;
            DataTable dt = SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005());
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model = Get(dr,true);
                    if (model != null)
                    {
                        list.Add(model);
                    }
                }
            }
            int count = Count(settings);
            return new PageOfList<AdminInfo>(list, settings.PageIndex, settings.PageSize, count);
        }
        private static int Count(SearchSetting settings)
        {
            StringBuilder sbCondition = new StringBuilder(" SELECT COUNT(*) FROM Admins WITH(NOLOCK) WHERE 1 = 1");
            if (!settings.ShowDeleted)
            {
                sbCondition.Append("    AND IsDeleted = 0 ");
            }
            return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text, sbCondition.ToString()));
        }

        #region == Get Info ==
        public static AdminInfo Get(string userName) {
            return Get(userName,false);
        }
        public static AdminInfo Get(string userName,bool isLoadRoles) {
            string strSQL = "SELECT * FROM Admins WITH(NOLOCK) WHERE UserName = @UserName";
            SqlParameter parm = new SqlParameter("UserName",userName);
            DataRow dr = SQLPlus.ExecuteDataRow(CommandType.Text,strSQL,parm);
            return Get(dr, isLoadRoles);
        }
        public static AdminInfo Get(int id) {
            return Get(id,false);
        }
        public static AdminInfo Get(int id,bool isLoadRoles)
        {
            string strSQL = string.Format("SELECT * FROM Admins WITH(NOLOCK) WHERE Id = {0}", id);
            return Get(SQLPlus.ExecuteDataRow(CommandType.Text, strSQL),isLoadRoles);
        }
        public static AdminInfo Get(DataRow dr) {
            return Get(dr,false);
        }
        public static AdminInfo Get(DataRow dr,bool isLoadRoles)
        {
            AdminInfo model = new AdminInfo();
            if(dr !=null){
                model.Id = dr.Field<int>("Id");
                model.IsDeleted = dr.Field<bool>("IsDeleted");
                model.IsEnabled = dr.Field<bool>("IsEnabled");
                model.UserName = dr.Field<string>("UserName");
                model.UserPwd = dr.Field<string>("UserPwd");
                model.CreateDateTime = dr.Field<DateTime>("CreateDateTime");
                if (isLoadRoles)
                {
                    //是否加载角色
                    model.Roles = GetRolesByAdminId(model.Id);
                }
                
            }
            return model;
        }
        #endregion

        #region == AdminInRoles ==
        public static List<RoleInfo> GetRolesByAdminId(int adminId) {
            string strSQL = "SELECT * FROM AdminInRoles WITH(NOLOCK) WHERE AdminId = @AdminId";
            SqlParameter parm = new SqlParameter("AdminId",adminId);
            DataTable dt = SQLPlus.ExecuteDataTable(CommandType.Text,strSQL,parm);
            List<RoleInfo> roleList = new List<RoleInfo>();
            if(dt != null && dt.Rows.Count >0){
                foreach(DataRow dr in dt.Rows){
                    //引用RoleManage.cs
                    roleList.Add(RoleManage.Get(dr.Field<int>("roleId")));
                }
            }
            return roleList;
        }
        public static void DeleteAdminInRolesByAdminId(int adminId) {
            string strSQL = "DELETE AdminInRoles WHERE AdminId = @AdminId";
            SqlParameter parm = new SqlParameter("AdminId",adminId);
            SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parm);
        }
        public static void InsertAdminInRoles(AdminInfo model) {
            List<RoleInfo> list = model.Roles;
            int adminId = model.Id;
            if(adminId > 0 && list.Count > 0){
                SqlParameter[] parms = { 
                                            new SqlParameter("AdminId",SqlDbType.Int),
                                            new SqlParameter("RoleId",SqlDbType.Int),
                                           };
                parms[0].Value = adminId;
                foreach(var item in list){
                    string strSQL = "INSERT INTO AdminInRoles(AdminId,RoleId) VALUES(@AdminId,@RoleId)";
                    parms[1].Value = item.Id;
                    SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parms);
                }
            }
        }
        #endregion
    }
}
