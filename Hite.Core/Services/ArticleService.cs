using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;


using Hite.Model;
using Hite.Data;
using Controleng.Common;
using System.Text.RegularExpressions;

namespace Hite.Services
{
    public static class ArticleService
    {
        private static volatile System.Web.Caching.Cache webCache = System.Web.HttpRuntime.Cache;
        private const int CACHETIMEOUT = 0;//缓存30分钟

        #region == Edit OR Add ==
        /// <summary>
        /// 添加或编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Create(ArticleInfo model) {
            if (model.Id > 0)
            {
                //Update
                ArticleManage.Update(model);
                
            }
            else {
                int i = ArticleManage.Insert(model);
                model.Id = i;
            }
            //Insert ArticleTag

            string[] tags = model.Tags.Split(new char[]{','});
            if(tags.Length>0){
                ArticleManage.InsertTags(model.Id, tags);
            }
            return model.Id;
        }
        #endregion

        #region == 把文章插入到文章分类对应表中 ==
        /// <summary>
        /// 把文章插入到文章分类对应表中
        /// </summary>
        /// <param name="modelList"></param>
        public static void InsertArticleInCategories(IList<ArticleInCategoryInfo> modelList) {
            ArticleManage.InsertArticleInCategories(modelList);
        }
        #endregion

        #region == 获得某一站点所有文章的发布时间，主要为后台查询文章用 ==
        /// <summary>
        /// 获得某一站点所有文章的发布时间，主要为后台查询文章用
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public static List<Tuple<string, string>> GetAllPublishDateBySiteId(int siteId) {
            return ArticleManage.GetAllPublishDateBySiteId(siteId);
        }
        #endregion

        #region == List With Pager ==
        public static IPageOfList<ArticleInfo> List(SearchSetting setting,WebLanguage language = WebLanguage.zh_cn) {
            var list = ArticleManage.List(setting);
            foreach(var item in list){
                LoadExtensionInfo(item,language);
            }
            return list;
        }
        #endregion

        #region == 根据类别ID获取TOP几条，没有分页 ==
        /// <summary>
        /// 根据类别ID获取TOP几条，没有分页
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryId"></param>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public static IList<ArticleInfo> ListWithoutPage(int siteId,int categoryId,int topCount,WebLanguage language = WebLanguage.zh_cn) {
            return ListWithoutPage(siteId,categoryId,topCount,false,language);
        }
        /// <summary>
        /// 根据类别ID获取TOP几条，没有分页
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryId"></param>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public static IList<ArticleInfo> ListWithoutPage(int siteId, int categoryId, int topCount, bool topOneImage,WebLanguage language) {
            var list = ArticleManage.ListWithoutPage(siteId, categoryId, topCount,topOneImage);
            foreach (var item in list)
            {
                LoadExtensionInfo(item,language);
            }
            return list;
        }
        /// <summary>
        /// 根据类别名获取文章
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="name">格式：【首页设置/焦点图片】或【首页设置】</param>
        /// <param name="topCout"></param>
        /// <returns></returns>
        public static IList<ArticleInfo> ListWithoutPageV2(int siteId, string categoryNames, int topCount, WebLanguage language = WebLanguage.zh_cn)
        {
            return ListWithoutPageV2(siteId,categoryNames,topCount,false,language);
        }
        /// <summary>
        /// 根据类别名获取文章
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryNames">格式：【首页设置/焦点图片】或【首页设置】</param>
        /// <param name="topCount"></param>
        /// <param name="topOneImage">第一条是否是图片，默认False</param>
        /// <returns></returns>
        public static IList<ArticleInfo> ListWithoutPageV2(int siteId, string categoryNames, int topCount, bool topOneImage, WebLanguage language = WebLanguage.zh_cn)
        {
            
             List<string> _categoryNames = categoryNames.Split(new char[]{'/'}, StringSplitOptions.RemoveEmptyEntries).ToList();
            Func<List<string>,int, int> fb = null;
            var categoryList = CategoryService.ListBySiteId(siteId,true);
            fb = (n,pid) => {
                string _name = n[0];
                var _item = categoryList.Where(p => p.Name == _name &&  p.ParentId == pid).FirstOrDefault();
                if (_item == null || _item.Id == 0) { return 0; }
                n.Remove(_name);
                if (n.Count == 0) return _item.Id;
                return fb(n,_item.Id);
            };

            int cid = fb(_categoryNames,0);
            return ListWithoutPage(siteId,cid,topCount,topOneImage,language);
        }
        
        #endregion

        #region == 对文章表中ParentCategoryIds数据，构造这样的格式{ id: 6, cat: { id: 8} }，在编辑的时候设置分类默认值 ==
        /// <summary>
        /// 对文章表中ParentCategoryIds数据，构造这样的格式{ id: 6, cat: { id: 8} }，在编辑的时候设置分类默认值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FormatParentCategoryIdsToJson(string str) {
            StringBuilder sbCurrentSelectedCatJSON = new StringBuilder("{");
            sbCurrentSelectedCatJSON.Append(BuildParentIdsSubToJson(str));
            sbCurrentSelectedCatJSON.Append("}");
            return sbCurrentSelectedCatJSON.ToString();
        }
        /// <summary>
        /// 构造这样的数据{ id: 6, cat: { id: 8} };
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string BuildParentIdsSubToJson(string str) {
            if (string.IsNullOrEmpty(str)) { return string.Empty; }
            string PATTERN = @"^(\d+)\-{0,1}";
            StringBuilder sb = new StringBuilder();
            if(Regex.IsMatch(str,PATTERN,RegexOptions.IgnoreCase)){
                sb.AppendFormat("id:{0}",Regex.Match(str,PATTERN).Groups[1].Value);
                str = Regex.Replace(str,PATTERN,string.Empty,RegexOptions.IgnoreCase);
                if(!string.IsNullOrEmpty(str)){
                    sb.Append(",cat:{");
                    sb.Append(BuildParentIdsSubToJson(str));
                    sb.Append("}");
                }
            }
            return sb.ToString();
        }
        #endregion

        #region == 对同步发布到其它站点的分类转换成JSON ==
        /// <summary>
        /// 对同步发布到其它站点的分类转换成JSON格式，保存到Article表中CatsJSON字段中
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FormatSitesStringToJson(string str) {
            //构造JSON
            /*var response = { "count":2,"item": [
             * { "sid": 1, "cat": { "pid": 0, "id": 3, "cat": { "pid": 3, "id": 6}} }
             * ]};*/
            var catsArr = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder catsJSON = new StringBuilder("{\"count\":" + catsArr.Count() + ",\"items\":[");
            for (int i = 0; i < catsArr.Count(); i++)
            {
                catsJSON.Append("{");
                string[] sites = catsArr[i].Split(new char[]{'#'},StringSplitOptions.RemoveEmptyEntries);
                if(sites.Length>0){
                    string siteId = sites[0];
                    catsJSON.AppendFormat("\"sid\":"+siteId);
                    if (sites.Length > 1)
                    {
                        if (!string.IsNullOrEmpty(sites[1]))
                        {
                            catsJSON.Append(FormatCatsStringToJson(sites[1], 0));
                        }
                    }
                }
                catsJSON.Append("}");
                if(i != catsArr.Count() -1){
                    catsJSON.Append(",");
                }
            }
            catsJSON.Append("]}");
            return catsJSON.ToString();
        }
        private static string FormatCatsStringToJson(string str,int parentId) {
            if(string.IsNullOrEmpty(str)){return string.Empty;}
            StringBuilder sb = new StringBuilder();
            string[] cats = str.Split(new char[]{'-'},StringSplitOptions.RemoveEmptyEntries);
            if (cats.Length>0 &&TypeConverter.StrToInt(cats[0]) > 0) {
                sb.Append(",\"cat\":{");
                sb.AppendFormat("\"pid\":{0},\"id\":{1}",parentId,cats[0]);
                if(cats.Length>1){
                    int pid = TypeConverter.StrToInt(cats[0]);
                    if(pid>0){
                        string sub = Regex.Replace(str, @"^\d+\-{0,1}", string.Empty);
                        sb.Append(FormatCatsStringToJson(sub, pid));
                    }
                }
                sb.Append("}");
            }            
            return sb.ToString();
            
        }
        #endregion

        #region == 根据文章ID,获得文章详细信息 ==
        /// <summary>
        /// 根据文章ID,获得文章详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ArticleInfo Get(int id, WebLanguage language = WebLanguage.zh_cn)
        {
            var model = ArticleManage.Get(id);
            LoadExtensionInfo(model,language);
            return model;
        }
        #endregion

        #region == 根据文章ID,获得相关文章 == 
        /// <summary>
        /// 根据文章ID,获得相关文章
        /// </summary>
        /// <param name="siteId">所属站点</param>
        /// <param name="article">所属文章</param>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public static IList<ArticleInfo> GetRelatedByArticleId(int siteId,int articleId,int topCount,WebLanguage language = WebLanguage.zh_cn) {
            var list = ArticleManage.GetRelatedByArticleId(siteId,articleId,topCount);
            foreach (var item in list) {
                LoadExtensionInfo(item,language);
            }
            return list;
        }
        #endregion

        #region == 查询 ==
        /// <summary>
        /// 查询页面所调用方法
        /// 获取所有的数据
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<ArticleInfo> Seek(int siteId, string key)
        {
            var list = ArticleManage.Seek(siteId, key);
            foreach(var item in list){
                LoadExtensionInfo(item);
            }
            return list;
        }
        #endregion

        #region == 添加扩展信息 ==
        public static void LoadExtensionInfo(ArticleInfo model,WebLanguage language = WebLanguage.zh_cn) {
            //
            if (!string.IsNullOrEmpty(model.LinkUrl))
            {
                model.Url = model.LinkUrl;
            }
            else
            {
                model.Url = string.Format("{2}/{0}/{1}.html", model.CreateDateTime.ToString("yyyy-MM-dd"), model.Timespan,language == WebLanguage.zh_cn ? string.Empty : "/"+language.ToString());
            }
        }
        #endregion

        #region == 根据文章URl，获得文章详细信息 ==
        /// <summary>
        /// 根据文章URl，获得文章详细信息
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static ArticleInfo GetByUrl(string url,WebLanguage language = WebLanguage.zh_cn)
        {
            var model = ArticleManage.GetByUrl(url);
            LoadExtensionInfo(model, language);
            return model;
        }
        #endregion

        #region == 根据SiteId和文章ID获得文章所属分类 ==
        /// <summary>
        /// 有的文章是同步发布到其它站点中的
        /// 所以取类别ID的时候去ArticleInCategories表中取
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public static int GetCategoryIdBySiteIdAndArticleId(int siteId,int articleId) {
            return ArticleManage.GetCategoryIdBySiteIdAndArticleId(siteId,articleId);
        }
        #endregion
    }
}
