/*******************************************************************  
 * Copyright (C) vancl.com
 * All rights reserved. 
 * 
 * Created By: VANCLOA\xingbaifang
 * Create Date:2011-08-17 16:33:44
 * Last Modified By:VANCLOA\xingbaifang
 * Last Modified Date:2011-08-17 16:33:44
 * Description: CSS,JS,以及统计用的static方法
 * ********************************************************************/  
using System;
using System.Text;
using System.Web.Mvc;
using System.IO;
using System.Web.Security;
using System.Web;
using System.Linq;
using System.Collections.Generic;

using Hite.Model;
using Hite.Services;
using Hite.Mvc.Filters;
using Hite.Mvc;
using Controleng.Common;

namespace Hite.Web.Controllers.Site
{
    public class StaticController : Controller
    {
        private static volatile System.Web.Caching.Cache webCache = System.Web.HttpRuntime.Cache;
        private const int CACHETIMEOUT = 60 * 4;//缓存4个小时 单位分钟
        //添加缓存

        #region == 输出JS或CSS，减少HTTP请求 ==
        /// <summary>
        /// 输出JS
        /// </summary>
        /// <returns></returns>
        [SilenceHandleError]
        public ActionResult Js() {
            string[] jsArray = CECRequest.GetQueryString("src").Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries);
            string KEY = string.Format("JS_{0}",Utils.MD5(CECRequest.GetQueryString("src")));
            var content = (string)webCache[KEY];
            if(string.IsNullOrEmpty(content)){
                content = LoadFile(jsArray);
                webCache.Insert(KEY, content, null, DateTime.Now.AddMinutes(CACHETIMEOUT), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
            }
            Response.AddHeader("Expires", DateTime.Now.Add(TimeSpan.FromHours(1)).ToUniversalTime().ToString("r"));
            return Content(content, "text/javascript");    
        }
        /// <summary>
        /// 输出CSS
        /// </summary>
        /// <returns></returns>
        [SilenceHandleError]
        public ActionResult Css() {
            string[] cssFiles = CECRequest.GetQueryString("href").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            string KEY = string.Format("CSS_{0}", Utils.MD5(CECRequest.GetQueryString("href")));
            var content = (string)webCache[KEY];
            if (string.IsNullOrEmpty(content))
            {
                content = LoadFile(cssFiles);
                webCache.Insert(KEY, content, null, DateTime.Now.AddMinutes(CACHETIMEOUT), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
            }

            Response.AddHeader("Expires", DateTime.Now.Add(TimeSpan.FromHours(1)).ToUniversalTime().ToString("r"));
            return Content(content, "text/css");
        }
        /// <summary>
        /// 在硬盘中加载文件
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private string LoadFile(string[] files)
        {
            string webRootDir = Server.MapPath("~");
            StringBuilder sbText = new StringBuilder();
            foreach (string item in files)
            {
                //判断文件是否存在
                string filePath = String.Concat(webRootDir, item.Replace("/", "\\"));
                if (System.IO.File.Exists(filePath))
                {
                    //存在
                    sbText.AppendLine(string.Format("/*{0}*/", item));
                    string content = Goodspeed.Library.IO.FileHelper.ReadText(filePath, Encoding.UTF8);
                    sbText.AppendLine(content);
                }
            }
            return sbText.ToString();
        }
        #endregion

        #region == 统计 ==
        /// <summary>
        /// 统计
        /// </summary>
        [SilenceHandleError]
        public void Statistics() {
            WebLogVisitInfo visitInfo = new WebLogVisitInfo();
            visitInfo.IP = Goodspeed.Common.BrowserInfo.Current.IP;
            visitInfo.Querys = Goodspeed.Common.BrowserInfo.Current.Referrer == null ? string.Empty : Goodspeed.Common.BrowserInfo.Current.Referrer.Query;
            visitInfo.Referrer = Controleng.Common.CECRequest.GetQueryString("referrer");
            visitInfo.Url = Goodspeed.Common.BrowserInfo.Current.Referrer == null ? string.Empty : Goodspeed.Common.BrowserInfo.Current.Referrer.ToString();
            visitInfo.UserAgent = Request == null ? string.Empty : (string.IsNullOrEmpty(Request.UserAgent) ? string.Empty : Request.UserAgent);
            visitInfo.SiteId = CECRequest.GetQueryInt("siteid",0);
            visitInfo.Brower = Goodspeed.Common.BrowserInfo.Current.Browser;
            visitInfo.OS = Goodspeed.Common.BrowserInfo.Current.OS;
            visitInfo.UserName = User.Identity.Name;

            if (string.IsNullOrEmpty(visitInfo.Url)) { return; }
            WebLogVisitService.Add(visitInfo);

            Response.AddHeader("Expires", DateTime.Now.Add(TimeSpan.FromHours(5)).ToUniversalTime().ToString("r"));
            Response.End();
        }
        #endregion     

        #region == Attachments Download ==
        /// <summary>
        /// 暂时没用
        /// </summary>
        /// <returns></returns>
        public ActionResult AttachDownload() {
            //if (!Request.IsAuthenticated) { return Content("请先登录！"); }
            int aid = CECRequest.GetQueryInt("aid",0);
            //
            var attachInfo = AttachmentService.Get(aid);
            if(attachInfo.Id >0){
                string ext = Path.GetExtension(attachInfo.Url);
                string type = string.Empty;
                switch(ext){
                    case ".pdf":
                        type = "pdf";
                        break;
                    case ".doc":
                    case ".docx":
                        type = "application/msword";
                        break;
                    case ".rar":
                        type = "application/octet-stream";
                        break;
                    case ".zip":
                        type = "application/zip";
                        break;
                }
                //Controleng.Common.Utils.ResponseFile(attachInfo.Url,attachInfo.Title,type);                
                //更新下载数
                AttachmentService.UpdateDownloadCount(aid);
            }
            
            return Content(string.Empty);
        }
        #endregion

        #region == LoginApi ==
        /// <summary>
        /// 单点登录所使用的api
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginApi() {
            string FORMAT = "var userCookie = {{ '_hr_':'{0}','_ht_':'{1}','username':'{2}','{3}':'{4}'}}";
            string hrValue = Utils.GetCookie("hr");
            string htValue = Utils.GetCookie("ht");
            string userNameValue = Utils.GetCookie("username");
            string usrValue = Utils.GetCookie(FormsAuthentication.FormsCookieName);            

            if (Request.UrlReferrer != null)
            {
                string refeHost = Request.UrlReferrer.Host;
                string domain = string.Empty;

                //把不是hite.com.cn域下的所有站点找出来
                List<string> list = HiteContext.Current.SiteAllUrlList.Where(p => p.IndexOf("hite.com.cn")<=0).ToList();

                //判断是否来自其他域
                foreach(var item in list){
                    if (item == refeHost)
                    {
                        domain = item;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(domain))
                {
                    //格式 xbf321:XXX:DesKey
                    //其中xxx是加密过的随机数
                    //DesKey为salt
                    string newTokenValue = Utils.MD5(string.Format("{0}:{1}:{2}", userNameValue, hrValue, System.Configuration.ConfigurationManager.AppSettings["DESKey"]));
                    if (newTokenValue == htValue)
                    {
                        return Content(string.Format(FORMAT, hrValue, htValue, userNameValue, FormsAuthentication.FormsCookieName, usrValue));
                    }
                }
            }
            return Content(string.Format(FORMAT,string.Empty,string.Empty,string.Empty,FormsAuthentication.FormsCookieName,string.Empty));
        }
        #endregion

    }
}
