/*******************************************************************  
 * Copyright (C) vancl.com
 * All rights reserved. 
 * 
 * Created By: VANCLOA\xingbaifang
 * Create Date:2011-08-17 11:56:51
 * Last Modified By:VANCLOA\xingbaifang
 * Last Modified Date:2011-08-17 11:56:51
 * Description: 请补充对该文件的描述
 * ********************************************************************/  
using System;
using System.Xml.Linq;
using System.Linq;
using System.Text.RegularExpressions;

using Hite.Model;
using Hite.Services;
using System.Web;
using System.Collections.Generic;

namespace Hite.Mvc
{
    public sealed class HiteContext
    {
        private static readonly string SITECONFIGPATH = "SiteConfig.xml";
        private const string URLPATTERN = @"(?:https?|ftp)://[-A-Z0-9a-z.]+(?:/([-A-Z0-9a-z.]+))?";
        private HiteContext() {
            int currentSiteId = 1;
            string domain = Goodspeed.Web.UrlHelper.Current.Domain.ToLower();   //当前域名称
            string url = Goodspeed.Web.UrlHelper.Current.Url.ToLower();         //当前URL
            Language = WebLanguage.zh_cn;                                          //默认中文


            try
            {
                Regex r = new Regex(URLPATTERN);
                Match m = r.Match(url);
                if (m.Success)
                {
                    string value = m.Groups[1].Value;
                    if (value == "en")
                    {
                        Language = WebLanguage.en;
                    }
                }
                //Controleng.Common.Utils.WriteCookie("language",Language.ToString());

                XElement sitesElement = XElement.Load(String.Concat(System.AppDomain.CurrentDomain.BaseDirectory, SITECONFIGPATH));
                var items = sitesElement.Elements();

                foreach(var item in items){
                    string _domain = (string)item.Attribute("url");
                    int _id = (int)item.Attribute("siteid");
                    int _enId = (int)item.Attribute("ensiteid");
                    if (_domain == domain)
                    {
                        currentSiteId = _id;
                        if(Language == WebLanguage.en){
                            if (_enId > 0)
                            {
                                currentSiteId = _enId;  //英文版
                            }
                        }
                        break;
                    }
                }
                SiteAllUrlList = items.Select(p => (string)p.Attribute("url")).ToList();
                
            }
            catch (Exception ex) {
                log4net.LogManager.GetLogger(typeof(HiteContext))
                        .Error(ex.ToString(), ex);
            }
            Site = SiteService.Get(currentSiteId, true);
            UserInfo = new UserInfo();
            if(HttpContext.Current.Request.IsAuthenticated){
                UserInfo = UserService.Get(HttpContext.Current.User.Identity.Name);
            }
        }
        public List<string> SiteAllUrlList { get; set; }
        public static HiteContext Current
        {
            get
            {
                return new HiteContext();
            }
        }
        public UserInfo UserInfo { get; set; }
        public WebLanguage Language { get; set; }
        public SiteInfo Site { get; set; }
    }
}
