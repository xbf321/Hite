/*******************************************************************  
 * Copyright (C) vancl.com
 * All rights reserved. 
 * 
 * Created By: VANCLOA\xingbaifang
 * Create Date:2011-08-17 16:34:26
 * Last Modified By:VANCLOA\xingbaifang
 * Last Modified Date:2011-08-17 16:34:26
 * Description: /Channel/xx.html和/hr.html用的
 * ********************************************************************/  
using System.Text;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Text.RegularExpressions;

using Hite.Mvc;
using Hite.Model;
using Hite.Services;
using Controleng.Common;
using Hite.Mvc.Filters;
using System.Resources;



namespace Hite.Web.Controllers.Site
{
    [SilenceHandleError]
    public class ChannelController : HiteController
    {
        #region == 人才招聘 ==
        /// <summary>
        /// 人才招聘Action
        /// /hr.html
        /// 自动跳到某一个栏目下（一般是未启用的类别）
        /// </summary>
        /// <returns></returns>
        
        public ActionResult Hr() {
            var siteInfo = HiteContext.Current.Site;
            int categoryId = 0;
            switch(siteInfo.IndexFileName){
                default:
                case IndexFileNameOfSite.WWW:
                    {
                        categoryId = siteInfo.Language == WebLanguage.zh_cn ? 287 : 728 ;
                    }
                    break;
                case IndexFileNameOfSite.Systematic:
                    categoryId = siteInfo.Language == WebLanguage.zh_cn ? 322 : 743;
                    break;
                case IndexFileNameOfSite.Hoisting:
                    categoryId = siteInfo.Language == WebLanguage.zh_cn ? 294 : 748;
                    break;
                case IndexFileNameOfSite.Electric:
                    categoryId = siteInfo.Language == WebLanguage.zh_cn ? 329 : 732;
                    break;
                case IndexFileNameOfSite.Energy:
                    categoryId = siteInfo.Language == WebLanguage.zh_cn ? 308 : 758;
                    break;
                case IndexFileNameOfSite.Mechatronics:
                    categoryId = siteInfo.Language == WebLanguage.zh_cn ? 315 : 753;
                    break;
                case IndexFileNameOfSite.Distribution:
                    categoryId = siteInfo.Language == WebLanguage.zh_cn ? 301 : 738;
                    break;
                case IndexFileNameOfSite.Hiscom:
                    categoryId = siteInfo.Language == WebLanguage.zh_cn ? 398 : 763;
                    break;
            }
            //当前节点
            var currentCategoryInfo = CategoryService.Get(categoryId);
            if (currentCategoryInfo.Id == 0)
            {
                return Content("Arguments Error!");
            }
            return LoadChannelShowPage(siteInfo, currentCategoryInfo);
        }
        #endregion

        #region == Channel Show ==
        public ActionResult Show()
        {
            /*
             总模板页需要以下变量
             * 1，根据Url获得根节点信息，因为左边需要导航信息
             * 2，根据Url获得所属类别，因为右边区域有个副导航
             */
            //Channel格式，必须为/Channel/(\d+).html
            //优先选择这样的格式
            var siteInfo = HiteContext.Current.Site;
            int categoryId = 0;
            string path = Goodspeed.Web.UrlHelper.Current.Path;
            string _urlCatName = Regex.Match(path, @"channel/(\w+)\.html", RegexOptions.IgnoreCase).Groups[1].Value;
            if (Regex.IsMatch(_urlCatName, @"\d+"))
            {
                //不是别名
                categoryId = Controleng.Common.Utils.StrToInt(_urlCatName, 0);
            }
            else
            {
                //是别名的情况
                categoryId = CategoryService.ListBySiteId(siteInfo.Id, true).FirstOrDefault(p => p.Alias == _urlCatName).Id;
            }
            //当前节点
            var currentCategoryInfo = CategoryService.Get(categoryId, true);
            if (currentCategoryInfo.Id == 0)
            {
                return Content("Arguments Error!");
            }
            return LoadChannelShowPage(siteInfo, currentCategoryInfo);
        }
        [NonAction]
        public ActionResult LoadChannelShowPage(SiteInfo currentSiteInfo,CategoryInfo catInfo) {
            //当前节点
            var currentCategoryInfo = catInfo;
            //根节点
            CategoryInfo rootCategoryInfo = currentCategoryInfo;
            //返回的模板HTML
            MvcHtmlString templateHtml = null;
            if (currentCategoryInfo != null && currentCategoryInfo.Id > 0)
            {

                //创建右边区域导航
                if (currentCategoryInfo.IsShowFirstChildNode)
                {
                    //如果显示一级子分类的第一个
                    var first = CategoryService.ListByParentId(currentCategoryInfo.Id, true).FirstOrDefault();
                    if (first != null && first.Id > 0)
                    {
                        currentCategoryInfo = first;
                    }
                }

                #region == 动态加载模板开始 ==
                //动态加载模板开始
                switch (currentCategoryInfo.TemplateType)
                {
                    case (int)TemplateType.ArticleListWithCategory:
                    case (int)TemplateType.ArticleList:
                    case (int)TemplateType.ArticleListWithTopOneImage:
                    case (int)TemplateType.ArticleListWithImage:
                        {
                            string viewName = ((TemplateType)Enum.Parse(typeof(TemplateType), currentCategoryInfo.TemplateType.ToString())).ToString();

                            //文章列表（头一条带有图片）
                            int pageIndex = CECRequest.GetQueryInt("page", 1);
                            int pageSize = 10;

                            var articleList = ArticleService.List(new SearchSetting() { 
                                PageIndex  = pageIndex,
                                PageSize = pageSize,
                                SiteId = currentSiteInfo.Id,
                                CategoryId = currentCategoryInfo.Id
                            },currentSiteInfo.Language);

                            ViewDataDictionary vdd = new ViewDataDictionary();
                            vdd.Add("PageIndex", pageIndex);
                            vdd.Add("PageSize", pageSize);
                            vdd.Add("CurrentCategoryInfo", currentCategoryInfo);
                            vdd.Add("Total", articleList.TotalItemCount);
                            vdd.Add("ArticleList", articleList);
                            vdd.Add("Language",currentSiteInfo.Language);

                            templateHtml = Partial(viewName, articleList, vdd);
                        }
                        break;
                    case (int)TemplateType.CategoryListWithOneColumn:
                        {
                            //根据当前ID获得一级子栏目
                            var categoryList = CategoryService.ListByParentId(currentCategoryInfo.Id, true);
                            //对每一个分类，添加几条文章
                            //分类中有个ArticleList的扩展属性
                            var rows = 5; //显示5条
                            foreach (var item in categoryList)
                            {
                                item.ArticleList = ArticleService.ListWithoutPage(currentSiteInfo.Id, item.Id, rows, currentSiteInfo.Language);
                            }
                            ViewDataDictionary vdd = new ViewDataDictionary();
                            vdd.Add("Language", currentSiteInfo.Language);
                            templateHtml = Partial(TemplateType.CategoryListWithOneColumn.ToString(), categoryList,vdd);
                        }
                        break;
                    case (int)TemplateType.CategoryListWithTwoColumn:
                        {
                            //根据当前ID获得一级子栏目
                            var categoryList = CategoryService.ListByParentId(currentCategoryInfo.Id, true) ;
                            templateHtml = Partial(TemplateType.CategoryListWithTwoColumn.ToString(), categoryList);
                        }
                        break;
                    case (int)TemplateType.JobList:
                        {
                            int pageIndex = CECRequest.GetQueryInt("page", 1);
                            int pageSize = 10;

                            var jobList = JobService.List(new SearchSetting() { 
                                SiteId = currentSiteInfo.Id,
                                PageIndex = pageIndex,
                                PageSize = pageSize
                            });
                            templateHtml = Partial(TemplateType.JobList.ToString(),jobList);
                        }
                        break;
                    case (int)TemplateType.AttachmentList: {
                        int pageIndex = CECRequest.GetQueryInt("page", 1);
                        int pageSize = 10;

                        var attachList = AttachmentService.List(new SearchSetting()
                        {
                            SiteId = currentSiteInfo.Id,
                            PageIndex = pageIndex,
                            PageSize = pageSize,
                            CategoryId = currentCategoryInfo.Id
                        });
                        templateHtml = Partial(TemplateType.AttachmentList.ToString(), attachList);
                    } break;
                    default:
                    case (int)TemplateType.ShowSingleCategoryInfo:
                        {
                            //显示一条类别详细信息（图片和描述）,不显示此类别下的文章
                            //如果此栏目默认显示自己子节点的第一条栏目，则取出子栏目的默认一条
                            //大部分是针对根节点来说的，有的时候根节点没有模板页，则点击根节点的时候，默认显示自己子节点的第一个栏目内容
                            templateHtml = Partial(TemplateType.ShowSingleCategoryInfo.ToString(), currentCategoryInfo);
                        }
                        break;
                        
                }
                #endregion

                //设置当前类别的根结点信息
                rootCategoryInfo = GetRootCategoryInfo(currentSiteInfo,currentCategoryInfo);
                ViewBag.RootCategoryInfo = rootCategoryInfo;
                ViewBag.CurrentCategoryInfo = currentCategoryInfo;
                //返回模板HTML
                ViewBag.TemplateHtml = templateHtml;
                //设置页面标题
                ViewBag.Title = currentCategoryInfo.Name;

            }
            return View("~/Views/Category/Show.cshtml");
        }
        #endregion

        #region == [ChildActionOnly] 输出Channel页面左边子栏目列表 ==
        /// <summary>
        /// 输出Channel页面左边子栏目列表
        /// </summary>
        /// <param name="rootId"></param>
        /// <param name="selectedId"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult SubCategoryListForChannelPage(int rootId, int selectedId,WebLanguage language = WebLanguage.zh_cn)
        {
            if(rootId == 0){
                //在当前页面不会列出此站点的所有类别的
                return Content(string.Empty);
            }
            StringBuilder sbHtml = new StringBuilder();
            Func<IEnumerable<CategoryInfo>, StringBuilder> fb = null;
            fb = (n1) =>
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in n1)
                {
                    sb.AppendFormat("<h3{1}>{0}</h3>", CategoryLinkUrlHelper.BuildLink(item,language), item.Id == selectedId ? " class=\"current\"" : string.Empty);
                    var subList = CategoryService.ListByParentId(item.Id, true);
                    if (subList.Count() > 0)
                    {
                        sb.Append("<ul>");
                        foreach (var subItem in subList)
                        {
                            sb.AppendFormat("<li{1}>{0}</li>", CategoryLinkUrlHelper.BuildLink(subItem,language), subItem.Id == selectedId ? " class=\"current\"" : string.Empty);
                        }
                        sb.Append("</ul>");
                    }
                }
                return sb;
            };
            var list = CategoryService.ListByParentId(rootId, true);
            sbHtml = fb(list);
            return Content(sbHtml.ToString());
        }
        #endregion

        #region == [ChildActionOnly] 输出Channel页面右边的当前栏目的导航信息 ==
        /// <summary>
        /// 输出Channel页面右边的当前栏目的导航信息
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="catId"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult RenderSubNavForChannelPage(int siteId, int catId,string customTitle,WebLanguage language = WebLanguage.zh_cn)
        {
            StringBuilder sbNav = new StringBuilder(string.Format("<a href=\"/{1}\">{0}</a>", 
                LanguageResourceHelper.GetString("channel-sub-nav-home-text", language),
                (language == WebLanguage.zh_cn ? string.Empty : language.ToString())
             ));
            if (catId > 0)
            {
                var upList = CategoryService.ListUpById(siteId, catId);

                foreach (var item in upList)
                {
                    sbNav.AppendFormat("&nbsp;&nbsp;>&nbsp;&nbsp;{0}", CategoryLinkUrlHelper.BuildLink(item,language));
                }
            }
            if (!string.IsNullOrEmpty(customTitle))
            {
                sbNav.AppendFormat("&nbsp;&nbsp;>&nbsp;&nbsp;{0}", customTitle);
            }

            return Content(sbNav.ToString());
        }
        #endregion
    }
}
