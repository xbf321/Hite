/*******************************************************************  
 * Copyright (C) vancl.com
 * All rights reserved. 
 * 
 * Created By: VANCLOA\xingbaifang
 * Create Date:2011-08-18 10:08:12
 * Last Modified By:VANCLOA\xingbaifang
 * Last Modified Date:2011-08-18 10:08:12
 * Description: 文章控制器
 * ********************************************************************/  
using System.Linq;
using System.Web.Mvc;
using System.Text.RegularExpressions;

using Hite.Mvc;
using Hite.Services;
using Hite.Model;
using Hite.Mvc.Filters;

namespace Hite.Web.Controllers.Site
{
    /// <summary>
    /// Article Controller
    /// </summary>
    [SilenceHandleError]
    public class ArticleController : HiteController
    {
        /// <summary>
        /// 文章详细页面
        /// </summary>
        /// <returns></returns>        
        public ActionResult Detail()
        {
            SiteInfo currentSiteInfo = HiteContext.Current.Site;
            /*
             总模板页需要以下变量
             * 1，根据Url获得根节点信息，因为左边需要导航信息
             * 2，根据Url获得所属类别，因为右边区域有个副导航
             */
            string path = Goodspeed.Web.UrlHelper.Current.Path;
            
            string url = Regex.Match(path, @"(?:\d{0,4}-\d{0,2}-\d{0,2})/(\d+)\.html", RegexOptions.IgnoreCase).Groups[1].Value;
            var articleInfo = ArticleService.GetByUrl(url,currentSiteInfo.Language);

            if (articleInfo.Id > 0)
            {
                //获取此文章所属类别ID,有可能此文章是同步发到此站点下的，所以需要根据站点ID和文章ID获得此分类ID
                int currentCategoryId = ArticleService.GetCategoryIdBySiteIdAndArticleId(currentSiteInfo.Id, articleInfo.Id);
                var currentCategoryInfo = CategoryService.ListBySiteId(currentSiteInfo.Id, true).Where(p => p.Id == currentCategoryId).FirstOrDefault();
                if(currentCategoryInfo == null){
                    log4net.LogManager.GetLogger(typeof(ArticleController)).Error("currentCategoryInfo is null!--Controller:Article,Action:Show");
                    return View("Error");
                }
                var rootCategoryInfo = GetRootCategoryInfo(currentSiteInfo,currentCategoryInfo);


                ViewBag.RootCategoryInfo = rootCategoryInfo;
                ViewBag.CurrentCategoryInfo = currentCategoryInfo;

                //相关文章
                var rows = 10;//显示10条
                ViewBag.RelatedArticleList = ArticleService.GetRelatedByArticleId(currentSiteInfo.Id, articleInfo.Id, rows,currentSiteInfo.Language);
                //设置页面标题
                ViewBag.Title = articleInfo.Title;
                ViewBag.Keywords = articleInfo.Tags;
                ViewBag.Description = Goodspeed.Common.CharHelper.Truncate(Controleng.Common.Utils.RemoveHtml(articleInfo.Content), 60);
            }
            return View("~/Views/Article/Detail.cshtml", articleInfo);
        }
    }
}
