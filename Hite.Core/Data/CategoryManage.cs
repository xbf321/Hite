using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

using Hite.Model;
using Hite.Common.Reflection;
using System.Text;


namespace Hite.Data
{
    internal static class ArticleCategoryManage
    {
        #region == Insert ==
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Insert(CategoryInfo model)
        {
            string strSQL = "INSERT INTO Categories(SiteId,ParentId,ParentIdList,Sort,Name,ImageUrl,LinkUrl,Introduction,CreateDateTime,Alias,TemplateType,IsShowFirstChildNode,BannerAdImageUrl,IsEnabled) VALUES(@SiteId,@parentId,@parentIdList,@Sort,@Name,@ImageUrl,@LinkUrl,@Introduction,GETDATE(),@Alias,@TemplateType,@IsShowFirstChildNode,@BannerAdImageUrl,@IsEnabled);SELECT @@IDENTITY;";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,strSQL,parms));
        }
        #endregion

        #region == Update ==
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Update(CategoryInfo model)
        {
            string strSQL = "UPDATE Categories SET Name = @Name,Sort = @Sort,ImageUrl = @ImageUrl,LinkUrl = @LinkUrl,Introduction = @Introduction,Alias = @Alias,TemplateType = @TemplateType,IsShowFirstChildNode = @IsShowFirstChildNode,BannerAdImageUrl = @BannerAdImageUrl,IsEnabled = @IsEnabled,IsDeleted = @IsDeleted WHERE ID = @ID";
            SqlParameter[] parms = ParameterHelper.GetClassSqlParameters(model);
            return Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parms);
        }
        #endregion

        /// <summary>
        /// 判断当前类别下是否名称重复
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public static bool ExistsName(int siteId,int id,string name,int parentId) { 
            //id == 0 说明添加，反之，编辑
            string strSQL = string.Empty;
            if (id == 0)
            {
                strSQL = "SELECT COUNT(*) FROM Categories WITH(NOLOCK) WHERE ParentId = @ParentId AND Name = @Name AND SiteId = @SiteId";
            }
            else {
                strSQL = "SELECT COUNT(*) FROM Categories WITH(NOLOCK) WHERE ParentId = @ParentId AND Name = @Name AND ID <> @ID AND SiteId = @SiteId";
            }
            SqlParameter[] parms = { 
                                    new SqlParameter("@ID",SqlDbType.Int),
                                    new SqlParameter("@Name",SqlDbType.VarChar),
                                    new SqlParameter("@ParentID",SqlDbType.Int),
                                    new SqlParameter("@SiteId",SqlDbType.Int)
                                   };
            parms[0].Value = id;
            parms[1].Value = name;
            parms[2].Value = parentId;
            parms[3].Value = siteId;

            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,strSQL,parms)) >0;
        }

        #region == 别名是否存在 ==
        public static bool ExistsAlias(int siteId,int cid,string alias) {
            StringBuilder sb = new StringBuilder();
            sb.Append(" SELECT COUNT(*) FROM Categories WITH(NOLOCK)");
            sb.AppendFormat("   WHERE SiteId = {0}   ",siteId);
            sb.AppendFormat("   AND Alias = '{0}'    ", alias);
            if(cid != 0){
                sb.AppendFormat("   AND Id <> {0}   ",cid);
            }
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,sb.ToString())) > 0;
        }
        #endregion

        #region == 获得分类详细信息 ==
        /// <summary>
        /// 获得详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CategoryInfo Get(int id)
        {
            string strSQL = string.Format("SELECT * FROM Categories WITH(NOLOCK) WHERE Id = {0}",id);
            return Get(Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text,strSQL));
        }
        private static CategoryInfo Get(DataRow dr)
        {
            CategoryInfo model = new CategoryInfo();
            ReflectionHelper.Fill(dr, model);
            return model;
        }
        #endregion

        #region == 删除分类 ==
        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static void Delete(int id) {
            //现在不真正删除了，所以下面的代码就没作用了
            //首先判断此分类下是否有子分类
            //int count = ListByParentId(id).Count;
            //if(count >0){
            //    return false;
            //}

            string strSQL = string.Format("UPDATE Categories SET IsDeleted = 1 WHERE Id = {0}",id);
            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL);
        }
        #endregion

        #region == 还原分类 ==
        public static void Restore(int id) {
            string strSQL = string.Format("UPDATE Categories SET IsDeleted = 0 WHERE Id = {0}", id);
            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL);
        }
        #endregion

        #region == 根据站点Id获得此站点下的所有分类 ==
        /// <summary>
        /// 根据站点Id获得此站点下的所有分类
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static IList<CategoryInfo> ListBySiteId(int siteId) {
            string strSQL = string.Format("SELECT * FROM Categories WITH(NOLOCK) WHERE SiteId = {0} ORDER BY Sort",siteId);
            var list = new List<CategoryInfo>();
            CategoryInfo model = null;
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text,strSQL);
            if(dt != null && dt.Rows.Count >0){
                foreach(DataRow dr in dt.Rows){
                    model = Get(dr);
                    list.Add(model);
                }
            }
            return list;
        }
        #endregion

        #region == 根据ParentId获得对应下的一级子分类==
        /// <summary>
        /// 根据ParentId获得对应下的一级子分类
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public static IList<CategoryInfo> ListByParentId(int parentId) {
            string strSQL = string.Format("SELECT * FROM Categories WITH(NOLOCK) WHERE ParentId = {0} ORDER BY Sort",parentId);
            IList<CategoryInfo> list = new List<CategoryInfo>();
            CategoryInfo model = null;
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text,strSQL);
            if(dt != null && dt.Rows.Count >0){
                foreach(DataRow dr in dt.Rows){
                    model = Get(dr);
                    list.Add(model);
                }
            }
            return list;
        }
        #endregion 
    }
}
