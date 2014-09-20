using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hite.Model;
using Hite.Data;
using Hite.Mvc;
using System.Web;
using System.Web.Caching;

namespace Hite.Services
{
    public static class CategoryService
    {
        private static volatile System.Web.Caching.Cache webCache = System.Web.HttpRuntime.Cache;
        private const int CACHETIMEOUT = 30;//缓存30分钟

        #region == Edit Or Add ==
        /// <summary>
        /// 添加或更新分类信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Create(CategoryInfo model)
        {
            if (model.Id == 0)
            {
                //Insert
                int i = ArticleCategoryManage.Insert(model);
                model.Id = i;
            }
            else
            {
                //Update
                ArticleCategoryManage.Update(model);
            }
            return model.Id;
        }
        #endregion

        #region == Delete ==
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static void Delete(int id) {
            ArticleCategoryManage.Delete(id);
        }
        #endregion

        #region == Restore ==
        /// <summary>
        /// 恢复分类
        /// </summary>
        /// <param name="id"></param>
        public static void Restore(int id) {
            ArticleCategoryManage.Restore(id);
        }
        #endregion

        #region ==获得此ID的类别详细信息==
        /// <summary>
        /// 获得此ID的类别详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CategoryInfo Get(int id)
        {
            return Get(id,false);
        }
        public static CategoryInfo Get(int id,bool useCache)
        {
            if(!useCache) return ArticleCategoryManage.Get(id);
            //需要加缓存
            string KEY = string.Format("GET_CATEGORY_INFO_{0}", id);
            var info = (CategoryInfo)webCache[KEY];
            if (info == null)
            {
                info = ArticleCategoryManage.Get(id);
                webCache.Insert(KEY, info, null, DateTime.Now.AddMinutes(CACHETIMEOUT), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
            }
            return info;
        }
        #endregion

        #region == 检测是否存在相同的分类名，在同一站点下 ==
        public static bool ExistsName(int siteId, int id, string name, int parentId)
        {
            return ArticleCategoryManage.ExistsName(siteId, id, name, parentId);
        }
        #endregion

        #region == 分类别名是否存在 ==
        /// <summary>
       /// 是否存在别名，别名不允许重复
       /// </summary>
       /// <param name="appId"></param>
       /// <param name="cid"></param>
       /// <param name="englishName"></param>
       /// <returns></returns>
        public static bool ExistsAlias(int appId,int cid,string englishName) {
            return ArticleCategoryManage.ExistsAlias(appId,cid,englishName);
        }
        #endregion

        #region ==  获得某一站点下的所有文章分类 ==
        public static IList<CategoryInfo> ListBySiteId(int siteId)
        {
            return ListBySiteId(siteId, false);
        }
        public static IList<CategoryInfo> ListBySiteId(int siteId,bool useCache)
        {
            if(!useCache) return ArticleCategoryManage.ListBySiteId(siteId);
            string KEY = string.Format("SITE_LIST_{0}", siteId);
            var list = (IList<CategoryInfo>)webCache[KEY];
            if (list == null)
            {
                list = ArticleCategoryManage.ListBySiteId(siteId);
                webCache.Insert(KEY, list, null, DateTime.Now.AddMinutes(CACHETIMEOUT), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
            }
            //在这需要加缓存
            return list;
        }
        #endregion

        #region == 根据父ID获取此ID下一级栏目，不能获取此栏目下的所有节点 ==
        /// <summary>
        /// 根据父ID获取此ID下一级栏目，不能获取此栏目下的所有节点
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="useCache">是否使用缓存，默认False</param>
        /// <param name="showEnabled">是否显示启用项，默认True</param>
        /// <param name="showDeleted">是否显示删掉项，默认False</param>
        /// <returns></returns>
        public static IList<CategoryInfo> ListByParentId(int parentId) {           
            return ListByParentId(parentId,false,true,false);
        }
        /// <summary>
        /// 根据父ID获取此ID下一级栏目，不能获取此栏目下的所有节点
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="useCache">是否使用缓存</param>
        /// <param name="useCache">是否使用缓存</param>
        /// <param name="showEnabled">是否显示启用项，默认True</param>
        /// <param name="showDeleted">是否显示删掉项，默认False</param>
        /// <returns></returns>
        public static IList<CategoryInfo> ListByParentId(int parentId, bool useCache) {
            return ListByParentId(parentId,useCache,true,false);
        }
        /// <summary>
        /// 根据父ID获取此ID下一级栏目，不能获取此栏目下的所有节点
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="useCache">是否使用缓存</param>
        /// <param name="showEnabled">是否显示启用项</param>
        /// <param name="showDeleted">是否显示删掉项</param>
        /// <returns></returns>
        public static IList<CategoryInfo> ListByParentId(int parentId, bool useCache,bool showEnabled,bool showDeleted) {
            if (!useCache) {
                IEnumerable<CategoryInfo> _list = ArticleCategoryManage.ListByParentId(parentId);
                if (showEnabled)
                {
                    //显示启用的
                    _list = _list.Where(p => p.IsEnabled == true);
                }
                if (!showDeleted)
                {
                    //显示已删除掉的
                    _list = _list.Where(p => p.IsDeleted == false);
                }
                return _list.ToList();
            };
            //需要加缓存
            string CACHEKEY = string.Format("LIST_BY_PARENT_ID_{0}", parentId);

            var list = (IEnumerable<CategoryInfo>)webCache[CACHEKEY];
            if (list == null)
            {
                list = ArticleCategoryManage.ListByParentId(parentId);
                if (showEnabled)
                {
                    //显示启用的
                    list = list.Where(p => p.IsEnabled == true);
                }
                if (!showDeleted)
                {
                    //显示已删除掉的
                    list = list.Where(p => p.IsDeleted == false);
                }
                webCache.Insert(CACHEKEY, list, null, DateTime.Now.AddMinutes(CACHETIMEOUT), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
            }
            return list.ToList();
        }
        #endregion

        #region == 根据此ID获得所有祖先，正序排列，包括此节点【递归实现】 ==
        /// <summary>
        /// 根据此ID获得所有祖先，正序排列，包括此节点【递归实现】
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IList<CategoryInfo> ListUpById(int siteId,int id) {
            IList<CategoryInfo> upList = new List<CategoryInfo>();
            var listAll = ListBySiteId(siteId,true);
            BuildListUpByParentId(listAll,id,ref upList);
            return upList.Reverse().ToList();
        }
        private static void BuildListUpByParentId(IList<CategoryInfo> list, int parentId, ref IList<CategoryInfo> upList)
        {
            var item = list.Where(p => p.Id == parentId).FirstOrDefault();
            if (item != null && item.Id > 0)
            {
                upList.Add(item);
                if (item.ParentId > 0)
                {
                    BuildListUpByParentId(list, item.ParentId, ref upList);
                }
            }
        }
        #endregion

        #region == 根据此ID获得所有孩子节点，包括此节点【递归实现】 ==
        /// <summary>
        /// 根据此ID获得所有孩子节点，包括此节点【递归实现】
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IList<CategoryInfo> ListDownById(int siteId,int id) {
            IList<CategoryInfo> downList = new List<CategoryInfo>();
            var listAll = ListBySiteId(siteId,true);
            //添加此节点本身
            downList.Add(listAll.Where( p => p.Id == id).FirstOrDefault());

            BuildListDownByParentId(listAll, id, ref downList);
            return downList;
        }
        private static void BuildListDownByParentId(IList<CategoryInfo> list, int parentId, ref IList<CategoryInfo> downList)
        {
            var itemList = list.Where(p=>p.ParentId == parentId);
            if (itemList != null && itemList.Count() > 0)
            {
                foreach (var item in itemList) {
                    downList.Add(item);
                    BuildListDownByParentId(list, item.Id, ref downList);
                }
                
            }
        }
        #endregion        

        #region == 创建TreeView列表（前台调用:IsEnabled=true AND IsDeleted = false）==

        public static string RenderTreeViewForHtml(int siteId)
        {
            return RenderTreeViewForHtml(siteId, 0);
        }
        public static string RenderTreeViewForHtml(int siteId, int parentId)
        {
            return RenderTreeViewForHtml(siteId, parentId,0);
        }
        public static string RenderTreeViewForHtml(int siteId, int parentId,int selectId) {
            var siteInfo = SiteService.Get(siteId,true);
            return BuildListForHtml(ListBySiteId(siteId,true), parentId, selectId,siteInfo.Language);
        }
        private static string BuildListForHtml(IEnumerable<CategoryInfo> list, int parentId = 0, int selectId = 0,WebLanguage language = WebLanguage.zh_cn)
        {
            var pList = list.Where(nc => nc.ParentId == parentId );
            if (pList.Count() == 0) { return string.Empty; }
            var sb = new StringBuilder();
            sb.AppendFormat("{0}", parentId != 0 ? "<ul>" : string.Empty);
            foreach (var item in pList)
            {
                if (item.IsEnabled && !item.IsDeleted)
                {
                    sb.AppendFormat("<li{0}>",item.Id == selectId ? " class=\"current\"" : string.Empty);

                    //创建链接 Start
                    sb.Append(CategoryLinkUrlHelper.BuildLink(item,string.Empty,null,language));
                    //创建链接 End

                    //递归
                    sb.Append(BuildListForHtml(list, item.Id,selectId,language));
                    sb.AppendLine("</li>");
                }
            }
            sb.AppendFormat("{0}", parentId != 0 ? "</ul>" : string.Empty);
            return sb.ToString();
        }
        #endregion

        #region == 根据某一站点创建分类的【下拉列表】树（只是后台调用） ==
        /// <summary>
        /// 根据某一站点创建分类的下拉列表树
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static HtmlString RenderDowndownListTreeForAdmin(int siteId, string name, string value)
        {

            var oldList = ListBySiteId(siteId).ToList();
            var newList = new List<CategoryInfo>();
            BuildListForTree(newList, oldList, 0);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<select id=\"{0}\" name=\"{0}\">", name);
            sb.Append("<option value=\"0\">==根路径==</option>");
            foreach (var item in newList)
            {
                sb.AppendFormat("<option value=\"{0}\" {2}>{1}</option>", item.Id, item.Name, value == item.Id.ToString() ? "selected=\"selected\"" : string.Empty);
            }
            sb.Append("</select>");
            return new HtmlString(sb.ToString());
        }
        /// <summary>
        /// 判断某子项的所有父项中是否存在指定父ID
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="child">子项</param>
        /// <param name="compareParentId">父ID</param>
        /// <returns></returns>
        private static bool CompareParentID(List<CategoryInfo> list, CategoryInfo child, int compareParentId)
        {
            if (child.ParentId == compareParentId) return true;
            var category = list.Find(c => c.Id == child.ParentId);
            while (category != null)
            {
                if (category.ParentId == compareParentId) return true;
                var nextParentId = category.ParentId;
                category = list.Find(c => c.Id == nextParentId);
            }
            return false;
        }
        private static string BuildLevelString(int level)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < level; i++)
            {
                sb.Append("∟");
            }
            return sb.ToString();
        }
        private static void BuildListForTree(List<CategoryInfo> newList, List<CategoryInfo> oldList, int parentId)
        {
            var plist = oldList.FindAll(nc => nc.ParentId == parentId);
            if (plist.Count == 0) { return; }
            foreach (var item in plist)
            {
                if (item.ParentId == 0)
                {
                    newList.Add(item);
                }
                int index = newList.FindIndex(delegate(CategoryInfo m) { return m.Id == item.ParentId; });
                if (index > -1)
                {
                    #region 判断level

                    int level = 0;
                    CategoryInfo ncTmp = newList.Find(x => x.Id == item.ParentId);
                    while (ncTmp != null)
                    {
                        ncTmp = newList.Find(x => x.Id == ncTmp.ParentId);
                        level++;
                    }
                    #endregion

                    #region 插入到父级索引后

                    index += 1;
                    //如果紧跟父级的项是属于该父级的子级或者子级的子级……(递归下去)
                    while (newList.Count > index && CompareParentID(newList, newList[index], item.ParentId))
                    {
                        //则插入到该子级索引后
                        index += 1;
                    }
                    item.Name = BuildLevelString(level) + item.Name;
                    newList.Insert(index, item);
                    #endregion
                }
                BuildListForTree(newList, oldList, item.Id);
            }
        }
        #endregion

        #region == 为后台创建TreeView(编辑或删除连接)（后台调用） ==
        public static string RenderTreeViewForAdminWithEdit(int siteId)
        {
            return BuildListForAdminWithEdit(ListBySiteId(siteId).ToList(), 0, siteId);
        }
        public static string BuildListForAdminWithEdit(List<CategoryInfo> list, int parentId, int siteId)
        {
            var pList = list.Where(nc => nc.ParentId == parentId);
            if (pList.Count() == 0) { return string.Empty; }
            var sb = new StringBuilder();
            sb.AppendFormat("<ul {0}>", parentId == 0 ? "class=\"treeview-black treeview\"" : string.Empty);
            foreach (var item in pList)
            {
                sb.Append("<li>");
                sb.Append("<div class=\"hitarea collapsable-hitarea\"></div>");
                //sb.AppendFormat("Id:{0}&nbsp;", item.Id);
                sb.AppendFormat("{0}",item.Name);
                sb.AppendFormat("（{0}-{1}-{2}）",item.Id,item.Alias,(TemplateType)Enum.Parse(typeof(TemplateType),item.TemplateType.ToString(),true));
                //sb.AppendFormat("<a id=\"{1}\" title=\"{0}\">{0}（{2}）</a>", item.Name, item.Id, item.Alias);
                //显示是调用的那个模板
                //sb.AppendFormat("&nbsp;&nbsp;（{0}）",(TemplateType)Enum.Parse(typeof(TemplateType),item.TemplateType.ToString(),true));
                if (!item.IsEnabled)
                {
                    sb.Append("&nbsp;&nbsp;<font color=\"red\">未启用</font>");
                }
                if (item.IsDeleted)
                {
                    sb.Append("&nbsp;&nbsp;<font color=\"red\">已删除</font>");
                }
                sb.AppendFormat("&nbsp;&nbsp;<a href=\"edit.cshtml?siteId={0}&id={1}\">编辑</a>", item.SiteId, item.Id);
                //sb.AppendFormat("&nbsp;&nbsp;<a href=\"javascript:void(0);\" onclick=\"deleteCategory({0},{1})\">删除</a>",item.Id,item.SiteId);
                //递归
                sb.Append(BuildListForAdminWithEdit(list, item.Id, siteId));
                sb.AppendLine("</li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }
        #endregion
    }

}
