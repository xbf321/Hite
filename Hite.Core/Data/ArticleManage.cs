using System;

using System.Linq;
using System.Data;
using System.Data.SqlClient;

using Hite.Model;
using Hite.Common;
using Hite.Common.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace Hite.Data
{
    internal static class ArticleManage
    {
        #region == Article ==

        #region == Add ==
        /// <summary>
        /// 插入文章表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Insert(ArticleInfo model) {
            string strSQL = "INSERT INTO Articles(SiteId,CategoryId,Title,Content,ImageUrl,Url,LinkUrl,Tags,Sort,IsTop,IsDeleted,PublishDateTime,CreateDateTime,CatsJSON,ParentCategoryIds,Remark,Timespan) VALUES(@SiteId,@CategoryId,@Title,@Content,@ImageUrl,@Url,@LinkUrl,@Tags,@Sort,@IsTop,@IsDeleted,@PublishDateTime,GETDATE(),@CatsJSON,@ParentCategoryIds,@Remark,@Timespan);SELECT @@IDENTITY;";

            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,strSQL,parms));
        }
        #endregion

        #region == Update ==
        /// <summary>
       /// 更新文章表
       /// </summary>
       /// <param name="model"></param>
       /// <returns></returns>
        public static int Update(ArticleInfo model) {
            string strSQL = "UPDATE Articles SET CategoryId = @CategoryId,Title = @Title,Content = @Content,ImageUrl = @ImageUrl,LinkUrl = @LinkUrl,Tags = @Tags,Sort = @Sort,IsTop = @IsTop,PublishDateTime = @PublishDateTime,CatsJSON = @CatsJSON,IsDeleted = @IsDeleted,ParentCategoryIds = @ParentCategoryIds,Remark = @Remark WHERE ID = @Id";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            return Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parms);
        }
        #endregion

        #region == 获得某一站点所有文章的发布时间，主要为查询文章用 ==
        /// <summary>
        /// 获得某一站点所有文章的发布时间，主要为查询文章用Key:date,Value:date
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public static List<Tuple<string, string>> GetAllPublishDateBySiteId(int siteId) {
            string strSQL = @"SELECT CONVERT(VARCHAR(10),PublishDateTime,120) AS P FROM Articles WITH(NOLOCK) WHERE EXISTS(
	SELECT * FROM ArticleInCategories WITH(NOLOCK) WHERE SiteId = @SiteId
	AND Articles.ID = ArticleInCategories.ArticleId
)
GROUP BY CONVERT(VARCHAR(10),PublishDateTime,120)
ORDER BY P DESC";
            SqlParameter parm = new SqlParameter("@SiteId",siteId);
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text,strSQL,parm);
            List<Tuple<string, string>> list = new List<Tuple<string, string>>();
            if(dt != null){
                foreach(DataRow row in dt.Rows){
                    string date = row[0].ToString();
                    list.Add(Tuple.Create(date,date));
                }
            }
            return list;
        }
        #endregion

        #region == ListWithoutPage ==
        public static List<ArticleInfo> ListWithoutPage(int siteId, int categoryId, int topCount)
        {
            return ListWithoutPage(siteId, categoryId, topCount, false);
        }
        /// <summary>
        /// 获得文章列表
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryId"></param>
        /// <param name="topCount">默认10条</param>
        /// <returns></returns>
        public static List<ArticleInfo> ListWithoutPage(int siteId, int categoryId, int topCount,bool isTopOneImg)
        {
            List<ArticleInfo> list = new List<ArticleInfo>();
            if (categoryId == 0) { return list; }
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append(@"WITH TEP_Articles AS (
	                        SELECT TOP(50) ROW_NUMBER() OVER(ORDER BY IsTop DESC,Sort ASC,PublishDateTime DESC/*首先按置顶，再按排序，其次按发布时间*/) AS RowNumber,* FROM Articles WITH(NOLOCK) WHERE EXISTS(
                                SELECT * FROM ArticleInCategories AS AIC WITH(NOLOCK) WHERE EXISTS(
                                    SELECT * FROM Categories AS AC WITH(NOLOCK)
                                    WHERE  (AC.ID = @CID OR CHARINDEX(','+CAST(@CID AS NVARCHAR(MAX))+',',','+AC.ParentIdList+',') >0)	/*获取此分类下所有的子分类*/
                                    AND SiteId  = @SiteId 
                                    AND IsDeleted = 0 /*获取未删除的*/
                                    AND AIC.CategoryId = AC.ID /* ArticleInCategories Exists 条件*/
                                )
                                AND Articles.ID = AIC.ArticleId /*Articles Exists条件*/
	                        ) AND IsDeleted = 0 /*获取未删除的*/
	                        AND PublishDateTime < DATEADD(day,1,GETDATE()) /*小于等于当天的新闻*/
)");
            if (isTopOneImg) {
                //第一张是图片
                sbSQL.Append(@"SELECT TOP(1)* FROM TEP_Articles WHERE ImageUrl <> ''");
                //如果就取TOP(1),则不用再取剩余的
                if(topCount > 1){
                    sbSQL.AppendFormat(@"   UNION ALL 
                                            SELECT TOP({0})* FROM TEP_Articles AS P WHERE NOT EXISTS(
	                                            SELECT TOP(1)* FROM TEP_Articles AS C WHERE ImageUrl <> ''
	                                            AND P.ID = C.ID
                                            )",topCount-1);
                }
            } else {
                sbSQL.AppendFormat("SELECT TOP({0})* FROM TEP_Articles",topCount);
            }
            SqlParameter[] parms = { 
                                    new SqlParameter("SiteId",SqlDbType.Int),
                                    new SqlParameter("CID",SqlDbType.Int),
                                    new SqlParameter("TopCount",SqlDbType.Int)
                                   };
            parms[0].Value = siteId;
            parms[1].Value = categoryId;
            parms[2].Value = topCount <= 0 ? 10 : topCount;
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, sbSQL.ToString(),parms);
            ArticleInfo model = null;
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model = Get(dr);
                    list.Add(model);
                }
            }
            return list;
        }
        #endregion

        #region == Get ArticleInfo ==
        public static ArticleInfo Get(int id) {
            if (id == 0) { return new ArticleInfo(); }
            string strSQL = "SELECT * FROM Articles WITH(NOLOCK) WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("Id",id);

            return Get(Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text,strSQL,parm));
        }
        public static ArticleInfo GetByUrl(string url) {
            string strSQL = "SELECT * FROM Articles WITH(NOLOCK) WHERE Timespan = @Url";
            SqlParameter parm = new SqlParameter("Url", url);

            return Get(Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text, strSQL, parm));
        }
        private static ArticleInfo Get(DataRow dr) {
            ArticleInfo model = new ArticleInfo();
            ReflectionHelper.Fill(dr,model);
            return model;
        }
        #endregion

        #region == 获得相关文章==
        /// <summary>
        /// 获得相关文章
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="articleId"></param>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public static List<ArticleInfo> GetRelatedByArticleId(int siteId,int articleId,int topCount) { 
            List<ArticleInfo> list = new List<ArticleInfo>();
            string strSQL = string.Format(@"SELECT TOP({0})* FROM Articles  WITH(NOLOCK)
                                            WHERE SiteId = {1}
                                            AND IsDeleted = 0
                                            AND EXISTS(
	                                            SELECT * FROM ArticleTags AS P WITH(NOLOCK)
	                                            WHERE EXISTS(
		                                            SELECT * FROM ArticleTags AS C WITH(NOLOCK) WHERE ArticleId = {2}
		                                            AND P.Tag LIKE C.Tag
		                                            AND P.ArticleId <> C.ArticleId
	                                            )
	                                            AND Articles.ID = P.ArticleId
                                            )
                                            ORDER BY PublishDateTime DESC", topCount,siteId,articleId);
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text,strSQL);
            ArticleInfo model = null;
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model = Get(dr);
                    list.Add(model);
                }
            }
            return list;
        }
        #endregion

        #region == 查询页面所调用方法,获得所有的数据 ==
        /// <summary>
        /// 查询页面所调用方法
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<ArticleInfo> Seek(int siteId, string keyCondition)
        {

            string strSQL = string.Format(@"WITH CatInfo AS /*首先获得所有启用并且未删除的分类，有可能父节点未启用，儿子节点启用了，所以在这边从上到下过滤一下，只要父节点未启用，其子节点一律不显示*/
(
	SELECT ID,ParentId,Name,ParentIdList FROM Categories  WITH(NOLOCK)
	WHERE SiteId = @SiteId 
	AND ParentId = 0
	AND IsEnabled = 1
	AND IsDeleted = 0
	UNION ALL
	SELECT A.ID,A.ParentId,A.Name,A.ParentIdList FROM Categories AS A WITH(NOLOCK),CatInfo AS B
	WHERE A.ParentId = B.ID
	AND A.IsEnabled = 1
	AND A.IsDeleted = 0
)
SELECT * FROM Articles WITH(NOLOCK) WHERE EXISTS(
	SELECT * FROM ArticleInCategories AS AIC WITH(NOLOCK) /*获得所有的文章*/
	WHERE EXISTS(
		SELECT * FROM CatInfo WITH(NOLOCK)
		WHERE AIC.CategoryId = CatInfo.ID
	)
	AND Articles.ID = AIC.ArticleId
)
AND IsDeleted = 0/*AND (CONTAINS(Title,@Key) OR CONTAINS(Title,'技术'))*/ {0}", keyCondition);
            SqlParameter[] parms = { 
                                   new SqlParameter("@SiteId",SqlDbType.Int),
                                   };
            parms[0].Value = siteId;
            List<ArticleInfo> list = new List<ArticleInfo>();
            ArticleInfo model = null;
            try
            {
                DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, strSQL, parms);
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
            }
            catch{ }
            return list;
        }
        #endregion

        #region == List ==
        public static IPageOfList<ArticleInfo> List(SearchSetting setting) {
            SqlParameter[] parms = { 
                                    new SqlParameter("SiteId",SqlDbType.Int),
                                    new SqlParameter("CID",SqlDbType.Int),
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("PublishDate",SqlDbType.NVarChar)
                                   };
            parms[0].Value = setting.SiteId;
            parms[1].Value = setting.CategoryId;
            parms[2].Value = setting.Title;
            parms[3].Value = setting.PublishDate;

            FastPaging fp = new FastPaging();
            fp.PageIndex = setting.PageIndex;
            fp.PageSize = setting.PageSize;
            fp.Ascending = false;
            fp.TableName = "Articles";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";
            fp.OverOrderBy = "IsTop DESC,Sort ASC,PublishDateTime DESC";
            StringBuilder sbCondition = new StringBuilder();
            sbCondition.Append(@"EXISTS(
	                                SELECT * FROM ArticleInCategories AS AIC WITH(NOLOCK)
	                                WHERE EXISTS(
		                                SELECT * FROM Categories AS AC WITH(NOLOCK) 
		                                WHERE SiteId = @SiteId
		                                AND (AC.ID = @CID OR CHARINDEX(','+CAST(@CID AS NVARCHAR(MAX))+',',','+AC.ParentIdList+',') >0)
		                                AND AIC.CategoryId = AC.ID
	                                )
	                                AND p.ID = AIC.ArticleId
                                )");
            sbCondition.Append("    AND IsDeleted = 0 /*获取未删除的*/");
            if(!string.IsNullOrEmpty(setting.Title)){
                sbCondition.Append("    AND CONTAINS(Title,@Title)  ");
            }
            if (Regex.IsMatch(setting.PublishDate, @"^\d{4}-\d{1,2}-\d{1,2}$", RegexOptions.IgnoreCase))
            {
                sbCondition.Append("    AND CONVERT(VARCHAR(10),PublishDateTime,120) = @PublishDate");
            }

            fp.Condition = sbCondition.ToString();
            //throw new Exception(fp.Build2005());
            IList<ArticleInfo> list = new List<ArticleInfo>();
            ArticleInfo model = null;
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
            int count = Count(setting);
            return new PageOfList<ArticleInfo>(list, setting.PageIndex, setting.PageSize, count);
        }
        private static int Count(SearchSetting setting) {
            SqlParameter[] parms = { 
                                    new SqlParameter("SiteId",SqlDbType.Int),
                                    new SqlParameter("CID",SqlDbType.Int),
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("PublishDate",SqlDbType.NVarChar)
                                   };
            parms[0].Value = setting.SiteId;
            parms[1].Value = setting.CategoryId;
            parms[2].Value = setting.Title;
            parms[3].Value = setting.PublishDate;
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append(@"SELECT COUNT(*) AS p FROM Articles WITH(NOLOCK)
                        WHERE EXISTS(
	                        SELECT * FROM ArticleInCategories AS AIC WITH(NOLOCK)
	                        WHERE EXISTS(
		                        SELECT * FROM Categories AS AC WITH(NOLOCK) 
		                        WHERE SiteId = @SiteId
		                        AND (AC.ID = @CID OR CHARINDEX(','+CAST(@CID AS NVARCHAR(MAX))+',',','+AC.ParentIdList+',') >0)
		                        AND AIC.CategoryId = AC.ID
	                        )
	                        AND Articles.ID = AIC.ArticleId
                        )");
            sbSQL.Append("  AND IsDeleted = 0   ");
            if (!string.IsNullOrEmpty(setting.Title))
            {
                sbSQL.Append("    AND CONTAINS(Title,@Title)  ");
            }
            if (Regex.IsMatch(setting.PublishDate, @"^\d{4}-\d{1,2}-\d{1,2}$", RegexOptions.IgnoreCase))
            {
                sbSQL.Append("    AND CONVERT(VARCHAR(10),PublishDateTime,120) = @PublishDate");
            }

            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,sbSQL.ToString(),parms));
        }
        #endregion

        #region == 已废掉 ==
//        public static IPageOfList<ArticleInfo> Search(int pageIndex, int pageSize, int siteId, int categoryId, string title)
//        {
//            SqlParameter[] parms = { 
//                                    new SqlParameter("SiteId",SqlDbType.Int),
//                                    new SqlParameter("CID",SqlDbType.Int),
//                                    new SqlParameter("Title",SqlDbType.NVarChar)
//                                   };
//            parms[0].Value = siteId;
//            parms[1].Value = categoryId;
//            parms[2].Value = title;
//            FastPaging fp = new FastPaging();
//            fp.PageIndex = pageIndex;
//            fp.PageSize = pageSize;
//            fp.Ascending = false;
//            fp.TableName = "Articles";
//            fp.TableReName = "p";
//            fp.PrimaryKey = "ID";
//            fp.QueryFields = "p.*";
//            fp.OverOrderBy = "IsTop DESC,Sort ASC,PublishDateTime DESC";
//            fp.Condition = @"EXISTS(
//	                            SELECT * FROM ArticleInCategories AS AIC WITH(NOLOCK)
//	                            WHERE EXISTS(
//		                            SELECT * FROM Categories AS AC WITH(NOLOCK)
//		                            WHERE  (AC.ID = @CID OR CHARINDEX(','+CAST(@CID AS NVARCHAR(MAX))+',',','+AC.ParentIdList+',') >0)
//		                            AND AC.ID = AIC.CategoryId
//                                    AND AC.SiteId = @SiteId
//	                            )
//	                            AND p.id =  AIC.ArticleId
//                            ) AND IsDeleted = 0 /*获取未删除的*/
//                            AND PublishDateTime < DATEADD(day,1,GETDATE()) /*小于等于当天的新闻*/";
//            //throw new Exception(fp.Build2005());
//            IList<ArticleInfo> list = new List<ArticleInfo>();
//            ArticleInfo model = null;
//            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text,fp.Build2005(),parms);
//            if(dt != null && dt.Rows.Count>0){
//                foreach(DataRow dr in dt.Rows){
//                    model = Get(dr);
//                    if(model != null){
//                        list.Add(model);
//                    }
//                }
//            }
//            int count = Count(siteId,categoryId,title);
//            return new PageOfList<ArticleInfo>(list,pageIndex,pageSize,count);
//        }
//        private static int Count(int siteId, int categoryId, string title) {
//            SqlParameter[] parms = { 
//                                    new SqlParameter("SiteId",SqlDbType.Int),
//                                    new SqlParameter("CID",SqlDbType.Int),
//                                    new SqlParameter("Title",SqlDbType.NVarChar)
//                                   };
//            parms[0].Value = siteId;
//            parms[1].Value = categoryId;
//            parms[2].Value = title;
//            string strSQL = @"SELECT COUNT(*) FROM Articles WITH(NOLOCK)
//                            WHERE EXISTS(
//	                            SELECT * FROM ArticleInCategories WITH(NOLOCK)
//	                            WHERE EXISTS(
//		                            SELECT * FROM Categories WITH(NOLOCK)
//		                            WHERE  (ID = @CID OR CHARINDEX(','+CAST(@CID AS NVARCHAR(MAX))+',',','+ParentIdList+',') >0)
//		                            AND Categories.ID = ArticleInCategories.CategoryId
//                                    AND Categories.SiteId = @SiteId
//	                            )
//	                            AND ArticleInCategories.ArticleId = Articles.ID
//                            )
//                            AND IsDeleted = 0 /*获取未删除的*/
//                            AND PublishDateTime < DATEADD(day,1,GETDATE()) /*小于等于当天的新闻*/";

//            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,strSQL,parms));
//        }
        #endregion

        #endregion

        #region == ArticleInCategories ==
        public static void InsertArticleInCategories(IList<ArticleInCategoryInfo> modelList) {            
            //首先根据ArticleIdDelete此文章下的信息
            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text, string.Format("DELETE ArticleInCategories WHERE ArticleId = {0}", modelList.First().ArticleId));

            //TODO
            //这里有问题，如果一篇文章，主分类和同时发布的分类一样，那么这个表中，就会出现两条同样的记录，所以在这应该处理一下,目前还没处理
            //已处理2011/7/13
            Dictionary<int,int> dic = new Dictionary<int,int>();

            foreach(var model in modelList){
                if(!dic.ContainsKey(model.CategoryId)){
                    dic.Add(model.CategoryId,model.ArticleId);
                    string strSQL = "INSERT INTO ArticleInCategories(SiteId,CategoryId,CategoryName,ArticleId,IsDeleted) VALUES(@SiteId,@CategoryId,@CategoryName,@ArticleId,0);SELECT @@IDENTITY;";
                    SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
                    Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
                }                
            }           

        }
        public static int GetCategoryIdBySiteIdAndArticleId(int siteId,int articleId) {
            string strSQL = "SELECT TOP(1) CategoryId FROM ArticleInCategories WITH(NOLOCK) WHERE SiteId = @SiteId AND ArticleId = @ArticleId";
            SqlParameter[] parms = { 
                                    new SqlParameter("SiteId",siteId),
                                    new SqlParameter("ArticleId",articleId)
                                   };
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,strSQL,parms));
        }
        #endregion

        #region == ArticleTags ==
        /// <summary>
        /// 插入文章标签表
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="tags"></param>
        public static void InsertTags(int articleId,string[] tags) {
            //先删除
            string strSQL = string.Format("DELETE ArticleTags WHERE ArticleId = {0}",articleId);
            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL);

            //插入

            foreach(string s in tags){
                if (!string.IsNullOrEmpty(s))
                {
                    strSQL = string.Format("INSERT INTO ArticleTags(ArticleId,Tag) VALUES({0},'{1}')", articleId, s);
                    Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL);
                }
            }
        }
        #endregion

        
    }
}
