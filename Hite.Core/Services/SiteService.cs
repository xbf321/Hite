using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Hite.Model;
using Hite.Data;
using System.Web;

namespace Hite.Services
{
    public static class SiteService
    {
        private static volatile System.Web.Caching.Cache webCache = System.Web.HttpRuntime.Cache;
        private const int CACHETIMEOUT = 30;//缓存30分钟
        public static int Create(SiteInfo model) {
            if (model.Id == 0)
            {
                //Insert
                int id = SiteManage.Insert(model);
                model.Id = id;
            }
            else { 
                //Update
                SiteManage.Update(model);
            }
            return model.Id;
        }
        public static SiteInfo Get(int id) {
            return Get(id,false);
        }
        public static SiteInfo Get(int id,bool useCache)
        {
            if(!useCache) return SiteManage.Get(id);
            //需要加缓存
            string KEY = string.Format("GET_SITE_INFO_{0}", id);
            var info = (SiteInfo)webCache[KEY];
            if (info == null)
            {
                info = SiteManage.Get(id);
                webCache.Insert(KEY, info, null, DateTime.Now.AddMinutes(CACHETIMEOUT), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
            }
            return info;
        }
        public static IList<SiteInfo> List() {
            return List(false);
        }
        public static IList<SiteInfo> List(bool useCache)
        {
            if (!useCache) return SiteManage.List();
            //需要加缓存
            string KEY = "SITE_LIST";
            var list = (IList<SiteInfo>)webCache[KEY];
            if (list == null)
            {
                list = SiteManage.List();
                webCache.Insert(KEY, list, null, DateTime.Now.AddMinutes(CACHETIMEOUT), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
            }
            return list;
        }
    }
}
