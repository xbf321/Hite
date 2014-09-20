using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hite.Model;
using System.Data.SqlClient;
using Hite.Common.Reflection;
using System.Data;
using Hite.Common;

namespace Hite.Data
{
    internal static class FeedbackManage
    {
        public static int Add(FeedbackInfo model) {
            string strSQL = "INSERT INTO dbo.Feedbacks(SiteId,UserId,UserName,RealName,Company,Phone,Email,Requirement,Intention,CreateDateTime,IP) VALUES(@SiteId,@UserId,@UserName,@RealName,@Company,@Phone,@Email,@Requirement,@Intention,GETDATE(),@IP);SELECT @@IDENTITY;";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parms));
        }
        private static FeedbackInfo Get(DataRow dr)
        {
            FeedbackInfo model = new FeedbackInfo();
            ReflectionHelper.Fill(dr, model);
            return model;
        }
        public static IPageOfList<FeedbackInfo> List(SearchSetting settings)
        {
            FastPaging fp = new FastPaging();
            fp.PageIndex = settings.PageIndex;
            fp.PageSize = settings.PageSize;
            fp.TableName = "Feedbacks";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";

            StringBuilder sbCondition = new StringBuilder();
            sbCondition.AppendFormat("  SiteId = @SiteId ");
            fp.Condition = sbCondition.ToString();
            fp.OverOrderBy = "CreateDateTime DESC";

            SqlParameter[] parms = { 
                                    new SqlParameter("@SiteId",SqlDbType.Int),
                                   };
            parms[0].Value = settings.SiteId;

            IList<FeedbackInfo> list = new List<FeedbackInfo>();
            FeedbackInfo model = null;
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005(), parms);
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
            return new PageOfList<FeedbackInfo>(list, settings.PageIndex, settings.PageSize, count);
        }
        private static int Count(SearchSetting settings)
        {
            StringBuilder sbCondition = new StringBuilder(" SELECT COUNT(*) FROM Feedbacks WITH(NOLOCK) WHERE 1 = 1");

            sbCondition.AppendFormat("  AND SiteId = @SiteId ");
            SqlParameter[] parms = { 
                                    new SqlParameter("@SiteId",SqlDbType.Int),
                                   };
            parms[0].Value = settings.SiteId;
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, sbCondition.ToString(), parms));
        }
    }
}
