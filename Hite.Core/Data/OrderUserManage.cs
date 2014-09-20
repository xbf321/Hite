using System;
using System.Linq;
using Hite.Model;
using System.Data.SqlClient;
using Hite.Common.Reflection;
using System.Data;
using Hite.Common;
using System.Collections.Generic;
using Goodspeed.Library.Data;

namespace Hite.Data
{
     internal static class OrderUserManage
    {
         public static int Add(OrderUserInfo model) {
             string strSQL = "INSERT INTO OrderUsers(UserName,UserPwd,CompanyName,CreateDateTime) VALUES(@UserName,@UserPwd,@CompanyName,GETDATE());SELECT @@IDENTITY;";
             SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
             return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parms));
         }
         public static void Update(OrderUserInfo model) {
             //同时更新Orders表中OrderCompanyName字段
             string strSQL = "UPDATE OrderUsers SET UserName = @UserName,UserPwd = @UserPwd,CompanyName = @CompanyName WHERE Id = @ID;UPDATE Orders SET OrderCompanyName = @CompanyName WHERE OrderUserId = @ID";
             SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parms);             
         }
         public static IList<Tuple<int,string>> CompanyList(){
             string strSQL = "SELECT * FROM OrderUsers WITH(NOLOCK)";
             IList<Tuple<int, string>> list = new List<Tuple<int, string>>();
             DataTable dt = SQLPlus.ExecuteDataTable(CommandType.Text, strSQL);
             if(dt != null && dt.Rows.Count>0){
                foreach(DataRow dr in dt.Rows){
                    list.Add(Tuple.Create(dr.Field<int>("id"), dr.Field<string>("CompanyName")));
                }
             }
             return list;
         }
         public static OrderUserInfo Get(int id) {
             string strSQL = "SELECT * FROM OrderUsers WITH(NOLOCK) WHERE Id = @ID";
             SqlParameter parm = new SqlParameter("ID",id);
             DataRow dr = SQLPlus.ExecuteDataRow(CommandType.Text,strSQL,parm);
             return Get(dr);
         }
         public static OrderUserInfo Get(string userName, string userPwd)
         {
             string strSQL = "SELECT * FROM OrderUsers WITH(NOLOCK) WHERE UserName = @UserName AND UserPwd = @UserPwd";
             SqlParameter[] parms = { 
                                        new SqlParameter("UserName",SqlDbType.NVarChar),
                                        new SqlParameter("UserPwd",SqlDbType.NVarChar)
                                    };
             parms[0].Value = userName;
             parms[1].Value = userPwd;
             return Get(SQLPlus.ExecuteDataRow(CommandType.Text,strSQL,parms));
         }
         public static IPageOfList<OrderUserInfo> List(int pageIndex,int pageSize)
         {
             FastPaging fp = new FastPaging();
             fp.PageIndex = pageIndex;
             fp.PageSize = pageSize;
             fp.TableName = "OrderUsers";
             fp.TableReName = "p";
             fp.PrimaryKey = "ID";
             fp.QueryFields = "p.*";
             fp.OverOrderBy = " CreateDateTime DESC";


             IList<OrderUserInfo> list = new List<OrderUserInfo>();
             OrderUserInfo model = null;
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
             int count = Count();
             return new PageOfList<OrderUserInfo>(list, pageIndex, pageSize, count);
         }
         private static int Count() {
             string strSQL = "SELECT COUNT(*) FROM OrderUsers WITH(NOLOCK)";
             return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text,strSQL));
         }
         public static OrderUserInfo Get(DataRow dr)
         {
             OrderUserInfo model = new OrderUserInfo();
             ReflectionHelper.Fill(dr, model);
             return model;
         }
         public static bool ExistsUserName(int id,string userName) {
             string strSQL = "SELECT COUNT(*) FROM OrderUsers WITH(NOLOCK) WHERE UserName = @UserName";
             if(id > 0){
                 strSQL += string.Format("  AND ID <> {0}",id);
             }
             SqlParameter parm = new SqlParameter("UserName",userName);
             return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,strSQL,parm)) >0;
         }
         public static bool ExistsCompanyName(int id,string companyName) {
             string strSQL = "SELECT COUNT(*) FROM OrderUsers WITH(NOLOCK) WHERE CompanyName = @CompanyName";
             if(id >0){
                 strSQL += string.Format("  AND ID <> {0}",id);
             }
             SqlParameter parm = new SqlParameter("CompanyName", companyName);
             return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parm)) > 0;
         }
    }
}
