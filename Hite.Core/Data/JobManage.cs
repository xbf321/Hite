using System;
using Hite.Model;
using System.Data.SqlClient;
using Hite.Common.Reflection;
using System.Data;
using Hite.Common;
using System.Collections.Generic;
using System.Text;

namespace Hite.Data
{
    internal static class JobManage
    {
        public static int Add(JobInfo model) {
            string strSQL = "INSERT INTO dbo.Jobs(SiteId,CategoryId,ParentCategoryIds,Title,Department,Area,Number,Introduction,Email,Sort,IsDeleted,EndDateTime,CreateDateTime) VALUES(@SiteId,@CategoryId,@ParentCategoryIds,@Title,@Department,@Area,@Number,@Introduction,@Email,@Sort,@IsDeleted,@EndDateTime,GETDATE());SELECT @@IDENTITY;";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parms));
        }
        public static void Update(JobInfo model) {
            string strSQL = "UPDATE Jobs SET CategoryId = @CategoryId,ParentCategoryIds = @ParentCategoryIds,Title = @Title,Department = @Department,Area = @Area,Number = @Number,Introduction = @Introduction ,Email = @Email,Sort = @Sort,IsDeleted = @IsDeleted,EndDateTime = @EndDateTime WHERE Id = @ID";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
        }

        public static JobInfo Get(int id) {
            string strSQL = "SELECT * FROM Jobs WITH(NOLOCK) WHERE Id = @ID";
            SqlParameter parm = new SqlParameter("@ID",id);
            return Get(Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text, strSQL,parm));
        }
        public static JobInfo Get(DataRow dr)
        {
            JobInfo model = new JobInfo();
            ReflectionHelper.Fill(dr, model);
            return model;
        }
        public static IPageOfList<JobInfo> List(SearchSetting settings)
        {
            FastPaging fp = new FastPaging();
            fp.PageIndex = settings.PageIndex;
            fp.PageSize = settings.PageSize;
            fp.TableName = "Jobs";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";

            StringBuilder sbCondition = new StringBuilder();
            sbCondition.AppendFormat("  SiteId = @SiteId ");
            if(!settings.ShowDeleted){
                sbCondition.Append("    AND IsDeleted = 0 ");
            }
            fp.Condition = sbCondition.ToString();
            fp.OverOrderBy = "Sort ASC , EndDateTime DESC";

            SqlParameter[] parms = { 
                                    new SqlParameter("@SiteId",SqlDbType.Int),
                                   };
            parms[0].Value = settings.SiteId;

            IList<JobInfo> list = new List<JobInfo>();
            JobInfo model = null;
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005(),parms);
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
            int count = Count(settings);
            return new PageOfList<JobInfo>(list, settings.PageIndex, settings.PageSize, count);
        }
        private static int Count(SearchSetting settings)
        {
            StringBuilder sbCondition = new StringBuilder(" SELECT COUNT(*) FROM Jobs WITH(NOLOCK) WHERE 1 = 1");
            
            sbCondition.AppendFormat("  AND SiteId = @SiteId ");
            if (!settings.ShowDeleted)
            {
                sbCondition.Append("    AND IsDeleted = 0 ");
            }

            SqlParameter[] parms = { 
                                    new SqlParameter("@SiteId",SqlDbType.Int),
                                   };
            parms[0].Value = settings.SiteId;
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, sbCondition.ToString(), parms));
        }
    }
}
