using System;
using Hite.Model;
using System.Data;
using Hite.Common.Reflection;
using System.Data.SqlClient;
using Hite.Common;
using System.Text;
using System.Collections.Generic;

namespace Hite.Data
{
    internal static class AttachmentManage
    {
        public static int Add(AttachmentInfo model) {
            string strSQL = "INSERT INTO Attachments(Siteid,CategoryId,ParentCategoryIds,Title,Extension,Size,Introduction,Url,Sort,IsDeleted,PublishDate) VALUES(@Siteid,@CategoryId,@ParentCategoryIds,@Title,@Extension,@Size,@Introduction,@Url,@Sort,@IsDeleted,@PublishDate);SELECT @@IDENTITY;";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, strSQL, parms));
        }
        public static void Update(AttachmentInfo model) {
            string strSQL = "UPDATE Attachments SET CategoryId = @CategoryId,ParentCategoryIds = @ParentCategoryIds,Title = @Title,Extension = @Extension,Size = @Size,Introduction = @Introduction,Url = @Url,Sort = @Sort ,IsDeleted = @IsDeleted,PublishDate = @PublishDate WHERE ID = @Id";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
        }
        public static AttachmentInfo Get(int id) {
            if (id == 0) { return new AttachmentInfo(); }
            string strSQL = "SELECT * FROM Attachments WITH(NOLOCK) WHERE Id = @ID";
            SqlParameter parm = new SqlParameter("@ID", id);
            return Get(Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text, strSQL, parm));
        }
        public static AttachmentInfo Get(DataRow dr)
        {
            AttachmentInfo model = new AttachmentInfo();
            ReflectionHelper.Fill(dr, model);
            return model;
        }
        public static void UpdateDownloadCount(int id) {
            string strSQL = "UPDATE Attachments SET DownloadCount = DownloadCount + 1 WHERE Id = @ID";
            SqlParameter parm = new SqlParameter("@ID", id);
            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parm);
        }
        public static IPageOfList<AttachmentInfo> List(SearchSetting settings)
        {
            FastPaging fp = new FastPaging();
            fp.PageIndex = settings.PageIndex;
            fp.PageSize = settings.PageSize;
            fp.TableName = "Attachments";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";

            StringBuilder sbCondition = new StringBuilder();
            sbCondition.AppendFormat("  SiteId = @SiteId ");
            if (!settings.ShowDeleted)
            {
                sbCondition.Append("    AND IsDeleted = 0 ");
            }
            if(settings.CategoryId >0){
                sbCondition.Append(@" AND EXISTS(
                                            SELECT * FROM dbo.Categories AS AC WITH(NOLOCK)
                                            WHERE  (AC.ID = @CID OR CHARINDEX(','+CAST(@CID AS NVARCHAR(MAX))+',',','+AC.ParentIdList+',') >0)	/*获取此分类下所有的子分类*/
                                            AND SiteId = @SiteId
                                            AND IsDeleted = 0
                                            AND p.CategoryId = AC.ID
                    )");
            }
            fp.Condition = sbCondition.ToString();
            fp.OverOrderBy = "Sort ASC ,PublishDate DESC";
            SqlParameter[] parms = { 
                                    new SqlParameter("@SiteId",SqlDbType.Int),
                                    new SqlParameter("@CID",SqlDbType.Int)
                                   };
            parms[0].Value = settings.SiteId;
            parms[1].Value = settings.CategoryId;

            IList<AttachmentInfo> list = new List<AttachmentInfo>();
            AttachmentInfo model = null;
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
            return new PageOfList<AttachmentInfo>(list, settings.PageIndex, settings.PageSize, count);
        }
        private static int Count(SearchSetting settings)
        {
            StringBuilder sbCondition = new StringBuilder(" SELECT COUNT(*) FROM Attachments WITH(NOLOCK) WHERE 1 = 1");

            sbCondition.AppendFormat("  AND SiteId = @SiteId ");
            if (!settings.ShowDeleted)
            {
                sbCondition.Append("    AND IsDeleted = 0 ");
            }
            if(settings.CategoryId >0){
                sbCondition.Append(@" AND EXISTS(
                                            SELECT * FROM dbo.Categories AS AC WITH(NOLOCK)
                                            WHERE  (AC.ID = @CID OR CHARINDEX(','+CAST(@CID AS NVARCHAR(MAX))+',',','+AC.ParentIdList+',') >0)	/*获取此分类下所有的子分类*/
                                            AND SiteId = @SiteId
                                            AND IsDeleted = 0
                                            AND Attachments.CategoryId = AC.ID
                    )");
            }

            SqlParameter[] parms = { 
                                    new SqlParameter("@SiteId",SqlDbType.Int),
                                    new SqlParameter("@CID",SqlDbType.Int)
                                   };
            parms[0].Value = settings.SiteId;
            parms[1].Value = settings.CategoryId;
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, sbCondition.ToString(), parms));
        }
    }
}
