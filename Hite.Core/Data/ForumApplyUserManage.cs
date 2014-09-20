using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hite.Model;
using Hite.Common.Reflection;
using System.Data.SqlClient;
using System.Data;
using Hite.Common;

namespace Hite.Data
{
    public static class ForumApplyUserManage
    {
        public static int Add(ForumApplyUserInfo model) {
            string strSQL = "INSERT INTO ForumApplyUsers(UserId,UserName,ForumGroupId,ContactPerson,[Status],CreateDateTime) VALUES(@UserId,@UserName,@ForumGroupId,@ContactPerson,0,GETDATE());SELECT @@IDENTITY;";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parms));
        }        
        //public static void Applying(List<ForumApplyUserInfo> modelList) {
        //    string strSQL = "INSERT INTO ForumApplyUsers(UserId,UserName,ForumGroupId,ContactPerson,[Status],CreateDateTime) VALUES(@UserId,@UserName,@ForumGroupId,@ContactPerson,0,GETDATE());";
        //    SqlParameter[] parms = null;
        //    foreach(var model in modelList){
        //        parms  = ParameterHelper.GetClassSqlParameters(model);
        //        Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parms);
        //    }
        //}
        public static IPageOfList<ForumApplyUserInfo> List(int pageIndex, int pageSize) {

            FastPaging fp = new FastPaging();
            fp.PageIndex = pageIndex;
            fp.PageSize = pageSize;
            fp.Ascending = false;
            fp.TableName = "ForumApplyUsers";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*,g.Name AS ForumGroupName";
            fp.OverOrderBy = "p.CreateDateTime DESC";
            fp.JoinSQL = "LEFT JOIN dbo.ForumGroups AS g ON p.ForumGroupId = g.Id";


            IList<ForumApplyUserInfo> list = new List<ForumApplyUserInfo>();
            ForumApplyUserInfo model = null;
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
            return new PageOfList<ForumApplyUserInfo>(list, pageIndex, pageSize, count);
        }
        private static int CountForList() {
            string strSQL = "SELECT COUNT(*) FROM ForumApplyUsers WITH(NOLOCK)";
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,strSQL));
        }
        public static void Update(ForumApplyUserInfo model) {
            string strSQL = "UPDATE ForumApplyUsers SET ContactPerson = @ContactPerson,[Status] = @Status WHERE Id = @Id";
            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("ContactPerson",SqlDbType.NVarChar),
                                    new SqlParameter("Status",SqlDbType.Int)
                                   };
            parms[0].Value = model.Id;
            parms[1].Value = model.ContactPerson;
            parms[2].Value = (int)model.Status;
            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
        }
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        public static void UpdateStatus(int id,ForumApplyStatus status) {
            string strSQL = "UPDATE ForumApplyUsers SET [Status] = @Status WHERE Id = @Id";
            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("Status",SqlDbType.Int)
                                   };
            parms[0].Value = id;
            parms[1].Value = (int)status;
            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
        }
        /// <summary>
        /// 根据用户ID获得此用户所有的信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<ForumApplyUserInfo> ListByUserId(int userId) {
            string strSQL = "SELECT ForumApplyUsers.*,ForumGroups.Name AS ForumGroupName FROM dbo.ForumApplyUsers WITH(NOLOCK) LEFT JOIN dbo.ForumGroups WITH(NOLOCK) ON ForumApplyUsers.ForumGroupId = ForumGroups.Id WHERE ForumApplyUsers.UserId = @UserId";
            SqlParameter parm = new SqlParameter("UserId", userId);
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text,strSQL,parm);
            List<ForumApplyUserInfo> list = new List<ForumApplyUserInfo>();
            if(dt != null && dt.Rows.Count>0){
                foreach(DataRow dr in dt.Rows){
                    list.Add(Get(dr));
                }
            }
            return list;
        }
        /// <summary>
        /// 根据用户ID获得通过的论坛版块
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<int> GetPassedGroupIdsByUserId(int userId) {
            string strSQL = "SELECT ForumGroupId FROM ForumApplyUsers WITH(NOLOCK) WHERE UserId = @UserId AND [Status] = 1";
            SqlParameter parm = new SqlParameter("UserId",userId);
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, strSQL, parm);
            List<int> ids = new List<int>();
            if(dt != null && dt.Rows.Count > 0){
                foreach(DataRow dr in dt.Rows){
                    ids.Add(dr.Field<int>("ForumGroupId"));
                }
            }
            return ids;
        }
        public static ForumApplyUserInfo Get(DataRow dr) {
            ForumApplyUserInfo model = new ForumApplyUserInfo();
            if (dr == null) { return model; }
            model.Id = dr.Field<int>("Id");
            model.ContactPerson = dr.Field<string>("ContactPerson");
            model.CreateDateTime = dr.Field<DateTime>("CreateDateTime");
            model.ForumGroupId = dr.Field<int>("ForumGroupId");
            model.ForumGroupName = dr.Field<string>("ForumGroupName");
            model.Status = (ForumApplyStatus)Enum.Parse(typeof(ForumApplyStatus), dr["Status"].ToString());
            model.UserId = dr.Field<int>("UserId");
            model.UserName = dr.Field<string>("UserName");
            return model;
        }
        public static ForumApplyUserInfo Get(int id) {
            string strSQL = "SELECT ForumApplyUsers.*,ForumGroups.Name AS ForumGroupName FROM dbo.ForumApplyUsers WITH (NOLOCK) LEFT JOIN dbo.ForumGroups WITH(NOLOCK) ON ForumApplyUsers.ForumGroupId = ForumGroups.Id WHERE ForumApplyUsers.Id = @Id";
            SqlParameter parm = new SqlParameter("Id",id);
            return Get(Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text,strSQL,parm));
        }
    }
}
