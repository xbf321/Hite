/*******************************************************************  
 * Copyright (C) vancl.com
 * All rights reserved. 
 * 
 * Created By: VANCLOA\xingbaifang
 * Create Date:2011-08-19 14:34:00
 * Last Modified By:VANCLOA\xingbaifang
 * Last Modified Date:2011-08-19 14:34:00
 * Description: 首页，查询，联系我们，反馈，404
 * ********************************************************************/  
using System.Text;
using System.Linq;
using System.Web.Mvc;
using System.Text.RegularExpressions;


using Hite.Mvc;
using Hite.Model;
using Hite.Services;
using Hite.Mvc.Filters;



namespace Hite.Web.Controllers.Site
{
    [SilenceHandleError]
    public class HomeController : HiteController
    {
        #region == 首页 ==
        /// <summary>
        /// 首页Action
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            
            SiteInfo  currentSiteInfo = HiteContext.Current.Site;

            #region == 中文 ==
            if (currentSiteInfo.Language == WebLanguage.zh_cn)
            {
                //焦点图片
                ViewBag.FocusImageList = ArticleService.ListWithoutPageV2(currentSiteInfo.Id, "首页设置/焦点图片", 5);
                #region == Switch ==
                switch (currentSiteInfo.IndexFileName)
                {
                    case IndexFileNameOfSite.None:
                    case IndexFileNameOfSite.WWW:   //海得集团
                        {
                            //获取集团新闻的文章,Id:2
                            ViewBag.Jituanxinwen = ArticleService.ListWithoutPage(currentSiteInfo.Id, 2, 5, currentSiteInfo.Language);
                            //获取业界案例的文章,Id:8
                            ViewBag.Yejianli = ArticleService.ListWithoutPage(currentSiteInfo.Id, 24, 8, currentSiteInfo.Language);
                            //获取主营业务的文章,Id:45
                            ViewBag.Zhuyingyewu = ArticleService.ListWithoutPage(currentSiteInfo.Id, 45, 6, currentSiteInfo.Language);
                            //获取海德品牌的文章,Id = 46
                            ViewBag.Haidepinpai = ArticleService.ListWithoutPage(currentSiteInfo.Id, 46, 10, currentSiteInfo.Language);
                        }
                        break;
                    case IndexFileNameOfSite.Energy:    //海得能源
                        {
                            ViewBag.Liangdian = ArticleService.ListWithoutPageV2(currentSiteInfo.Id, "首页设置/亮点展示", 1, currentSiteInfo.Language);
                            ViewBag.News = ArticleService.ListWithoutPageV2(currentSiteInfo.Id, "新闻中心", 6,true,currentSiteInfo.Language);
                            ViewBag.CustomerService = CategoryService.ListByParentId(69, true);
                            ViewBag.Product = ArticleService.ListWithoutPageV2(currentSiteInfo.Id, "首页设置/产品业绩展示", 5, currentSiteInfo.Language);
                        }
                        break;
                    case IndexFileNameOfSite.Electric:  //海得电气
                        {

                            ViewBag.News = ArticleService.ListWithoutPageV2(currentSiteInfo.Id, "新闻中心", 4, currentSiteInfo.Language);
                        }
                        break;
                    case IndexFileNameOfSite.Distribution: //海得成套
                        {
                            ViewBag.News = ArticleService.ListWithoutPageV2(currentSiteInfo.Id, "新闻中心", 5, currentSiteInfo.Language);
                            var cats = CategoryService.ListByParentId(105, true);
                            foreach (var item in cats)
                            {
                                item.ArticleList = ArticleService.ListWithoutPage(currentSiteInfo.Id, item.Id, 3, currentSiteInfo.Language);
                            }
                            ViewBag.Categories = cats;
                        }
                        break;
                    case IndexFileNameOfSite.Hoisting: //海得起重
                        {
                            ViewBag.News = ArticleService.ListWithoutPageV2(currentSiteInfo.Id, "新闻中心", 6, currentSiteInfo.Language);
                            ViewBag.ProductSolution = ArticleService.ListWithoutPageV2(currentSiteInfo.Id, "产品解决方案/起重机电控系统", 20, currentSiteInfo.Language);
                            ViewBag.Brand = ArticleService.ListWithoutPageV2(currentSiteInfo.Id, "首页设置/自主品牌", 6, currentSiteInfo.Language);
                            var cats = CategoryService.ListByParentId(90, true);
                            foreach (var item in cats)
                            {
                                item.ArticleList = ArticleService.ListWithoutPage(currentSiteInfo.Id, item.Id, 1, currentSiteInfo.Language);
                            }
                            ViewBag.Cases = cats;
                        }
                        break;
                    case IndexFileNameOfSite.Mechatronics: //机电一体化
                        {
                            ViewBag.Products = CategoryService.ListByParentId(121, true);
                            ViewBag.SolutionTopOneImg = ArticleService.ListWithoutPageV2(currentSiteInfo.Id, "行业解决方案", 1, true, currentSiteInfo.Language);
                            var cats = CategoryService.ListByParentId(122, true);
                            foreach (var item in cats)
                            {
                                item.ArticleList = ArticleService.ListWithoutPage(currentSiteInfo.Id, item.Id, 1, currentSiteInfo.Language);
                            }
                            ViewBag.Categories = cats;
                        }
                        break;
                    case IndexFileNameOfSite.Systematic:   //系统业务
                        {
                            ViewBag.ProductCats = CategoryService.ListByParentId(163, true);
                            //新品发布
                            ViewBag.NewProduct = ArticleService.ListWithoutPage(currentSiteInfo.Id, 169, 5, currentSiteInfo.Language);
                            //企业动态
                            ViewBag.EnterpriseTrace = ArticleService.ListWithoutPage(currentSiteInfo.Id, 168, 4, currentSiteInfo.Language);
                            //系统集成
                            ViewBag.Xitongjicheng = ArticleService.ListWithoutPage(currentSiteInfo.Id, 164, 3, currentSiteInfo.Language);
                            //工业网络
                            ViewBag.Gongye = ArticleService.ListWithoutPage(currentSiteInfo.Id, 165, 3, currentSiteInfo.Language);
                            //容错服务器
                            ViewBag.Server = ArticleService.ListWithoutPage(currentSiteInfo.Id, 166, 3, currentSiteInfo.Language);

                        }
                        break;
                    case IndexFileNameOfSite.Hiscom:    //海思科
                        {
                            //新闻
                            ViewBag.News = ArticleService.ListWithoutPageV2(currentSiteInfo.Id, "新闻与活动", 1, true, currentSiteInfo.Language);
                            //解决方案
                            ViewBag.Solution = ArticleService.ListWithoutPageV2(currentSiteInfo.Id, "解决方案", 5, currentSiteInfo.Language);

                            ViewBag.Products = ArticleService.ListWithoutPageV2(currentSiteInfo.Id, "产品中心", 2, currentSiteInfo.Language);
                        }
                        break;
                    case IndexFileNameOfSite.Chengdu:
                        {
                            ViewBag.NewsImageList = ArticleService.ListWithoutPageV2(currentSiteInfo.Id, "首页设置/新闻图片", 5);
                            //新闻
                            ViewBag.News = ArticleService.ListWithoutPage(currentSiteInfo.Id, 706, 5, currentSiteInfo.Language);
                        }
                        break;
                    case IndexFileNameOfSite.Jiayi:
                        {
                            ViewBag.News = ArticleService.ListWithoutPageV2(currentSiteInfo.Id, "公司新闻", 4, currentSiteInfo.Language);
                        }
                        break;
                }
                #endregion
            }
            #endregion

            #region == 英文 ==
            if(currentSiteInfo.Language == WebLanguage.en){
                
                #region == Switch ==
                switch (currentSiteInfo.IndexFileName)
                {
                    case IndexFileNameOfSite.None:
                    case IndexFileNameOfSite.WWW:   //海得集团
                        {
                            //焦点图片
                            ViewBag.FocusImageList = ArticleService.ListWithoutPage(currentSiteInfo.Id, 468, 5);
                            //获取集团新闻的文章,Id:416
                            ViewBag.Jituanxinwen = ArticleService.ListWithoutPage(currentSiteInfo.Id, 416, 5, currentSiteInfo.Language);
                            //获取业界案例的文章,Id:419
                            ViewBag.Yejianli = ArticleService.ListWithoutPage(currentSiteInfo.Id, 419, 8, currentSiteInfo.Language);
                            //获取主营业务的文章,Id:429
                            ViewBag.Zhuyingyewu = ArticleService.ListWithoutPage(currentSiteInfo.Id, 429, 6, currentSiteInfo.Language);
                            //获取海德品牌的文章,Id = 430
                            ViewBag.Haidepinpai = ArticleService.ListWithoutPage(currentSiteInfo.Id, 430, 10, currentSiteInfo.Language);
                        }
                        break;
                    case IndexFileNameOfSite.Energy:    //海得能源
                        {
                            //焦点图片
                            ViewBag.FocusImageList = ArticleService.ListWithoutPage(currentSiteInfo.Id, 692, 5);

                            ViewBag.Liangdian = CategoryService.Get(702);

                            ViewBag.News = ArticleService.ListWithoutPage(currentSiteInfo.Id,615, 6,true, currentSiteInfo.Language);
                            ViewBag.CustomerService = CategoryService.ListByParentId(637, true);
                            ViewBag.Product = ArticleService.ListWithoutPage(currentSiteInfo.Id,703, 5, currentSiteInfo.Language);
                        }
                        break;
                    case IndexFileNameOfSite.Electric:  //海得电气
                        {
                            //焦点图片
                            ViewBag.FocusImageList = ArticleService.ListWithoutPage(currentSiteInfo.Id, 686, 5);
                            ViewBag.News = ArticleService.ListWithoutPage(currentSiteInfo.Id, 498, 4, currentSiteInfo.Language);
                        }
                        break;
                    case IndexFileNameOfSite.Distribution: //海得成套
                        {
                            //焦点图片
                            ViewBag.FocusImageList = ArticleService.ListWithoutPage(currentSiteInfo.Id, 688, 5);
                            
                            ViewBag.News = ArticleService.ListWithoutPage(currentSiteInfo.Id,476, 5, currentSiteInfo.Language);
                            var cats = CategoryService.ListByParentId(477, true);
                            foreach (var item in cats)
                            {
                                item.ArticleList = ArticleService.ListWithoutPage(currentSiteInfo.Id, item.Id, 3, currentSiteInfo.Language);
                            }
                            ViewBag.Categories = cats;
                             
                        }
                        break;
                    case IndexFileNameOfSite.Hoisting: //海得起重
                        {
                            //焦点图片
                            ViewBag.FocusImageList = ArticleService.ListWithoutPage(currentSiteInfo.Id, 690, 5);
                             
                            ViewBag.News = ArticleService.ListWithoutPage(currentSiteInfo.Id,563, 6, currentSiteInfo.Language);
                            ViewBag.ProductSolution = ArticleService.ListWithoutPage(currentSiteInfo.Id,564, 20, currentSiteInfo.Language);
                            ViewBag.Brand = ArticleService.ListWithoutPage(currentSiteInfo.Id,700, 6, currentSiteInfo.Language);
                            var cats = CategoryService.ListByParentId(577, true);
                            foreach (var item in cats)
                            {
                                item.ArticleList = ArticleService.ListWithoutPage(currentSiteInfo.Id, item.Id, 1, currentSiteInfo.Language);
                            }
                            ViewBag.Cases = cats;
                            
                        }
                        break;
                    case IndexFileNameOfSite.Mechatronics: //机电一体化
                        {
                            //焦点图片
                            ViewBag.FocusImageList = ArticleService.ListWithoutPage(currentSiteInfo.Id, 696, 5);
                            
                            ViewBag.Products = CategoryService.ListByParentId(588, true);
                            ViewBag.SolutionTopOneImg = ArticleService.ListWithoutPage(currentSiteInfo.Id,593, 1, true, currentSiteInfo.Language);
                            var cats = CategoryService.ListByParentId(593, true);
                            foreach (var item in cats)
                            {
                                item.ArticleList = ArticleService.ListWithoutPage(currentSiteInfo.Id, item.Id, 1, currentSiteInfo.Language);
                            }
                            ViewBag.Categories = cats;
                             
                        }
                        break;
                    case IndexFileNameOfSite.Systematic:   //系统业务
                        {
                            //焦点图片
                            ViewBag.FocusImageList = ArticleService.ListWithoutPage(currentSiteInfo.Id, 694, 5);

                            ViewBag.ProductCats = CategoryService.ListByParentId(516, true);
                            //新品发布
                            ViewBag.NewProduct = ArticleService.ListWithoutPage(currentSiteInfo.Id, 514, 5, currentSiteInfo.Language);
                            //企业动态
                            ViewBag.EnterpriseTrace = ArticleService.ListWithoutPage(currentSiteInfo.Id, 513, 4, currentSiteInfo.Language);
                            //系统集成
                            ViewBag.Xitongjicheng = ArticleService.ListWithoutPage(currentSiteInfo.Id, 524, 3, currentSiteInfo.Language);
                            //工业网络
                            ViewBag.Gongye = ArticleService.ListWithoutPage(currentSiteInfo.Id, 537, 3, currentSiteInfo.Language);
                            //容错服务器
                            ViewBag.Server = ArticleService.ListWithoutPage(currentSiteInfo.Id, 550, 3, currentSiteInfo.Language);
                             

                        }
                        break;
                    case IndexFileNameOfSite.Hiscom:    //海思科
                        {
                            //焦点图片
                            ViewBag.FocusImageList = ArticleService.ListWithoutPage(currentSiteInfo.Id, 698, 5);
                            
                            //新闻
                            ViewBag.News = ArticleService.ListWithoutPage(currentSiteInfo.Id,643, 1, true, currentSiteInfo.Language);
                            //解决方案
                            ViewBag.Solution = ArticleService.ListWithoutPage(currentSiteInfo.Id, 666, 5, currentSiteInfo.Language);

                            ViewBag.Products = ArticleService.ListWithoutPage(currentSiteInfo.Id, 647, 2, currentSiteInfo.Language);
                             
                        }
                        break;
                }
                #endregion
            }
            #endregion

            if (currentSiteInfo.IndexFileName == IndexFileNameOfSite.None) currentSiteInfo.IndexFileName = IndexFileNameOfSite.WWW;
            //string viewName = string.Format("/Views/Home/{0}.cshtml", currentSiteInfo.IndexFileName.ToString());
            string viewName = string.Format("/Views/Home/{1}/{0}.cshtml",currentSiteInfo.IndexFileName.ToString(),currentSiteInfo.Language);
            
            return View(viewName);
        }
        #endregion 

        #region == 查询 ==        
        public ActionResult Search() {
            SiteInfo currentSiteInfo = HiteContext.Current.Site;

            //对_ChannelLayout.cshtml的变量赋值
            CategoryInfo catInfo = new CategoryInfo();
            catInfo.BannerAdImageUrl = "images/banner_search.jpg";
            catInfo.Name = ViewBag.SubNavCustomTitle = Hite.Mvc.LanguageResourceHelper.GetString("search-text", currentSiteInfo.Language); ;
            ViewBag.RootCategoryInfo = ViewBag.CurrentCategoryInfo = catInfo;

            //Search
            string key = Controleng.Common.CECRequest.GetQueryString("q");
            int pageIndex = Controleng.Common.CECRequest.GetQueryInt("page",0);
            int pageSize = 10;
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.Total = 0;

            int takeCount = pageIndex <= 1 ? 0 : (pageIndex - 1) * pageSize;

            //记录查询的词，看看是否有非法字符
           

            if (!string.IsNullOrEmpty(key.Trim())){
                /*对空格进行处理，已支持多个关键词查询*/
                #region == 对空格进行处理，已支持多个关键词查询 ==
                key = Regex.Replace(key, @"['=\s]+", "+");
                var keys = key.Split(new char[] { '+' }, System.StringSplitOptions.RemoveEmptyEntries);
                StringBuilder sbCondtion = new StringBuilder();
                foreach (var s in keys)
                {
                    string _key = s.TrimStart().TrimEnd().Trim();                    
                    if (!string.IsNullOrEmpty(_key))
                    {
                        sbCondtion.AppendFormat(" OR CONTAINS(Title,'{0}')", _key);
                    }
                }
                string condition = string.Format(" AND ({0})", Regex.Replace(sbCondtion.ToString(), @"^\s+OR", string.Empty));
                #endregion            
                var list = ArticleService.Seek(currentSiteInfo.Id, condition);
                ViewBag.Total = list.Count;
                var model = list.Skip(takeCount).Take(pageSize);

                foreach(var item in model){
                    item.Content = Goodspeed.Library.Char.HtmlHelper.RemoveHtml(item.Content);
                    //高亮
                    foreach(var pattern in keys){
                        item.Title = Regex.Replace(item.Title, pattern, c => { return string.Format("<font color=\"red\">{0}</font>",c); },RegexOptions.IgnoreCase);

                        item.Content = Regex.Replace(item.Content, pattern, c => { return string.Format("<font color=\"red\">{0}</font>", c); }, RegexOptions.IgnoreCase);
                    }
                }

                return View("~/Views/Search/Index.cshtml", model);
            }
            


            return View("~/Views/Search/Index.cshtml");
        }
        #endregion

        #region == 联系我们 ==
        /// <summary>
        /// 联系我们
        /// </summary>
        /// <returns></returns>
        public ActionResult ContactUs()
        {
            return View("~/Views/Contactus/Contactus.cshtml");
        }
        #endregion

        #region == 反馈 ==
        public ActionResult Feedback() {
            var siteInfo = HiteContext.Current.Site;
            //对_ChannelLayout.cshtml的变量赋值
            CategoryInfo catInfo = new CategoryInfo();
            catInfo.BannerAdImageUrl = string.Format("/images/{0}feedback.jpg",siteInfo.Language == WebLanguage.zh_cn ? string.Empty : "en_");
            catInfo.Name = ViewBag.SubNavCustomTitle = Hite.Mvc.LanguageResourceHelper.GetString("feedback-text", siteInfo.Language);
            ViewBag.RootCategoryInfo = ViewBag.CurrentCategoryInfo = catInfo;

            return View("~/Views/Feedback/Index.cshtml");
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken(Salt="POSTFEEDBACK")]
        public ActionResult Feedback(FormCollection fc) {
            var siteInfo = HiteContext.Current.Site;
            //对_ChannelLayout.cshtml的变量赋值
            CategoryInfo catInfo = new CategoryInfo();
            catInfo.BannerAdImageUrl = string.Format("/images/{0}feedback.jpg", siteInfo.Language == WebLanguage.zh_cn ? string.Empty : "en_");
            catInfo.Name = ViewBag.SubNavCustomTitle = Hite.Mvc.LanguageResourceHelper.GetString("feedback-text",siteInfo.Language);
            ViewBag.RootCategoryInfo = ViewBag.CurrentCategoryInfo = catInfo;

            int flag = 0 ;

            FeedbackInfo model = new FeedbackInfo();
            model.Company = fc["txtCompany"];
            model.Email = fc["txtEmail"];
            model.Intention = Goodspeed.Library.Char.HtmlHelper.RemoveHtml(fc["txtIntention"]);
            model.Intention = Goodspeed.Common.CharHelper.Truncate(model.Intention,200);
            model.IP = Goodspeed.Common.BrowserInfo.Current.IP;
            model.Phone = fc["txtPhone"];
            model.RealName = fc["txtRealName"];
            model.Requirement = fc["cbRequirement"];
            model.SiteId = siteInfo.Id;
            model.UserId = HiteContext.Current.UserInfo.Id;
            model.UserName = HiteContext.Current.UserInfo.UserName;

            int id = FeedbackService.Create(model).Id;
            if(id >0){
                flag = 1;
                //Success
                ViewBag.Flag = flag;
            }

            return View("~/Views/Feedback/Index.cshtml");
        }
        #endregion

        #region == 输出页面的Header导航 ==
        /// <summary>
        /// 输出页面的Header导航
        /// _Layout.cshtml
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult CategoryListForHeader(int siteId)
        {
            string html = string.Empty;
            var siteInfo = SiteService.Get(siteId, true);
            if (siteInfo.Language == WebLanguage.en)
            {
                //英文的只显示二级分类
                var sb = new StringBuilder();
                var pList = CategoryService.ListBySiteId(siteId, true);
                var rootList = pList.Where(p=>(p.ParentId ==0 && p.IsEnabled && !p.IsDeleted) );
                foreach (var item in rootList)
                {
                    sb.Append("<li>");

                    //创建链接 Start
                    sb.Append(CategoryLinkUrlHelper.BuildLink(item, string.Empty, null, WebLanguage.en));
                    //创建链接 End

                    var subList = pList.Where(p=>(p.ParentId == item.Id && p.IsEnabled && !p.IsDeleted));
                    if(subList.Count()>0){
                        sb.Append("<ul>");
                        foreach(var sub in subList){
                            sb.Append("<li>");
                            //创建链接 Start
                            sb.Append(CategoryLinkUrlHelper.BuildLink(sub, string.Empty, null, WebLanguage.en));
                            //创建链接 End
                            sb.Append("</li>");
                        }
                        sb.Append("</ul>");
                    }
                    sb.Append("</li>");
                }
                html = sb.ToString();
            }
            else
            {
                html = CategoryService.RenderTreeViewForHtml(siteId);
            }
            return Content(html);
        }
        #endregion

        #region == 404 ==
        /// <summary>
        /// 404
        /// </summary>
        /// <returns></returns>
        public ActionResult NotFound404(){
            log4net.LogManager.GetLogger(GetType()).Error(string.Format("Url:{0},Not Found!", Request.Url.ToString()));
            return View("404");
        }
        #endregion

        #region == Ping ==
        public ActionResult Ping() {
            return Content("OK");
        }
        #endregion

        #region == Test ==
        public ActionResult Test() {
            return View();
        }
        #endregion

    }
}
