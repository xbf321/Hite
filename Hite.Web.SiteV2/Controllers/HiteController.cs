/*******************************************************************  
 * Copyright (C) vancl.com
 * All rights reserved. 
 * 
 * Created By: VANCLOA\xingbaifang
 * Create Date:2011-08-19 14:34:38
 * Last Modified By:VANCLOA\xingbaifang
 * Last Modified Date:2011-08-19 14:34:38
 * Description: 所有Controller的基类
 * ********************************************************************/  
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Globalization;

using Hite.Model;
using Hite.Services;

namespace Hite.Web.Controllers.Site
{
    /// <summary>
    /// 所有的Controller都继承此Controller
    /// </summary>
    public class HiteController : Controller
    {
        protected CategoryInfo GetRootCategoryInfo(SiteInfo currentSiteInfo,CategoryInfo current) {
            if (current.ParentId == 0) { return current; }
            var list = CategoryService.ListBySiteId(currentSiteInfo.Id,true);
            Func<CategoryInfo, CategoryInfo> fb = null;
            fb = n => {
                if(n.ParentId != 0){
                    var item = list.Where(p => p.Id == n.ParentId).FirstOrDefault();
                    return fb(item);
                }
                return n;
            };
            return fb(current);
        }

        #region == 输出模板信息 ==
        protected MvcHtmlString Partial(string partialViewName)
        {
            return Partial(partialViewName, null, null);
        }
        protected MvcHtmlString Partial(string partialViewName, object model)
        {
            return Partial(partialViewName, model, null);
        }
        protected MvcHtmlString Partial(string partialViewName, object model, ViewDataDictionary viewData)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.CurrentCulture))
            {
                RenderPartialInternal(partialViewName, viewData, model, writer, ViewEngines.Engines);
                return MvcHtmlString.Create(writer.ToString());
            }
        }
        private void RenderPartialInternal(string partialViewName, ViewDataDictionary viewData, object model, TextWriter writer, ViewEngineCollection viewEngineCollection)
        {
            if (String.IsNullOrEmpty(partialViewName))
            {
                throw new ArgumentException("partialView Is Empty!", "partialViewName");
            }
            partialViewName = string.Format("~/Views/Templates/{0}.cshtml", partialViewName);
            ViewDataDictionary newViewData = null;

            if (model == null)
            {
                if (viewData == null)
                {
                    newViewData = new ViewDataDictionary(ViewData);
                }
                else
                {
                    newViewData = new ViewDataDictionary(viewData);
                }
            }
            else
            {
                if (viewData == null)
                {
                    newViewData = new ViewDataDictionary(model);
                }
                else
                {
                    newViewData = new ViewDataDictionary(viewData) { Model = model };
                }
            }
            IView view = viewEngineCollection.FindPartialView(this.ControllerContext, partialViewName).View;
            ViewContext newViewContext = new ViewContext(this.ControllerContext, view, newViewData, this.TempData, writer);

            IView newView = FindPartialView(newViewContext, partialViewName, viewEngineCollection);
            newView.Render(newViewContext, writer);
        }
        private IView FindPartialView(ViewContext viewContext, string partialViewName, ViewEngineCollection viewEngineCollection)
        {
            ViewEngineResult result = viewEngineCollection.FindPartialView(viewContext, partialViewName);
            if (result.View != null)
            {
                return result.View;
            }
            throw new Exception("Find Template Error!");
        }
        #endregion
        
    }
}
